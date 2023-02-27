using System.Text.Json.Serialization;

namespace APIDoubleV.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Estatus { get; set; }
    }

}
