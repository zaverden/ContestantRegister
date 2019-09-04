using FluentScheduler;

namespace ContestantRegister.BackgroundJobs
{
    public class FluentSchedulerRegistry : Registry
    {
        public FluentSchedulerRegistry()
        {
#if !DEBUG
            Schedule<EmailJob>().NonReentrant().ToRunEvery(15).Seconds();

            Schedule<ContestStatusJob>().NonReentrant().ToRunEvery(5).Minutes();
#endif
        }
    }
}
