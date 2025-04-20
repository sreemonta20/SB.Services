using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBERP.DataAccessLayer
{
    public class OleDataHandler : IDatabaseHandler
    {
        private string ConnectionString { get; set; }

        public OleDataHandler(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            return new OleDbConnection(ConnectionString);
            #pragma warning restore CA1416 // Validate platform compatibility
        }

        public void CloseConnection(IDbConnection connection)
        {
            var oleDbConnection = (OleDbConnection)connection;
            #pragma warning disable CA1416 // Validate platform compatibility
            oleDbConnection.Close();
            #pragma warning restore CA1416 // Validate platform compatibility
            oleDbConnection.Dispose();
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            return new OleDbCommand
            {
                CommandText = commandText,
                Connection = (OleDbConnection)connection,
                CommandType = commandType
            };
            #pragma warning restore CA1416 // Validate platform compatibility
        }

        public IDataAdapter CreateAdapter(IDbCommand command)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            return new OleDbDataAdapter((OleDbCommand)command);
            #pragma warning restore CA1416 // Validate platform compatibility
        }

        //public IDbDataParameter CreateParameter(IDbCommand command)
        //{
        //    OleDbCommand SQLcommand = (OleDbCommand)command;
        //    return SQLcommand.CreateParameter();

        //}
        public IDbDataParameter CreateParameter(IDbCommand command)
        {
            if (command is OleDbCommand oleDbCommand)
            {
                #pragma warning disable CA1416 // Validate platform compatibility
                return oleDbCommand.CreateParameter();
                #pragma warning restore CA1416 // Validate platform compatibility
            }

            throw new ArgumentException("The provided command is not of type OleDbCommand.", nameof(command));
        }
    }
}
