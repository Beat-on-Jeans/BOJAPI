using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
    public class Generos_Usuarios
    {
        public int ID { get; set; }

        public int Genero_Id { get; set; }

        public int Usuario_Id { get; set; }

        public virtual ICollection<GenerosMusicales> Generos { get; set; }
    }
}