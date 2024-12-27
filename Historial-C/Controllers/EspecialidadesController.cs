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


namespace Historial_C.Controllers
{
    [Authorize]
    public class EspecialidadesController : Controller
    {
        private readonly HistorialContext _context;

        public EspecialidadesController(HistorialContext context)
        {
            _context = context;
        }

        // GET: Especialidades
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.Especialidad.ToListAsync());
        }

        // GET: Especialidades/Details/5
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Especialidad == null)
            {
                return NotFound();
            }

            var especialidad = await _context.Especialidad
                .FirstOrDefaultAsync(m => m.Id == id);
            if (especialidad == null)
            {
                return NotFound();
            }

            return View(especialidad);
        }

        // GET: Especialidades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Especialidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Especialidad especialidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(especialidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(especialidad);
        }


      
        private bool EspecialidadExists(int id)
        {
          return _context.Especialidad.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BuscarEspecialidad(string especialidadBuscada)
        {
            if (especialidadBuscada == null || _context.Medico == null || _context.Especialidad == null)
            {
                return NotFound();
            }
            Especialidad especialidad = await _context.Especialidad.FirstOrDefaultAsync(e => e.Nombre.Equals(especialidadBuscada));

            if (especialidad == null)
            {
                return NotFound();
            }
            else
            {
                var medicosEspecialidad =await _context.Medico.FirstOrDefaultAsync(m => m.EspecialidadId == especialidad.Id);
                if (medicosEspecialidad == null)
                {
                    return NotFound();
                }
            }

            return View("Details",especialidad);


        }
    }
}
