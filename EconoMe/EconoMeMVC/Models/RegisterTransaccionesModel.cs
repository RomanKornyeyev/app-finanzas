using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EconoMe.Models
{
    public class RegisterTransaccionesModel
    {
        [Required(ErrorMessage = "El tipo de transacción es obligatorio")]
        public int TipoTransaccionId { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor que cero")]
        public decimal Importe { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        public string Concepto { get; set; }

        public string Descripcion { get; set; }
    }
}
