using System;
using System.Data;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NotORM {
    public class DbContext {
        public DbContext(IDbConnection conn) {
        }

        public IEnumerable<DynamicObject> exec(string sql) {
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
