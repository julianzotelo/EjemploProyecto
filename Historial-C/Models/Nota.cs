using Historial_C.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Historial_C.Models
{
	public class Nota
	{
        public int Id { get; set; }
         
        public int EmpleadoId { get; set; }

        public Empleado Empleado { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = ErrorMsg.MsgRange)]
        public string Mensaje { get; set; }

        public DateTime FechaYHora { get;} = DateTime.Now;

        public int EvolucionId { get; set; } 

        public Evolucion Evolucion { get; set; } 

        
    }
}