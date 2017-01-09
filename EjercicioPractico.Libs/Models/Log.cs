using System;

namespace EjercicioPractico.Libs.Models
{
    public class Log
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return Date.ToShortDateString() + " - [" + Type.ToString().ToUpper() + "]: " + Message;
        }
    }
}
