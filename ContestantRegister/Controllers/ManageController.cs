using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.ViewModels.ListItemViewModels;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          ILogger<ManageController> logger,
          IMapper mapper,
          IUserService userService,
          ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var studyPlace = await _context.StudyPlaces.SingleAsync(sp => sp.Id == user.StudyPlaceId);

            var viewModel = new IndexViewModel
            {
                CanSuggestStudyPlace = true,
                StatusMessage = StatusMessage,
                CityId = studyPlace.CityId,
            };

            _mapper.Map(user, viewModel);

            ViewData["StudyPlaces"] = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel viewModel)
        {
            var validationResult = await _userService.ValidateUserAsync(viewModel);
            validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

            if (!ModelState.IsValid)
            {
                ViewData["StudyPlaces"] = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _mapper.Map(viewModel, user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            StatusMessage = "Профиль был успешно обновлен.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddErrors(changePasswordResult.Errors);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Пароль был успешно изменен.";

            return RedirectToAction(nameof(ChangePassword));
        }

    }
}
