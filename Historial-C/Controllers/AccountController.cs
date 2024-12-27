using Historial_C.Helpers;
using Historial_C.Models;
using Historial_C.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Historial_C.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Persona> _userManager;
        private SignInManager<Persona> _SignInManager;


        public AccountController(UserManager<Persona> userManager, SignInManager<Persona> signInManager)
        {
            this._userManager = userManager;
            this._SignInManager = signInManager;
        }

        

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([Bind("ObraSocial","Nombre", "Apellido", "Dni", "Telefono", "Direccion", "Email", "Password", "confirmacionPassword")] RegistroUsuario model)
      
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Registracion
                    Paciente pacienteACrear = new Paciente()
                    {
                        ObraSocial = model.ObraSocial,
                        Nombre = model.Nombre,
                        Apellido = model.Apellido,
                        Dni = model.Dni,
                        Telefono = model.Telefono,
                        Direccion = model.Direccion,
                        Email = model.Email,
                        UserName = model.Email
                    };

                    //Usamos el metodo CreateAsyng de UserManager
                    //y a su vez le damos un tratamiento a la password guardandola en la propiedad passwordHasher de _userManager
                    var resultadoCreate = await _userManager.CreateAsync(pacienteACrear, model.Password);


                    //Si pudo crear a la persona 
                    if (resultadoCreate.Succeeded)
                    {
                        //le asigno el rol usuario normal
                        var resultadAddRole = await _userManager.AddToRoleAsync(pacienteACrear, Configs.PacienteRolName);

                        if (resultadAddRole.Succeeded)
                        {
                            await _SignInManager.SignInAsync(pacienteACrear, isPersistent: false);

                            //Al terminar de registrarse redireccionaremos a
                            //la persona a llenar su formulario para terminar de completar sus datos
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ModelState.AddModelError(String.Empty, $"No se pudo agregar el rol de {Configs.PacienteRolName}");
                        }
                        //Al terminar de registrarse redireccionaremos a
                        //la persona a llenar su formulario para terminar de completar sus datos
                        return RedirectToAction("Edit", "Personas", new { id = pacienteACrear.Id });
                    }

                    //Si hubo un inconveniente al crear
                    foreach (var error in resultadoCreate.Errors)
                    {
                        //Errores al momento de crear
                        ModelState.AddModelError(String.Empty, error.Description);
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
              
            
            return View(model);
        }

        public IActionResult IniciarSesion(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Login viewModel)
        {
            string returnUrl = TempData["ReturnUrl"] as string;


            if (ModelState.IsValid)
            {
                var resultado = await _SignInManager.PasswordSignInAsync(viewModel.Email,viewModel.Password,viewModel.Recordarme,false);

                if (resultado.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Inicio de sesion invalido");
            }

            
            return View(viewModel);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AccesoDenegado(string returnURL) {
            ViewBag.ReturnURL = returnURL;
            return View();
        }

    }
}
