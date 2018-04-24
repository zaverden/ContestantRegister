﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ListItemViewModels;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace ContestantRegister.Controllers
{
    public class ContestParticipantFilter
    {
        public string ParticipantName { get; set; }
        public string TrainerName { get; set; }
        public string ManagerName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public string StudyPlace { get; set; }
    }

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly IUserService _userService;
        private readonly MailOptions _options;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<MailOptions> options,
            IContestRegistrationService contestRegistrationService,
            IUserService userService)
        { 
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _contestRegistrationService = contestRegistrationService;
            _userService = userService;
            _options = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = _context.Contests.Where(c => !c.IsArchive);

            return View(await actualContests.OrderBy(item => item.Id).ToListAsync());
        }

        public async Task<IActionResult> Archive()
        {
            var archiveContests = _context.Contests.Where(c => c.IsArchive);

            return View(await archiveContests.OrderByDescending(item => item.Id).ToListAsync());
        }

        public async Task<IActionResult> Details(int id, ContestParticipantFilter filter) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .Include(c => c.ContestRegistrations)
                .Include("ContestRegistrations.Participant1")
                .Include("ContestRegistrations.Trainer")
                .Include("ContestRegistrations.Manager")
                .Include("ContestRegistrations.StudyPlace")
                .Include("ContestRegistrations.StudyPlace.City")
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            ViewData["ParticipantName"] = filter.ParticipantName;
            ViewData["TrainerName"] = filter.TrainerName;
            ViewData["ManagerName"] = filter.ManagerName;
            ViewData["City"] = filter.City;
            ViewData["Area"] = filter.Area;
            ViewData["StudyPlace"] = filter.StudyPlace;
            ViewData["Status"] = filter.Status;

            var user = await _userManager.GetUserAsync(User);
            ICollection<ContestRegistration> userRegistrations = new List<ContestRegistration>();
            if (User.Identity.IsAuthenticated)
            {
                userRegistrations = await _context.ContestRegistrations
                    .Where(r => r.ContestId == id &&
                                (r.Participant1Id == user.Id || r.TrainerId == user.Id || r.ManagerId == user.Id))
                    .ToListAsync();
            }

            IEnumerable<ContestRegistration> contestRegistrations = contest.ContestRegistrations;
            if (!string.IsNullOrEmpty(filter.ParticipantName))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.Participant1.Surname.IndexOf(filter.ParticipantName, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.TrainerName))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.Trainer.Surname.IndexOf(filter.TrainerName, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.ManagerName))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.Manager != null && 
                                r.Manager.Surname.IndexOf(filter.ManagerName, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.Area))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => !string.IsNullOrEmpty(r.Area) && 
                                r.Area.IndexOf(filter.Area, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.City))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.StudyPlace.City.Name.IndexOf(filter.City, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.StudyPlace))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.StudyPlace.ShortName.IndexOf(filter.StudyPlace, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                var types = Enum.GetValues(typeof(ContestRegistrationStatus))
                    .Cast<ContestRegistrationStatus>()
                    .Where(type => HtmlHelperExtensions.GetDisplayName(type).Contains(filter.Status))
                    .ToList();

                if (types.Count == 1)
                {
                    contestRegistrations = contestRegistrations.Where(r => r.Status == types.First());
                }
            }

            var viewModel = new IndividualContestDetailsViewModel
            {
                Contest = contest,
                ContestRegistrations = contestRegistrations.ToList(),
                UserRegistrations = userRegistrations,
                ParticipantRegistration = userRegistrations.SingleOrDefault(r => r.Participant1Id == user.Id),
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
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

        [Authorize]
        public async Task<IActionResult> Register(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (contest.ContestType == ContestType.Collegiate) throw new NotImplementedException();

            var viewModel = new CreateIndividualContestRegistrationViewModel
            {
                ContestName = contest.Name,
                ContestId = contest.Id,
                ParticipantType = contest.ParticipantType,
                IsAreaRequired = contest.IsAreaRequired,
                IsProgrammingLanguageNeeded = contest.IsProgrammingLanguageNeeded,
            };

            var user = await _userManager.GetUserAsync(User);
            user.StudyPlace = _context.StudyPlaces.Find(user.StudyPlaceId);
            switch (user.UserType)
            {
                case UserType.Trainer:
                    viewModel.TrainerId = user.Id;
                    break;

                case UserType.Pupil:
                    if (contest.ParticipantType == ParticipantType.Pupil)
                    {
                        viewModel.Participant1Id = user.Id;
                    }
                    else //Школьник пытается зарегаться на студенческое соревнование, этого делать нельзя
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    break;

                case UserType.Student:
                    if (contest.ParticipantType == ParticipantType.Pupil)
                    {
                        viewModel.TrainerId = user.Id;
                    }
                    else
                    {
                        viewModel.Participant1Id = user.Id;
                    }
                    break;
            }

            if (contest.ParticipantType == ParticipantType.Pupil && user.StudyPlace is School ||
                contest.ParticipantType == ParticipantType.Student && user.StudyPlace is Institution)
            {
                viewModel.StudyPlaceId = user.StudyPlaceId;
                viewModel.CityId = user.StudyPlace.CityId;
            }

            await FillViewDataForIndividualContestRegistration(viewModel, contest);

            return View(viewModel);
        }

        private async Task<Contest> GetContest(int contestId)
        {
            return await _context.Contests.SingleOrDefaultAsync(c => c.Id == contestId);
        }

        private async Task FillViewDataForIndividualContestRegistration(IndividualContestRegistrationViewModel viewModel, Contest contest)
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", viewModel.CityId);
            var users = await GetListItemsAsync<ApplicationUser, UserListItemViewModel>(_context, _mapper);
            users = users.OrderBy(u => u.DisplayName).ToList();
            var createVM = viewModel as CreateIndividualContestRegistrationViewModel;

            if (viewModel.ParticipantType == ParticipantType.Pupil)
            {
                if (createVM != null)
                {
                    ViewData["Participant1Id"] = new SelectList(users.Where(u => u.UserType == UserType.Pupil), "Id", "DisplayName", createVM.Participant1Id);
                }
                ViewData["TrainerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.ManagerId);
                var schools = await GetListItemsAsync<School, StudyPlaceListItemViewModel>(_context, _mapper);
                ViewData["StudyPlaces"] = schools.OrderBy(s => s.ShortName);
            }
            else
            {
                if (createVM != null)
                {
                    ViewData["Participant1Id"] = new SelectList(users.Where(u => u.UserType == UserType.Student), "Id", "DisplayName", createVM.Participant1Id);
                }
                ViewData["TrainerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.ManagerId);
                var institutions = await GetListItemsAsync<Institution, StudyPlaceListItemViewModel>(_context, _mapper);
                ViewData["StudyPlaces"] = institutions.OrderBy(inst => inst.ShortName);
            }

            if (contest.IsAreaRequired)
            {
                var areas = contest.Areas.SplitByNewLineEndAndRemoveWindowsLineEnds();
                Array.Sort(areas);
                ViewData["Area"] = new SelectList(areas);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, CreateIndividualContestRegistrationViewModel viewModel)
        {
            var contest = await GetContest(id);
            if (contest == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateCreateContestRegistrationAsync(viewModel, User);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForIndividualContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            var registration = new IndividualContestRegistration();

            _mapper.Map(viewModel, registration);
            registration.RegistrationDateTime = Extensions.SfuServerNow;
            registration.RegistredBy = await _userManager.GetUserAsync(User);
            registration.Status = ContestRegistrationStatus.Completed;

            var yacontestaccount = contest.YaContestAccountsCSV
                .SplitByNewLineEndAndRemoveWindowsLineEnds()
                .Skip(contest.UsedAccountsCount)
                .First()
                .Split(',');

            registration.YaContestLogin = yacontestaccount[0];
            registration.YaContestPassword = yacontestaccount[1];
            registration.Number = contest.RegistrationsCount + 1;

            contest.RegistrationsCount++;
            contest.UsedAccountsCount++;

            _context.ContestRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            
            //TODO Если регистрирует админ, то email не отправляется?
            if (contest.SendRegistrationEmail)
            {
                var participant = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant1Id);
                
                await _emailSender.SendEmailAsync(participant.Email,
                    "Вы зарегистрированы на соревнование по программированию ИКИТ СФУ",
                    $"Вы успешно зарегистрированы на соревнование: {contest.Name}<br>" +
                    $"Ваши учетные данные для входа в систему:<br>" +
                    $"логин {registration.YaContestLogin}<br>" +
                    $"пароль {registration.YaContestPassword}<br>" +
                    $"cсылка для входа: {contest.YaContestLink}<br>");
            }

            if (contest.ShowRegistrationInfo)
            {
                //TODO стоит ли показывать эту страницу для тренера?
                return RedirectToAction(nameof(Registration), new {registration.Id});
            }

            return RedirectToAction(nameof(Details), new {id});
        }

        [Authorize(Roles = Roles.Admin)]
        public FileResult ExportParticipants(int id)
        {
            var registrations = _context.IndividualContestRegistrations
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace)
                .Include(r => r.StudyPlace.City)
                .Include(r => r.StudyPlace.City.Region)
                .Include(r => r.Participant1)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Where(r => r.ContestId == id);

            var package = new ExcelPackage();
            
            var worksheet = package.Workbook.Worksheets.Add("Participants");
            worksheet.Cells["A1"].Value = "Email";
            worksheet.Cells["B1"].Value = "Name";
            worksheet.Cells["C1"].Value = "Surname";
            worksheet.Cells["D1"].Value = "Patronymic";
            worksheet.Cells["E1"].Value = "TrainerEmail";
            worksheet.Cells["F1"].Value = "TrainerName";
            worksheet.Cells["G1"].Value = "ManagerEmail";
            worksheet.Cells["H1"].Value = "ManagerName";
            worksheet.Cells["I1"].Value = "Region";
            worksheet.Cells["J1"].Value = "City";
            worksheet.Cells["K1"].Value = "StudyPlace";
            worksheet.Cells["L1"].Value = "Status";
            worksheet.Cells["M1"].Value = "YaContestLogin";
            worksheet.Cells["N1"].Value = "YaContestPassword";
            worksheet.Cells["O1"].Value = "Area";
            worksheet.Cells["P1"].Value = "Number";
            worksheet.Cells["Q1"].Value = "ComputerName";
            worksheet.Cells["R1"].Value = "ProgrammingLanguage";

            int row = 1;
            foreach (var registration in registrations)
            {
                row++;

                worksheet.Cells[row, 1].Value = registration.Participant1.Email;
                worksheet.Cells[row, 2].Value = registration.Participant1.Name;
                worksheet.Cells[row, 3].Value = registration.Participant1.Surname;
                worksheet.Cells[row, 4].Value = registration.Participant1.Patronymic;
                worksheet.Cells[row, 5].Value = registration.Trainer.Email;
                worksheet.Cells[row, 6].Value = $"{registration.Trainer.Surname} {registration.Trainer.Name} {registration.Trainer.Patronymic}";
                    
                if (registration.Manager != null)
                {
                    worksheet.Cells[row, 7].Value = registration.Manager.Email;
                    worksheet.Cells[row, 8].Value = $"{registration.Manager.Surname} {registration.Manager.Name} {registration.Manager.Patronymic}";
                }

                worksheet.Cells[row, 9].Value = registration.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 10].Value = registration.StudyPlace.City.Name;
                worksheet.Cells[row, 11].Value = registration.StudyPlace.ShortName;
                worksheet.Cells[row, 12].Value = registration.Status;
                worksheet.Cells[row, 13].Value = registration.YaContestLogin;
                worksheet.Cells[row, 14].Value = registration.YaContestPassword;
                worksheet.Cells[row, 15].Value = registration.Area;
                worksheet.Cells[row, 16].Value = registration.Number;
                worksheet.Cells[row, 17].Value = registration.ComputerName;
                worksheet.Cells[row, 18].Value = registration.ProgrammingLanguage;
            }

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Participants.xlsx");
        }

        [Authorize]
        public async Task<IActionResult> Registration(int id)
        {
            var registration = await _context.IndividualContestRegistrations
                .Include(r => r.Contest)
                .SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ImportParticipants(int id)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var vm = new ImportParticipantsViewModel
            {
                ContestName = contest.Name
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ImportParticipants(int id, ImportParticipantsViewModel viewModel)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var sr = new StringReader(viewModel.Data);
            var csv = new CsvReader(sr);
            csv.Configuration.MissingFieldFound = null;
            csv.Read();
            csv.ReadHeader();
            while(csv.Read())
            {
                var dto = csv.GetRecord<IndividualRegistrationDTO>();
                if (string.IsNullOrEmpty(dto.YaContestLogin)) continue;
                var registration = await _context.IndividualContestRegistrations.SingleOrDefaultAsync
                    (r => r.ContestId == id && r.YaContestLogin == dto.YaContestLogin);

                if (registration == null) continue;

                _mapper.Map(dto, registration);

                if (dto.Number.HasValue)
                {
                    registration.Number = dto.Number.Value;
                }

                if (!string.IsNullOrEmpty(dto.Status))
                {
                    if (Enum.TryParse<ContestRegistrationStatus>(dto.Status, out var status))
                    {
                        registration.Status = status;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new {id});
        }

        [Authorize]
        public IActionResult RegisterStudent(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Student });
        }

        [Authorize]
        public IActionResult RegisterPupil(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Pupil });
        }

        [Authorize]
        public IActionResult RegisterTrainer(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Trainer });
        }

        [Authorize]
        public async Task<IActionResult> RegisterContestParticipant(int contestId, UserType userType)
        {
            var vm = new RegisterContestParticipantViewModel
            {
                ContestId = contestId,
                UserType = userType,
                IsUserTypeDisabled = true
            };


            await FillRegisterContestParticipantViewData();

            return View(vm);
        }

        private async Task FillRegisterContestParticipantViewData()
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name");
            var studyPlaces = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["StudyPlaces"] = studyPlaces.OrderBy(s => s.ShortName);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterContestParticipant(RegisterContestParticipantViewModel viewModel)
        {
            var validationResult = await _userService.ValidateUserAsync(viewModel);
            validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    RegistrationDateTime = Extensions.SfuServerNow,
                    RegistredBy = await _userManager.GetUserAsync(User)
                };

                _mapper.Map(viewModel, user);

                var result = await _userManager.CreateAsync(user, viewModel.Email);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(viewModel.Email, callbackUrl);

                    return RedirectToAction(nameof(Register), new { id = viewModel.ContestId });
                }

                ModelState.AddErrors(result.Errors);
            }

            await FillRegisterContestParticipantViewData();

            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(exceptionFeature.Error, $"Unhandled exception at {exceptionFeature.Path}");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> SuggestSchool()
        {
            var viewModel = new SuggectSchoolViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestSchool(SuggectSchoolViewModel viewModel)
        {
            await _emailSender.SendEmailAsync(_options.Email, "Новая школа",
                $"Предложил {viewModel.Email}<br>" +
                $"Краткое название {viewModel.ShortName}<br>" +
                $"Полное название {viewModel.FullName}<br>" +
                $"Регион {viewModel.Region}<br>" +
                $"Город {viewModel.City}<br>" +
                $"Официальный email {viewModel.SchoolEmail}<br>" +
                $"Сайт {viewModel.Site}<br>");

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        public IActionResult StudyPlaceSuggested()
        {
            return View();
        }

        public async Task<IActionResult> SuggestInstitution()
        {
            var viewModel = new SuggectInstitutionViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestInstitution(SuggectInstitutionViewModel viewModel)
        {
            await _emailSender.SendEmailAsync(_options.Email, "Новый вуз",
                $"Предложил {viewModel.Email}<br>" +
                $"Краткое название {viewModel.ShortName}<br>" +
                $"Полное название {viewModel.FullName}<br>" +
                $"Регион {viewModel.Region}<br>" +
                $"Город {viewModel.City}<br>" +
                $"Краткое название англ {viewModel.ShortNameEn}<br>" +
                $"Полное название англ {viewModel.FullNameEn}<br>" +
                $"Сайт {viewModel.Site}<br>");

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        [Authorize]
        public async Task<IActionResult> EditRegistration(int id)
        {
            var registration = await _context.ContestRegistrations
                .OfType<IndividualContestRegistration>()
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace)
                .Include(r => r.RegistredBy)
                .Include(r => r.Participant1)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            var viewModel = new EditIndividualContestRegistrationViewModel
            {
                ContestName = registration.Contest.Name,
                IsAreaRequired = registration.Contest.IsAreaRequired,
                IsProgrammingLanguageNeeded = registration.Contest.IsProgrammingLanguageNeeded,
                RegistrationId = registration.Id,
                ParticipantType = registration.Contest.ParticipantType,
                CityId = registration.StudyPlace.CityId,
                ParticipantName = $"{registration.Participant1.Name} {registration.Participant1.Surname} ({registration.Participant1.Email})"
            };

            _mapper.Map(registration, viewModel);

            //Выставлять RegistredBy надо после маппинга, а то шибко умный маппер в поле RegistredByName кладет значение RegistredBy.Name, фамилия и email пропадают
            if (registration.RegistredBy != null)
            {
                viewModel.RegistredByName = $"{registration.RegistredBy.Name} {registration.RegistredBy.Surname} ({registration.RegistredBy.Email})";
            }

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            await FillViewDataForIndividualContestRegistration(viewModel, contest);

            return View(viewModel);
        }

        /// <summary>
        /// Метод не меняет состояние регистрации. Если было ожидание подтверждения, то ожидание и остается. Или лучше сразу подтверждать после изменения статуса?
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditRegistration(int id, EditIndividualContestRegistrationViewModel viewModel)
        {
            var dbRedistration = await _context.ContestRegistrations
                .OfType<IndividualContestRegistration>()
                .SingleOrDefaultAsync(r => r.Id == id);
            if (dbRedistration == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateEditContestRegistrationAsync(viewModel, User);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

                await FillViewDataForIndividualContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            _mapper.Map(viewModel, dbRedistration);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dbRedistration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> CancelRegistration(int id)
        {
            //TODO На UI переспросить "Вы точно уверены, что хотите отменить регистрацию?"

            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = ContestRegistrationStatus.Canceled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> ConfirmParticipation(int id)
        {
            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = ContestRegistrationStatus.Completed;
            registration.RegistrationDateTime = Extensions.SfuServerNow;
            registration.RegistredBy = await _userManager.GetUserAsync(User);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }
    }
}
