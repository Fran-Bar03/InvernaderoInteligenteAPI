using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Model
{
    public class UsuarioInvernaderoModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public int UsuarioInvernadero_id { get; set; }

        [BsonElement("Usuario_id")]
        public string? Usuario_id { get; set; }

        [BsonElement("Invernadero_id")]
        public string? Invernadero_id { get; set; }

        [BsonElement("Rol")]
        public int Rol {  get; set; }
    }
}
