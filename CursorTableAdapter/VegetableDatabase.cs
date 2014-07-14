using System;
using Android.Database.Sqlite;
using Android.Content;

namespace CursorTableAdapter {
    class VegetableDatabase  : SQLiteOpenHelper {
        public static readonly string create_table_sql =
            "CREATE TABLE [vegetables] ([_id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [name] TEXT NOT NULL UNIQUE)";
        public static readonly string DatabaseName = "vegetables.db";
        public static readonly int DatabaseVersion = 1;
        public VegetableDatabase(Context context) : base(context, DatabaseName, null, DatabaseVersion) { }
        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(create_table_sql);
            // seed with data
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Vegetables')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Fruits')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Flower Buds')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Legumes')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Bulbs')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Tubers')");
        }
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {   // not 
            throw new NotImplementedException();
        }
    }
}
