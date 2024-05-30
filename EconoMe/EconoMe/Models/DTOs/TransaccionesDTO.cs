using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Util;

namespace EconoMe.Models.DTOs
{
    public class TransaccionesDTO
    {

        public int id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Range(0.01, Double.MaxValue, ErrorMessage = "El importe debe ser mayor que cero.")]
        public decimal Importe { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El concepto no puede tener más de 100 caracteres.")]
        public string Concepto { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public int TipoTransaccionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public string TiposDeTransacciones { get; set; }

        public List<string> TransaccionesCategorias { get; set; }

        public TransaccionesDTO() { }

        public TransaccionesDTO(Transaciones transacciones)
        {
            id = transacciones.id;
            Fecha = transacciones.Fecha;
            Importe = transacciones.Importe;
            Concepto = transacciones.Concepto;
            Descripcion = transacciones.Descripcion;
            TipoTransaccionId = transacciones.TipoTransaccionId;
            UsuarioId = transacciones.UsuarioId;
            TiposDeTransacciones = transacciones.TiposDeTransacciones.Nombre;
            TransaccionesCategorias = new List<string>();
            foreach (var item in transacciones.TransaccionesCategorias)
            {
                TransaccionesCategorias.Add(item.Categorias.Nombre);
            }
        }

        public Transaciones ToEntity()
        {
            var transaccion = new Transaciones
            {
                id = id,
                Fecha = Fecha,
                Importe = Importe,
                Concepto = Concepto,
                Descripcion = Descripcion,
                TipoTransaccionId = TipoTransaccionId,
                UsuarioId = UsuarioId
            };

            return transaccion;
        }

    }
}