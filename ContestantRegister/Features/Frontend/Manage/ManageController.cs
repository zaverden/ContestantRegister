using System.Threading.Tasks;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContestantRegister.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly IHandlerDispatcher _handlerDispatcher;

        public ManageController(IHandlerDispatcher handlerDispatcher)
        {
            _handlerDispatcher = handlerDispatcher;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await _handlerDispatcher.ExecuteQueryAsync(new GetUserDetailsQuery {CurrentUser = User});
            viewModel.StatusMessage = StatusMessage;
                
            await FillViewDataAsync(viewModel);

            return View(viewModel);
        }

        private async Task FillViewDataAsync(IndexViewModel viewModel)
        {
            var data = await _handlerDispatcher.ExecuteQueryAsync(new GetDataForProfileQuery());

            ViewData["StudyPlaces"] = data.StudyPlaces;
            ViewData["CityId"] = new SelectList(data.Cities, "Id", "Name", viewModel.CityId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel viewModel)
        {
            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new SaveUserCommand { ViewModel = viewModel, CurrentUser = User});
            }
            catch (ValidationException e)
            {
                e.ValidationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));
                await FillViewDataAsync(viewModel);
                return View(viewModel);
            }
            
            StatusMessage = "Профиль был успешно обновлен.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            // не понял нафига доставать из базы того же пользователя. 
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new ChangePasswordCommand
                {
                    CurrentUser = User,
                    OldPassword = viewModel.OldPassword,
                    NewPassword = viewModel.NewPassword,
                });
            }
            catch (UnableToChangePasswordException e)
            {
                ModelState.AddErrors(e.Errors);
                return View(viewModel);
            }
            
            StatusMessage = "Пароль был успешно изменен.";

            return RedirectToAction(nameof(ChangePassword));
        }

    }
}
