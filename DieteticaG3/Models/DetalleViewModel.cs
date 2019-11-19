using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace DieteticaG3.Models
{
    public class DetalleViewModel
    {
        [Display(Name = "Producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe seleccionar un producto")]
        public int SelectedProductoId { get; set; }
        
        [Display(Name = "Producto")]
        public String productoNombre { get; set; }
        
        [Display(Name = "Cantidad")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe seleccionar la cantidad")]
        [Range(1, Int32.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int cantidad { get; set; }

        [Display(Name = "Nro pedido")]
        public int numPed { get; set; }

        [Display(Name = "Precio unitario")]
        public Decimal precioUnitario { get; set; }

        [Display(Name = "Total")]
        public Decimal totalDetalle { get; set; }
    }
}