using FluentScheduler;

namespace ContestantRegister.Services.Email
{
    public class FluentSchedulerRegistry : Registry
    {
        public FluentSchedulerRegistry()
        {
            Schedule<EmailJob>().NonReentrant().ToRunEvery(1).Minutes();
        }
    }
}
