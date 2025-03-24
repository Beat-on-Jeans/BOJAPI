using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
    public class Mensajes
    {
        public int ID { get; set; }
        public int Chat_ID { get; set; }
        public int Emisor_ID { get; set; }
        public string Mensaje { get; set; }
        public DateTime Hora { get; set; }

        // Relación inversa, si se desea
        public virtual Chat Chat { get; set; }
    }
}