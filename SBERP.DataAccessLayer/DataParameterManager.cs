using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBERP.DataAccessLayer
{
    public class DataParameterManager
    {
        #region Deprecated
        //public static IDbDataParameter CreateParameter(string providerName, string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        //{
        //    IDbDataParameter? parameter = null;
        //    switch (providerName)
        //    {
        //        case "Microsoft.Data.SqlClient":
        //            return CreateSqlParameter(name, value, dbType, direction);
        //        case "System.Data.OracleClient":
        //            return CreateOracleParameter(name, value, dbType, direction);
        //        case "System.Data.OleDb":
        //            break;
        //        case "System.Data.Odbc":
        //            break;
        //        default:
        //            break;
        //    }

        //    return parameter;
        //}

        //public static IDbDataParameter CreateParameter(string providerName, string name, int size, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        //{
        //    IDbDataParameter? parameter = null;
        //    switch (providerName)
        //    {
        //        case "Microsoft.Data.SqlClient":
        //            return CreateSqlParameter(name, size, value, dbType, direction);
        //        case "System.Data.OracleClient":
        //            return CreateOracleParameter(name, size, value, dbType, direction);
        //        case "System.Data.OleDb":
        //            break;
        //        case "System.Data.Odbc":
        //            break;
        //        default:
        //            break;
        //    }

        //    return parameter;
        //}
        #endregion

        public static IDbDataParameter CreateParameter(string providerName,string name, object value,DbType dbType,ParameterDirection direction = ParameterDirection.Input)
        {
            return providerName switch
            {
                "Microsoft.Data.SqlClient" => CreateSqlParameter(name, value, dbType, direction),
                "System.Data.OracleClient" => CreateOracleParameter(name, value, dbType, direction),
                "System.Data.OleDb" => CreateOleDbParameter(name, value, dbType, direction),
                "System.Data.Odbc" => CreateOdbcParameter(name, value, dbType, direction),
                _ => throw new ArgumentException($"Unsupported provider: {providerName}", nameof(providerName))
            };
        }

        public static IDbDataParameter CreateParameter(string providerName,string name,int size,object value,DbType dbType,ParameterDirection direction = ParameterDirection.Input)
        {
            return providerName switch
            {
                "Microsoft.Data.SqlClient" => CreateSqlParameter(name, size, value, dbType, direction),
                "System.Data.OracleClient" => CreateOracleParameter(name, size, value, dbType, direction),
                "System.Data.OleDb" => CreateOleDbParameter(name, size, value, dbType, direction),
                "System.Data.Odbc" => CreateOdbcParameter(name, size, value, dbType, direction),
                _ => throw new ArgumentException($"Unsupported provider: {providerName}", nameof(providerName))
            };
        }
        

        private static IDbDataParameter CreateSqlParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateSqlParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateOracleParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateOracleParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static IDbDataParameter CreateOleDbParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            // Example implementation for OleDb parameter creation
            #pragma warning disable CA1416 // Validate platform compatibility
            return  new OleDbParameter
            {
                ParameterName = name,
                Value = value ?? DBNull.Value,
                DbType = dbType,
                Direction = direction
            };
            #pragma warning restore CA1416 // Validate platform compatibility

        }

        private static IDbDataParameter CreateOleDbParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            return new OleDbParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
            #pragma warning restore CA1416 // Validate platform compatibility
        }

        private static IDbDataParameter CreateOdbcParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            // Example implementation for Odbc parameter creation
            return new OdbcParameter
            {
                ParameterName = name,
                Value = value ?? DBNull.Value,
                DbType = dbType,
                Direction = direction
            };
        }

        private static IDbDataParameter CreateOdbcParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            // Example implementation for Odbc parameter creation
            return new OdbcParameter
            {
                DbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }
    }
}
