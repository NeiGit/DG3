using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DieteticaG3.Models
{
    public class ABMProductos
    {
        [Required(ErrorMessage = "Debe ingresar un nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar un precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Debe ingresar el stock")]
        public int Stock { get; set; }
    }
}