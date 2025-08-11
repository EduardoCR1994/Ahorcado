using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ahorcado.Models;

namespace Ahorcado.Controllers
{
    public class PalabrasController : Controller
    {
        private AhorcadoDBEntities db = new AhorcadoDBEntities();

        // GET: Palabras
        public ActionResult Index()
        {
            return View(db.Palabra.ToList());
        }

        // GET: Palabras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Palabra palabra = db.Palabra.Find(id);
            if (palabra == null)
            {
                return HttpNotFound();
            }
            return View(palabra);
        }

        // GET: Palabras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Palabras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PalabraID,Texto,TieneTilde")] Palabra palabra)
        {
            if (ModelState.IsValid)
            {
                if (palabra.Texto.Length < 5 || palabra.Texto.Length > 10)
                {
                    ModelState.AddModelError("Texto", "La palabra debe tener entre 5 y 10 caracteres.");
                    return View(palabra);
                }

                string textoNormalizado = palabra.Texto
                    .ToUpper()
                    .Replace("Á", "A").Replace("É", "E").Replace("Í", "I")
                    .Replace("Ó", "O").Replace("Ú", "U")
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                    .Replace("ó", "o").Replace("ú", "u");

                bool existe = db.Palabra.Any(p =>
                    p.Texto.ToUpper().Replace("Á", "A").Replace("É", "E").Replace("Í", "I")
                    .Replace("Ó", "O").Replace("Ú", "U")
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                    .Replace("ó", "o").Replace("ú", "u") == textoNormalizado);

                if (existe)
                {
                    ModelState.AddModelError("Texto", "Ya existe una palabra similar en el diccionario.");
                    return View(palabra);
                }

                palabra.Usada = false;
                db.Palabra.Add(palabra);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(palabra);
        }

        // GET: Palabras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Palabra palabra = db.Palabra.Find(id);
            if (palabra == null)
            {
                return HttpNotFound();
            }
            return View(palabra);
        }

        // POST: Palabras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PalabraID,Texto,TieneTilde,Usada")] Palabra palabra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(palabra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(palabra);
        }

        // GET: Palabras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Palabra palabra = db.Palabra.Find(id);
            if (palabra == null)
            {
                return HttpNotFound();
            }
            return View(palabra);
        }

        // POST: Palabras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Palabra palabra = db.Palabra.Find(id);
            db.Palabra.Remove(palabra);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: Palabras/ResetUsadas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetUsadas()
        {
            // Opción rápida/eficiente (una sola sentencia SQL)
            db.Database.ExecuteSqlCommand("UPDATE dbo.Palabra SET Usada = 0 WHERE Usada = 1");
            TempData["Msg"] = "Se reiniciaron todas las palabras usadas.";
            return RedirectToAction("Index");
        }

    }
}