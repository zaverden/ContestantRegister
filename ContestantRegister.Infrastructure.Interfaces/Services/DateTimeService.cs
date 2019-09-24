using System;

namespace ContestantRegister.Services.InfrastructureServices
{
    //TODO как дойдут руки - отрефакторить через отдельный сервис 
    public static class DateTimeService
    {
        public static DateTime SfuServerNow => DateTime.Now.AddHours(7);

    }
}
