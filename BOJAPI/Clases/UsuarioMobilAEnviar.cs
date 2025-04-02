using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
	public class UsuarioMobilAEnviar
	{
        public int ID { get; set; } 
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double? ValoracionTotal { get; set; }
        public int ROL_ID { get; set; }
        public string Url_Imagen { get; set; }
        public string Ubicacion { get; set; }
    }
}