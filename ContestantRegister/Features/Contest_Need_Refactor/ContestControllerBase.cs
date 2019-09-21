using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Filter;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.InfrastructureServices;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.Contest;
using ContestantRegister.ViewModels.Contest.Registration;
using ContestantRegister.ViewModels.ListItem;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ContestantRegister.Controllers
{
    public abstract class ContestControllerBase : BaseController
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IMapper _mapper;
        protected readonly IEmailSender _emailSender;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly MailOptions _options;
        protected readonly IUserService _userService;

        protected ContestControllerBase(
            ApplicationDbContext context, 
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<MailOptions> options,
            IUserService userService
            )
        {
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _options = options.Value;
            _userService = userService;
        }

        protected abstract Task<List<ContestRegistration>> GetContestRegistrationsAsync(int id);

        public async Task<IActionResult> Details(int id, ContestParticipantFilter filter) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            contest.ContestRegistrations = await GetContestRegistrationsAsync(id);

            ViewData["ParticipantName"] = filter.ParticipantName;
            ViewData["TrainerName"] = filter.TrainerName;
            ViewData["ManagerName"] = filter.ManagerName;
            ViewData["City"] = filter.City;
            ViewData["Area"] = filter.Area;
            ViewData["StudyPlace"] = filter.StudyPlace;
            ViewData["Status"] = filter.Status;

            var user = await _userManager.GetUserAsync(User);
            ICollection<IndividualContestRegistration> userIndividualRegistrations = new List<IndividualContestRegistration>();
            ICollection<TeamContestRegistration> userTeamRegistrations = new List<TeamContestRegistration>();
            if (User.Identity.IsAuthenticated)
            {
                if (contest.ContestType == ContestType.Individual)
                {
                    userIndividualRegistrations = await _context.IndividualContestRegistrations
                        .Where(r => r.ContestId == id &&
                                    (r.Participant1Id == user.Id || r.TrainerId == user.Id || r.ManagerId == user.Id))
                        .ToListAsync();
                }
                else
                {
                    userTeamRegistrations = await _context.TeamContestRegistrations
                        .Where(r => r.ContestId == id &&
                                    (r.Participant1Id == user.Id ||
                                     r.Participant2Id == user.Id ||
                                     r.Participant3Id == user.Id ||
                                     r.TrainerId == user.Id || 
                                     r.ManagerId == user.Id))
                        .ToListAsync();
                }
            }

            IEnumerable<ContestRegistration> contestRegistrations = contest.ContestRegistrations;
                        
            contestRegistrations = contestRegistrations.AutoFilter(filter);

            object viewModel;
            if (contest.ContestType == ContestType.Individual)
            {
                viewModel = new IndividualContestDetailsViewModel
                {
                    Contest = contest,
                    ContestRegistrations = contestRegistrations.Cast<IndividualContestRegistration>().ToList(),
                    UserRegistrations = userIndividualRegistrations.ToList(),
                    ParticipantRegistration = userIndividualRegistrations.SingleOrDefault(r => r.Participant1Id == user.Id),
                };
            }
            else
            {
                viewModel = new TeamContestDetailsViewModel
                {
                    Contest = contest,
                    ContestRegistrations = contestRegistrations.Cast<TeamContestRegistration>().ToList(),
                    UserRegistrations = userTeamRegistrations.ToList(),
                    ParticipantRegistration = userTeamRegistrations.SingleOrDefault(r => (r.Participant1Id == user.Id || r.Participant2Id == user.Id || r.Participant3Id == user.Id) && r.Status == ContestRegistrationStatus.Completed),
                };
            }

            return View(viewModel);
        }

        protected abstract ContestRegistrationViewModel CreateContestRegistrationViewModel();

        [Authorize]
        public async Task<IActionResult> Register(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .Include(x => x.ContestAreas).ThenInclude(y => y.Area)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var viewModel = CreateContestRegistrationViewModel();
            viewModel.ContestName = contest.Name;
            viewModel.ContestId = contest.Id;
            viewModel.ContestTrainerCont = contest.TrainerCount;
            viewModel.ParticipantType = contest.ParticipantType;
            viewModel.IsAreaRequired = contest.IsAreaRequired;
            viewModel.IsProgrammingLanguageNeeded = contest.IsProgrammingLanguageNeeded;
            viewModel.IsOutOfCompetitionAllowed = contest.IsOutOfCompetitionAllowed;
            viewModel.IsEnglishLanguage = contest.IsEnglishLanguage;

            var user = await _userManager.GetUserAsync(User);
            user.StudyPlace = _context.StudyPlaces.Find(user.StudyPlaceId);
            switch (user.UserType)
            {
                case UserType.Trainer:
                    viewModel.TrainerId = user.Id;
                    break;

                case UserType.Pupil:
                    viewModel.Participant1Id = user.Id;                    
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

            await FillViewDataForContestRegistrationAsync(viewModel, contest);

            return View(viewModel);
        }

        protected async Task<IActionResult> RegisterInternalAsync(ContestRegistrationViewModel viewModel, ContestRegistration registration, Contest contest)
        {
            registration.RegistrationDateTime = DateTimeService.SfuServerNow;
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
                string email;
                if (contest.ContestType == ContestType.Individual)
                {
                    var participant = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant1Id);
                    email = participant.Email;
                }
                else // contest.ContestType == ContestType.Team
                {
                    //Нужно ли отправлять email каждому члену команды?
                    var currentUser = await _userManager.GetUserAsync(User);
                    email = currentUser.Email;
                }

                await _emailSender.SendEmailAsync(email,
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

            return RedirectToAction(nameof(Details), new { contest.Id });
        }

        protected virtual async Task<Contest> GetContestForRegistration(int contestId)
        {
            return await _context.Contests
                .Include(x => x.ContestAreas).ThenInclude(y => y.Area)
                .SingleOrDefaultAsync(c => c.Id == contestId);
        }

        [Authorize]
        public async Task<IActionResult> EditRegistration(int id)
        {
            var registration = await GetContestRegistrationForEditAsync(id);

            if (registration == null)
            {
                return NotFound();
            }

            var viewModel = CreateEditContestRegistrationViewModel();
            viewModel.ContestName = registration.Contest.Name;
            viewModel.ContestTrainerCont = registration.Contest.TrainerCount;
            viewModel.IsAreaRequired = registration.Contest.IsAreaRequired;
            viewModel.IsProgrammingLanguageNeeded = registration.Contest.IsProgrammingLanguageNeeded;
            viewModel.IsOutOfCompetitionAllowed = registration.Contest.IsOutOfCompetitionAllowed;
            viewModel.RegistrationId = registration.Id;
            viewModel.ParticipantType = registration.Contest.ParticipantType;
            viewModel.CityId = registration.StudyPlace.CityId;
            viewModel.IsEnglishLanguage = registration.Contest.IsEnglishLanguage;

            IniteEditContestRegistrationViewModel(viewModel, registration);

            _mapper.Map(registration, viewModel);

            viewModel.Status = viewModel.CheckRegistrationStatus();
            
            //Выставлять RegistredBy надо после маппинга, а то шибко умный маппер в поле RegistredByName кладет значение RegistredBy.Name, фамилия и email пропадают
            if (registration.RegistredBy != null)
            {
                viewModel.RegistredByName = $"{registration.RegistredBy.Name} {registration.RegistredBy.Surname} ({registration.RegistredBy.Email})";
            }

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            await FillViewDataForContestRegistrationAsync(viewModel, contest);

            return View(viewModel);
        }

        protected abstract Task<ContestRegistration> GetContestRegistrationForEditAsync(int registrationId);

        protected async Task FillViewDataForContestRegistrationAsync(ContestRegistrationViewModel viewModel, Contest contest)
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", viewModel.CityId);
            var users = await GetListItemsAsync<ApplicationUser, ViewModels.ListItemViewModels.UserListItemViewModel>(_context, _mapper);
            users = users.OrderBy(u => u.DisplayName).ToList();
            var studyPlaces = await GetListItemsAsync<StudyPlace, StudyPlaceDropdownItemViewModel>(_context, _mapper);
            studyPlaces = studyPlaces.OrderBy(sp => sp.ShortName).ToList();
            var creatIndividualVM = viewModel as IndividualContestRegistrationViewModel;
            var creatTeamVM = viewModel as TeamContestRegistrationViewModel;
            
            if (creatIndividualVM != null)
            {
                ViewData["Participant1Id"] = new SelectList(users, "Id", "DisplayName", creatIndividualVM.Participant1Id);
            }

            if (creatTeamVM != null)
            {
                ViewData["Participant1Id"] = new SelectList(users, "Id", "DisplayName", creatTeamVM.Participant1Id);
                ViewData["Participant2Id"] = new SelectList(users, "Id", "DisplayName", creatTeamVM.Participant2Id);
                ViewData["Participant3Id"] = new SelectList(users, "Id", "DisplayName", creatTeamVM.Participant3Id);
                ViewData["ReserveParticipantId"] = new SelectList(users, "Id", "DisplayName", creatTeamVM.ReserveParticipantId);

                if (contest.TrainerCount > 1)
                {
                    ViewData["Trainer2Id"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", creatTeamVM.Trainer2Id);
                }
                if (contest.TrainerCount > 2)
                {
                    ViewData["Trainer3Id"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", creatTeamVM.Trainer3Id);
                }
            }
            ViewData["TrainerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.TrainerId);

            ViewData["ManagerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.ManagerId);
            ViewData["StudyPlaces"] = studyPlaces;
            
            if (contest.IsAreaRequired)
            {
                ViewData["Area"] = new SelectList(contest.ContestAreas.OrderBy(a => a.Area.Name), "Id", "Area.Name", viewModel.ContestAreaId);
            }
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

        

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Sorting(int id, SortingViewModel viewModel)
        {
            var contest = await _context.Contests
                .Include(x => x.ContestAreas).ThenInclude(y => y.Area)
                .Include(x => x.ContestRegistrations).ThenInclude(y => y.ContestArea)
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
                await FillSortingViewDataAsync(contest, viewModel.SelectedContestAreaId, viewModel.SelectedCompClassIds);

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

            while (!IsSortingAcceptable(registrations, computers))
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

            await FillSortingViewDataAsync(contest, viewModel.SelectedContestAreaId, viewModel.SelectedCompClassIds);

            return View(viewModel);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Sorting(int id)
        {
            var contest = await _context.Contests
                .Include(x => x.ContestAreas).ThenInclude(y => y.Area)
                .Include(x => x.ContestRegistrations).ThenInclude(y => y.Participant1)
                .Include(x => x.ContestRegistrations).ThenInclude(y => y.StudyPlace)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (contest == null) return NotFound();

            var viewModel = new SortingViewModel();

            _mapper.Map(contest, viewModel);

            var compClassIds = contest.ContestAreas
                .Where(ca => !string.IsNullOrEmpty(ca.SortingCompClassIds))
                .SelectMany(c => c.SortingCompClassIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();
            await FillSortingViewDataAsync(contest, 0, compClassIds);

            return View(viewModel);
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

                        var newRegistration = CreateContestRegistrationForImportFromContest(registration);
                        newRegistration.Status = ContestRegistrationStatus.NotCompleted;
                        newRegistration.ProgrammingLanguage = registration.ProgrammingLanguage;
                        newRegistration.Participant1Id = registration.Participant1Id;
                        newRegistration.TrainerId = registration.TrainerId;
                        newRegistration.ManagerId = registration.ManagerId;
                        newRegistration.StudyPlaceId = registration.StudyPlaceId;
                        newRegistration.ContestId = id;
                        newRegistration.YaContestLogin = account[0];
                        newRegistration.YaContestPassword = account[1];
                        newRegistration.Number = contest.RegistrationsCount + 1;
                    
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

        protected abstract ContestRegistration CreateContestRegistrationForImportFromContest(ContestRegistration registration);
        
        protected abstract ContestRegistrationViewModel CreateEditContestRegistrationViewModel();

        protected virtual void IniteEditContestRegistrationViewModel(ContestRegistrationViewModel viewModel, ContestRegistration registration)
        {
            
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> CancelRegistration(int id)
        {
            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = ContestRegistrationStatus.NotCompleted;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public IActionResult CompleteRegistration(int id)
        {
            return RedirectToAction(nameof(EditRegistration), new { id = id });
        }

        private async Task FillSortingViewDataAsync(Contest contest, int selectedContestAreaId = 0, int[] selectedCompClassIds = null)
        {
            ViewData["Areas"] = GetListItems<ContestArea, ContestAreaListItemViewModel>(contest.ContestAreas.OrderBy(ca => ca.Area.Name).ToList(), _mapper, selectedContestAreaId);
            ViewData["CompClasses"] = GetListItems<CompClass, CompClassListItemViewModel>(await _context.CompClasses.OrderBy(c => c.Name).ToListAsync(), _mapper, selectedCompClassIds);
        }

        protected string GetSortingResults(List<Computer> computers)
        {
            var classes = computers
                .GroupBy(c => c.CompClass.Name)
                .OrderBy(g => g.Key);

            var sb = new StringBuilder();
            foreach (var compClass in classes)
            {
                sb.AppendLine($"{compClass.Key}: {compClass.Count()} из {compClass.First().CompClass.CompNumber}");
            }
            return sb.ToString();
        }

        protected bool IsSortingAcceptable(List<ContestRegistration> registrations, List<Computer> computers)
        {
            var pairs = new List<(ContestRegistration ContestRegistration, Computer Computer)>();
            for (int i = 0; i < registrations.Count; i++)
            {
                pairs.Add((registrations[i], computers[i]));
            }

            foreach (var studyPlaceGroup in pairs.GroupBy(p => p.ContestRegistration.StudyPlaceId))
            {
                var classes = studyPlaceGroup.Select(el => el.Computer).GroupBy(e => e.CompClass);
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

        [Authorize]
        public async Task<IActionResult> Registration(int id)
        {
            var registration = await _context.ContestRegistrations
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
                .Include(x => x.ContestAreas).ThenInclude(y => y.Area)
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
                var dto = csv.GetRecord<ContestRegistrationDTO>();
                if (string.IsNullOrEmpty(dto.YaContestLogin)) continue;
                var registration = await _context.ContestRegistrations.SingleOrDefaultAsync
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

    }
}
