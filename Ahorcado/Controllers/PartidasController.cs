using Ahorcado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ahorcado.Models;

namespace Ahorcado.Controllers
{
    public class PartidasController : Controller
    {
        public ActionResult Jugar(int idPartida)
        {
            using (var db = new AhorcadoDBEntities()) 
            {
                var partida = db.Partida.Find(idPartida);
                if (partida == null)
                    return HttpNotFound();

                var palabra = partida.Palabra.Texto.ToUpper();

                var intentos = db.Intento
                    .Where(i => i.PartidaID == idPartida)
                    .ToList();

                // Obtener letras adivinadas
                var letrasAdivinadas = intentos
                    .Where(i => i.EsCorrecta)
                    .Select(i => i.Letra.ToUpper())
                    .ToList();

                // Palabra en progreso
                var progreso = string.Join(" ", palabra.Select(c =>
                    letrasAdivinadas.Contains(c.ToString().ToUpper()) ? c : '_'));

                ViewBag.PalabraProgreso = progreso;
                ViewBag.IntentosFallidos = intentos.Count(i => !i.EsCorrecta);
                ViewBag.IntentosRestantes = 5 - ViewBag.IntentosFallidos;
                ViewBag.TodasLetras = Enumerable.Range('A', 26).Select(x => ((char)x).ToString());

                ViewBag.LetrasUsadas = intentos
                    .Select(i => i.Letra.ToUpper())
                    .ToList();

                ViewBag.PartidaID = idPartida;
                ViewBag.Resultado = partida.Resultado;

                return View();
            }
        }
    }
}
