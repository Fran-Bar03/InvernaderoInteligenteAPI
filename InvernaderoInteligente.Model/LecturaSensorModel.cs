using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Model {
  public class LecturaSensorModel 
  {
    [BsonId]
    [BsonRepresentation (BsonType.ObjectId)]
    public string LecturaId { get; set; }

    [BsonElement ("SensorId")]
    public string SensorId { get; set; }  

    [BsonElement ("Valor")]
    public decimal Valor { get; set; }  

    [BsonElement ("Estado")]
    public bool Estado { get; set; }  

    [BsonElement ("FechaLectura")]
    public DateTime FechaLectura { get; set; }  

    [BsonElement ("Invernadero")]
    public string Invernadero { get; set; }
  }
}
