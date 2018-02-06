using System.Threading.Tasks;
using ContestantRegister.Data;

namespace ContestantRegister.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ApplicationDbContext _context;

        public EmailSender(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = new Models.Email
            {
                Address = email,
                Subject = subject,
                Message = message
            };
            _context.Emails.Add(mail);
            await _context.SaveChangesAsync();
        }
    }
}
