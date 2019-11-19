using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace DieteticaG3.Models
{
    public class PedidoModel
    {

        [Display(Name = "Nº de pedido")]
        public int numPed { get; set; }
        public int idCliente { get; set; }
        [Display(Name = "Fecha")]
        public System.DateTime PedFec { get; set; }
        [Display(Name = "Precio total")]
        public Decimal PrecioTotal { get; set; }


    }
}