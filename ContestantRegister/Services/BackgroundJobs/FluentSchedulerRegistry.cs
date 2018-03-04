using FluentScheduler;

namespace ContestantRegister.Services.BackgroundJobs
{
    public class FluentSchedulerRegistry : Registry
    {
        public FluentSchedulerRegistry()
        {
            Schedule<EmailJob>().NonReentrant().ToRunEvery(1).Minutes();

            Schedule<ContestStatusJob>().NonReentrant().ToRunEvery(5).Minutes();
        }
    }
}
