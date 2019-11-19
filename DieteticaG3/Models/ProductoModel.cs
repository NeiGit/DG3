using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Web;

namespace DieteticaG3.Models
{
    public class ProductoModel
    {
        public ProductoModel()
        {
            Detalle = new HashSet<Detalle>();
        }

        [Display(Name = "Nombre")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar el nombre del producto"), StringLength(100, ErrorMessage = "Longitud inválida")]
        public string Nombre { get; set; }

        [Display(Name = "Precio")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar el precio del producto")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Debe ingresar un número válido")]
        [Range(1, Int32.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Precio { get; set; }

        [Display(Name = "Stock")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar el stock del producto")]
        [Range(1, Int32.MaxValue, ErrorMessage = "El stock debe ser mayor que 0")]
        public int Stock { get; set; }

        public virtual ICollection<Detalle> Detalle { get; set; }
    }
}