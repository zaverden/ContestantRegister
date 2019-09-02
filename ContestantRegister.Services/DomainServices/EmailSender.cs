using System;
using System.Threading.Tasks;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Utils;

namespace ContestantRegister.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IRepository _repository;

        public EmailSender(IRepository repository)
        {
            _repository = repository;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = new Models.Email
            {
                Address = email,
                Subject = subject,
                Message = message,
                CreateDate = DateTimeExtensions.SfuServerNow,
            };
            _repository.Add(mail);
            await _repository.SaveChangesAsync();
        }
    }
}
