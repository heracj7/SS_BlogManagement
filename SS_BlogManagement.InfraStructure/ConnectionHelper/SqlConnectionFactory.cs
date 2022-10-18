using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SS_BlogManagement.InfraStructure.ConnectionHelper
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection createConnection();
        void closeConnection();
    }

    public class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
             _connectionString = connectionString;
        }

        public IDbConnection createConnection()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        public void closeConnection() => createConnection().Dispose();

    }
}
