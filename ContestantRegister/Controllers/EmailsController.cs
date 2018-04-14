using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class EmailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Emails
        public async Task<IActionResult> Index(string emailFilter, int? sendedFilter, string messageFilter)
        {
            ViewData["emailFilter"] = emailFilter;
            ViewData["sendedFilter"] = sendedFilter;
            ViewData["messageFilter"] = messageFilter;

            IQueryable<Email> emails = _context.Emails;
            bool filtered = false; 
            if (!string.IsNullOrEmpty(emailFilter))
            {
                filtered = true;
                emails = emails.Where(e => e.Address.Contains(emailFilter));
            }

            if (sendedFilter.HasValue)
            {
                filtered = true;
                var sended = sendedFilter.Value != 0;
                emails = emails.Where(e => e.IsSended == sended);
            }

            if (!string.IsNullOrEmpty(messageFilter))
            {
                filtered = true;
                emails = emails.Where(e => e.Message.Contains(messageFilter));
            }
            
            if (!filtered)
            {
                emails = emails.OrderByDescending(e => e.Id).Take(100);
            }

            return View(emails);
        }
    }
}
