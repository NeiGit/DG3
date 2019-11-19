using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DieteticaG3;
using DieteticaG3.Models;

namespace DieteticaG3.Controllers
{
    public class ProductosController : Controller
    {


        private DieteticaEntities db = new DieteticaEntities();

        // GET: Productos
        public ActionResult Index()
        {
            return View(db.Producto.ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }
        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codigo,nombre,precio,stock")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Producto.Find(id);
            db.Producto.Remove(producto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarProducto(ProductoModel objProductoModel)
        {
            if (ModelState.IsValid)
            {
                if (!db.Producto.Any(p => p.nombre.Equals(objProductoModel.Nombre)))
                {
                    Producto producto = createProducto(objProductoModel);
                    db.Producto.Add(producto);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError("Error", "Ya existe un producto con ese nombre");
                return View(objProductoModel);
            }

            return View(objProductoModel);
        }

        public ActionResult AgregarProducto()
        {
            return View();
        }

        private Producto createProducto(ProductoModel objProductoModel)
        {
            Producto producto = new Producto();
            producto.nombre = objProductoModel.Nombre;
            producto.precio = objProductoModel.Precio;
            producto.stock = objProductoModel.Stock;
            return producto;
        }
    }
}
