using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels;
using ContestantRegister.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContestantRegister.ViewModels.ListItemViewModels;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(
            ApplicationDbContext context,
            IMapper mapper,
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users
                .Include(c => c.StudyPlace)
                .Include(c => c.StudyPlace.City);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admins
        public async Task<IActionResult> Admins()
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);

            var adminIds = admins.Select(u => u.Id).ToList();
            var notAdmins = await _context.Users.Where(u => !adminIds.Contains(u.Id)).ToListAsync();

            var defaultAdmin = admins.First(a => a.Email == UserService.DefaultAdminEmail);
            admins.Remove(defaultAdmin);
            var viewModels = admins.Select(u => new UserAdminViewModel {IsAdmin = true, User = u}).ToList();
            var notAdminViewModels = notAdmins.Select(u => new UserAdminViewModel {User = u}).ToList();

            viewModels.AddRange(notAdminViewModels);

            return View(viewModels);
        }

        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.AddToRoleAsync(user, Roles.Admin);

            return RedirectToAction(nameof(Admins));
        }

        public async Task<IActionResult> MakeNotAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.RemoveFromRoleAsync(user, Roles.Admin);

            return RedirectToAction(nameof(Admins));
        }

        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StudyPlaces"] = await GetListItemsJsonAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel)
        {
            var validationResult = await _userService.ValidateUserAsync(viewModel);
            validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

            if (!ModelState.IsValid)
            {
                ViewData["StudyPlaces"] = await GetListItemsJsonAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

                return View(viewModel);
            }

            var user = new ApplicationUser
            {
                UserName = viewModel.Email,
                RegistrationDateTime = DateTime.Now,
                RegistredBy = await _userManager.GetUserAsync(User) //Хотя пользователя регистрирует админ, все равно проставляем кто зарегал, иначе не отличить от тех, кто зарегался сам
            };

            _mapper.Map(viewModel, user);

            var result = await _userManager.CreateAsync(user, viewModel.PasswordViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result.Errors);

            ViewData["StudyPlaces"] = await GetListItemsJsonAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

            return View(viewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.StudyPlace).SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel();
            _mapper.Map(user, viewModel);
            viewModel.CityId = user.StudyPlace.CityId;

            ViewData["StudyPlaces"] = await GetListItemsJsonAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

            return View(viewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel viewModel)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (dbUser == null)
            {
                return NotFound();
            }

            var validationResult = await _userService.ValidateUserAsync(viewModel);
            validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(viewModel, dbUser);

                    _context.Update(dbUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestantUserExists(dbUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudyPlaces"] = await GetListItemsJsonAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

            return View(viewModel);
        }



        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestantUser = await _context.Users
                .Include(u => u.StudyPlace)
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contestantUser == null)
            {
                return NotFound();
            }

            return View(contestantUser);
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, PasswordViewModel viewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, viewModel.Password);

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var contestantUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(contestantUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContestantUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
