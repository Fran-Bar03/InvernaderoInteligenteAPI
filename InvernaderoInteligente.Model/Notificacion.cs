using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Model
{
    public class Notificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? NotificacionId { get; set; }

        [BsonElement("Mensaje")]
        public string? Mensaje { get; set; }

        [BsonElement("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [BsonElement("Invernadero_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? InvernaderoId { get; set; }

        [BsonElement("TipoEvento")]
        public string? TipoEvento { get; set; }

        [BsonElement("Detalles")]
        public DetallesNotificacion? Detalles { get; set; }
    }

    public class DetallesNotificacion
    {
        [BsonElement("Sensor_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? SensorId { get; set; }

        [BsonElement("Estado")]
        public string? Estado { get; set; }
    }
}
