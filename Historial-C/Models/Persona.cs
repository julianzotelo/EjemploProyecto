﻿using Historial_C.Helpers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Historial_C.Models
{
    public class Persona : IdentityUser<int>
    {
      

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(50, MinimumLength =2, ErrorMessage = ErrorMsg.MsgRange)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ErrorMsg.MsgRange)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(15, MinimumLength = 7, ErrorMessage = ErrorMsg.MsgRange)]
        public string Dni { get; set; }

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(20, MinimumLength = 7, ErrorMessage = ErrorMsg.MsgRange)]
       
        public string Telefono { get; set; }

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = ErrorMsg.MsgRange)]
        public string Direccion { get; set; }

        [Display(Name = "Fecha de Alta del usuario")]
        public DateTime FechaAlta { get; set; }

        [Required(ErrorMessage = ErrorMsg.MsgRequired)]
        [StringLength(150, MinimumLength = 6, ErrorMessage = ErrorMsg.MsgRange)]
        [Display(Name = "Correo Electronico")]
        [EmailAddress(ErrorMessage = ErrorMsg.MsgFormatEmail)]
        public override string Email {get {return base.Email; } set {base.Email = value;} }


    }
}
