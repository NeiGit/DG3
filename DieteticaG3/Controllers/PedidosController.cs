using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DieteticaG3.Models;

namespace DieteticaG3.Controllers
{
    public class PedidosController : Controller
    {
        private DieteticaEntities db = new DieteticaEntities();

        // GET: Pedidos
        public ActionResult Index()
        {
            int codCli = getIntFromSessionField(Session["codCliente"].ToString());
            List<Pedido> pedidosDB = db.Pedido.Where(pedido => pedido.idCliente == codCli).ToList();
            List<PedidoModel> pedidosDeCliente = convertToPedidoModelList(pedidosDB);
            
            //Asignamos a la Session una nueva lista de DetalleViewModel, en la cual iremos guardando
            //los detalles antes de persistirlos en la base (ya que el cliente puede cancelar el pedido)
            ViewBag.pedidosDeCliente = pedidosDeCliente;
            return View();
        }

        /*Usamos PedidoModel para incorporar el atributo PrecioTotal. Entonces tenemos que convertir
            la lista que obtenemos de la base a una lista de PedidoModel, y por cada pedido calcular
            el total. Esa es la lista que le pasamos a la View a través de la ViewBag*/
        private List<PedidoModel> convertToPedidoModelList(List<Pedido> pedidosDB)
        {
            List<PedidoModel> pedidosDelCliente = new List<PedidoModel>();
            pedidosDB.ForEach(pedido =>
            {
                PedidoModel model = convertPedidoToPedidoModel(pedido);
                pedidosDelCliente.Add(model);
            });
            return pedidosDelCliente;

        }

        private PedidoModel convertPedidoToPedidoModel(Pedido pedido)
        {
            PedidoModel model = new PedidoModel();
            model.idCliente = pedido.idCliente;
            model.numPed = pedido.numPed;
            model.PedFec = pedido.PedFec;
            model.PrecioTotal = calcularTotalDelPedido(pedido.numPed);
            return model;
        }

        private Decimal calcularTotalDelPedido(int numPed)
        {
            Decimal total = Decimal.Zero;
            List<Detalle> detalles = db.Detalle.Where(d => d.numPed == numPed).ToList();
            detalles.ForEach(d =>
            {
                Producto producto = db.Producto.Where(p => p.codigo == d.codProd).FirstOrDefault();
                total += d.cantidad * producto.precio;
            });
            return total;
        }

        private int getIntFromSessionField(string sessionField)
        {
            return Convert.ToInt32(sessionField);
        }

        public ActionResult NuevoPedido()
        {
            Session["detalles"] = new List<DetalleViewModel>();
            ViewBag.detalles = getDetallesListFromSession();
            configurarDropDownMenu();
            return View();
        }

        private Pedido crearPedido(int codCliente)
        {
            Pedido pedido = new Pedido();
            pedido.idCliente = codCliente;
            pedido.PedFec = DateTime.Now;
            db.Pedido.Add(pedido);
            db.SaveChanges();
            return pedido;
        }

        private void configurarDropDownMenu()
        {
            //Agregamos el listado de productos disponibles para mostrar en el menu
            List<Producto> productos = db.Producto.ToList();
            ViewBag.productos = new SelectList(db.Producto, "codigo", "nombre");
        }

        //REFACTORIZAR PORQUE NO TIENE EN CUENTA EL EXISTENTE PARA CHEQUEAR STOCK
        public ActionResult AgregarPreDetalle(DetalleViewModel detalleAgregado)
        {
      
         //Completamos la información del detalle para insertarlo en la tabla de la view (nombre y precio unitario del producto)
         detalleAgregado = completarDetalle(detalleAgregado);

         //Buscamos el producto, y chequeamos stock
         Producto producto = db.Producto.Find(detalleAgregado.SelectedProductoId);
            if (hayStock(producto, detalleAgregado.cantidad))
            {
                //Primero chequeamos si ya hay un detalle con el mismo producto
                List<DetalleViewModel> detalles = getDetallesListFromSession();
                DetalleViewModel detalleConElMismoProducto = detalles.Where(d => d.SelectedProductoId == detalleAgregado.SelectedProductoId).FirstOrDefault();
                if (detalleConElMismoProducto != null)
                {
                    detalleConElMismoProducto.cantidad += detalleAgregado.cantidad;
                    detalleConElMismoProducto.totalDetalle = producto.precio * detalleConElMismoProducto.cantidad;
                }
                else //no existe un detalle con el mismo producto
                {
                    //calculamos el total y lo insertamos en la tabla
                    detalleAgregado.totalDetalle = producto.precio * detalleAgregado.cantidad;
                    detalles.Add(detalleAgregado); 
                } 
            Session["detalles"] = detalles;
            } else //no hay stock
            {
                ModelState.AddModelError("Error", "No hay stock suficiente");
            }
            ViewBag.detalles = getDetallesListFromSession();
            configurarDropDownMenu();

         return View("NuevoPedido");

        }

