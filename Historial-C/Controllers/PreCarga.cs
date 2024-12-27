using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Historial_C.Models;
using Historial_C.Data;
using Microsoft.EntityFrameworkCore;
using Historial_C.Helpers;
using Microsoft.Extensions.Primitives;

namespace Historial_C.Controllers
{
    public class PreCarga : Controller
    {
        private readonly UserManager<Persona> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly HistorialContext _context;

        private readonly List<string> roles = new List<string>() { "Paciente", "Empleado", "Medico" };

        public PreCarga(UserManager<Persona> userManager, RoleManager<Rol> roleManager, HistorialContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;


        }

        public async Task<IActionResult> Seed()
        {

            CrearEspecialidades().Wait();
            CrearRoles().Wait();
            CrearEmpleado().Wait();
            CrearMedico().Wait();
            CrearPaciente().Wait();
            crearEpisodio().Wait();
            crearEvolucion().Wait();
            //crearEpicrisis().Wait();
            //crearDiagnostico().Wait();
            crearNota().Wait();

            return RedirectToAction("Index", "Home", new { mensaje = "Proceso Seed Finalizado" });
        }


        private async Task CrearRoles()
        {
            foreach (var rolName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(rolName))
                {
                    await _roleManager.CreateAsync(new Rol(rolName));
                }
            }
        }

