using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers
{
    // TODO: [Authorize]
    public class InstitutionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        public InstitutionsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Institutions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Institutions.Include(i => i.City);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Institutions/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            return View();
        }

        // POST: Institutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortNameEnglish,FullNameEnglish,BaylorLink,CityId,ShortName,FullName,Site,Id")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(institution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", institution.CityId);
            return View(institution);
        }

        // GET: Institutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institution = await _context.Institutions.SingleOrDefaultAsync(m => m.Id == id);
            if (institution == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", institution.CityId);
            return View(institution);
        }

        // POST: Institutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShortNameEnglish,FullNameEnglish,BaylorLink,CityId,ShortName,FullName,Site,Id")] Institution institution)
        {
            if (id != institution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dbInstitution = await _context.Institutions.SingleOrDefaultAsync(x => x.Id == id);
                if (dbInstitution == null)
                {
                    return NotFound();
                }

                _mapper.Map(institution, dbInstitution);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", institution.CityId);
            return View(institution);
        }

        // GET: Institutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institution = await _context.Institutions
                .Include(i => i.City)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (institution == null)
            {
                return NotFound();
            }

            return View(institution);
        }

        // POST: Institutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var institution = await _context.Institutions.SingleOrDefaultAsync(m => m.Id == id);
            _context.Institutions.Remove(institution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
