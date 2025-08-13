using Ahorcado.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

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
        public ActionResult Create(int? JugadorID, string Nivel = null)
        {
            ViewBag.JugadorID = new SelectList(db.Jugador, "Identificacion", "Nombre", JugadorID);
            ViewBag.Nivel = new SelectList(
                new[] {
            new { Value="Facil",   Text="Fácil"   },
            new { Value="Normal",  Text="Normal"  },
            new { Value="Dificil", Text="Difícil" }
                },
                "Value", "Text", Nivel
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int JugadorID, string Nivel)
        {
            var permitidos = new[] { "Facil", "Normal", "Dificil" };
            // normaliza entrada común (quita tildes)
            var mapa = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Fácil"] = "Facil",
                ["Facil"] = "Facil",
                ["Normal"] = "Normal",
                ["Difícil"] = "Dificil",
                ["Dificil"] = "Dificil"
            };
            if (!mapa.ContainsKey(Nivel))
                ModelState.AddModelError("Nivel", "Debe seleccionar un nivel válido.");

            if (!ModelState.IsValid)
            {
                ViewBag.JugadorID = new SelectList(db.Jugador, "Identificacion", "Nombre", JugadorID);
                ViewBag.Nivel = new SelectList(
                    new[] {
                new { Value="Facil",   Text="Fácil"   },
                new { Value="Normal",  Text="Normal"  },
                new { Value="Dificil", Text="Difícil" }
                    }, "Value", "Text", Nivel);
                return View(new Partida { JugadorID = JugadorID, Nivel = Nivel });
            }

            var palabra = db.Palabra.FirstOrDefault(p => !p.Usada);
            if (palabra == null) { /* ...tu manejo... */ }

            var partida = new Partida
            {
                JugadorID = JugadorID,
                PalabraID = palabra.PalabraID,
                Nivel = mapa[Nivel], // <-- siempre "Facil"/"Normal"/"Dificil"
                FechaInicio = DateTime.Now,
                Resultado = "Pendiente",
                DuracionSegundos = 0
            };

            palabra.Usada = true;
            db.Partida.Add(partida);
            db.SaveChanges();
            return RedirectToAction("Play", new { id = partida.PartidaID });
        }


        // GET: Partidas/Play/{id}
        public ActionResult Play(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var partida = db.Partida
                .Include(p => p.Palabra)
                .Include(p => p.Jugador)
                .FirstOrDefault(p => p.PartidaID == id);

            if (partida == null) return HttpNotFound();

            if (partida.Palabra != null && partida.Palabra.Texto != null)
            {
                partida.Palabra.Texto = partida.Palabra.Texto.Normalize(NormalizationForm.FormC);
            }

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
            string palabraCorrecta = null;

            using (var db = new AhorcadoDBEntities())
            {
                var partida = db.Partida.Include(p => p.Palabra).FirstOrDefault(p => p.PartidaID == id);
                if (partida != null)
                {
                    partida.Resultado = ganada ? "Ganada" : "Perdida";
                    partida.DuracionSegundos = duracion;
                    if (!ganada)
                    {
                        palabraCorrecta = partida.Palabra.Texto; // Suponiendo que el campo se llama Texto
                    }
                    db.SaveChanges();
                }
            }

            return Json(new
            {
                success = true,
                palabra = palabraCorrecta
            });
        }
        public ActionResult Escalafon()
        {
            var escalafon = db.Database.SqlQuery<EscalafonViewModel>(
                @"
        SELECT 
            j.Identificacion,
            j.Nombre,
            SUM(CASE 
                WHEN p.Resultado = 'Ganada'  AND p.Nivel COLLATE Latin1_General_CI_AI = 'Facil'   THEN 1
                WHEN p.Resultado = 'Ganada'  AND p.Nivel COLLATE Latin1_General_CI_AI = 'Normal'  THEN 2
                WHEN p.Resultado = 'Ganada'  AND p.Nivel COLLATE Latin1_General_CI_AI = 'Dificil' THEN 3
                WHEN p.Resultado = 'Perdida' AND p.Nivel COLLATE Latin1_General_CI_AI = 'Facil'   THEN -1
                WHEN p.Resultado = 'Perdida' AND p.Nivel COLLATE Latin1_General_CI_AI = 'Normal'  THEN -2
                WHEN p.Resultado = 'Perdida' AND p.Nivel COLLATE Latin1_General_CI_AI = 'Dificil' THEN -3
                ELSE 0 END) AS Marcador,
            COUNT(CASE WHEN p.Resultado = 'Ganada'  THEN 1 END) AS Ganadas,
            COUNT(CASE WHEN p.Resultado = 'Perdida' THEN 1 END) AS Perdidas
        FROM Jugador j
        LEFT JOIN Partida p ON j.Identificacion = p.JugadorID
        GROUP BY j.Identificacion, j.Nombre"
            ).ToList();

            return View(escalafon);
        }


    }
}
