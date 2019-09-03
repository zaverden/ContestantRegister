using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.IO;
using OfficeOpenXml;
using ContestantRegister.Utils.Filter;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Account.ViewModels;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Infrastructure.Filter.Attributes;
using ContestantRegister.Infrastructure.Filter.Contervers;

namespace ContestantRegister.Controllers
{   
    public class GetUsersQuery
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Email { get; set; }

        [ConvertFilter(typeof(NullableIntToNullableBooleanConverter))]
        public int? EmailConfirmed { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Surname { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Name { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("StudyPlace.City", PropertyName = "Name")]
        public string City { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("StudyPlace", PropertyName = "ShortName")]        
        public string StudyPlace { get; set; }

        [ConvertFilter(typeof(EnumDisplayToValueConverter<UserType>))]
        [PropertyName("UserType")]
        public string UserTypeName { get; set; }        
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string City { get; set; }
        public string StudyPlace { get; set; }       

    }

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
        public async Task<IActionResult> Index(GetUsersQuery filter)
        {
            ViewData["Email"] = filter.Email;
            ViewData["EmailConfirmed"] = filter.EmailConfirmed;
            ViewData["Surname"] = filter.Surname;
            ViewData["Name"] = filter.Name;
            ViewData["City"] = filter.City;
            ViewData["StudyPlace"] = filter.StudyPlace;
            ViewData["UserTypeName"] = filter.UserTypeName;

            IQueryable<UserListItemViewModel> users = ConventionsExtensions.AutoFilter(_context
                    .Users
                    .ProjectTo<UserListItemViewModel>(), filter)
                .OrderBy<UserListItemViewModel, string>(u => u.Id);

            return View(await users.ToListAsync());
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

            return View(viewModels.OrderBy(vm => vm.User.Id));
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
            await FillViewDataAsync();

            return View(new CreateUserViewModel());
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
                await FillViewDataAsync(viewModel);

                return View(viewModel);
            }

            var user = new ApplicationUser
            {
                UserName = viewModel.Email,
                RegistrationDateTime = DateTimeExtensions.SfuServerNow,
                RegistredBy = await _userManager.GetUserAsync(User) //Хотя пользователя регистрирует админ, все равно проставляем кто зарегал, иначе не отличить от тех, кто зарегался сам
            };

            _mapper.Map(viewModel, user);

            var result = await _userManager.CreateAsync(user, viewModel.PasswordViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result.Errors);

            await FillViewDataAsync(viewModel);

            return View(viewModel);
        }

        private async Task FillViewDataAsync(UserViewModelBase viewModel = null)
        {
            ViewData["StudyPlaces"] = await GetListItemsAsync<StudyPlace, StudyPlaceDropdownItemViewModel>(_context, _mapper);
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", viewModel?.CityId);
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

            await FillViewDataAsync(viewModel);

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

            await FillViewDataAsync(viewModel); 

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

        public FileResult Export()
        {
            var users = _context.Users
                .Include(r => r.StudyPlace.City.Region);

            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Users");
            worksheet.Cells["A1"].Value = "Email";
            worksheet.Cells["B1"].Value = "EmailConfirmed";
            worksheet.Cells["C1"].Value = "Surname";
            worksheet.Cells["D1"].Value = "Name";
            worksheet.Cells["E1"].Value = "Patronymic";
            worksheet.Cells["F1"].Value = "FirstName";
            worksheet.Cells["G1"].Value = "LastName";
            worksheet.Cells["H1"].Value = "UserType";
            worksheet.Cells["I1"].Value = "StudyPlace";
            worksheet.Cells["J1"].Value = "StudyPlaceType";
            worksheet.Cells["K1"].Value = "City";
            worksheet.Cells["L1"].Value = "Region";
            worksheet.Cells["M1"].Value = "StudentType";                        

            int row = 1;
            foreach (var user in users)
            {
                row++;

                worksheet.Cells[row, 1].Value = user.Email;
                worksheet.Cells[row, 2].Value = user.EmailConfirmed;

                worksheet.Cells[row, 3].Value = user.Surname;
                worksheet.Cells[row, 4].Value = user.Name;
                worksheet.Cells[row, 5].Value = user.Patronymic;
                worksheet.Cells[row, 6].Value = user.FirstName;
                worksheet.Cells[row, 7].Value = user.LastName;

                worksheet.Cells[row, 8].Value = user.UserType;
                worksheet.Cells[row, 9].Value = user.StudyPlace.ShortName;
                worksheet.Cells[row, 10].Value = user.StudyPlace is School ? "School" : "Institution";
                worksheet.Cells[row, 11].Value = user.StudyPlace.City.Name;
                worksheet.Cells[row, 12].Value = user.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 13].Value = user.StudentType;
            } 

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
        }

        private bool ContestantUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
