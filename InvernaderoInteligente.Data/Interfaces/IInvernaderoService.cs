using InvernaderoInteligente.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Interfaces
{
    public interface IInvernaderoService
    {

        public Task<InvernaderoModel?> AgregarInvernadero(InvernaderoModel agregarinvernadero);
        public Task<InvernaderoModel?> ActualizarInvernadero(InvernaderoModel actualizarinvernadero);
        public Task<List<InvernaderoModel?>> ListarInvernaderos();
        public Task<InvernaderoModel> BuscarPorNombre(string nombre);
        public Task<InvernaderoModel> EliminarPorNombre(string nombre);

    }
}
