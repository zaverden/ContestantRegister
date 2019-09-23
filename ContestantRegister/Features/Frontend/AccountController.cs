using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Cqrs.Features.Frontend.Account.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Account.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.Exceptions;

namespace ContestantRegister.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHandlerDispatcher _handlerDispatcher;
        public AccountController(IHandlerDispatcher handlerDispatcher)
        {
            _handlerDispatcher = handlerDispatcher;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return View(model);
            }

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new LoginCommand
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                });
            }
            catch (Exception ex) when (ex is EntityNotFoundException || ex is EmailNotConfirmedException)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден или email не подтвержден.");
                return View(model);
            }
            catch (UserLockedOutException)
            {                
                return RedirectToAction(nameof(Lockout));
            }
            catch (InvalidLoginAttemptException)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            
            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            await FillViewDataAsync();

            var vm = new RegisterViewModel
            {
                CanSuggestStudyPlace = true,
            };

            return View(vm);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new RegisterCommand { RegisterViewModel = viewModel, RequestScheme = Request.Scheme, Action = nameof(AccountController.ConfirmEmail), Controller = "Account"});
            }
            catch (Exception ex) when (ex is ValidationException || ex is UnableToCreateUserException)
            {
                if (ex is ValidationException validationException)
                    validationException.ValidationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));
                if (ex is UnableToCreateUserException failedCreationException)
                    ModelState.AddErrors(failedCreationException.Errors);

                await FillViewDataAsync();
                // If we got this far, something failed, redisplay form
                return View(viewModel);
            }
            
            return RedirectToAction(nameof(WaitEmailConfirmation));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _handlerDispatcher.ExecuteCommandAsync(new LogoutCommand());
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult WaitEmailConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new ConfirmEmailCommand { UserId = userId, Code = code});
            }
            //TODO можно вставить InvalidOperationException
            catch (UnableToConfirmEmailException ex)
            {
                var sb = new StringBuilder();
                foreach (var error in ex.Errors)
                {
                    sb.AppendLine($"{error.Code}: {error.Description}<br>");
                }

                //TODO может просто показывать сообщение, а не ошибку?
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = sb.ToString(),
                });
            }

            return View("ConfirmEmail");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            // If we got this far, something failed, redisplay form
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new ForgotPasswordCommand
                {
                    Email = model.Email,
                    Scheme = Request.Scheme,
                    Controller = "Account",
                    Action = nameof(AccountController.ResetPassword)
                });
            }
            catch (EntityNotFoundException)
            {
                // Don't reveal that the user does not exist 
                return RedirectToAction(nameof(UnknownUser));
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UnknownUser()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new ResetPasswordCommand
                {
                    Email = model.Email,
                    Code = model.Code,
                    Password = model.Password,
                });
            }
            catch (EntityNotFoundException)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(UnknownUser));
            }
            catch (Exception ex) when (ex is UnableToResetPasswordException || ex is UnableToConfirmEmailException)
            {
                ModelState.AddErrors(((IdentityException) ex).Errors);
                return View();
            }
            
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task FillViewDataAsync()
        {
            var dataForRegistration = await _handlerDispatcher.ExecuteQueryAsync(new GetDataForRegistrationQuery());

            ViewData["CityId"] = new SelectList(dataForRegistration.Cities, "Id", "Name");
            ViewData["StudyPlaces"] = dataForRegistration.StudyPlaces;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
