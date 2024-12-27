using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Historial_C.Data;
using Historial_C.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Historial_C.Controllers
{
    [Authorize]
    public class EpisodiosController : Controller
    {
        private readonly HistorialContext _context;

        public EpisodiosController(HistorialContext context)
        {
            _context = context;
        }

        // GET: Episodios
        public async Task<IActionResult> Index(int? empleadoId)
        {

            if(empleadoId != null)
            {
                Empleado empleado = await _context.Empleado.FirstOrDefaultAsync(e => e.Nombre.Equals(empleadoId));
                await _context.Episodio.Include(e => e.EmpleadoRegistra).Where(e => e.EmpleadoId == empleado.Id).OrderBy(e => Convert.ToInt32(e.Id)).ToArrayAsync();
            }
              return View(await _context.Episodio.ToListAsync());
        }

        // GET: Episodios/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Episodio == null)
            {
                return NotFound();
            }

            //List<Episodio> episodios;
            Episodio episodio = await _context.Episodio
                 .Include(e => e.Evoluciones)
                 .Include(e => e.Paciente)
                 .Include(e => e.Epicrisis)
                 .ThenInclude(e => e.Diagnostico)
                 .FirstOrDefaultAsync(m => m.Id == id);
                 
            

            

            Evolucion evolucion = await _context.Evolucion
                .Include(m => m.Medico)
                .Include(m => m.Notas)
                .FirstOrDefaultAsync(m => m.EpisodioId == id);



        if (episodio == null)
            {
                return NotFound();
            }

            return View(episodio);
        }

        // GET: Episodios/Create
        [Authorize(Roles = "Empleado")]
        public IActionResult Create(int? pacienteId)
        {
            if(pacienteId == null)
            {
                return NotFound();
            }

            Episodio episodio = new Episodio() { PacienteId = pacienteId.Value};

            return View(episodio);
        }

        // POST: Episodios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, PacienteId,Motivo,Descripcion, EmpleadoId, FechaYHoraInicio")] Episodio episodio)
        { 
            if (ModelState.IsValid)
            {

                var empleado = await _context.Episodio.Include(m => m.EmpleadoRegistra).FirstOrDefaultAsync(m => m.Id == episodio.Id);

                episodio.FechaYHoraInicio = DateTime.Now;
                episodio.EstadoAbierto = true;
                episodio.EmpleadoId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                
                _context.Add(episodio);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id=episodio.Id});

             
                
             
                
            }
            return View(episodio);
        }






        private bool EpisodioExists(int id)
        {
          return _context.Episodio.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> CierreEpisodio(int id)
        {
            Boolean sePuedeCerrar;
            int i = 0;

            if(id == 0 || !EpisodioExists(id))
            {
                sePuedeCerrar = false;
                
            }
            else
            {
                Episodio episodio = await _context.Episodio
                    .Include(e => e.Epicrisis)
                    .Include(e => e.Evoluciones)
                    .ThenInclude(e=>e.Notas)
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                int cantEvoluciones = episodio.Evoluciones.Count();
                if (cantEvoluciones > 0 )
                {
                    if(!EvolucionesCerradas(episodio))
                    {
                        sePuedeCerrar = false;
                    }
                    else
                    {
                        sePuedeCerrar = true;
                    }

                    if (sePuedeCerrar) 
                    {
                        
                        return View("CierreEpisodio", episodio);

                    }
                    

                }
                else
                {
                    return Content("No hay evoluciones cargadas, no se puede cerrar el episodio.");
                }
                
            }
                
            return Content("No se pudo cerrar el episodio ya que hay evoluciones en estado Abierto.");

        }
        
        [HttpPost, ActionName("CierreEpisodio")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CierreEpisodio(int id, [Bind("Id,PacienteId,FechaYHoraInicio,Motivo,Descripcion,FechaYHoraAlta, Epicrisis")] Episodio episodio)
        {

            episodio.EmpleadoId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            if (id != episodio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(episodio.FechaYHoraAlta != null)
                    {
                        if(episodio.FechaYHoraAlta > episodio.FechaYHoraInicio)
                        {
                            episodio.EstadoAbierto = false;
                            episodio.FechaYHoraCierre = DateTime.Now;
                            episodio.Epicrisis.MedicoId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                            _context.Update(episodio);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return Content("La fecha de alta no puede menor a la fecha de inicio del episodio.");
                        }
                        
                    }
                    else
                    {
                        return Content("La fecha de alta no puede ser nula.");
                    }
                
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodioExists(episodio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", episodio);
            }
            return View(episodio);
        }


        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> CierreAdministrativo(int id)
        {
            if(id == 0  || !EpisodioExists(id))
            {
                return NotFound();
            }

            Episodio episodio = await _context.Episodio
                    .Include(e => e.Evoluciones)
                    .Include(e => e.Epicrisis).FirstOrDefaultAsync(p => p.Id == id);

            if(!episodio.Evoluciones.Any())
            {
                return View("CierreAdministrativo", episodio);
            }
            else
            {
                return Content("No se puede realizar el cierre administrativo ya que el episodio contiene evoluciones cargadas.");
            }
            
        }
        
        [HttpPost, ActionName("CierreAdministrativo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CierreAdministrativo(int id, [Bind("Id,FechaYHoraInicio,PacienteId,Motivo,Descripcion,FechaYHoraAlta,Epicrisis")] Episodio episodio)
        {
            if (ModelState.IsValid)
            {


                episodio.EstadoAbierto = false;
                episodio.FechaYHoraCierre = DateTime.Now;
                episodio.FechaYHoraAlta = DateTime.Now;
                episodio.EmpleadoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                episodio.Epicrisis.MedicoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                episodio.Epicrisis.Diagnostico.Descripcion = "Cierre administrativo";
                episodio.Epicrisis.EpisodioId = episodio.Id;
                _context.Update(episodio);
                await _context.SaveChangesAsync();
                

                return RedirectToAction("Details", episodio);
            }
            return View(episodio);
        }

        private DateTime FechaInicioAnterior(Episodio episodio)
        {
            Episodio episodioAnterior = _context.Episodio.Find(episodio.Id);
            DateTime fechaInicioAnterior = episodioAnterior.FechaYHoraInicio;

            return fechaInicioAnterior;
        }


        public Boolean EvolucionesCerradas(Episodio episodio)
        {
            Boolean cerrados = true;
            int i = 0;
            
            while(i < episodio.Evoluciones.Count() && cerrados)
            {
                if (episodio.Evoluciones[i].EstadoAbierto)
                {
                    cerrados = false;
                }
                i++;
            }
            return cerrados;
        }
    }
}
