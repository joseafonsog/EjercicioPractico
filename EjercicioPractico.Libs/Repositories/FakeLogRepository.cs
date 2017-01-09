using EjercicioPractico.Libs.Models;
using System.Collections.Generic;

namespace EjercicioPractico.Libs.Repositories
{
    public class FakeLogRepository : ILogRepository
    {
        private readonly IList<Log> _logList = new List<Log>();

        public Log Get(int id)
        {
            return _logList[id];
        }

        public IEnumerable<Log> GetAll()
        {
            return _logList;
        }

        public bool Save(Log log)
        {
            _logList.Add(log);
            return true;
        }
    }
}
