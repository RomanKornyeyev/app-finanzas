using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EconoMeMVC.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Confirmar la clave es obligatorio")]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las claves no coinciden")]
        public string ConfirmarClave { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; }
    }
}
