using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Mono.Data.Sqlite;
using FoxyORM;

[TestFixture]
public class TestMain
{
	private SqliteConnection conn;
	
	[SetUp]
	public void Setup() {
		conn = new SqliteConnection("URI=file::memory:,version=3");
		conn.Open();
		createDatabase(conn);
	}

	[Test]
	public void runSimpleQuery() {
        var ctx = new DbContext<SqliteConnection, SqliteDataAdapter, SqliteCommandBuilder>(conn);
		var users = ctx.exec("SELECT * from user");
		Assert.Greater(users.Count(), 0);
	}
	
	private void createDatabase(SqliteConnection conn) {
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
}
