using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DieteticaG3.Models
{
    //modelo del Login que luego se compara con la base de Clientes para saber si existe y si los datos son correctos (todo esto en ClientesController)
    public class LoginModel
    {
        [Required(ErrorMessage = "Debe ingresar su dni")]
        public String Dni { get; set; }

        [Required(ErrorMessage = "Debe ingresar su contraseña")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}