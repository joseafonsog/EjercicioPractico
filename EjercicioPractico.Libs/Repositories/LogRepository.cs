using EjercicioPractico.Libs.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EjercicioPractico.Libs.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly string _table;
        private readonly SqlConnection _conn;

        public LogRepository(string connectionString, string table)
        {
            _table = table;
            _conn = new SqlConnection(connectionString);
        }
        public Log Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Log> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Save(Log log)
        {
            _conn.Open();

            var command = _conn.CreateCommand();

            var transaction = _conn.BeginTransaction("Insert Logs");
            command.Connection = _conn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "INSERT INTO " + _table + " VALUES ('" + log.Message + "','" + log.Type + "','" + log.Date.ToShortDateString() + "')";
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
            _conn.Close();
            _conn.Dispose();

            return true;
        }
    }
}
