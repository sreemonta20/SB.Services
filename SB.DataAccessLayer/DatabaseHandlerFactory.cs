using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
{
    public class DatabaseHandlerFactory : IDatabaseHandlerFactory
    {

        public IDatabaseHandler CreateDatabaseHandler(string connectionString, string provider)
        {
            IDatabaseHandler? database;
            switch (provider)
            {
                case "Microsoft.Data.SqlClient":
                    database = new SqlServerDataHandler(connectionString);
                    break;

                case "System.Data.OracleClient":
                    database = new OracleDataHandler(connectionString);
                    break;

                case "System.Data.OleDb":
                    database = new OleDataHandler(connectionString);
                    break;

                case "System.Data.Odbc":
                    database = new OdbcDataHandler(connectionString);
                    break;
                default:
                    database = null;
                    break;
            }
            return database;

        }
    }
}
