using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Domain;
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
    public class IndividualContestController : ContestControllerBase
    {
        private readonly IContestRegistrationService _contestRegistrationService;
        
        public IndividualContestController(
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
        
        protected override async Task<ContestRegistration> GetContestRegistrationForEditAsync(int registrationId)
        {
            var registration = await _context.ContestRegistrations
                .Include("Contest.ContestAreas.Area")
                .Include(r => r.StudyPlace)
                .Include(r => r.RegistredBy)
                .Include(r => r.Participant1)
                .SingleOrDefaultAsync(r => r.Id == registrationId);

            return registration;
        }

        protected override ContestRegistrationViewModel CreateEditContestRegistrationViewModel()
        {
            return new EditIndividualContestRegistrationViewModel();
        }

        protected override void IniteEditContestRegistrationViewModel(ContestRegistrationViewModel viewModel, ContestRegistration registration)
        {
            base.IniteEditContestRegistrationViewModel(viewModel, registration);

            var individualVM = (EditIndividualContestRegistrationViewModel) viewModel;
            individualVM.ParticipantName =
                $"{registration.Participant1.Name} {registration.Participant1.Surname} ({registration.Participant1.Email})";
        }

        /// <summary>
        /// Метод не меняет состояние регистрации. Если было ожидание подтверждения, то ожидание и остается. Или лучше сразу подтверждать после изменения статуса?
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditRegistration(int id, EditIndividualContestRegistrationViewModel viewModel)
        {
            var dbRedistration = await _context.IndividualContestRegistrations
                .SingleOrDefaultAsync(r => r.Id == id);
            if (dbRedistration == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateEditIndividualContestRegistrationAsync(viewModel, User);
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
                dbRedistration.RegistrationDateTime = DateTimeExtensions.SfuServerNow;
                dbRedistration.RegistredBy = await _userManager.GetUserAsync(User);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dbRedistration.ContestId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, CreateIndividualContestRegistrationViewModel viewModel)
        {
            var contest = await GetContestForRegistration(id);
            if (contest == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateCreateIndividualContestRegistrationAsync(viewModel, User);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            var registration = new IndividualContestRegistration();
            
            _mapper.Map(viewModel, registration);
            
            return await RegisterInternalAsync(viewModel, registration, contest);
        }
        
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ExportParticipants(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            if (contest == null) return NotFound();

            var registrations = await _context.IndividualContestRegistrations
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace.City.Region)
                .Include(r => r.Participant1)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == id)
                .OrderBy(r => r.Number)
                .ToListAsync();

            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Participants");
            worksheet.Cells["A1"].Value = "Email";
            worksheet.Cells["B1"].Value = "Surname";
            worksheet.Cells["C1"].Value = "Name";
            worksheet.Cells["D1"].Value = "Patronymic";
            worksheet.Cells["E1"].Value = "TrainerEmail";
            worksheet.Cells["F1"].Value = "TrainerSurname";
            worksheet.Cells["G1"].Value = "TrainerName";
            worksheet.Cells["H1"].Value = "TrainerPatronymic";
            worksheet.Cells["I1"].Value = "ManagerEmail";
            worksheet.Cells["J1"].Value = "ManagerSurname";
            worksheet.Cells["K1"].Value = "ManagerName";
            worksheet.Cells["L1"].Value = "ManagerPatronymic";
            worksheet.Cells["M1"].Value = "Region";
            worksheet.Cells["N1"].Value = "City";
            worksheet.Cells["O1"].Value = "StudyPlace";
            worksheet.Cells["P1"].Value = "Status";
            worksheet.Cells["Q1"].Value = "YaContestLogin";
            worksheet.Cells["R1"].Value = "YaContestPassword";
            worksheet.Cells["S1"].Value = "Area";
            worksheet.Cells["T1"].Value = "Number";
            worksheet.Cells["U1"].Value = "ComputerName";
            worksheet.Cells["V1"].Value = "ProgrammingLanguage";
            worksheet.Cells["W1"].Value = "DateOfBirth";
            worksheet.Cells["X1"].Value = "Class";
            worksheet.Cells["Y1"].Value = "Course";
            worksheet.Cells["Z1"].Value = "StudentType";
            worksheet.Cells["AA1"].Value = "StudyPlace_FullName";

            int row = 1;
            foreach (var registration in registrations)
            {
                row++;

                worksheet.Cells[row, 1].Value = registration.Participant1.Email;
                worksheet.Cells[row, 2].Value = registration.Participant1.Surname;
                worksheet.Cells[row, 3].Value = registration.Participant1.Name;
                worksheet.Cells[row, 4].Value = registration.Participant1.Patronymic;
                worksheet.Cells[row, 5].Value = registration.Trainer.Email;
                worksheet.Cells[row, 6].Value = registration.Trainer.Surname;
                worksheet.Cells[row, 7].Value = registration.Trainer.Name;
                worksheet.Cells[row, 8].Value = registration.Trainer.Patronymic;

                if (registration.Manager != null)
                {
                    worksheet.Cells[row, 9].Value = registration.Manager.Email;
                    worksheet.Cells[row, 10].Value = registration.Manager.Surname;
                    worksheet.Cells[row, 11].Value = registration.Manager.Name;
                    worksheet.Cells[row, 12].Value = registration.Manager.Patronymic;
                }

                worksheet.Cells[row, 13].Value = registration.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 14].Value = registration.StudyPlace.City.Name;
                worksheet.Cells[row, 15].Value = registration.StudyPlace.ShortName;
                worksheet.Cells[row, 16].Value = registration.Status;
                worksheet.Cells[row, 17].Value = registration.YaContestLogin;
                worksheet.Cells[row, 18].Value = registration.YaContestPassword;
                worksheet.Cells[row, 19].Value = registration.ContestArea?.Area.Name;
                worksheet.Cells[row, 20].Value = registration.Number;
                worksheet.Cells[row, 21].Value = registration.ComputerName;
                worksheet.Cells[row, 22].Value = registration.ProgrammingLanguage;
                worksheet.Cells[row, 23].Value = registration.Participant1.DateOfBirth;
                worksheet.Cells[row, 24].Value = registration.Class;
                worksheet.Cells[row, 25].Value = registration.Course;
                worksheet.Cells[row, 26].Value = registration.StudentType;
                worksheet.Cells[row, 27].Value = registration.StudyPlace.FullName;

            }

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{contest.Name}.xlsx");
        }

        protected override Task<List<ContestRegistration>> GetContestRegistrationsAsync(int id)
        {
            return _context.IndividualContestRegistrations
                .Include(r => r.Participant1)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Include(r => r.StudyPlace.City)
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == id)
                .Cast<ContestRegistration>()
                .ToListAsync();
        }

        protected override ContestRegistrationViewModel CreateContestRegistrationViewModel()
        {
            return new CreateIndividualContestRegistrationViewModel();
        }

        protected override ContestRegistration CreateContestRegistrationForImportFromContest(ContestRegistration registration)
        {
            var source = (IndividualContestRegistration) registration;
            var res = new IndividualContestRegistration
            {
                Class = source.Class,
                Course = source.Course,
                StudentType = source.StudentType
            };
            return res;
        }
    }
}
