using System;
using System.Linq;
using ContestantRegister.Data;
using FluentScheduler;
using MailKit.Net.Smtp;
using MimeKit;

namespace ContestantRegister.Services.Email
{
    public class EmailJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public EmailJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            using (var ctx = _context)
            {
                var emails = _context.Emails.Where(e => !e.IsSended);

                foreach (var e in emails)
                {
                    //TODO Settings
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Жюри", "acm@sfu-kras.ru"));
                    message.To.Add(new MailboxAddress(e.Address));
                    message.Subject = e.Subject;

                    message.Body = new TextPart("plain")
                    {
                        Text = e.Message
                    };

                    using (var client = new SmtpClient())
                    {
                        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                        //client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        client.Connect("mail.sfu-kras.ru", 465, true);

                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate("acm@sfu-kras.ru", "67ChFylD");

                        try
                        {
                            client.Send(message);
                            client.Disconnect(true);

                            e.IsSended = true;
                        }
                        catch (Exception exception)
                        {
                            //TODO Log
                        }
                    }
                }

                ctx.SaveChangesAsync().Wait();
            }
        }
    }
}
