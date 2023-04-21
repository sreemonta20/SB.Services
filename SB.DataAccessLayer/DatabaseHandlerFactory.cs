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
        //private ConnectionStringSettings connectionStringSettings;

        //public DatabaseHandlerFactory(string connectionStringName)
        //{
        //    connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        //}

        //public IDatabaseHandler CreateDatabase()
        //{
        //    IDatabaseHandler database = null;

        //    switch (connectionStringSettings.ProviderName)
        //    {
        //        case "Microsoft.Data.SqlClient":
        //            database = new SqlServerDataHandler(connectionStringSettings.ConnectionString);
        //            break;
        //        case "System.Data.OracleClient":
        //            database = new OracleDataHandler(connectionStringSettings.ConnectionString);
        //            break;
        //        case "System.Data.OleDb":
        //            database = new OleDataHandler(connectionStringSettings.ConnectionString);
        //            break;
        //        case "System.Data.Odbc":
        //            database = new OdbcDataHandler(connectionStringSettings.ConnectionString);
        //            break;
        //    }

        //    return database;
        //}

        //public string GetProviderName()
        //{
        //    return connectionStringSettings.ProviderName;
        //}
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
