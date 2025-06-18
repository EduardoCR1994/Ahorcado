using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ahorcado.Models
{
    public class EscalafonViewModel
    {
        public int Identificacion { get; set; }
        public string Nombre { get; set; }
        public int Marcador { get; set; }
        public int Ganadas { get; set; }
        public int Perdidas { get; set; }
    }
}
