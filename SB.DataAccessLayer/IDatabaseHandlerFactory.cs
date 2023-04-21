using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.DataAccessLayer
{
    public interface IDatabaseHandlerFactory
    {
        IDatabaseHandler CreateDatabaseHandler(string connectionString, string provider);
    }
}
