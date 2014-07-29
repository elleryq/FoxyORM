using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FoxyORM {
    public class DbContext<TConn, TAdapter> 
			where TConn: IDbConnection, new()
			where TAdapter: DbDataAdapter, new()
	{
		private TConn _conn;
		
        public DbContext(IDbConnection conn) {
			_conn = (TConn)conn;
        }

        public IEnumerable<DynamicObject> exec(string sql) {
			if(sql.StartsWith("SELECT")) {
				TAdapter adapter = (TAdapter)Activator.CreateInstance(
				    typeof(TAdapter),
				    new object[] {sql, this._conn});
				DataSet dataset = new DataSet();
				adapter.Fill(dataset);
				return toEnumerable(dataset.Tables[0]);
			}

            return null;
        }

        public IEnumerable<DynamicObject> toEnumerable(DataTable table) {
          List<DbExpandoObject> list = new List<DbExpandoObject>();
          foreach(DataRow row in table.Rows) {
            list.Add(new DbExpandoObject(row));
          }
          return list;
        }
    }
}
