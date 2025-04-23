using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOJAPI.Clases
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using BOJAPI.Models;

    public class Incidencia
    {
        public int ID { get; set; }

        public int Usuario_ID { get; set; }

        public int Tecnico_ID { get; set; }

        public int Tipo_Incidencia_ID { get; set; }

        [Required]
        public DateTime Fecha_Creacion { get; set; }

        public DateTime? Fecha_Cierre { get; set; }

    }

}