        public ActionResult AgregarDetalle(DetalleViewModel nuevoDetalle)
        {

            //Completamos la información del detalle para insertarlo en la tabla de la view (nombre y precio unitario del producto)
            nuevoDetalle = completarDetalle(nuevoDetalle);

            //Buscamos el producto para luego chequear stock
            Producto producto = db.Producto.Find(nuevoDetalle.SelectedProductoId);

            //Guardamos la lista de detalles de la Session
            List<DetalleViewModel> detalles = getDetallesListFromSession();

            //Primero vemos si ya existe un detalle con el mismo producto en el pedido
            DetalleViewModel detalleExistente = detalles.Where(d => d.SelectedProductoId == nuevoDetalle.SelectedProductoId).FirstOrDefault();
            
            if(detalleExistente != null)
            { 
                //consultamos si hay stock teniendo en cuenta la cantidad pedida en el detalle existente, y la del nuevo detalle
                int cantidadTotal = detalleExistente.cantidad + nuevoDetalle.cantidad;
                if (hayStock(producto, cantidadTotal))
                {
                    //actualizamos el detalle existente, para evitar duplicados en la lista
                    detalleExistente.cantidad = cantidadTotal;
                    detalleExistente.totalDetalle = producto.precio * cantidadTotal;
                } else //no hay stock para la cantidad pedida sumada a la del detalle existente
                {
                  ModelState.AddModelError("Error", "No hay stock suficiente");
                }
            } else //no hay un detalle existente con el mismo producto
            {
                if(hayStock(producto, nuevoDetalle.cantidad))
                {
                    //esta vez si agregamos el detalle a la lista
                    nuevoDetalle.totalDetalle = producto.precio * nuevoDetalle.cantidad;
                    detalles.Add(nuevoDetalle);
                } else //no hay stock para la cantidad solicitada
                {
                    ModelState.AddModelError("Error", "No hay stock suficiente");
                }
            }

            ////En cualquier caso, volvemos a asignar detalles a la Session, ya que la lista se pudo haber actualizado
            Session["detalles"] = detalles;
            ViewBag.detalles = detalles;
            //Configuramos de nuevo el menu de productos, ya que se guarda temporalmente en la ViewBag
            configurarDropDownMenu();
            return View("NuevoPedido");
        }


        private DetalleViewModel completarDetalle(DetalleViewModel detalle)
        {
            Producto producto = db.Producto.Where(p => p.codigo == detalle.SelectedProductoId).FirstOrDefault();
            detalle.productoNombre = producto.nombre;
            detalle.precioUnitario = producto.precio;
            return detalle;
        }

        private List<DetalleViewModel> getDetallesListFromSession()
        {
           return (List<DetalleViewModel>)Session["detalles"];
        }

        private List<DetalleViewModel> getDetallesFromPedido(int numPed)
        {
            List <Detalle> detallesPorPedido = db.Detalle.Where(detalle => detalle.numPed == numPed).ToList();
            List <DetalleViewModel> modelList = new List<DetalleViewModel>();
            detallesPorPedido.ForEach(detalle =>
            {
                Producto producto = db.Producto.Find(detalle.codProd);
                DetalleViewModel model = new DetalleViewModel();
                model.SelectedProductoId = detalle.codProd;
                model.productoNombre = producto.nombre;
                model.cantidad = detalle.cantidad;
                model.precioUnitario = producto.precio;
                model.totalDetalle = producto.precio * detalle.cantidad;
                modelList.Add(model);
            });
            
            return modelList;
        }
        private bool hayStock(Producto producto, int cantidad)
        {
            return producto.stock >= cantidad;
        }

        private int getCantidadFromSession(int codigo)
        {
            int cantidadFromSession = 0;
            List<DetalleViewModel> detalles = getDetallesListFromSession();
            detalles.ForEach(d =>
            {
                if (d.SelectedProductoId == codigo)
                    cantidadFromSession += d.cantidad;
            });
            return cantidadFromSession;
        }

        private Detalle createDetalleFromModel(DetalleViewModel detalleViewModel)
        {
            Detalle detalle = new Detalle();
            detalle.codProd = detalleViewModel.SelectedProductoId;
            detalle.cantidad = detalleViewModel.cantidad;
            return detalle;
        }

        public ActionResult ConfirmarPedido()
        {
            //recién en este punto creamos el pedido que va a contener a los detalles
            Pedido pedido = crearPedido(getIntFromSessionField(Session["codCliente"].ToString()));
            //pedimos a la Session la lista de detalles (modelos) del pedido
            List<DetalleViewModel> modelList = getDetallesListFromSession();
            modelList.ForEach(model =>
            {
                // recorremos la lista de modelos, y por cada uno actualizamos el stock (solo podrán estar
                // en la lista si hay stock disponible). Luego creamos un Detalle a partir del modelo y lo
                // insertamos en la tabla de detalles. 
                actualizarStock(model.SelectedProductoId, model.cantidad);
                Detalle detalle = createDetalleFromModel(model);
                detalle.numPed = pedido.numPed;
                db.Detalle.Add(detalle);
                db.SaveChanges();
            });
            return RedirectToAction("Index");
        }

        private void actualizarStock(int codigo, int cantidad)
        {
            Producto producto = db.Producto.Where(p => p.codigo == codigo).FirstOrDefault();
            producto.stock -= cantidad;
            db.SaveChanges();
        }

        public ActionResult CancelarPedido()
        {
            //Ya que no insertamos detalles ni pedidos hasta la confirmación, sólo tenemos que volver al Index
            return RedirectToAction("Index");
        }


        private Cliente getClienteFromPedido(int numPed)
        {
            Pedido pedido = db.Pedido.Find(numPed);
            Cliente cliente = null;
            if (pedido != null)
                cliente = db.Cliente.Find(pedido.idCliente);
            return cliente;
        }

        // GET: Pedidos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            List<DetalleViewModel> detalles = getDetallesFromPedido(pedido.numPed);
            ViewBag.detalles = detalles;
            PedidoModel model = convertPedidoToPedidoModel(pedido);
            return View(model);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
