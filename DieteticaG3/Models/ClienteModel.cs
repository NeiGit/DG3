using System;
using System.ComponentModel.DataAnnotations;

namespace DieteticaG3.Models
{
    //model de Cliente con los mismos datos que en la tabla db. Incluye las validaciones a nivel formulario.
    public class ClienteModel
    {
        [Display(Name = "Nombre")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar su nombre"), StringLength(100, ErrorMessage = "Longitud inválida")]
        public String Nombre { get; set; }
        
        [Display(Name = "Apellido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar su apellido"), StringLength(100, ErrorMessage = "Longitud inválida")]
        public String Apellido { get; set; }

        [Display(Name = "Dni")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar su dni"), StringLength(8, ErrorMessage = "Longitud inválida")]
        public String Dni { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar su email")]
        [DataType(DataType.Password)]
        public String Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar una contraseña")]
        public String Password { get; set; }
        
        [Display(Name = "Confirmar constraseña")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe confirmar su contraseña")]
        public String PasswordConfirm { get; set; }

        [Display(Name = "Acepto términos y condiciones de uso")]
        public bool TermsAndCond { get; set; }

    }
}