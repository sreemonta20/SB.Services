using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
{
    public interface IDatabaseManager
    {
        void InitializeDatabase(string? connStr, string? provideName);
        IDbDataParameter CreateParameter(string? name, object value, DbType dbType);
        IDbDataParameter CreateParameter(string? name, int size, object value, DbType dbType);
        IDbDataParameter CreateParameter(string? name, object value, DbType dbType, ParameterDirection direction);
        Task<DataTable> GetDataTableAsync(string? commandText, CommandType commandType, IDbDataParameter[]? parameters = null);
        Task<IDataReader> GetDataReaderAsync(string? commandText, CommandType commandType, IDbDataParameter[] parameters);
        Task<object> GetScalarValueAsync(string? commandText, CommandType commandType, IDbDataParameter[]? parameters = null);
        Task<object> InsertExecuteScalarAsync(string? commandText, CommandType commandType, IDbDataParameter[] parameters);
        Task<object> InsertExecuteScalarTransAsync(string? commandText, CommandType commandType, IDbDataParameter[] parameters);
        Task<int> InsertExecuteScalarTransAsync(string? commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters);
        Task<object> InsertExecuteNonQueryTransAsync(string? commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters);
        Task<object> UpdateAsync(string? commandText, CommandType commandType, IDbDataParameter[] parameters);
        Task<object> UpdateWithTransactionAsync(string? commandText, CommandType commandType, IDbDataParameter[] parameters);
        Task<object> UpdateWithTransactionAsync(string? commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters);
        Task<object> DeleteAsync(string? commandText, CommandType commandType, IDbDataParameter[]? parameters = null);
        //IDbDataParameter CreateParameter(string v, object? refreshTokenValue, object value);
    }
}
