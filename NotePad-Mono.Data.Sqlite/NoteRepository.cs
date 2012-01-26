/*
 * Copyright (C) 2012 Xamarin Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace Mono.Samples.Notepad
{
	class NoteRepository
	{
		private static string db_file = "notes.db3";

		private static SqliteConnection GetConnection ()
		{
			var dbPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), db_file);
			bool exists = File.Exists (dbPath);

			if (!exists)
				SqliteConnection.CreateFile (dbPath);

			var conn = new SqliteConnection ("Data Source=" + dbPath);

			if (!exists)
				CreateDatabase (conn);

			return conn;
		}

		private static void CreateDatabase (SqliteConnection connection)
		{
			var sql = "CREATE TABLE ITEMS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Body ntext, Modified datetime);";

			connection.Open ();

			using (var cmd = connection.CreateCommand ()) {
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery ();
			}

			// Create a sample note to get the user started
			sql = "INSERT INTO ITEMS (Body, Modified) VALUES (@Body, @Modified);";

			using (var cmd = connection.CreateCommand ()) {
				cmd.CommandText = sql;
				cmd.Parameters.AddWithValue ("@Body", "Sample Note");
				cmd.Parameters.AddWithValue ("@Modified", DateTime.Now);

				cmd.ExecuteNonQuery ();
			}

			connection.Close ();
		}

		public static IEnumerable<Note> GetAllNotes ()
		{
			var sql = "SELECT * FROM ITEMS;";

			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;

					using (var reader = cmd.ExecuteReader ()) {
						while (reader.Read ())
							yield return new Note (reader.GetInt32 (0), reader.GetString (1), reader.GetDateTime (2)); 
					}
				}
			}
		}

		public static Note GetNote (long id)
		{
			var sql = "SELECT * FROM ITEMS WHERE Id = id;";

			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;

					using (var reader = cmd.ExecuteReader ()) {
						if (reader.Read ())
							return new Note (reader.GetInt32 (0), reader.GetString (1), reader.GetDateTime (2)); 
						else
							return null;
					}
				}
			}
		}

		public static void DeleteNote (Note note)
		{
			var sql = string.Format ("DELETE FROM ITEMS WHERE Id = {0};", note.Id);

			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery ();
				}
			}
		}

		public static void SaveNote (Note note)
		{
			using (var conn = GetConnection ()) {
				conn.Open ();

				using (var cmd = conn.CreateCommand ()) {

					if (note.Id < 0) {
						// Do an insert
						cmd.CommandText = "INSERT INTO ITEMS (Body, Modified) VALUES (@Body, @Modified); SELECT last_insert_rowid();";
						cmd.Parameters.AddWithValue ("@Body", note.Body);
						cmd.Parameters.AddWithValue ("@Modified", DateTime.Now);

						note.Id = (long)cmd.ExecuteScalar ();
					} else {
						// Do an update
						cmd.CommandText = "UPDATE ITEMS SET Body = @Body, Modified = @Modified WHERE Id = @Id";
						cmd.Parameters.AddWithValue ("@Id", note.Id);
						cmd.Parameters.AddWithValue ("@Body", note.Body);
						cmd.Parameters.AddWithValue ("@Modified", DateTime.Now);
					
						cmd.ExecuteNonQuery ();
					}
				}
			}
		}
	}
}