using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Model
{
    public class InvernaderoModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public int Invernadero_id { get; set; }

        [BsonElement("Nombre")]
        public string? Nombre { get; set; }

        [BsonElement("NombrePlanta")]
        public string? NombrePlanta { get; set; }

        [BsonElement("TipoPlanta")]
        public string? TipoPlanta { get; set; }

        [BsonElement("MinTemperatura")]
        public decimal MinTemperatura { get; set; }

        [BsonElement("MaxTemperatura")]
        public decimal MaxTemperatura { get;set; }

        [BsonElement("MinHumedad")]
        public decimal MinHumedad { get; set; }

        [BsonElement("MaxHumedad")]
        public decimal MaxHumedad { get;set; }

        [BsonElement("Usuarios")]
        public List<ObjectId> Usuarios { get; set; }

        [BsonElement("Sensores")]
        public List<Sensor> Sensores { get; set; }

    }
}
