using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Historial_C.Data;
using Historial_C.Models;
using Microsoft.AspNetCore.Identity;
using Historial_C.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace Historial_C.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private readonly HistorialContext _context;
        private readonly UserManager<Persona> _userManager;

        public EmpleadosController(HistorialContext context, UserManager<Persona> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }

        // GET: Empleados
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.Empleado.ToListAsync());
        }

        // GET: Empleados/Details/5
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        [Authorize(Roles = "Empleado")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Legajo,Id,Nombre,Apellido,Dni,Telefono,Direccion,Email")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
              
                empleado.FechaAlta = DateTime.Now;
                empleado.UserName = empleado.Email;
                empleado.Legajo = GenerarLegajo();
                try {

                    var resultado = await _userManager.CreateAsync(empleado, Configs.PasswordGenerica);

                    if (resultado.Succeeded)
                    {
                        //si pude crear el empleado entonces le agrego un rol
                        await _userManager.AddToRoleAsync(empleado, "Empleado");
                    }
                    return RedirectToAction(nameof(Index));

                } catch (DbUpdateException dbex){

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
            return View(empleado);
        }
       
        private bool EmpleadoExists(int id)
        {
          return _context.Empleado.Any(e => e.Id == id);
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
