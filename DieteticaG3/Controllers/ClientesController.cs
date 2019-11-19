using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using DieteticaG3.Models;

namespace DieteticaG3.Controllers
{
    public class ClientesController : Controller
    {
        private DieteticaEntities db = new DieteticaEntities();

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        [HttpPost]
        public ActionResult Registrar(ClienteModel objClienteModel)
        {
            int errorCount = 0;
            String errorMessage = "Registro inválido: ";
            if (ModelState.IsValid)
            {
                //-------Validaciones del lado del servidor--------

                // Que el formato del email sea válido
                if (!mailIsValid(objClienteModel.Email))
                {
                    errorMessage = addErrorMessage(errorMessage,"Formato de email inválido. ");
                    //ModelState.AddModelError("Error", "Formato de email inválido");
                    errorCount++;
                }
                // Que las contraseñas coincidan
                if (!passwordsAreEqual(objClienteModel.Password, objClienteModel.PasswordConfirm))
                {
                    errorMessage = addErrorMessage(errorMessage, "Las contraseñas no coinciden. ");
                    //ModelState.AddModelError("Error", "Las contraseñas no coinciden");
                    errorCount++;
                }

                // Que se hayan aceptado los términos y condiciones de uso
                if (!objClienteModel.TermsAndCond)
                {
                    errorMessage = addErrorMessage(errorMessage, "Debe aceptar los términos y condiciones de uso. ");
                    //ModelState.AddModelError("Error", "Debe aceptar los términos y condiciones de uso");
                    errorCount++;
                }

                //Validamos que no haya un cliente registrado con el mismo dni
                if (db.Cliente.Any(c => c.dni == objClienteModel.Dni))
                {
                    errorMessage = addErrorMessage(errorMessage, "Ya existe un usuario registrado con ese dni. ");
                    //ModelState.AddModelError("Error", "Ya existe un usuario registrado con ese dni");
                    errorCount++;
                }
                // Si no hubo errores, creamos al cliente y lo insertamos en la base
                if(errorCount == 0)
                {
                    Cliente cliente = CreateCliente(objClienteModel);
                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    objClienteModel = new ClienteModel();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Error", errorMessage);
                }
            }
            return View();
        }

        private string addErrorMessage(string errorMessage, string error)
        {
            return errorMessage + error;
        }

        private bool passwordsAreEqual(string password, string passwordConfirm)
        {
            return passwordConfirm.Equals(password);
        }

        private bool mailIsValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private Cliente CreateCliente(ClienteModel objClienteModel)
        {
            Cliente cliente = new Cliente();
            cliente.nombre = objClienteModel.Nombre;
            cliente.apellido = objClienteModel.Apellido;
            cliente.dni = objClienteModel.Dni;
            cliente.email = objClienteModel.Email;
            cliente.password = objClienteModel.Password;

            return cliente;
        }

        public ActionResult Login()
        {
            LoginModel objLoginModel = new LoginModel();
            return View(objLoginModel);
        }

        //una vez completado el login se guarda el dni y el nombre del cliente en un espacio reservado de la Session. Con esos datos se resuelve el "Bienvenide x". Fijense como se resuelve en la view _loginPartial.cshtml
        //habria que preguntarle al profe si le parece bien que usemos Session para manejar los datos de la app (sobretodo me preocupa la parte del carrito)
        [HttpPost]
        public ActionResult Login(LoginModel objLoginModel)
        {
            if (ModelState.IsValid)
            {
                Cliente cliente = db.Cliente.Where(c => c.dni == objLoginModel.Dni && c.password == objLoginModel.Password).FirstOrDefault();
                if (cliente != null)
                {
               
                    Session["Dni"] = cliente.dni;
                    Session["Nombre"] = cliente.nombre;
                    Session["codCliente"] = cliente.Id;
                  
                        
                }
                else
                {
                    ModelState.AddModelError("Error", "Datos incorrectos");
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Clientes");
        }
    }
}
