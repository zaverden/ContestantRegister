using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Models.AccountViewModels;
using ContestantRegister.Services;
using ContestantRegister.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
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
            var applicationDbContext = _context.ContestantUsers
                .Include(c => c.StudyPlace)
                .Include(c => c.StudyPlace.City);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName");
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
                ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);

                return View(viewModel);
            }
            
            //TODO копипаста с регистрацией, вынести в какой-то сервис
            ContestantUser user = null;

            switch (viewModel.UserType)
            {
                case UserType.Pupil: user = new Pupil(); break;
                case UserType.Student: user = new Student(); break;
                case UserType.Trainer: user = new Trainer(); break;
            }

            user.UserName = viewModel.Email;
            user.RegistrationDateTime = DateTime.Now;
            //Хотя пользователя регистрирует админ, все равно проставляем кто зарегал, иначе не отличить от тех, кто зарегался сам
            user.RegistredBy = await _userService.GetCurrentUserAsync(User);

            _mapper.Map(viewModel, user);

            var result = await _userManager.CreateAsync(user, viewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);

            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);
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

            var contestantUser = await _context.ContestantUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (contestantUser == null)
            {
                return NotFound();
            }
            var viewModel = new EditUserViewModel();
            _mapper.Map(contestantUser, viewModel);

            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);
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
            var dbUser = await _context.ContestantUsers.SingleOrDefaultAsync(u => u.Id == id);
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
            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);
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

            var contestantUser = await _context.ContestantUsers
                .Include(u => u.StudyPlace)
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contestantUser == null)
            {
                return NotFound();
            }

            return View(contestantUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var contestantUser = await _context.ContestantUsers.SingleOrDefaultAsync(m => m.Id == id);
            _context.ContestantUsers.Remove(contestantUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContestantUserExists(string id)
        {
            return _context.ContestantUsers.Any(e => e.Id == id);
        }

        //TODO копипаста из AccountController
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
