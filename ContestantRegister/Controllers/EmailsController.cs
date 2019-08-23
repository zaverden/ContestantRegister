using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Utils.Filter;

namespace ContestantRegister.Controllers
{
    public class EmailFilter
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        [PropertyName("Address")]
        public string Email { get; set; }

        [ConvertFilter(typeof(NullableIntToNullableBooleanConverter))]
        [PropertyName("IsSended")]
        public int? Sended { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Message { get; set; }
    }

    [Authorize(Roles = Roles.Admin)]
    public class EmailsController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public EmailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Emails
        public async Task<IActionResult> Index(EmailFilter filter)
        {
            ViewData["Email"] = filter.Email;
            ViewData["Sended"] = filter.Sended;
            ViewData["Message"] = filter.Message;

            IQueryable<Email> emails = _context.Emails;

            emails = emails.AutoFilter(filter);
            
            emails = emails.OrderByDescending(e => e.Id).Take(100);

            return View(await emails.ToListAsync());
        }
    }
}
