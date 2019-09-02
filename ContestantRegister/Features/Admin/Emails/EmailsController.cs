using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Admin.Emails.Queries;
using Microsoft.AspNetCore.Mvc;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;
using Microsoft.AspNetCore.Authorization;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class EmailsController : Controller
    {
        private readonly IHandlerDispatcher _handlerDispatcher; 

        public EmailsController(IHandlerDispatcher handlerDispatcher)
        {
            _handlerDispatcher = handlerDispatcher;
        }

        // GET: Emails
        public async Task<IActionResult> Index(GetEmailsQuery query)
        {
            ViewData["Email"] = query.Email;
            ViewData["Sended"] = query.Sended;
            ViewData["Message"] = query.Message;

            var emails = await _handlerDispatcher.ExecuteQueryAsync(new GetEmailsQuery());
            
            return View(emails);
        }
    }
}
