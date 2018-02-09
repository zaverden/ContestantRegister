using System;
using System.Linq;
using ContestantRegister.Data;
using FluentScheduler;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ContestantRegister.Services.Email
{
    public class EmailJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfigurationSection _configuration;

        public EmailJob(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration.GetSection("SendEmail");
        }

        public void Execute()
        {
            using (var ctx = _context)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(_configuration["Server"], _configuration.GetValue<int>("Port"), _configuration.GetValue<bool>("UseSsl"));
                    client.Authenticate(_configuration["Email"], _configuration["Password"]);

                    foreach (var email in _context.Emails.Where(e => !e.IsSended))
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(_configuration["FromName"], _configuration["Email"]));
                        message.To.Add(new MailboxAddress(email.Address));
                        message.Subject = email.Subject;
                        message.Body = new TextPart("plain") { Text = email.Message };

                        try
                        {
                            client.Send(message);
                            email.IsSended = true;
                            //TODO Коммит можно делать после каждой успешной отправки email. Но тогда список email нужно будет выбирать заранее по ToList
                        }
                        catch (Exception ex)
                        {
                            //TODO Log
                        }
                    }

                    client.Disconnect(true);
                }

                ctx.SaveChangesAsync().Wait();
            }
        }
    }
}
