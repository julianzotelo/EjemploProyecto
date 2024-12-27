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
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Historial_C.Controllers
{
    [Authorize]
    public class EvolucionesController : Controller
    {
        private readonly HistorialContext _context;

        public EvolucionesController(HistorialContext context)
        {
            _context = context;
        }

        // GET: Evoluciones
        
        public async Task<IActionResult> Index()
        {
            var historialContext = _context.Evolucion.Include(e => e.Medico);
            return View(await historialContext.ToListAsync());
        }

        // GET: Evoluciones/Details/5
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Evolucion == null)
            {
                return NotFound();
            }

            var evolucion = await _context.Evolucion
                .Include(e => e.Medico)
                .Include(m => m.Notas)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            var notas = _context.Nota.Include(t => t.Empleado).ToList();
            if (evolucion == null)
            {
                return NotFound();
            }

            return View(evolucion);
        }

        // GET: Evoluciones/Create
        [Authorize(Roles = "Medico")]
        public IActionResult Create(int? episodioId)
        {
            if(episodioId == null )
            {
                return NotFound();
            }
           
            Evolucion evolucion = new Evolucion() { EpisodioId = episodioId.Value };
           
            return View(evolucion);
        }

        // POST: Evoluciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, MedicoId,EpisodioId,DescripcionAtencion,NotaId")] Evolucion evolucion)
        {
            if (ModelState.IsValid)
            {
                
                evolucion.EstadoAbierto = true;
                evolucion.MedicoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                evolucion.FechaYHoraInicio = DateTime.Now;
                //evolucion = await _context.Evolucion.Include(m => m.Notas).FirstOrDefaultAsync(m => m.Id == evolucion.Id);
                _context.Add(evolucion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = evolucion.Id });
            }

            return View(evolucion);
        }

      
        private bool EvolucionExists(int id)
        {
          return _context.Evolucion.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> CerrarEvolucion(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

         
            Evolucion evolucion = await _context.Evolucion
                .Include(t => t.Medico)
                .Include(t => t.Episodio)
                .Include(t => t.Notas).FirstOrDefaultAsync(t => t.Id == id);

            if (evolucion == null)
            {
                return NotFound();
            }

           

            return View(evolucion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarEvolucion(int id, [Bind("Id,MedicoId, FechaYHoraInicio,EpisodioId,DescripcionAtencion,FechaYHoraAlta")] Evolucion evolucion)
        {

            Medico medico = await _context.Medico.FirstOrDefaultAsync(m => m.Id == evolucion.MedicoId);
            if (id != evolucion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    evolucion.FechaYHoraCierre = DateTime.Now;
                    evolucion.Medico = medico;
                    if (evolucion.FechaYHoraAlta == null)
                    {
                        return Content("La fecha de alta es obligatoria.");
                    }
                    else if ((evolucion.FechaYHoraAlta < evolucion.FechaYHoraInicio))
                    {
                        return Content("La fecha de alta no puede ser menor a la fecha de inicio de la evolucion.");
                    }
                    
                    evolucion.FechaYHoraAlta = evolucion.FechaYHoraAlta;
                    _context.Update(evolucion);
                    await _context.SaveChangesAsync();
                  
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvolucionExists(evolucion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("Details", evolucion);
            }


            return View(evolucion);
        }

    }
}
