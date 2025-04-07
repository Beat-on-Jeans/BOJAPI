using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
	public class Match
	{
        public int ID { get; set; }
        public int CreadorId { get; set; }
        public int? FinalizadorId { get; set; }
        public int Estado { get; set; }
    }
}