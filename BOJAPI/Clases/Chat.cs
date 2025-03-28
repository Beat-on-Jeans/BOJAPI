using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
    public class Chat
    {
        public int ID { get; set; }
        public int UsuarioMobil_Musico_ID { get; set; }
        public int UsuarioMobil_Local_ID { get; set; }
        public virtual ICollection<Mensajes> Mensajes { get; set; }
    }
}