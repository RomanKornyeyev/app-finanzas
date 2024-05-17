//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EconoMe.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class Transaciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transaciones()
        {
            this.TransaccionesCategorias = new HashSet<TransaccionesCategorias>();
        }
    
        public int id { get; set; }
        public System.DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public string Concepto { get; set; }
        public string Descripcion { get; set; }
        public int TipoTransaccionId { get; set; }
        public int UsuarioId { get; set; }

        public virtual TiposDeTransacciones TiposDeTransacciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [JsonIgnore]
        public virtual ICollection<TransaccionesCategorias> TransaccionesCategorias { get; set; }
        public virtual Usuarios Usuarios { get; set; }
    }
}