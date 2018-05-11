using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CompClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CompClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.CompClasses.Include(c => c.Area).ToListAsync());
        }

        private void FillViewData(CompClass currentItem = null)
        {
             ViewData["Area"] = new SelectList(_context.Areas, "Id", "Name", currentItem?.AreaId);
        }

        // GET: CompClasses/Create
        public IActionResult Create()
        {
            FillViewData();

            return View();
        }

        // POST: CompClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompClass compClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(compClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            FillViewData(compClass);

            return View(compClass);
        }

        // GET: CompClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compClass = await _context.CompClasses.SingleOrDefaultAsync(m => m.Id == id);
            if (compClass == null)
            {
                return NotFound();
            }

            FillViewData(compClass);
            return View(compClass);
        }

        // POST: CompClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompClass compClass)
        {
            if (id != compClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompClassExists(compClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            FillViewData(compClass);
            return View(compClass);
        }

        // GET: CompClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compClass = await _context.CompClasses
                .SingleOrDefaultAsync(m => m.Id == id);
            if (compClass == null)
            {
                return NotFound();
            }

            return View(compClass);
        }

        // POST: CompClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compClass = await _context.CompClasses.SingleOrDefaultAsync(m => m.Id == id);
            _context.CompClasses.Remove(compClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompClassExists(int id)
        {
            return _context.CompClasses.Any(e => e.Id == id);
        }
    }
}
