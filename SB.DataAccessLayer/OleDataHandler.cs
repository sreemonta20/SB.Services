using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
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
            return new OleDbConnection(ConnectionString);
        }

        public void CloseConnection(IDbConnection connection)
        {
            var oleDbConnection = (OleDbConnection)connection;
            oleDbConnection.Close();
            oleDbConnection.Dispose();
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            return new OleDbCommand
            {
                CommandText = commandText,
                Connection = (OleDbConnection)connection,
                CommandType = commandType
            };
        }

        public IDataAdapter CreateAdapter(IDbCommand command)
        {
            return new OleDbDataAdapter((OleDbCommand)command);
        }

        public IDbDataParameter CreateParameter(IDbCommand command)
        {
            OleDbCommand SQLcommand = (OleDbCommand)command;
            return SQLcommand.CreateParameter();
        }
    }
}
