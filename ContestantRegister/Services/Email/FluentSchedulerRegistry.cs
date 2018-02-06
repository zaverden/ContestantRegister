using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;

namespace ContestantRegister.Services.Email
{
    public class FluentSchedulerRegistry : Registry
    {
        public FluentSchedulerRegistry()
        {
            Schedule<EmailJob>().NonReentrant().ToRunEvery(5).Seconds();
        }
    }
}
