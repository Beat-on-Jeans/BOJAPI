﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
	public class UsuarioRecibido
	{
        public int ID { get; set; } 
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int ROL_ID { get; set; }
        public string Url_Imagen { get; set; }
        public string Ubicacion { get; set; }
    }
}