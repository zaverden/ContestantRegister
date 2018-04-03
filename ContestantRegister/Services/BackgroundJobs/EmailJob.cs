using System;
using System.Linq;
using ContestantRegister.Data;
using ContestantRegister.Utils;
using FluentScheduler;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace ContestantRegister.Services.BackgroundJobs
{
    public class EmailJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailJob> _logger;
        private readonly MailOptions _options;

        public EmailJob(ApplicationDbContext context, IOptions<MailOptions> options, ILogger<EmailJob> logger)
        {
            _context = context;
            _logger = logger;
            _options = options.Value;
        }

        public void Execute()
        {
            using (var ctx = _context)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(_options.Server, _options.Port, _options.UseSsl);
                    if (!string.IsNullOrEmpty(_options.Password))
                    {
                        client.Authenticate(_options.Email, _options.Password);
                    }

                    foreach (var email in _context.Emails.Where(e => !e.IsSended && e.SendAttempts < 2))
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(_options.FromName, _options.Email));
                        message.To.Add(new MailboxAddress(email.Address));
                        message.Subject = email.Subject;
                        message.Body = new TextPart(TextFormat.Html) { Text = email.Message };

                        try
                        {
                            client.Send(message);
                            email.IsSended = true;
                            //TODO Коммит можно делать после каждой успешной отправки email. Но тогда список email нужно будет выбирать заранее по ToList
                        }
                        catch (Exception ex)
                        {
                            email.SendAttempts++;
                            _logger.LogError(ex, $"Unable to send email to {email.Address}");
                        }
                        email.ChangeDate = DateTime.Now;
                    }

                    client.Disconnect(true);
                }

                ctx.SaveChanges();
            }
        }
    }
}
