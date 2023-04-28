using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly IDatabaseHandlerFactory _dbFactory;
        private IDatabaseHandler database;
        private string provider;
        public DatabaseManager(IDatabaseHandlerFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public void InitializeDatabase(string connStr, string provideName)
        {
            database = _dbFactory.CreateDatabaseHandler(connStr, provideName);
            provider = provideName;
        }


        public IDbConnection GetDatabasecOnnection()
        {
            return database.CreateConnection();
        }

        public void CloseConnection(IDbConnection connection)
        {
            database.CloseConnection(connection);
        }

        #region Exposed Methods
        public IDbDataParameter CreateParameter(string name, object value, DbType dbType)
        {
            return DataParameterManager.CreateParameter(provider, name, value, dbType, ParameterDirection.Input);
        }

        public IDbDataParameter CreateParameter(string name, int size, object value, DbType dbType)
        {
            return DataParameterManager.CreateParameter(provider, name, size, value, dbType, ParameterDirection.Input);
        }

        public IDbDataParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return DataParameterManager.CreateParameter(provider, name, value, dbType, direction);
        }

        public async Task<DataTable> GetDataTableAsync(string commandText, CommandType commandType, IDbDataParameter[]? parameters = null)
        {
            return await Task.Run(() =>
            {
                using IDbConnection connection = database.CreateConnection();
                connection.Open();

                using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                DataSet dataset = new();
                IDataAdapter dataAdaper = database.CreateAdapter(command);
                dataAdaper.Fill(dataset);

                return dataset.Tables[0];
            });

        }

        public async Task<IDataReader> GetDataReaderAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            return await Task.Run(() =>
            {
                IDataReader? reader = null;
                using IDbConnection connection = database.CreateConnection();
                connection.Open();

                IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                reader = command.ExecuteReader();

                return reader;
            });

        }

        public async Task<object> GetScalarValueAsync(string commandText, CommandType commandType, IDbDataParameter[]? parameters = null)
        {
            object? result;
            return await Task.Run(() =>
            {
                using IDbConnection connection = database.CreateConnection();
                connection.Open();

                using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                result = command.ExecuteScalar();
                return result ?? 0;
            });

        }

        public async Task<object> InsertExecuteScalarAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object? result;
            return await Task.Run(() =>
            {

                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    result = command.ExecuteScalar();
                }
                return result ?? 0;
            });
        }

        public async Task<object> InsertExecuteScalarTransAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object? result = 0;
            return await Task.Run(() =>
            {
                IDbTransaction? transactionScope = null;
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();
                    transactionScope = connection.BeginTransaction();

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        result = command.ExecuteScalar();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result ?? 0;
            });
        }

        public async Task<int> InsertExecuteScalarTransAsync(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            int rowsAffected = 0;
            IDbTransaction? transactionScope = null;
            return await Task.Run(() =>
            {
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();

                    using (transactionScope = connection.BeginTransaction(isolationLevel))
                    {
                        try
                        {
                            using (IDbCommand command = database.CreateCommand(commandText, commandType, connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Transaction = transactionScope;

                                // Add parameters
                                if (parameters != null)
                                {
                                    foreach (IDbDataParameter parameter in parameters)
                                    {
                                        command.Parameters.Add(parameter);
                                    }
                                }

                                // Execute the stored procedure and get the rows affected
                                rowsAffected = Convert.ToInt32(command.ExecuteScalar());
                            }

                            // Commit the transaction
                            transactionScope.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            transactionScope.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }

                return rowsAffected;
            });

        }

        #region Insert With Transaction Async Block
        public async Task<object> InsertExecuteNonQueryTransAsync(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            object result = 0;
            return await Task.Run(() =>
            {
                IDbTransaction? transactionScope = null;
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();
                    transactionScope = connection.BeginTransaction(isolationLevel);

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        result = command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            });

        }
        #endregion



        public async Task<object> UpdateAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object result;
            return await Task.Run(() =>
            {
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }


                    result = command.ExecuteNonQuery();
                }
                return result ?? 0;
            });
        }

        public async Task<object> UpdateWithTransactionAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object result = 0;
            return await Task.Run(() =>
            {
                IDbTransaction? transactionScope = null;
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();
                    transactionScope = connection.BeginTransaction();

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        result = command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            });
        }

        public async Task<object> UpdateWithTransactionAsync(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            object result = 0;
            return await Task.Run(() =>
            {
                IDbTransaction? transactionScope = null;
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();
                    transactionScope = connection.BeginTransaction(isolationLevel);

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        result = command.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            });
        }

        public async Task<object> DeleteAsync(string commandText, CommandType commandType, IDbDataParameter[]? parameters = null)
        {
            object result = 0;
            return await Task.Run(() =>
            {
                using (IDbConnection connection = database.CreateConnection())
                {
                    connection.Open();

                    using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    result = command.ExecuteNonQuery();
                }
                return result;
            });
        }
        #endregion


        public IDbDataParameter CreateParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            return DataParameterManager.CreateParameter(provider, name, size, value, dbType, direction);
        }

        public DataSet GetDataSet(string commandText, CommandType commandType, IDbDataParameter[]? parameters = null)
        {
            using IDbConnection connection = database.CreateConnection();
            connection.Open();

            using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            DataSet dataset = new DataSet();
            IDataAdapter dataAdaper = database.CreateAdapter(command);
            dataAdaper.Fill(dataset);

            return dataset;
        }

        public object Insert(string commandText, CommandType commandType, IDbDataParameter[] parameters, out object? result)
        {
            result = 0;
            using (IDbConnection? connection = database.CreateConnection())
            {
                connection.Open();

                using IDbCommand? command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                result = command.ExecuteScalar();
            }

            return result;
        }

        public object InsertWithTransaction(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object result = 0;
            using IDbConnection connection = database.CreateConnection();
            connection.Open();
            IDbTransaction? transactionScope = connection.BeginTransaction();

            using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
            if (parameters != null)
            {
                foreach (IDbDataParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            try
            {
                result = command.ExecuteNonQuery();
                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public object InsertWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            object result = 0;
            using (IDbConnection connection = database.CreateConnection())
            {
                connection.Open();
                IDbTransaction transactionScope = connection.BeginTransaction(isolationLevel);

                using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                try
                {
                    result = command.ExecuteNonQuery();
                    transactionScope.Commit();
                }
                catch (Exception)
                {
                    transactionScope.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public object UpdateWithTransaction(string commandText, CommandType commandType, IDbDataParameter[] parameters)
        {
            object result = 0;
            using (IDbConnection connection = database.CreateConnection())
            {
                connection.Open();
                IDbTransaction transactionScope = connection.BeginTransaction();

                using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                try
                {
                    result = command.ExecuteNonQuery();
                    transactionScope.Commit();
                }
                catch (Exception)
                {
                    transactionScope.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public object UpdateWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
        {
            object result = 0;
            using (IDbConnection connection = database.CreateConnection())
            {
                connection.Open();
                IDbTransaction transactionScope = connection.BeginTransaction(isolationLevel);

                using IDbCommand command = database.CreateCommand(commandText, commandType, connection);
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                try
                {
                    result = command.ExecuteNonQuery();
                    transactionScope.Commit();
                }
                catch (Exception)
                {
                    transactionScope.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
    }
}
