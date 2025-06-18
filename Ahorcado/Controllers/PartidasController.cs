using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ahorcado.Models;

namespace Ahorcado.Controllers
{
    public class PartidasController : Controller
    {
        private AhorcadoDBEntities db = new AhorcadoDBEntities();

        // GET: Partidas
        public ActionResult Index()
        {
            var partidas = db.Partida.Include(p => p.Jugador).Include(p => p.Palabra);
            return View(partidas.ToList());
        }

        // GET: Partidas/Create
        public ActionResult Create()
        {
            ViewBag.JugadorID = new SelectList(db.Jugador, "Identificacion", "Nombre");
            ViewBag.Nivel = new SelectList(new[] { "Facil", "Normal", "Dificil" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int JugadorID, string Nivel)
        {
            if (string.IsNullOrEmpty(Nivel) || !(new[] { "Facil", "Normal", "Dificil" }).Contains(Nivel))
            {
                ModelState.AddModelError("Nivel", "Debe seleccionar un nivel válido.");
            }

            if (ModelState.IsValid)
            {
                var palabra = db.Palabra.FirstOrDefault(p => !p.Usada);
                if (palabra == null)
                {
                    ModelState.AddModelError("", "No hay palabras disponibles en el diccionario.");
                    ViewBag.JugadorID = new SelectList(db.Jugador, "Identificacion", "Nombre", JugadorID);
                    ViewBag.Nivel = new SelectList(new[] { "Facil", "Normal", "Dificil" }, Nivel);
                    return View(new Partida { JugadorID = JugadorID, Nivel = Nivel });
                }

                var partida = new Partida
                {
                    JugadorID = JugadorID,
                    PalabraID = palabra.PalabraID,
                    Nivel = Nivel,
                    FechaInicio = DateTime.Now,
                    Resultado = "Pendiente", // Usa 'Pendiente' si tu constraint lo permite, o cambia a null si la BD lo acepta
                    DuracionSegundos = 0
                };

                palabra.Usada = true;
                db.Partida.Add(partida);
                db.SaveChanges();

                return RedirectToAction("Play", new { id = partida.PartidaID });
            }

            ViewBag.JugadorID = new SelectList(db.Jugador, "Identificacion", "Nombre", JugadorID);
            ViewBag.Nivel = new SelectList(new[] { "Facil", "Normal", "Dificil" }, Nivel);
            return View(new Partida { JugadorID = JugadorID, Nivel = Nivel });
        }



        // GET: Partidas/Play/{id}
        public ActionResult Play(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var partida = db.Partida.Include(p => p.Palabra).Include(p => p.Jugador).FirstOrDefault(p => p.PartidaID == id);
            if (partida == null) return HttpNotFound();

            // lógica de vista de juego pendiente
            return View(partida);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public JsonResult FinalizarPartida(int id, bool ganada, int duracion)
        {
            using (var db = new AhorcadoDBEntities())
            {
                var partida = db.Partida.Find(id);
                if (partida != null)
                {
                    partida.Resultado = ganada ? "Ganada" : "Perdida";
                    partida.DuracionSegundos = duracion;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true });
        }
        public ActionResult Escalafon()
        {
            var escalafon = db.Database.SqlQuery<EscalafonViewModel>(
                @"SELECT 
            j.Identificacion,
            j.Nombre,
            SUM(CASE 
                WHEN p.Resultado = 'Ganada' AND p.Nivel = 'Facil' THEN 1
                WHEN p.Resultado = 'Ganada' AND p.Nivel = 'Normal' THEN 2
                WHEN p.Resultado = 'Ganada' AND p.Nivel = 'Dificil' THEN 3
                WHEN p.Resultado = 'Perdida' AND p.Nivel = 'Facil' THEN -1
                WHEN p.Resultado = 'Perdida' AND p.Nivel = 'Normal' THEN -2
                WHEN p.Resultado = 'Perdida' AND p.Nivel = 'Dificil' THEN -3
                ELSE 0 END) AS Marcador,
            COUNT(CASE WHEN p.Resultado = 'Ganada' THEN 1 END) AS Ganadas,
            COUNT(CASE WHEN p.Resultado = 'Perdida' THEN 1 END) AS Perdidas
        FROM Jugador j
        LEFT JOIN Partida p ON j.Identificacion = p.JugadorID
        GROUP BY j.Identificacion, j.Nombre"
            ).ToList();

            return View(escalafon);
        }

    }
}
