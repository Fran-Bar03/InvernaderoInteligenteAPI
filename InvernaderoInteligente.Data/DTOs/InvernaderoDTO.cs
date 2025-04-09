using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs {
  public class InvernaderoDTO 
  {
    [BsonId]
    [BsonRepresentation (BsonType.ObjectId)]
    public string? InvernaderoId { get; set; }
    public string Nombre { get; set; }
    public string NombrePlanta { get; set; }
    public string TipoPlanta { get; set; }
    public string Imagen { get; set; }
  }
}
