using System.Linq;
using System.Web.Mvc;
using Ahorcado.Models;

namespace Ahorcado.Controllers
{
    public class EscalafonController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new AhorcadoDBEntities())
            {
                var escalafon = db.Escalafon
                    .OrderByDescending(e => e.Marcador)
                    .ToList();

                return View(escalafon);
            }
        }
    }
}
