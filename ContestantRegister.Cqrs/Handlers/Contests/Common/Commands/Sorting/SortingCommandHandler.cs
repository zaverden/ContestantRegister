using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers
{
    internal class SortingCommandHandler : RepositoryCommandBaseHandler<SortingCommand>
    {
        public SortingCommandHandler(IRepository repository) : base(repository)
        {
        }

        internal class Computer
        {
            public CompClass CompClass { get; set; }

            public int Number { get; set; }
        }

        public override async Task HandleAsync(SortingCommand command)
        {
            var contest = await Repository.Set<Contest>()
                //.Include(x => x.ContestAreas).ThenInclude(y => y.Area)
                //.Include(x => x.ContestRegistrations).ThenInclude(y => y.ContestArea)
                .Include($"{nameof(Contest.ContestAreas)}.{nameof(ContestArea.Area)}")
                .Include($"{nameof(Contest.ContestRegistrations)}.{nameof(ContestRegistration.ContestArea)}")
                .SingleOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null) throw new EntityNotFoundException();

            //нафига?
            //_mapper.Map(contest, viewModel);

            if (command.ViewModel.SelectedCompClassIds == null || !command.ViewModel.SelectedCompClassIds.Any())
            {
                throw new ValidationException(nameof(command.ViewModel.SelectedCompClassIds), "Не выбраны комп. классы");
            }
            var classes = Repository.Set<CompClass>()
                .Where(c => command.ViewModel.SelectedCompClassIds.Contains(c.Id))
                .ToList();
            var registrations = contest.ContestRegistrations
                .Where(r => r.ContestArea.Id == command.ViewModel.SelectedContestAreaId &&
                            r.Status == ContestRegistrationStatus.Completed)
                .ToList();
            var sum = classes.Sum(c => c.CompNumber);
            if (registrations.Count > sum)
            {
                throw new ValidationException(nameof(command.ViewModel.SelectedCompClassIds), $"Недостаточно машин. Выбрано {sum}, необходимо {registrations.Count}");
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

            var contestArea = contest.ContestAreas.Single(ca => ca.Id == command.ViewModel.SelectedContestAreaId);
            contestArea.SortingResults = GetSortingResults(computers);
            contestArea.SortingCompClassIds = string.Join(',', command.ViewModel.SelectedCompClassIds);

            await Repository.SaveChangesAsync();
        }

        private bool IsSortingAcceptable(List<ContestRegistration> registrations, List<Computer> computers)
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

        private string GetSortingResults(List<Computer> computers)
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
    }

    internal static class Extensions
    {
        internal static void Shuffle<T>(this IList<T> list)
        {
            var rnd = new Random();

            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        internal static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
