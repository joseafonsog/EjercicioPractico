using EjercicioPractico.Libs.Models;
using System.Collections.Generic;

namespace EjercicioPractico.Libs.Repositories
{
    public interface ILogRepository
    {
        Log Get(int id);

        IEnumerable<Log> GetAll();

        bool Save(Log log);
    }
}
