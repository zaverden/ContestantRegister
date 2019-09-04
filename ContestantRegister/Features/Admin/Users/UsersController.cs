using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.Queries;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.Exceptions;
using PasswordViewModel = ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels.PasswordViewModel;

namespace ContestantRegister.Controllers
{   
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : CrudController<string,
        ApplicationUser, UserListItemViewModel, CreateUserViewModel, EditUserViewModel,
        GetUsersQuery, GetEntityByIdQuery<ApplicationUser, string>, GetEntityByIdForDeleteQuery<ApplicationUser, string>,
        CreateUserCommand, EditMappedEntityCommand<ApplicationUser, EditUserViewModel, string>, DeleteEntityByIdCommand<ApplicationUser, string>>
    {
        public UsersController(IHandlerDispatcher handlerDispatcher, IMapper mapper) : base(handlerDispatcher, mapper)
        {
        }
        protected override CreateUserViewModel BuildCreateViewModel()
        {
            return new CreateUserViewModel();
        }
        protected override async Task FillViewDataForEditAsync(EditUserViewModel viewModel = null)
        {
            await FillViewDataAsync(viewModel);
        }
        protected override async Task FillViewDataForCreateAsync(CreateUserViewModel viewModel = null)
        {
            await FillViewDataAsync(viewModel);
        }
        protected override void InitCreateCommand(CreateUserCommand command)
        {
            command.CurrentUserEmail = User.Identity.Name;
        }
        private async Task FillViewDataAsync(UserViewModelBase viewModel = null)
        {
            var data = await HandlerDispatcher.ExecuteQueryAsync(new GetDataForUserDetailsQuery());

            ViewData["StudyPlaces"] = data.StudyPlaces;
            ViewData["CityId"] = new SelectList(data.Cities, "Id", "Name", viewModel?.CityId);
        }
        protected override string[] GetIncludePropertiesForEdit()
        {
            return new[] {nameof(ApplicationUser.StudyPlace)};
        }
        protected override string[] GetIncludePropertiesForDelete()
        {
            return new[] { $"{nameof(ApplicationUser.StudyPlace)}.{nameof(StudyPlace.City)}" };
        }

        #region Export Excel

        public async Task<FileResult> Export()
        {
            var excel = await HandlerDispatcher.ExecuteQueryAsync(new GetExportedUsersQuery());

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
                await HandlerDispatcher.ExecuteCommandAsync(new UserChangePasswordCommand
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
            var admins = await HandlerDispatcher.ExecuteQueryAsync(new GetAdminsQuery());

            return View(admins);
        }

        public async Task<IActionResult> MakeAdmin(string id)
        {
            await HandlerDispatcher.ExecuteCommandAsync(new UserAddRoleCommand
            {
                Id = id,
                Role = Roles.Admin,
            });

            return RedirectToAction(nameof(Admins));
        }

        public async Task<IActionResult> MakeNotAdmin(string id)
        {
            await HandlerDispatcher.ExecuteCommandAsync(new UserRemoveRoleCommand
            {
                Id = id,
                Role = Roles.Admin
            });

            return RedirectToAction(nameof(Admins));
        }

        #endregion

    }
}
