using InvernaderoInteligente.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Interfaces
{
    public interface ISensorService
    {

        
        public Task<SensorModel> AgregarSensor(SensorModel sensorModel);

        public Task<List<SensorModel>> MostrarSensores();

        public Task<SensorModel> BuscarSensor(string Tipo);

        public Task EliminarSensor(string Tipo);
    }
}
