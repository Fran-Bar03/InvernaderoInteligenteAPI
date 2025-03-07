using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Model
{
    public class Sensor
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public int Sensor_id { get; set; }

        [BsonElement("Tipo")]
        public string? Tipo { get; set; }

        [BsonElement("Valor")]
        public decimal Valor { get; set; }

        [BsonElement("Estado")]
        public bool Estado { get; set; }

        [BsonElement("FechaLectura")]
        public DateTime FechaLectura { get; set; } = DateTime.UtcNow;


    }
}
