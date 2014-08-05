using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FoxyORM {
    public class DbContext<TConn, TAdapter, TCommandBuilder> 
			where TConn: IDbConnection, new()
			where TAdapter: DbDataAdapter, new()
			where TCommandBuilder: DbCommandBuilder, new()
	{
		private TConn _conn;
		private TAdapter _adapter;
		private TCommandBuilder _cmdBuilder;
		private DataSet _dataset;
		
        public DbContext(IDbConnection conn) {
			_conn = (TConn)conn;
        }

        public IEnumerable<DynamicObject> exec(string sql) {
			if(sql.StartsWith("SELECT")) {
				_adapter = (TAdapter)Activator.CreateInstance(
				    typeof(TAdapter),
				    new object[] {sql, this._conn});
				_cmdBuilder = (TCommandBuilder)Activator.CreateInstance (
					typeof(TCommandBuilder),
					new object[] {_adapter});
				_dataset = new DataSet();
				_adapter.Fill(_dataset);
				return toEnumerable(_dataset.Tables[0]);
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
		
		public void commit() {
			// TODO: Perform update if dirty.
			_adapter.Update (_dataset.Tables[0]);
		}
    }
}