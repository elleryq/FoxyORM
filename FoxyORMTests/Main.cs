using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using FoxyORM;

public class FoxyORMTests {
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
	
	public static void createDatabase(SqliteConnection conn) {
		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = "" +
			"CREATE TABLE user (" + 
			"	id INTEGER NOT NULL, " +
			"	active BOOLEAN NOT NULL, " +
			"	email VARCHAR(255) NOT NULL, " +
			"	password VARCHAR(255) NOT NULL, " +
			"	username VARCHAR(50) NOT NULL, " +
			"	confirmed_at DATETIME, " +
			"	reset_password_token VARCHAR(100) NOT NULL, user_profile_id INTEGER, " +
			"	PRIMARY KEY (id), " +
			"	CHECK (active IN (0, 1)), " +
			"	UNIQUE (email), " +
			"	UNIQUE (username));";
		cmd.CommandType = CommandType.Text;
		cmd.ExecuteNonQuery();
		
		for(int i=0; i<5; i++) {
			cmd.CommandText = "INSERT INTO user (id, active, email, password, username, reset_password_token)" +
				"VALUES (@id, @active, @email, @password, @username, @reset_password_token)";
			cmd.Parameters.Add("@id", DbType.Int32).Value = i;
			cmd.Parameters.Add ("@active", DbType.Boolean).Value = true;
			cmd.Parameters.Add ("@email", DbType.String).Value = String.Format ("user{0}@example.com", i);
			cmd.Parameters.Add ("@password", DbType.String).Value = "none";
			cmd.Parameters.Add ("@username", DbType.String).Value = String.Format ("user{0}", i);
			cmd.Parameters.Add ("@reset_password_token", DbType.String).Value = "";
			cmd.ExecuteNonQuery();
		}
	}

    public static void Main()
    {
		SqliteConnection conn = new SqliteConnection("URI=file::memory:,version=3");
		conn.Open();
		createDatabase(conn);
		
        var ctx = new DbContext<SqliteConnection, SqliteDataAdapter, SqliteCommandBuilder>(conn);
		var users = ctx.exec("SELECT * from user");
		Console.WriteLine ("=== From SQLite in memory ===");
		Console.WriteLine ("users.count = {0}", users.Count());
        foreach(dynamic user in users) {
            Console.WriteLine("{0} {{ id={1} email={2} }}", user.username, user.id, user.email);
        }
		
        var ctx2 = new DbContext<SqliteConnection, SqliteDataAdapter, SqliteCommandBuilder>(conn);
		var users2 = ctx2.exec("SELECT * from user");
		var enumerator = users2.GetEnumerator();
		enumerator.MoveNext();
		dynamic firstUser = enumerator.Current;
		Console.WriteLine ("first user's username is " + firstUser.username);
		firstUser.username="New User!!";
		ctx2.commit();
		
		Console.WriteLine ("=== After Update, From SQLite in memory ===");
		Console.WriteLine ("users.count = {0}", users2.Count());
        foreach(dynamic user in users2) {
            Console.WriteLine("{0} {{ id={1} email={2} }}", user.username, user.id, user.email);
        }

		// Test toEnumerable().
		Console.WriteLine ("=== From DataSet/DataTable ===");
        DataSet ds = createDataSet();
        var source = ctx.toEnumerable(ds.Tables[0]);
        var query = from obj in source select obj;
        foreach(dynamic obj in query) {
            Console.WriteLine("id={0} ParentItem={1}", obj.id, obj.ParentItem);
        }
		
		conn.Close();
    }
}
