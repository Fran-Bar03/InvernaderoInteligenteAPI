using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace InvernaderoInteligente.Model
{
    public class UsuarioModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UsuarioId { get; set; }

        [BsonElement("NombreCompleto")]
        public string? NombreCompleto { get;set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Contrasena")]
        public string? Contrasena { get; set; }

        [BsonElement ("Rol")]
        public int Rol { get; set; } 
    } 
}
