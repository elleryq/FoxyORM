using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using FoxyORM;

public class FoxPro {
    public static DataSet createDataSet() {
        DataTable table = new DataTable("Example");
        DataColumn column;
        DataRow row;

        // Create new DataColumn, set DataType, 
        // ColumnName and add to DataTable.    
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.Int32");
        column.ColumnName = "id";
        column.ReadOnly = true;
        column.Unique = true;
        // Add the Column to the DataColumnCollection.
        table.Columns.Add(column);

        // Create second column.
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "ParentItem";
        column.AutoIncrement = false;
        column.Caption = "ParentItem";
        column.ReadOnly = false;
        column.Unique = false;
        // Add the column to the table.
        table.Columns.Add(column);

        // Make the ID column the primary key column.
        DataColumn[] PrimaryKeyColumns = new DataColumn[1];
        PrimaryKeyColumns[0] = table.Columns["id"];
        table.PrimaryKey = PrimaryKeyColumns;

        // Create three new DataRow objects and add 
        // them to the DataTable
        for (int i = 0; i<= 2; i++)
        {
            row = table.NewRow();
            row["id"] = i;
            row["ParentItem"] = "ParentItem " + i;
            table.Rows.Add(row);
        }
        
        // Instantiate the DataSet variable.
        DataSet dataSet = new DataSet();
        // Add the new DataTable to the DataSet.
        dataSet.Tables.Add(table);

        return dataSet;
    }

    public static void Main()
    {
        DataSet ds = createDataSet();
        var ctx = new DbContext(null);
        var source = ctx.toEnumerable(ds.Tables[0]);
        var query = from obj in source select obj;
        foreach(dynamic obj in query) {
            Console.WriteLine("id={0}", obj.id);
        }
    }
}
