using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EconoMeMVC.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        public string Clave { get; set; }
    }
}