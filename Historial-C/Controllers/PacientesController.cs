using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Historial_C.Data;
using Historial_C.Models;
using Microsoft.AspNetCore.Identity;
using Historial_C.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
//using AspNetCore;

namespace Historial_C.Controllers
{
    
    public class PacientesController : Controller
    {
        private readonly HistorialContext _context;
        private readonly UserManager<Persona> _userManager;

        public PacientesController(HistorialContext context, UserManager<Persona> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }
        
        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Paciente.ToListAsync());
        }


        public async Task<IActionResult> DetailsPerfil(string? pacienteName)
        {
            if (pacienteName == null || _context.Paciente == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente
                .FirstOrDefaultAsync(m => m.UserName == pacienteName);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Paciente == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        [Authorize(Roles = "Empleado")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObraSocial,Id,Nombre,Apellido,Dni,Telefono,Direccion,Email")] Paciente paciente)
        {
            if (ModelState.IsValid)
            {
               
                paciente.FechaAlta = DateTime.Now;
                paciente.UserName = paciente.Email;


                //si pude crear 
                try {
                    var resultado = await _userManager.CreateAsync(paciente, Configs.PasswordGenerica);

                    if (resultado.Succeeded)
                    {
                        //si pude crear el empleado entonces le agrego un rol
                        paciente.FechaAlta = DateTime.Now;
                        await _userManager.AddToRoleAsync(paciente, "Paciente");
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbex) {
                   
                    SqlException innerException = dbex.InnerException as SqlException;

                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        ModelState.AddModelError("Dni", "Dni ya existente");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, dbex.Message);
                    }

                }
               
            }
            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paciente == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [Bind("ObraSocial,Id,Nombre,Apellido,Dni,Telefono,Direccion,Email")] Paciente pacienteEnFormulario)
        {
            if (id != pacienteEnFormulario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pacienteEnDb = _context.Paciente.FirstOrDefault(p => p.Id == pacienteEnFormulario.Id);

                    if (pacienteEnDb != null)                        
                    {
                        //actualizan todo lo que necesitan - Esto es como si fuera un auto mapper 
                        pacienteEnDb.ObraSocial = pacienteEnFormulario.ObraSocial;
                        pacienteEnDb.Nombre = pacienteEnFormulario.Nombre;
                        pacienteEnDb.Apellido = pacienteEnFormulario.Apellido;
                        pacienteEnDb.Dni = pacienteEnFormulario.Dni;
                        pacienteEnDb.Telefono = pacienteEnFormulario.Telefono;
                        pacienteEnDb.Direccion = pacienteEnFormulario.Direccion;
                        pacienteEnDb.Email = pacienteEnFormulario.Email;

                        _context.Update(pacienteEnDb);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                     
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(pacienteEnFormulario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dbex)
                {

                    SqlException innerException = dbex.InnerException as SqlException;

                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        ModelState.AddModelError("Dni", "Dni ya existente");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, dbex.Message);
                    }

                }
                
            }
            return View(pacienteEnFormulario);
        }

    
        private bool PacienteExists(int id)
        {
          return _context.Paciente.Any(e => e.Id == id);
        }


        [HttpPost]
        public async Task<IActionResult> BuscarPaciente(string pacienteDni)
        {
            if (pacienteDni == null) {
                return NotFound();
            }
            
            Paciente paciente = await _context.Paciente
                .Include(m => m.Episodios)
                .ThenInclude(m => m.Evoluciones)
                .FirstOrDefaultAsync(p => p.Dni.Equals(pacienteDni));

            if (paciente != null)
            {
                return View("Details", paciente);
            }
            return Content("El dni ingresado no existe en el sistema.");
        }

       
        public async Task<IActionResult> HistorialClinicaPaciente(string pacienteUserName)
        {
            if(pacienteUserName == null || _context.Paciente == null)
            {
                return NotFound();
            }
            
            Paciente paciente = await _context.Paciente
                                            .Include(m => m.Episodios)
                                                .ThenInclude(m => m.EmpleadoRegistra)
                                            .Include(m => m.Episodios)
                                                .ThenInclude(m => m.Evoluciones)

                                        .FirstOrDefaultAsync(p => p.UserName == pacienteUserName);

            if(paciente == null)
            {
                return NotFound();
            }
            return View("HistorialClinicaPaciente", paciente);
        }

    }
}
