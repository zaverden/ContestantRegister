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
using System.IO;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Users.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Users.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Utils.Exceptions;
using PasswordViewModel = ContestantRegister.ViewModels.PasswordViewModel;

namespace ContestantRegister.Controllers
{   
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : CrudController<string,
        ApplicationUser, UserListItemViewModel, CreateUserViewModel, EditUserViewModel,
        GetUsersQuery, GetEntityByIdQuery<ApplicationUser, string>, GetEntityByIdForDeleteQuery<ApplicationUser, string>,
        CreateMappedEntityCommand<ApplicationUser, CreateUserViewModel>, EditMappedEntityCommand<ApplicationUser, EditUserViewModel, string>, DeleteEntityByIdCommand<ApplicationUser>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHandlerDispatcher _handlerDispatcher;
        public UsersController(
            ApplicationDbContext context,
            IMapper mapper,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            IHandlerDispatcher handlerDispatcher) : base(handlerDispatcher, mapper)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _userManager = userManager;
            _handlerDispatcher = handlerDispatcher;
        }
        
        // GET: Users/Create
        //public async Task<IActionResult> Create()
        //{
        //    await FillViewDataAsync();

        //    return View(new CreateUserViewModel());
        //}

        protected override async Task FillViewDataForEditAsync(EditUserViewModel viewModel = null)
        {
            await FillViewDataAsync(viewModel);
        }

        protected override async Task FillViewDataForCreateAsync(CreateUserViewModel viewModel = null)
        {
            await FillViewDataAsync(viewModel);
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
            var data = await _handlerDispatcher.ExecuteQueryAsync(new GetDataForUserDetailsQuery());

            ViewData["StudyPlaces"] = data.StudyPlaces;
            ViewData["CityId"] = new SelectList(data.Cities, "Id", "Name", viewModel?.CityId);
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
        protected override string[] GetIncludePropertiesForDelete()
        {
            return new[] { $"{nameof(ApplicationUser.StudyPlace)}.{nameof(StudyPlace.City)}" };
        }

        private bool ContestantUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        #region Export Excel

        public async Task<FileResult> Export()
        {
            var excel = await _handlerDispatcher.ExecuteQueryAsync(new GetExportedUsersQuery());

            var ms = new MemoryStream();
            excel.SaveAs(ms);
            ms.Position = 0;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
        }

        #endregion

        #region Change Password
        public IActionResult ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //TODO нафиг проверку
            //var user = await _userManager.FindByIdAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, PasswordViewModel viewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new UserChangePasswordCommand
                {
                    Id = id,
                    Password = viewModel.Password
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Admins

        // GET: Admins
        public async Task<IActionResult> Admins()
        {
            var admins = await _handlerDispatcher.ExecuteQueryAsync(new GetAdminsQuery());

            return View(admins);
        }

        public async Task<IActionResult> MakeAdmin(string id)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new UserAddRoleCommand
            {
                Id = id,
                Role = Roles.Admin,
            });

            return RedirectToAction(nameof(Admins));
        }

        public async Task<IActionResult> MakeNotAdmin(string id)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new UserRemoveRoleCommand
            {
                Id = id,
                Role = Roles.Admin
            });

            return RedirectToAction(nameof(Admins));
        }

        #endregion

    }
}
