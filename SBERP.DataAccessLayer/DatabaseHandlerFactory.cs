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

        #region Deprecated
        //public IDatabaseHandler CreateDatabaseHandler(string connectionString, string provider)
        //{
        //    IDatabaseHandler? database;
        //    switch (provider)
        //    {
        //        case "Microsoft.Data.SqlClient":
        //            database = new SqlServerDataHandler(connectionString);
        //            break;

        //        case "System.Data.OracleClient":
        //            database = new OracleDataHandler(connectionString);
        //            break;

        //        case "System.Data.OleDb":
        //            database = new OleDataHandler(connectionString);
        //            break;

        //        case "System.Data.Odbc":
        //            database = new OdbcDataHandler(connectionString);
        //            break;
        //        default:
        //            database = null;
        //            break;
        //    }
        //    return database;

        //}
        #endregion

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
