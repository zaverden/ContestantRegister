using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using ContestantRegister.Data;
using ContestantRegister.Utils;
using FluentScheduler;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json.Linq;

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
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("X-Secure-Token", _options.SecurityToken);
                    
                    foreach (var email in _context.Emails.Where(e => !e.IsSended && e.SendAttempts < 2))
                    {
                        dynamic message = new JObject();
                        message.from = _options.Email;
                        message.to = new JArray(email.Address);
                        message.subject = email.Subject;
                        message.html_body = email.Message;
                        
                        try
                        {
                            var content = new StringContent(message.ToString(), Encoding.UTF8, "application/json");
                            client.PostAsync("http://api.mailhandler.ru/message/send/", content).Wait();
                            
                            email.IsSended = true;
                        }
                        catch (Exception ex)
                        {
                            email.SendAttempts++;

                            _logger.LogError(ex, $"Unable to send email to {email.Address}");
                        }
                        email.ChangeDate = DateTime.Now;
                    }

                    
                }

                ctx.SaveChanges();
            }
        }
    }
}
