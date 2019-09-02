using System;
using System.Collections.Generic;
using System.Text;

namespace ContestantRegister.Services
{
    //TODO как дойдут руки - отрефакторить через отдельный сервис 
    public static class DateTimeExtensions
    {
        public static DateTime SfuServerNow => DateTime.Now.AddHours(7);

    }
}
