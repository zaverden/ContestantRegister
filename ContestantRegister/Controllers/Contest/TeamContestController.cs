using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.Contest.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace ContestantRegister.Controllers
{
    public class TeamContestController : ContestControllerBase
    {
        private readonly IContestRegistrationService _contestRegistrationService;
        
        public TeamContestController(
            ApplicationDbContext context,
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<MailOptions> options,
            IContestRegistrationService contestRegistrationService,
            IUserService userService
        )
            : base(context, mapper, emailSender, userManager, options, userService)
        {
            _contestRegistrationService = contestRegistrationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, CreateTeamContestRegistrationViewModel viewModel)
        {
            var contest = await GetContestForRegistration(id);
            if (contest == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateCreateTeamContestRegistrationAsync(viewModel, User);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            var registration = new TeamContestRegistration();

            _mapper.Map(viewModel, registration);

            var registreledForStudyPlaceCount = contest.ContestRegistrations.Where(r => r.StudyPlaceId == registration.StudyPlaceId).Count();
            string studyPlaceName = string.Empty;
            var studyPlace = await _context.StudyPlaces.FindAsync(registration.StudyPlaceId);

            if (contest.ParticipantType == ParticipantType.Student && contest.IsEnglishLanguage)
            {
                studyPlaceName = ((Institution)studyPlace).ShortNameEnglish;
            }
            else
            {
                studyPlaceName = studyPlace.ShortName;
            }

            registration.OfficialTeamName = $"{studyPlaceName} {registreledForStudyPlaceCount + 1}";

            return await RegisterInternalAsync(viewModel, registration, contest);
        }

        protected override async Task<Contest> GetContestForRegistration(int contestId)
        {
            return await _context.Contests
                .Include(c => c.ContestAreas)
                .Include(c => c.ContestRegistrations)
                .Include("ContestAreas.Area")
                .SingleOrDefaultAsync(c => c.Id == contestId);
        }

        protected override ContestRegistrationViewModel CreateEditContestRegistrationViewModel()
        {
            return new EditTeamContestRegistrationViewModel();
        }

        protected override async Task<ContestRegistration> GetContestRegistrationForEditAsync(int registrationId)
        {
            var registration = await _context.TeamContestRegistrations
                .Include(r => r.Contest)
                .Include(r => r.Contest.ContestAreas)
                .Include("Contest.ContestAreas.Area")
                .Include(r => r.StudyPlace)
                .Include(r => r.RegistredBy)
                .Include(r => r.Participant1)
                .Include(r => r.Participant2)
                .Include(r => r.Participant3)
                .SingleOrDefaultAsync(r => r.Id == registrationId);

            return registration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditRegistration(int id, EditTeamContestRegistrationViewModel viewModel)
        {
            var dbRedistration = await _context.TeamContestRegistrations
                .SingleOrDefaultAsync(r => r.Id == id);
            if (dbRedistration == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateEditTeamContestRegistrationAsync(viewModel, User);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                var contest = await GetContestForRegistration(viewModel.ContestId);

                await FillViewDataForContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            _mapper.Map(viewModel, dbRedistration);

            if (dbRedistration.Status == ContestRegistrationStatus.Completed && dbRedistration.RegistrationDateTime == null)
            {
                dbRedistration.RegistrationDateTime = Extensions.SfuServerNow;
                dbRedistration.RegistredBy = await _userManager.GetUserAsync(User);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dbRedistration.ContestId });
        }

        [Authorize(Roles = Roles.Admin)]
        public FileResult ExportParticipants(int id)
        {
            var registrations = _context.TeamContestRegistrations
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace)
                .Include(r => r.StudyPlace.City)
                .Include(r => r.StudyPlace.City.Region)
                .Include(r => r.Participant1)
                .Include(r => r.Participant2)
                .Include(r => r.Participant3)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == id)
                .OrderBy(r => r.Number);

            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Participants");
            worksheet.Cells["A1"].Value = "Area";
            worksheet.Cells["B1"].Value = "StudyPlace";
            worksheet.Cells["C1"].Value = "TeamName";
            worksheet.Cells["D1"].Value = "Status";

            worksheet.Cells["E1"].Value = "Email1";
            worksheet.Cells["F1"].Value = "Surname1";
            worksheet.Cells["G1"].Value = "Name1";
            worksheet.Cells["H1"].Value = "Patronymic1";

            worksheet.Cells["I1"].Value = "Email2";
            worksheet.Cells["J1"].Value = "Surname2";
            worksheet.Cells["K1"].Value = "Name2";
            worksheet.Cells["L1"].Value = "Patronymic2";

            worksheet.Cells["M1"].Value = "Email3";
            worksheet.Cells["N1"].Value = "Surname3";
            worksheet.Cells["O1"].Value = "Name3";
            worksheet.Cells["P1"].Value = "Patronymic3";

            worksheet.Cells["Q1"].Value = "TrainerEmail";
            worksheet.Cells["R1"].Value = "TrainerSurname";
            worksheet.Cells["S1"].Value = "TrainerName";
            worksheet.Cells["T1"].Value = "TrainerPatronymic";

            worksheet.Cells["U1"].Value = "ManagerEmail";
            worksheet.Cells["V1"].Value = "ManagerSurname";
            worksheet.Cells["W1"].Value = "ManagerName";
            worksheet.Cells["X1"].Value = "ManagerPatronymic";

            worksheet.Cells["Y1"].Value = "Region";
            worksheet.Cells["Z1"].Value = "City";
            worksheet.Cells["AA1"].Value = "YaContestLogin";
            worksheet.Cells["AB1"].Value = "YaContestPassword";

            worksheet.Cells["AC1"].Value = "Number";
            worksheet.Cells["AD1"].Value = "ComputerName";
            worksheet.Cells["AE1"].Value = "ProgrammingLanguage";
            worksheet.Cells["AF1"].Value = "OfficialTeamName";
            worksheet.Cells["AG1"].Value = "DisplayTeamName";
            worksheet.Cells["AH1"].Value = "StudyPlace_FullName";
            worksheet.Cells["AI1"].Value = "StudyPlace_ShortName_En";
            worksheet.Cells["AJ1"].Value = "StudyPlace_FullName_En";
            worksheet.Cells["AK1"].Value = "IsOutOfCompetition";

            worksheet.Cells["AL1"].Value = "FirstName1";
            worksheet.Cells["AM1"].Value = "LastName1";

            worksheet.Cells["AN1"].Value = "FirstName2";
            worksheet.Cells["AO1"].Value = "LastName2";

            worksheet.Cells["AP1"].Value = "FirstName3";
            worksheet.Cells["AQ1"].Value = "LastName3";

            worksheet.Cells["AR1"].Value = "TrainerFirstName";
            worksheet.Cells["AS1"].Value = "TrainerLastName";

            worksheet.Cells["AT1"].Value = "ManagerFirstName";
            worksheet.Cells["AU1"].Value = "ManagerLastName";

            int row = 1;
            foreach (var registration in registrations)
            {
                row++;

                worksheet.Cells[row, 1].Value = registration.ContestArea?.Area.Name;
                worksheet.Cells[row, 2].Value = registration.StudyPlace.ShortName;
                worksheet.Cells[row, 3].Value = registration.TeamName;
                worksheet.Cells[row, 4].Value = registration.Status;

                worksheet.Cells[row, 5].Value = registration.Participant1.Email;
                worksheet.Cells[row, 6].Value = registration.Participant1.Surname;
                worksheet.Cells[row, 7].Value = registration.Participant1.Name;
                worksheet.Cells[row, 8].Value = registration.Participant1.Patronymic;

                worksheet.Cells[row, 9].Value = registration.Participant2.Email;
                worksheet.Cells[row, 10].Value = registration.Participant2.Surname;
                worksheet.Cells[row, 11].Value = registration.Participant2.Name;
                worksheet.Cells[row, 12].Value = registration.Participant2.Patronymic;

                worksheet.Cells[row, 12].Value = registration.Participant3.Email;
                worksheet.Cells[row, 14].Value = registration.Participant3.Surname;
                worksheet.Cells[row, 15].Value = registration.Participant3.Name;
                worksheet.Cells[row, 16].Value = registration.Participant3.Patronymic;

                worksheet.Cells[row, 17].Value = registration.Trainer.Email;
                worksheet.Cells[row, 18].Value = registration.Trainer.Surname;
                worksheet.Cells[row, 19].Value = registration.Trainer.Name;
                worksheet.Cells[row, 20].Value = registration.Trainer.Patronymic;

                if (registration.Manager != null)
                {
                    worksheet.Cells[row, 21].Value = registration.Manager.Email;
                    worksheet.Cells[row, 22].Value = registration.Manager.Surname;
                    worksheet.Cells[row, 23].Value = registration.Manager.Name;
                    worksheet.Cells[row, 24].Value = registration.Manager.Patronymic;
                }

                worksheet.Cells[row, 25].Value = registration.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 26].Value = registration.StudyPlace.City.Name;
                worksheet.Cells[row, 27].Value = registration.YaContestLogin;
                worksheet.Cells[row, 28].Value = registration.YaContestPassword;

                worksheet.Cells[row, 29].Value = registration.Number;
                worksheet.Cells[row, 30].Value = registration.ComputerName;
                worksheet.Cells[row, 31].Value = registration.ProgrammingLanguage;
                worksheet.Cells[row, 32].Value = registration.OfficialTeamName;
                worksheet.Cells[row, 33].Value = registration.DisplayTeamName;
                worksheet.Cells[row, 34].Value = registration.StudyPlace.FullName;
                if (registration.StudyPlace is Institution institution)
                {
                    worksheet.Cells[row, 35].Value = institution.ShortNameEnglish;
                    worksheet.Cells[row, 36].Value = institution.FullNameEnglish;
                }

                worksheet.Cells[row, 37].Value = registration.IsOutOfCompetition;

                if (registration.Contest.ParticipantType == ParticipantType.Student && registration.Contest.IsEnglishLanguage)
                {
                    worksheet.Cells[row, 38].Value = registration.Participant1.FirstName;
                    worksheet.Cells[row, 39].Value = registration.Participant1.LastName;

                    worksheet.Cells[row, 40].Value = registration.Participant2.FirstName;
                    worksheet.Cells[row, 41].Value = registration.Participant2.LastName;

                    worksheet.Cells[row, 42].Value = registration.Participant3.FirstName;
                    worksheet.Cells[row, 43].Value = registration.Participant3.LastName;

                    worksheet.Cells[row, 44].Value = registration.Trainer.FirstName;
                    worksheet.Cells[row, 45].Value = registration.Trainer.LastName;
                    
                    worksheet.Cells[row, 46].Value = registration.Manager?.FirstName;
                    worksheet.Cells[row, 47].Value = registration.Manager?.LastName;

                }
            }

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Participants.xlsx");
        }

        protected override Task<List<ContestRegistration>> GetContestRegistrationsAsync(int id)
        {
            return _context.TeamContestRegistrations
                .Include(r => r.Participant1)
                .Include(r => r.Participant2)
                .Include(r => r.Participant3)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Include(r => r.StudyPlace)
                .Include(r => r.StudyPlace.City)
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == id)
                .Cast<ContestRegistration>()
                .ToListAsync();
        }

        protected override ContestRegistrationViewModel CreateContestRegistrationViewModel()
        {
            return new CreateTeamContestRegistrationViewModel();
        }

        protected override ContestRegistration CreateContestRegistrationForImportFromContest(ContestRegistration registration)
        {
            var source = (TeamContestRegistration) registration;

            var res = new TeamContestRegistration
            {
                Participant2Id = source.Participant2Id,
                Participant3Id = source.Participant3Id,
                TeamName = source.TeamName,
                OfficialTeamName = source.OfficialTeamName,
            };

            return res;
        }
    }
}
