using InvernaderoInteligente.Data.DTOs;
using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Services {
  public class LecturaSensorService {
    private readonly IMongoCollection<LecturaSensorModel> _lecturaSensors;
    private readonly IMongoCollection<SensorModel> _sensores;

    public LecturaSensorService (IMongoDatabase database) {
      _lecturaSensors = database.GetCollection<LecturaSensorModel> ("LecturaSensor");
      _sensores = database.GetCollection<SensorModel> ("Sensor");
    }

    #region ObtenerLecturas
    // Método para obtener las lecturas de los sensores, incluyendo el nombre del invernadero
    public async Task<List<SensorLecturaDTO>> ObtenerLecturasConInvernadero () {
      var lecturas = await _lecturaSensors.Find (_ => true).ToListAsync ();  // Obtener todas las lecturas
      var lecturasConInvernadero = new List<SensorLecturaDTO> ();

      foreach (var lectura in lecturas) {
        var sensor = await _sensores.Find (s => s.SensorId == lectura.SensorId).FirstOrDefaultAsync ();  // Obtener el sensor por ID
        if (sensor != null) {
          // Crear el DTO con los datos necesarios
          var lecturaDTO = new SensorLecturaDTO {
            LecturaId = lectura.LecturaId,
            SensorId = lectura.SensorId,
            Valor = lectura.Valor,
            Estado = lectura.Estado,
            FechaLectura = lectura.FechaLectura,  // Formatear fecha a ISO 8601
            Invernadero = sensor.Invernadero
          };

          lecturasConInvernadero.Add (lecturaDTO);
        }
      }

      return lecturasConInvernadero;
    }
    #endregion

    #region RegistrarLectura
    public async Task RegistrarLectura (LecturaSensorModel lectura) {
      // Verificar que el SensorId existe en la colección de sensores
      var sensor = await _sensores.Find (s => s.SensorId == lectura.SensorId).FirstOrDefaultAsync ();

      if (sensor == null) {
        throw new Exception ("El SensorId no existe en la base de datos.");
      }
      lectura.Invernadero = sensor.Invernadero;
      // Si el sensor existe, registrar la lectura
      await _lecturaSensors.InsertOneAsync (lectura);  // Insertar la lectura en la colección LecturaSensors
    }
    #endregion

  }

}
