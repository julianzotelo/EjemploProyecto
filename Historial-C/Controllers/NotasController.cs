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
    public class NotasController : Controller
    {
        private readonly HistorialContext _context;

        public NotasController(HistorialContext context)
        {
            _context = context;
        }

        // GET: Notas
        public async Task<IActionResult> Index()
        {
            var historialContext = _context.Nota.Include(n => n.Empleado).Include(n => n.Evolucion);
            return View(await historialContext.ToListAsync());
        }

        // GET: Notas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Nota == null)
            {
                return NotFound();
            }

            var nota = await _context.Nota
                .Include(n => n.Empleado)
                .Include(n => n.Evolucion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // GET: Notas/Create
        [Authorize(Roles = "Medico,Empleado")]
        public IActionResult Create(int? evolucionId)
        {
           
            if(evolucionId == null)
            {
                return NotFound();
            }

            Nota nota = new Nota { EvolucionId = evolucionId.Value };
            Evolucion evolucion = _context.Evolucion.FirstOrDefault(e => e.Id == evolucionId.Value);
            var empleado = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

            if (evolucion.MedicoId == empleado)
            {
                if (!evolucion.EstadoAbierto)
                {
                    return View();
                }
                else
                {
                    return Content("Solo se pueden cargar notas cuando la evolución se encuentra cerrada.");
                }
                
            }
            else
            {
                return Content("Solo puede cargar notas el medico que cargó la evolución.");
            }
            
            
        }

        // POST: Notas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpleadoId,Mensaje,EvolucionId")] Nota nota)
        {
            //var notas = _context.Nota.Include(n => n.Empleado).Include(n => n.Evolucion);

            var empleado = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

            nota.EmpleadoId = empleado;
            if (ModelState.IsValid)
            {
                _context.Add(nota);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new {id = nota.Id });
            }
            
            return View(nota);
        }

    

        private bool NotaExists(int id)
        {
          return _context.Nota.Any(e => e.Id == id);
        }
    }
}
