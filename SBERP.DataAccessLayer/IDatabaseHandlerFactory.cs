using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBERP.DataAccessLayer
{
    public interface IDatabaseHandlerFactory
    {
        IDatabaseHandler CreateDatabaseHandler(string connectionString, string provider);
    }
}
