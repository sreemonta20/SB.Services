using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
{
    public class DataParameterManager
    {
        public static IDbDataParameter CreateParameter(string providerName, string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            IDbDataParameter? parameter = null;
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    return CreateSqlParameter(name, value, dbType, direction);
                case "System.Data.OracleClient":
                    return CreateOracleParameter(name, value, dbType, direction);
                case "System.Data.OleDb":
                    break;
                case "System.Data.Odbc":
                    break;
                default:
                    break;
            }

            return parameter;
        }

        public static IDbDataParameter CreateParameter(string providerName, string name, int size, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            IDbDataParameter? parameter = null;
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    return CreateSqlParameter(name, size, value, dbType, direction);
                case "System.Data.OracleClient":
                    return CreateOracleParameter(name, size, value, dbType, direction);
                case "System.Data.OleDb":
                    break;
                case "System.Data.Odbc":
                    break;
                default:
                    break;
            }

            return parameter;
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
    }
}
