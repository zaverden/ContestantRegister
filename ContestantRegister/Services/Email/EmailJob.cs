using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ContestantRegister.Data;
using EASendMail;
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

                foreach (var e in emails)
                {
                    /*
                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    client.EnableSsl = true;

                    client.Credentials = new NetworkCredential("acm@sfu-kras.ru", "67ChFylD");
                    client.Host = "mail.sfu-kras.ru";
                    client.Port = 465;

                    //client.Credentials = new NetworkCredential("isit.open@yandex.ru", "contest");
                    //client.Host = "smtp.yandex.ru";
                    //client.Port = 587;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("acm@sfu-kras.ru", "Жюри");
                    //mailMessage.From = new MailAddress("isit.open@yandex.ru", "Жюри");
                    mailMessage.To.Add(e.Address);
                    mailMessage.Body = e.Message;
                    mailMessage.IsBodyHtml = false;
                    mailMessage.Subject = e.Subject;

                    try
                    {
                        client.Send(mailMessage);
                        e.IsSended = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(e);
                    }*/

                    SmtpServer server = new SmtpServer("mail.sfu-kras.ru", 465);

                    server.User = "acm@sfu-kras.ru";
                    server.Password = "67ChFylD";
                    server.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    server.AuthType = SmtpAuthType.AuthPlain;

                    SmtpMail mail = new SmtpMail("TryIt");
                    mail.Subject = e.Subject;
                    mail.TextBody = e.Message;
                    mail.From = new EASendMail.MailAddress("Жюри", "acm@sfu-kras.ru");
                    mail.To.Add(new EASendMail.MailAddress(e.Address));

                    EASendMail.SmtpClient client = new EASendMail.SmtpClient();

                    try
                    {
                        client.SendMail(server, mail);
                        e.IsSended = true;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(e);
                    }
                    

                }

                ctx.SaveChangesAsync().Wait();
            }
        }
    }
}
