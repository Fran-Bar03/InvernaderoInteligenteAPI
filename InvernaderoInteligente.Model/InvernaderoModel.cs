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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? InvernaderoId { get; set; }

        [BsonElement("Nombre")]
        public string? Nombre { get; set; }

        [BsonElement("NombrePlanta")]
        public string? NombrePlanta { get; set; }

        [BsonElement("TipoPlanta")]
        public string? TipoPlanta { get; set; }

        [BsonElement("Imagen")]
        public string? Imagen { get; set; }

        [BsonElement("MinTemperatura")]
        public decimal? MinTemperatura { get; set; }

        [BsonElement("MaxTemperatura")]
        public decimal? MaxTemperatura { get;set; }

        [BsonElement("MinHumedad")]
        public decimal? MinHumedad { get; set; }

        [BsonElement("MaxHumedad")]
        public decimal? MaxHumedad { get;set; }


        [BsonElement("Usuarios")]
    public List<string>? Usuarios { get; set; } = new List<string>();

        [BsonElement("Sensores")]
        public List<string>? Sensores { get; set; } = new List<string>();

    }
}
