using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Historial_C.Data;
using Historial_C.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Historial_C.Controllers
{
    [Authorize]
    public class EpicrisisController : Controller
    {
        private readonly HistorialContext _context;

        public EpicrisisController(HistorialContext context)
        {
            _context = context;
        }

        // GET: Epicrisis
        public async Task<IActionResult> Index()
        {
              return View(await _context.Epicrisis.ToListAsync());
        }

        // GET: Epicrisis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Epicrisis == null)
            {
                return NotFound();
            }

            var epicrisis = await _context.Epicrisis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (epicrisis == null)
            {
                return NotFound();
            }

            return View(epicrisis);
        }



        // GET: Epicrisis/Create
        [Authorize(Roles = "Medico,Empleado")]
        public IActionResult Create(int? episodioId)
        {
            if (episodioId == null)
            {
                return NotFound();
            }
            Epicrisis epicrisis = new Epicrisis() { EpisodioId = episodioId.Value };
            return View(epicrisis);
        }

        // POST: Epicrisis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicoId,EpisodioId")] Epicrisis epicrisis)
        {
            if (ModelState.IsValid)
            {
                 
                epicrisis.MedicoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                _context.Add(epicrisis);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = epicrisis.Id });
            }
            return View(epicrisis);
        }

       
        private bool EpicrisisExists(int id)
        {
          return _context.Epicrisis.Any(e => e.Id == id);
        }

      
    }
}
