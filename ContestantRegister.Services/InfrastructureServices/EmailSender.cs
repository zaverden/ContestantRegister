using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Services.InfrastructureServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Подтвердите email на сайте олимпиад ИКИТ СФУ",
                $"Подтвердите ваш email на сайте олимпиад ИКИТ СФУ olimp.ikit.sfu-kras.ru, кликнув по ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>ссылка</a>");
        }
    }
}
