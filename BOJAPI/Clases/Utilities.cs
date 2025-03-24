using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BOJAPI.Clases
{
    public class Utilities
    {
        public static String MissatgeError(SqlException sqlException)
        {
            String missatge = "";

            switch (sqlException.Number)
            {
                case 2:
                    missatge = "El servidor no està operatiu";
                    break;

                case 53:
                    missatge = "No hi ha connexió amb el servidor de base de dades";
                    break;
                case 547:
                    missatge = "EL registre té dades relacionades";
                    break;

                case 2601:
                    missatge = "Registre duplicat";
                    break;

                case 2627:
                    missatge = "Registre duplicat";
                    break;

                case 4060:
                    missatge = "No es pot obrir la base de dades";
                    break;

                case 18456:
                    missatge = "Error a l'iniciar la sessió";
                    break;

                default:
                    missatge = sqlException.Number + " - " + sqlException.Message;
                    break;

            }
            return missatge;
        }
    }
}