using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBERP.DataAccessLayer
{
    public class DatabaseHandlerFactory : IDatabaseHandlerFactory
    {


        public IDatabaseHandler CreateDatabaseHandler(string connectionString, string provider)
        {
            return provider switch
            {
                "Microsoft.Data.SqlClient" => new SqlServerDataHandler(connectionString),
                "System.Data.OracleClient" => new OracleDataHandler(connectionString),
                "System.Data.OleDb" => new OleDataHandler(connectionString),
                "System.Data.Odbc" => new OdbcDataHandler(connectionString),
                _ => throw new ArgumentException($"Unsupported provider: {provider}", nameof(provider))
            };
        }
    }
}
