using System;
using System.Data;
using System.Dynamic;

namespace FoxyORM {
    public class DbExpandoObject: DynamicObject {
      private DataRow _row;
      
      public DbExpandoObject(DataRow row) {
        _row = row;
      }
      
      public override bool TryGetMember(GetMemberBinder binder, out object result)
      {
        result = null;
        bool ret=false;
        try {
          result = _row[binder.Name];
          ret = true;
        }
        catch(Exception ex) {
        }
        return ret;
      }
      
      public override bool TrySetMember(SetMemberBinder binder, object value)
      {
        _row[binder.Name] = value;
        return true;
      }
    }
}
