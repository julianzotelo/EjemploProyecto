using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Historial_C.Data;
using Historial_C.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Historial_C.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;

namespace Historial_C.Controllers
{

    
    public class MedicosController : Controller
    {
        private readonly HistorialContext _context;
        private readonly UserManager<Persona> _userManager;

        public MedicosController(HistorialContext context, UserManager<Persona> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }

        // GET: Medicos
    

        public async Task<IActionResult> Index(string? especialidadBuscada)
        {
            List<Medico> medicos;
            var listaEspecialidades = await _context.Especialidad.ToListAsync();
            ViewBag.Especialidades = listaEspecialidades;

            if (especialidadBuscada != null)
            {
                Especialidad especialidad = await _context.Especialidad.FirstOrDefaultAsync(e => e.Nombre.Contains(especialidadBuscada));
                if(especialidad != null)
                {
                    medicos = await _context.Medico.Include(m => m.Especialidad).Where(m => m.EspecialidadId == especialidad.Id).ToListAsync();
                }
                else
                {
                    medicos = _context.Medico.Include(m => m.Especialidad).ToList();
                }

            }
            else
            {
                medicos = _context.Medico.Include(m => m.Especialidad).ToList();
            }
            return View(medicos);
        }


        // GET: Medicos/Details/5
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Medico == null)
            {
                return NotFound();
            }

            var medico = await _context.Medico
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        // GET: Medicos/Create
        [Authorize(Roles= "Empleado")]
        public async Task<IActionResult> Create()
        {
            var listaEspecialidades = await _context.Especialidad.ToListAsync();
            ViewBag.Especialidades = listaEspecialidades;
            return View();
        }

        // POST: Medicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Matricula,Legajo,Id,Nombre,Apellido,Dni,Telefono,Direccion,Email, EspecialidadId")] Medico medico)
        {
            if (ModelState.IsValid)
            {
                // _context.Add(medico);
                //await _context.SaveChangesAsync();
                medico.FechaAlta = DateTime.Now;
                medico.UserName = medico.Email;
                medico.Legajo = GenerarLegajo();
                var especialidad = _context.Medico.Include(m => m.EspecialidadId);
                try
                {

                    
                    var resultado = await _userManager.CreateAsync(medico, Configs.PasswordGenerica);
                    if (resultado.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(medico, "Medico");
                    }
                    return RedirectToAction(nameof(Index));
                } catch (DbUpdateException dbex){

                    SqlException innerException = dbex.InnerException as SqlException;
                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        ModelState.AddModelError("Dni", "Dni ya existente");
                        var listaEspecialidades = await _context.Especialidad.ToListAsync();
                        ViewBag.Especialidades = listaEspecialidades;
                    }
                    else {
                        ModelState.AddModelError(String.Empty, dbex.Message);
                    }
                
                }
            }
            return View(medico);
        }

      
        private bool MedicoExists(int id)
        {
            return _context.Medico.Any(e => e.Id == id);
        }

        private string GenerarLegajo()
        {
            string ultimoLegajo = _context.Empleado.Max(m => m.Legajo); 
            int legajoNuevo = Convert.ToInt32(ultimoLegajo);
            if (legajoNuevo == 0)
            {
                legajoNuevo = 999;
            }
            legajoNuevo++;
            string legajoString = Convert.ToString(legajoNuevo);
            return legajoString;
        }


    }
}
