using System;
using System.Linq;
using ContestantRegister.Data;
using ContestantRegister.Models;
using FluentScheduler;

namespace ContestantRegister.Services.BackgroundJobs
{
    public class ContestStatusJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public ContestStatusJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            using (_context)
            {
                var contests = _context.Contests.Where(c => c.RegistrationEnd < DateTime.Now &&
                                                            (c.ContestStatus == ContestStatus.RegistrationOpened ||
                                                             c.ContestStatus == ContestStatus.ConfirmParticipation ||
                                                             c.ContestStatus == ContestStatus.ConfirmParticipationOrRegister));
                foreach (var contest in contests)
                {
                    contest.ContestStatus = ContestStatus.RegistrationClosed;
                }

                _context.SaveChanges();
            }
        }
    }
}
