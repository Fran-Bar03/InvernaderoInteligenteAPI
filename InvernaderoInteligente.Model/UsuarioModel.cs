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
        public int Usuario_id { get; set; }

        [BsonElement("Nombre")]
        public string? Nombre { get;set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Contraseña")]
        public string? Contraseña { get; set; }

        [BsonElement("Rol")]
        public int Rol {  get; set; }

        [BsonElement("Inverndaeros")]

        public List<ObjectId> Invernaderos { get; set; }

        

    } 
}
