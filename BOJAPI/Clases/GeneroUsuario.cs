using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
    public class GeneroUsuario
    {
        public int ID { get; set; }
        public int Genero_Id { get; set; }
        public int Usuario_Id { get; set; }

        // Referencia a la entidad Generos_Musicales
        public virtual Generos_Musicales Genero { get; set; }
    }

}