        private async Task crearNota()
        {
            Nota nota1 = new Nota { EmpleadoId = 3, Mensaje = "Paciente indica que ya se siente mejor y manifiesta que quiere irse a su casa.", EvolucionId = 1 };
            nota1.Empleado = await _context.Empleado.FindAsync(nota1.EmpleadoId);
            nota1.Evolucion = await _context.Evolucion.FindAsync(nota1.EvolucionId);

            if (!NotaExists(nota1.Mensaje, nota1.EmpleadoId, nota1.EvolucionId))
            {
                _context.Nota.Add(nota1);
                await _context.SaveChangesAsync();
            }

            Nota nota2 = new Nota { EmpleadoId = 4, Mensaje = "Paciente indica que se cansó de esperar y se retira.", EvolucionId = 2 };
            nota2.Evolucion = await _context.Evolucion.FindAsync(nota2.EvolucionId);
            nota2.Empleado = await _context.Empleado.FindAsync(nota2.EmpleadoId);
            if (!NotaExists(nota2.Mensaje, nota2.EmpleadoId, nota2.EvolucionId))
            {
                _context.Nota.Add(nota2);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();

        }

        //private async Task crearDiagnostico()
        //{
        //    Diagnostico diagnostico1 = new Diagnostico { EpicrisisId = 1, Descripcion = "Cortes en la frente", Recomendacion = "volver para sacar putos de corte en frente" };
        //    if (!DiagnosticoExists(diagnostico1.EpicrisisId, diagnostico1.Descripcion))
        //    {
        //        _context.Diagnostico.Add(diagnostico1);
        //        await _context.SaveChangesAsync();
        //    }


        //    Diagnostico diagnostico2 = new Diagnostico { EpicrisisId = 2, Descripcion = "Desgarre de ligamentos", Recomendacion = "Reposo 1 mes con yeso, volver transcurrido el mes" };
        //    if (!DiagnosticoExists(diagnostico2.EpicrisisId, diagnostico2.Descripcion))
        //    {
        //        _context.Diagnostico.Add(diagnostico2);
        //        await _context.SaveChangesAsync();
        //    }



        //}

        //private async Task crearEpicrisis()
        //{
        //    Epicrisis epicrisis1 = new Epicrisis { MedicoId = 3, EpisodioId = 1};
        //    epicrisis1.Medico = await _context.Medico.FindAsync(epicrisis1.MedicoId);
        //    epicrisis1.Episodio = await _context.Episodio.FindAsync(epicrisis1.EpisodioId);
        //    if (!EpicrisisExists(epicrisis1.MedicoId, epicrisis1.EpisodioId, epicrisis1.FechaYHora))
        //    {
        //        _context.Epicrisis.Add(epicrisis1);
        //        await _context.SaveChangesAsync();

        //    }

        //    Epicrisis epicrisis2 = new Epicrisis { MedicoId = 4, EpisodioId = 2 };
        //    epicrisis2.Medico = await _context.Medico.FindAsync(epicrisis2.MedicoId);
        //    epicrisis2.Episodio = await _context.Episodio.FindAsync(epicrisis2.EpisodioId);
        //    if (!EpicrisisExists(epicrisis2.MedicoId, epicrisis2.EpisodioId, epicrisis2.FechaYHora))
        //    {
        //        _context.Epicrisis.Add(epicrisis2);
        //        await _context.SaveChangesAsync();

        //    }

        //}

        private async Task crearEvolucion()
        {
            Evolucion evolucion1 = new Evolucion { MedicoId = 3, EpisodioId = 1, DescripcionAtencion = "Cirujia exitosa (union de ligamentos), se traslada a enyesar pierna", EstadoAbierto = false, FechaYHoraInicio = new DateTime(2022,11,29, 00,00,00), FechaYHoraAlta = new DateTime(2022, 11, 30, 15, 00, 00), FechaYHoraCierre= new DateTime(2022, 11, 29, 15, 00, 00) };
            evolucion1.Medico = await _context.Medico.FindAsync(evolucion1.MedicoId);
            evolucion1.Episodio = await _context.Episodio.FindAsync(evolucion1.EpisodioId);
            if (!EvolucionExists(evolucion1.MedicoId, evolucion1.EpisodioId, evolucion1.DescripcionAtencion))
            {
                _context.Evolucion.Add(evolucion1);
                await _context.SaveChangesAsync();
            }

            Evolucion evolucion2 = new Evolucion { MedicoId = 4, EpisodioId = 2, DescripcionAtencion = "corte en frente tratada con 7 puntos", EstadoAbierto = false, FechaYHoraInicio = new DateTime(2022, 11, 29, 01, 00, 00), FechaYHoraAlta = new DateTime(2022, 11, 30, 16, 00, 00), FechaYHoraCierre = new DateTime(2022, 11, 29, 16, 00, 00) };
            evolucion2.Medico = await _context.Medico.FindAsync(evolucion2.MedicoId);
            evolucion2.Episodio = await _context.Episodio.FindAsync(evolucion2.EpisodioId);
            if (!EvolucionExists(evolucion2.MedicoId, evolucion2.EpisodioId, evolucion2.DescripcionAtencion))
            {
                _context.Evolucion.Add(evolucion2);
                await _context.SaveChangesAsync();
            }

            Evolucion evolucion3 = new Evolucion { MedicoId = 3, EpisodioId = 4, DescripcionAtencion = "prueba para cerrar evolucion", EstadoAbierto = true, FechaYHoraInicio = new DateTime(2022, 11, 29, 01, 00, 00) };
            evolucion3.Medico = await _context.Medico.FindAsync(evolucion3.MedicoId);
            evolucion3.Episodio = await _context.Episodio.FindAsync(evolucion3.EpisodioId);
            if (!EvolucionExists(evolucion3.MedicoId, evolucion3.EpisodioId, evolucion3.DescripcionAtencion))
            {
                _context.Evolucion.Add(evolucion3);
                await _context.SaveChangesAsync();
            }

        }

        private async Task crearEpisodio()
        {
            Episodio episodio1 = new Episodio { Motivo = "Accidente de transito", Descripcion = "Cortes en la frente", EstadoAbierto = false, EmpleadoId = 1, PacienteId = 5, FechaYHoraInicio = new DateTime(2022, 11, 29, 00, 00, 00), FechaYHoraAlta = new DateTime(2022, 11, 30, 15, 00, 00), FechaYHoraCierre = new DateTime(2022, 11, 29, 15, 00, 00), Epicrisis = new Epicrisis { MedicoId = 3, EpisodioId = 1, Diagnostico = new Diagnostico { EpicrisisId = 1, Descripcion = "Cortes en la frente", Recomendacion = "volver para sacar putos de corte en frente" } } };
            episodio1.Paciente = await _context.Paciente.FindAsync(episodio1.PacienteId);
            episodio1.EmpleadoRegistra = await _context.Empleado.FindAsync(episodio1.EmpleadoId);
            episodio1.Epicrisis.Medico = await _context.Medico.FindAsync(3);
            episodio1.Epicrisis.Episodio = await _context.Episodio.FindAsync(1);
            if (!EpisodioExists(episodio1.Motivo, episodio1.Descripcion, episodio1.PacienteId, episodio1.EmpleadoId, episodio1.FechaYHoraInicio))
            {
                _context.Episodio.Add(episodio1);
                await _context.SaveChangesAsync();
            }


            Episodio episodio2 = new Episodio { Motivo = "Tobillo torcido mientras corria", Descripcion = "Desgarre de ligamentos", EstadoAbierto = true, EmpleadoId = 2, PacienteId = 6, FechaYHoraInicio = new DateTime(2022, 11, 29, 21, 00, 00) };
            episodio2.Paciente = await _context.Paciente.FindAsync(episodio2.PacienteId);
            episodio2.EmpleadoRegistra = await _context.Empleado.FindAsync(episodio2.EmpleadoId);
            _context.Episodio.Add(episodio2);
            if (!EpisodioExists(episodio2.Motivo, episodio2.Descripcion, episodio2.PacienteId, episodio2.EmpleadoId, episodio2.FechaYHoraInicio))
            {
                _context.Episodio.Add(episodio2);
                await _context.SaveChangesAsync();
            }

            Episodio episodio3 = new Episodio { Motivo = "Prueba cierre administrativo", Descripcion = "Prueba cierre episodio sin evoluciones", EstadoAbierto = true, EmpleadoId = 1, PacienteId = 5, FechaYHoraInicio = new DateTime(2022, 11, 29, 21, 05, 00) };
            episodio3.Paciente = await _context.Paciente.FindAsync(episodio3.PacienteId);
            episodio3.EmpleadoRegistra = await _context.Empleado.FindAsync(episodio3.EmpleadoId);
            if (!EpisodioExists(episodio3.Motivo, episodio3.Descripcion, episodio3.PacienteId, episodio3.EmpleadoId, episodio3.FechaYHoraInicio))
            {

                _context.Episodio.Add(episodio3);
                await _context.SaveChangesAsync();

            }

            Episodio episodio4 = new Episodio { Motivo = "Prueba cierre normal con evoluciones", Descripcion = "Prueba cierre episodio y evoluciones", EstadoAbierto = true, EmpleadoId = 1, PacienteId = 5, FechaYHoraInicio = new DateTime(2022, 11, 29, 21, 10, 00) };
            episodio4.Paciente = await _context.Paciente.FindAsync(episodio4.PacienteId);
            episodio4.EmpleadoRegistra = await _context.Empleado.FindAsync(episodio4.EmpleadoId);
            if (!EpisodioExists(episodio4.Motivo, episodio4.Descripcion, episodio4.PacienteId, episodio4.EmpleadoId, episodio4.FechaYHoraInicio))
            {

                _context.Episodio.Add(episodio4);
                await _context.SaveChangesAsync();

            }

        }

        private async Task CrearEspecialidades()
        {
            Especialidad especialidad1 = new Especialidad { Nombre = "Urologo" };
            if (!EspecialidadExists(especialidad1.Nombre))
            {
                _context.Especialidad.Add(especialidad1);
            }


            Especialidad especialidad2 = new Especialidad { Nombre = "Traumatologia" };
            if (!EspecialidadExists(especialidad2.Nombre))
            {
                _context.Especialidad.Add(especialidad2);
            }

            Especialidad especialidad3 = new Especialidad { Nombre = "Cardiologia" };
            if (!EspecialidadExists(especialidad3.Nombre))
            {
                _context.Especialidad.Add(especialidad3);
            }


            Especialidad especialidad4 = new Especialidad { Nombre = "Clinica Medica" };
            if (!EspecialidadExists(especialidad4.Nombre))
            {
                _context.Especialidad.Add(especialidad4);
            }

            Especialidad especialidad5 = new Especialidad { Nombre = "Neurologia" };
            if (!EspecialidadExists(especialidad5.Nombre))
            {
                _context.Especialidad.Add(especialidad5);
            }

            Especialidad especialidad6 = new Especialidad { Nombre = "Otorrinonaringologia" };
            if (!EspecialidadExists(especialidad6.Nombre))
            {
                _context.Especialidad.Add(especialidad6);
            }

            Especialidad especialidad7 = new Especialidad { Nombre = "Gastroenterologo" };
            if (!EspecialidadExists(especialidad6.Nombre))
            {
                _context.Especialidad.Add(especialidad7);
            }


            Especialidad especialidad8 = new Especialidad { Nombre = "Flebologia" };
            if (!EspecialidadExists(especialidad8.Nombre))
            {
                _context.Especialidad.Add(especialidad8);
            }

            Especialidad especialidad9 = new Especialidad { Nombre = "Dermatologia" };
            if (!EspecialidadExists(especialidad9.Nombre))
            {
                _context.Especialidad.Add(especialidad9);
            }

            Especialidad especialidad10 = new Especialidad { Nombre = "Pediatria" };
            if (!EspecialidadExists(especialidad9.Nombre))
            {
                _context.Especialidad.Add(especialidad10);
            }

            await _context.SaveChangesAsync();
        }


        //private async Task CrearUsuario()
        //{
        //    Persona usuario1 = new Paciente
        //    {
        //        Nombre = "Maria",
        //        Apellido = "Gomez",
        //        Dni = "11114444",
        //        Telefono = "11222555",
        //        Direccion = "calle 102",
        //        Email = "Maria@unlam.edu.ar",
        //        UserName = "usuario1@unlam.edu.ar"
        //    };


        //    if (!PersonaExists(usuario1.Dni))
        //    {
        //        var resultado1 = await _userManager.CreateAsync(usuario1, Configs.PasswordGenerica);
        //        if (resultado1.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(usuario1, Configs.UsuarioRolName);
        //        }
        //    }

        //    Persona usuario2 = new Paciente { Nombre = "Marta", Apellido = "Gaona", Dni = "22228888", Telefono = "11332244", Direccion = "calle 923", Email = "Marta@unlam.edu.ar", UserName = "usuario2@unlam.edu.ar" };

        //    if (!PersonaExists(usuario2.Dni))
        //    {
        //        var resultado2 = await _userManager.CreateAsync(usuario2, Configs.PasswordGenerica);
        //        if (resultado2.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(usuario2, Configs.UsuarioRolName);
        //        }
        //    }

        //}


        private async Task CrearEmpleado()
        {
            Empleado empleado1 = new Empleado {  Nombre = "Eduardo", Apellido = "Perez", Dni = "11111111", Telefono = "11222222", Direccion = "calle 444", Email = "eudardo@unlam.edu.ar", UserName = "empleado1@unlam.edu.ar", FechaAlta = DateTime.Now };



            if (!PersonaExists(empleado1.Dni))
            {
                var resultado1 = await _userManager.CreateAsync(empleado1, Configs.PasswordGenerica);
                empleado1.Legajo = GenerarLegajo();
                if (resultado1.Succeeded)
                {
                    await _userManager.AddToRoleAsync(empleado1, Configs.EmpleadoRolName);
                }
            }

            Empleado empleado2 = new Empleado { Nombre = "Juan", Apellido = "Perez", Dni = "22222222", Telefono = "11333333", Direccion = "calle 555", Email = "juan@unlam.edu.ar", UserName = "empleado2@unlam.edu.ar", FechaAlta = DateTime.Now };

            if (!PersonaExists(empleado2.Dni))
            {
                var resultado2 = await _userManager.CreateAsync(empleado2, Configs.PasswordGenerica);
                empleado2.Legajo = GenerarLegajo();
                if (resultado2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(empleado2, Configs.EmpleadoRolName);
                }
            }

        }

        private async Task CrearMedico()
        {
            Persona medico1 = new Medico { Legajo = "1002", Matricula = "1111/224", Nombre = "Adrian", Apellido = "Diaz", Dni = "44444444", Telefono = "11222243", Direccion = "calle 934", Email = "adrian@unlam.edu.ar", UserName = "medico1@unlam.edu.ar", EspecialidadId = 1, FechaAlta = DateTime.Now };

            if (!PersonaExists(medico1.Dni))
            {
                var resultado1 = await _userManager.CreateAsync(medico1, Configs.PasswordGenerica);
                if (resultado1.Succeeded)
                {
                    await _userManager.AddToRoleAsync(medico1, Configs.MedicoRolName);
                }

            }


            Persona medico2 = new Medico { Legajo = "1003", Matricula = "2222/332", Nombre = "damian", Apellido = "Lopez", Dni = "55555555", Telefono = "11333852", Direccion = "calle 694", Email = "damian@unlam.edu.ar", UserName = "medico2@unlam.edu.ar", EspecialidadId = 2, FechaAlta = DateTime.Now };

            if (!PersonaExists(medico2.Dni))
            {
                var resultado2 = await _userManager.CreateAsync(medico2, Configs.PasswordGenerica);
                if (resultado2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(medico2, Configs.MedicoRolName);

                }
            }
        }

        private async Task CrearPaciente()
        {
            Persona paciente1 = new Paciente { Nombre = "Pablo", Apellido = "Pedrozo", Dni = "12722845", Telefono = "1123929433", Direccion = "Suipacha 123", Email = "pablo@unlam.edu.ar", ObraSocial = "Omint", UserName = "paciente1@unlam.edu.ar", FechaAlta = DateTime.Now };

            if (!PersonaExists(paciente1.Dni))
            {
                var resultado1 = await _userManager.CreateAsync(paciente1, Configs.PasswordGenerica);
                if (resultado1.Succeeded)
                {
                    await _userManager.AddToRoleAsync(paciente1, Configs.PacienteRolName);
                }

            }


            Persona paciente2 = new Paciente { Nombre = "Matias", Apellido = "Allioti", Dni = "23499455", Telefono = "1192348455", Direccion = "calle 694", Email = "matias@unlam.edu.ar", ObraSocial = "Omint", UserName = "paciente2@unlam.edu.ar", FechaAlta = DateTime.Now };

            if (!PersonaExists(paciente2.Dni))
            {
                var resultado2 = await _userManager.CreateAsync(paciente2, Configs.PasswordGenerica);
                if (resultado2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(paciente2, Configs.PacienteRolName);

                }
            }
        }

        private bool PersonaExists(string dni)
        {
            return _context.Persona.Any(p => p.Dni.Equals(dni));
        }

        private bool EspecialidadExists(string nombre)
        {
            return _context.Especialidad.Any(e => e.Nombre.Equals(nombre));
        }

        private bool NotaExists(string mensaje, int empleadoId, int evolucionId)
        {
            bool existe = false;

            Nota notaMensajeId = _context.Nota.FirstOrDefault(m => m.Mensaje.Equals(mensaje));
            Nota notaEmpleadoId = _context.Nota.FirstOrDefault(m => m.EmpleadoId == empleadoId);
            Nota notaEvolucionId = _context.Nota.FirstOrDefault(m => m.EvolucionId == evolucionId);
            if(notaMensajeId != null && notaEmpleadoId != null && notaEvolucionId!= null)
            {
                if((notaMensajeId == notaEmpleadoId) && (notaEmpleadoId == notaEvolucionId))
                {
                    existe = true;
                }
                
            }
            return existe;
        }

        private bool DiagnosticoExists(int epicrisisId, string descripcion)
        {
            bool existe = false;

            Diagnostico diagnosticoEpicrisisId = _context.Diagnostico.FirstOrDefault(m => m.EpicrisisId == epicrisisId);
            Diagnostico diagnosticoDescripcion = _context.Diagnostico.FirstOrDefault(m => m.Descripcion.Equals(descripcion));
            if (diagnosticoEpicrisisId != null && diagnosticoDescripcion != null )
            {
                if((diagnosticoEpicrisisId == diagnosticoDescripcion))
                existe = true;
            }
            return existe;
        }

        private bool EpicrisisExists(int medicoId, int episodioId, DateTime fecha)
        {
            bool existe = false;

            Epicrisis epicrisisMedicoId = _context.Epicrisis.FirstOrDefault(m => m.MedicoId == medicoId);
            Epicrisis epicrisisEpisodioId = _context.Epicrisis.FirstOrDefault(m => m.EpisodioId == episodioId);
            Epicrisis epicrisisFecha = _context.Epicrisis.FirstOrDefault(m => m.FechaYHora.Equals(fecha));
            if (epicrisisMedicoId != null && epicrisisEpisodioId != null && epicrisisFecha != null)
            {
                if((epicrisisMedicoId == epicrisisEpisodioId) && (epicrisisEpisodioId == epicrisisFecha))
                {
                    existe = true;
                }
               
            }
            return existe;
        }
        private bool EvolucionExists(int medicoId, int episodioId, string descripcion)
        {
            bool existe = false;

            Evolucion evolucionMedicoId = _context.Evolucion.FirstOrDefault(m => m.MedicoId == medicoId);
            Evolucion evolucionEpisodioId = _context.Evolucion.FirstOrDefault(m => m.EpisodioId == episodioId);
            Evolucion evolucionDescripcion = _context.Evolucion.FirstOrDefault(m => m.DescripcionAtencion.Equals(descripcion));
            if ((evolucionMedicoId != null) && (evolucionEpisodioId != null) && (evolucionDescripcion != null))
            {
                if((evolucionMedicoId == evolucionEpisodioId)&&(evolucionEpisodioId == evolucionDescripcion))
                {
                    existe = true;
                }
                
            }
            return existe;
        }

        private bool EpisodioExists(string motivo, string descripcion, int pacienteId, int empleadoId, DateTime fecha)
        {
            bool existe = false;

            Episodio episodioMotivo = _context.Episodio.FirstOrDefault(m => m.Motivo.Equals(motivo));
            Episodio episodioDescripcion = _context.Episodio.FirstOrDefault(m => m.Descripcion.Equals(descripcion));
            Episodio episodioPacienteId = _context.Episodio.FirstOrDefault(m => m.PacienteId == pacienteId);
            Episodio episodioEmpleadoId = _context.Episodio.FirstOrDefault(m => m.EmpleadoId == empleadoId);
            Episodio episodioFecha = _context.Episodio.FirstOrDefault(m => m.FechaYHoraInicio.Equals(fecha));
            if (episodioMotivo != null && episodioDescripcion != null && episodioPacienteId != null && episodioEmpleadoId != null && episodioFecha != null)
            {
                if ((episodioMotivo == episodioDescripcion) && (episodioDescripcion == episodioPacienteId) && (episodioPacienteId == episodioEmpleadoId) && (episodioEmpleadoId == episodioFecha))
                {
                    existe = true;
                }
               
            }
            return existe;
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