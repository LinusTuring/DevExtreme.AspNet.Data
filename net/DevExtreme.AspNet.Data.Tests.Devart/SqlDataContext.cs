using Devart.Data.Linq;
using Devart.Data.Linq.Mapping;
using Devart.Data.SqlServer.Linq.Provider;
using System.Data;

namespace DevExtreme.AspNet.Data.Tests.Devart {
    [Provider(typeof(SqlDataProvider))]
    class SqlDataContext : DataContext {
        public SqlDataContext(IDbConnection connection) : base(connection) {
        }
    }
}
