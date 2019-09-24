using System.IO;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationException = ContestantRegister.Services.Exceptions.ValidationException;

namespace ContestantRegister.Controllers
{
    public class TeamContestController : ContestControllerBase
    {
        public TeamContestController(IHandlerDispatcher handlerDispatcher) : base(handlerDispatcher)
        {
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, CreateTeamContestRegistrationViewModel viewModel)
        {
            try
            {
                var command = new CreateTeamContestRegistrationCommand { ViewModel = viewModel };
                await HandlerDispatcher.ExecuteCommandAsync(command);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (ValidationException e)
            {
                e.ValidationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForContestRegistrationAsync(viewModel);

                return View(viewModel);
            }
            
            if (viewModel.ShowRegistrationInfo)
            {
                //TODO стоит ли показывать эту страницу для тренера?
                return RedirectToAction(nameof(Registration)/*, new { id = result.RegistrationId }*/);
            }

            return RedirectToAction(nameof(Details), new { id = viewModel.ContestId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditRegistration(int id, EditTeamContestRegistrationViewModel viewModel)
        {
            try
            {
                await HandlerDispatcher.ExecuteCommandAsync(new EditTeamContestRegistrationCommand
                {
                    RegistrationId = id,
                    ViewModel = viewModel
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (ValidationException e)
            {
                e.ValidationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForContestRegistrationAsync(viewModel);

                return View(viewModel);
            }

            return RedirectToAction(nameof(Details), new { id = viewModel.ContestId });
        }
        
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ExportParticipants(int id)
        {
            try
            {
                var package = await HandlerDispatcher.ExecuteQueryAsync(new GetExportedTeamContestParticipantsQuery { ContestId = id });
                var ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Position = 0;
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Participants.xlsx");
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ExportTeams(int id)
        {
            try
            {
                var result = await HandlerDispatcher.ExecuteQueryAsync(new GetExportedTeamsForContestQuery { ContestId = id });
                var ms = new MemoryStream();
                result.ExcelPackage.SaveAs(ms);
                ms.Position = 0;
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{result.ContestName}.xlsx");
            }
            //TODO добавить фильтр исключений чтобы не перехватывать в кажом методе контроллера
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ImportBaylorRegistration(int id)
        {
            try
            {
                var vm = await HandlerDispatcher.ExecuteQueryAsync(new GetImportBaylorRegistrationDataQuery { ContestId = id });
                return View(vm);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ImportBaylorRegistration(int id, ImportParticipantsViewModel viewModel)
        {
            await HandlerDispatcher.ExecuteCommandAsync(new ImportBaylorRegistrationsCommand { ViewModel = viewModel});
            
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}


