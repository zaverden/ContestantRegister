using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ContestantRegister.Data;
using FluentScheduler;

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

                foreach (var email in emails)
                {
                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential("acm@sfu-kras.ru", "67ChFylD");
                    
                    client.Host = "mail.sfu-kras.ru";
                    client.Port = 465;
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("acm@sfu-kras.ru", "Жюри");
                    mailMessage.To.Add(email.Address);
                    mailMessage.Body = email.Message;
                    mailMessage.Subject = email.Subject;
                    try
                    {
                        client.Send(mailMessage);
                        email.IsSended = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }

                ctx.SaveChangesAsync().Wait();
            }
        }
    }
}
