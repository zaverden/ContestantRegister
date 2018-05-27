using System;
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
using ContestantRegister.ViewModels.Home;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ListItem;
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
using Newtonsoft.Json;
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
                .Include("ContestRegistrations.ContestArea.Area")
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
                    .Where(r => r.Participant1.Surname.ContainsIgnoreCase(filter.ParticipantName));
            }
            if (!string.IsNullOrEmpty(filter.TrainerName))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.Trainer.Surname.ContainsIgnoreCase(filter.TrainerName));
            }
            if (!string.IsNullOrEmpty(filter.ManagerName))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.Manager != null &&
                                r.Manager.Surname.ContainsIgnoreCase(filter.ManagerName));
            }
            if (!string.IsNullOrEmpty(filter.Area))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.ContestArea.Area != null &&
                                r.ContestArea.Area.Name.ContainsIgnoreCase(filter.Area));
            }
            if (!string.IsNullOrEmpty(filter.City))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.StudyPlace.City.Name.ContainsIgnoreCase(filter.City));
            }
            if (!string.IsNullOrEmpty(filter.StudyPlace))
            {
                contestRegistrations = contestRegistrations
                    .Where(r => r.StudyPlace.ShortName.ContainsIgnoreCase(filter.StudyPlace));
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                var types = Enum.GetValues(typeof(ContestRegistrationStatus))
                    .Cast<ContestRegistrationStatus>()
                    .Where(type => HtmlHelperExtensions.GetDisplayName(type).ContainsIgnoreCase(filter.Status))
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
                .Include(c => c.ContestAreas)
                .Include("ContestAreas.Area")
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
            return await _context.Contests
                .Include(c => c.ContestAreas)
                .Include("ContestAreas.Area")
                .SingleOrDefaultAsync(c => c.Id == contestId);
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
                ViewData["Area"] = new SelectList(contest.ContestAreas.OrderBy(a => a.Area.Name), "Id", "Area.Name", viewModel.ContestAreaId);
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
                return RedirectToAction(nameof(Registration), new { registration.Id });
            }

            return RedirectToAction(nameof(Details), new { id });
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
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == id)
                .OrderBy(r => r.Number);

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
                worksheet.Cells[row, 25].Value = registration.StudentType;
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
            var contest = await _context.Contests
                .Include(c => c.ContestAreas)
                .Include("ContestAreas.Area")
                .SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var sr = new StringReader(viewModel.Data);
            var csv = new CsvReader(sr);
            csv.Configuration.MissingFieldFound = null;
            if (viewModel.TabDelimeter)
            {
                csv.Configuration.Delimiter = "\t";
            }
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
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

                if (!string.IsNullOrEmpty(dto.Area))
                {
                    var contestArea = contest.ContestAreas.FirstOrDefault(ca => ca.Area.Name == dto.Area);
                    if (contestArea != null)
                    {
                        registration.ContestAreaId = contestArea.Id;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> ImportFromContest(int id)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(contest.YaContestAccountsCSV))
            {
                throw new Exception("В контесте нет логинов-паролей для участников");
            }

            var viewModel = new ImportContestParticipantsViewModel();

            var contests = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                        c.ContestType == contest.ContestType &&
                                                        c.Id != contest.Id);
            ViewData["FromContestId"] = new SelectList(contests, "Id", "Name");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromContest(int id, ImportContestParticipantsViewModel viewModel)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var loginsForImport = viewModel.ParticipantYaContestLogins.SplitByNewLineEndAndRemoveWindowsLineEnds().ToHashSet();
                var accounts = contest.YaContestAccountsCSV.SplitByNewLineEndAndRemoveWindowsLineEnds();
                var registrations = _context.ContestRegistrations.Where(r => r.ContestId == viewModel.FromContestId);
                foreach (var registration in registrations)
                {
                    if (loginsForImport.Contains(registration.YaContestLogin))
                    {
                        if (contest.UsedAccountsCount == accounts.Length)
                        {
                            ModelState.AddModelError(string.Empty, "В контесте, в который импортируются участники, не хватает яконтест аккаунтов для завершения импорта");
                            var contests1 = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                                        c.ContestType == contest.ContestType &&
                                                                        c.Id != contest.Id);
                            ViewData["FromContestId"] = new SelectList(contests1, "Id", "Name", viewModel.FromContestId);
                            return View(viewModel);
                        }

                        var account = accounts[contest.UsedAccountsCount].Split(',');
                        //Здесь не нужно выставлять время регистрации и зарегистрировавшего, т.к. эти данные подставляются при подтверждении регистрации
                        var newRegistration = new IndividualContestRegistration
                        {
                            Status = ContestRegistrationStatus.ConfirmParticipation,
                            ProgrammingLanguage = registration.ProgrammingLanguage,
                            Participant1Id = registration.Participant1Id,
                            TrainerId = registration.TrainerId,
                            ManagerId = registration.ManagerId,
                            StudyPlaceId = registration.StudyPlaceId,
                            ContestId = id,
                            YaContestLogin = account[0],
                            YaContestPassword = account[1],
                            Number = contest.RegistrationsCount + 1,
                        };
                        contest.UsedAccountsCount++;
                        contest.RegistrationsCount++;
                        _context.ContestRegistrations.Add(newRegistration);
                    }
                }

                await _context.SaveChangesAsync();

                //TODO Сказать о том, что участники успешно импортированы

                return RedirectToAction(nameof(Details), new { id });
            }

            var contests = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                        c.ContestType == contest.ContestType &&
                                                        c.Id != contest.Id);
            ViewData["FromContestId"] = new SelectList(contests, "Id", "Name", viewModel.FromContestId);
            return View(viewModel);
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
                .Include(r => r.Contest.ContestAreas)
                .Include("Contest.ContestAreas.Area")
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

                var contest = await GetContest(viewModel.ContestId);

                await FillViewDataForIndividualContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            _mapper.Map(viewModel, dbRedistration);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dbRedistration.ContestId });
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            //TODO На UI переспросить "Вы точно уверены, что хотите удалить регистрацию?"

            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            _context.ContestRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> CancelRegistration(int id)
        {
            //TODO На UI переспросить "Вы точно уверены, что хотите отменить регистрацию?"

            return await ChangeRegistrationStatus(id, ContestRegistrationStatus.Canceled);
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> ConfirmParticipation(int id)
        {
            return await ChangeRegistrationStatus(id, ContestRegistrationStatus.ConfirmParticipation, OnConfirmParticipation);
        }

        private async Task OnConfirmParticipation(ContestRegistration registration)
        {
            registration.RegistrationDateTime = Extensions.SfuServerNow;
            registration.RegistredBy = await _userManager.GetUserAsync(User);
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> StatusToConfirmParticipation(int id)
        {
            return await ChangeRegistrationStatus(id, ContestRegistrationStatus.ConfirmParticipation);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Sorting(int id)
        {
            var contest = await _context.Contests
                .Include(c => c.ContestAreas)
                .Include("ContestAreas.Area")
                .Include(c => c.ContestRegistrations)
                .Include("ContestRegistrations.Participant1")
                .Include("ContestRegistrations.StudyPlace")
                .SingleOrDefaultAsync(m => m.Id == id);

            if (contest == null) return NotFound();

            var viewModel = new SortingViewModel();

            _mapper.Map(contest, viewModel);

            var compClassIds = contest.ContestAreas
                .Where(ca => !string.IsNullOrEmpty(ca.SortingCompClassIds))
                .SelectMany(c => c.SortingCompClassIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();
            await FillSortingViewData(contest, 0, compClassIds);

            return View(viewModel);
        }

        private async Task FillSortingViewData(Contest contest, int selectedContestAreaId = 0, int[] selectedCompClassIds = null)
        {
            ViewData["Areas"] = GetListItems<ContestArea, ContestAreaListItemViewModel>(contest.ContestAreas.OrderBy(ca => ca.Area.Name), _mapper, selectedContestAreaId);
            ViewData["CompClasses"] = GetListItems<CompClass, CompClassListItemViewModel>(_context.CompClasses.OrderBy(c => c.Name), _mapper, selectedCompClassIds);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Sorting(int id, SortingViewModel viewModel)
        {
            var contest = await _context.Contests
                .Include("ContestAreas.Area")
                .Include("ContestRegistrations.ContestArea")
                .SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null) return NotFound();

            _mapper.Map(contest, viewModel);

            if (viewModel.SelectedCompClassIds == null || !viewModel.SelectedCompClassIds.Any())
            {
                ModelState.AddModelError(nameof(viewModel.SelectedCompClassIds), "Не выбраны комп. классы");
            }
            var classes = _context.CompClasses
                .Where(c => viewModel.SelectedCompClassIds.Contains(c.Id))
                .ToList();
            var registrations = contest.ContestRegistrations
                .Where(r => r.ContestArea.Id == viewModel.SelectedContestAreaId &&
                            r.Status == ContestRegistrationStatus.Completed)
                .ToList();
            var sum = classes.Sum(c => c.CompNumber);
            if (registrations.Count > sum)
            {
                ModelState.AddModelError(nameof(viewModel.SelectedCompClassIds), $"Недостаточно машин. Выбрано {sum}, необходимо {registrations.Count}");
            }
            if (!ModelState.IsValid)
            {
                await FillSortingViewData(contest, viewModel.SelectedContestAreaId, viewModel.SelectedCompClassIds);

                return View(viewModel);
            }

            var computers = new List<Computer>();
            foreach (var compClass in classes)
            {
                for (int i = 1; i <= compClass.CompNumber; i++)
                {
                    computers.Add(new Computer { Number = i, CompClass = compClass });
                }
            }
            computers = computers.OrderBy(c => c.Number).ToList();
            computers.RemoveRange(registrations.Count, computers.Count - registrations.Count);

            while (!Ok(registrations, computers))
            {
                computers.Shuffle();
            }

            for (int i = 0; i < registrations.Count; i++)
            {
                registrations[i].ComputerName = $"{computers[i].CompClass.Name}-{computers[i].Number}";
            }

            var contestArea = contest.ContestAreas.Single(ca => ca.Id == viewModel.SelectedContestAreaId);
            contestArea.SortingResults = GetSortingResults(computers);
            contestArea.SortingCompClassIds = string.Join(',', viewModel.SelectedCompClassIds);

            await _context.SaveChangesAsync();

            await FillSortingViewData(contest, viewModel.SelectedContestAreaId, viewModel.SelectedCompClassIds);

            return View(viewModel); 
        }

        private string GetSortingResults(List<Computer> computers)
        {
            var classes = computers
                .GroupBy(c => c.CompClass.Name)
                .OrderBy(g => g.Key);                

            var sb = new StringBuilder();
            foreach(var compClass in classes)
            {
                sb.AppendLine($"{compClass.Key}: {compClass.Count()} из {compClass.First().CompClass.CompNumber}");
            }
            return sb.ToString();
        }

        private bool Ok(List<ContestRegistration> registrations, List<Computer> computers)
        {
            var pairs = new List<Tuple<ContestRegistration, Computer>>();
            for (int i = 0; i < registrations.Count; i++)
            {
                pairs.Add(Tuple.Create(registrations[i], computers[i]));
            }

            foreach (var studyPlaceGroup in pairs.GroupBy(p => p.Item1.StudyPlaceId))
            {
                var classes = studyPlaceGroup.Select(el => el.Item2).GroupBy(e => e.CompClass);
                foreach (var classGroup in classes)
                {
                    var numbers = classGroup.OrderBy(el => el.Number).Select(el => el.Number).ToList();
                    for (int i = 1; i < numbers.Count - 1; i++)
                    {
                        if (numbers[i] + 1 == numbers[i + 1])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        class Computer
        {
            public CompClass CompClass { get; set; }

            public int Number { get; set; }
        }

        private async Task<IActionResult> ChangeRegistrationStatus(int registrationId, ContestRegistrationStatus status, Func<ContestRegistration, Task> onConfirmParticipation = null)
        {
            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == registrationId);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = status;

            if (onConfirmParticipation != null)
            {
                await onConfirmParticipation(registration);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }
    }
}
