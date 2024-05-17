using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Util;

namespace EconoMe.Models.DTOs
{
    public class TransaccionesDTO
    {

        public int id { get; set; }
        public System.DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public string Concepto { get; set; }
        public string Descripcion { get; set; }
        public int TipoTransaccionId { get; set; }
        public int UsuarioId { get; set; }
        public string TiposDeTransacciones { get; set; }
        public List<string> TransaccionesCategorias { get; set; }

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

            //var _context = new ControlFinanzasEntities();
            //_context.Transaciones.Add(retVal);
        }

    }
}