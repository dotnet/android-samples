using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using Java.Util;
using Android.Database;
using Android.Database.Sqlite;
using Android.Util;

namespace MonoIO
{
	/**
	 * Helper for building selection clauses for {@link SQLiteDatabase}. Each
	 * appended clause is combined using {@code AND}. This class is <em>not</em>
	 * thread safe.
	 */
	public class SelectionBuilder
	{
		private const String TAG = "SelectionBuilder";
	    private const bool LOGV = true;
	
	    private String mTable = null;
	    private Dictionary<String, String> mProjectionMap = new Dictionary<string, string>();
	    private StringBuilder mSelection = new StringBuilder();
	    private List<String> mSelectionArgs = new List<string>();
		
		/**
	     * Reset any internal state, allowing this builder to be recycled.
	     */
	    public SelectionBuilder Reset() {
	        mTable = null;
	        mSelection.Length = 0;
	        mSelectionArgs.Clear();
	        return this;
	    }
		
		
		public SelectionBuilder Where(String selection, string selectionArgs)
		{
			return Where (selection, new [] { selectionArgs } );	
		}
		
		/**
	     * Append the given selection clause to the internal state. Each clause is
	     * surrounded with parenthesis and combined using {@code AND}.
	     */
	    public SelectionBuilder Where(String selection, String[] selectionArgs) {
	        if (TextUtils.IsEmpty(selection)) {
	            if (selectionArgs != null && selectionArgs.Length > 0) {
	                throw new Exception(
	                        "Valid selection required when including arguments=");
	            }
	
	            // Shortcut when clause is empty
	            return this;
	        }
	
	        if (mSelection.Length > 0) {
	            mSelection.Append(" AND ");
	        }
	
	        mSelection.Append("(").Append(selection).Append(")");
	        if (selectionArgs != null) {
	            foreach (String arg in selectionArgs) {
	                if(arg != null)
						mSelectionArgs.Add(arg);
	            }
	        }
	
	        return this;
	    }
		
		public SelectionBuilder Table(String table) {
	        mTable = table;
	        return this;
	    }
		
		private void AssertTable() {
	        if (mTable == null) {
	            throw new Exception("Table not specified");
	        }
	    }
		
		public SelectionBuilder MapToTable(String column, String table) {
	        mProjectionMap.Add(column, table + "." + column);
	        return this;
	    }
	
	    public SelectionBuilder Map(String fromColumn, String toClause) {
	        mProjectionMap.Add(fromColumn, toClause + " AS " + fromColumn);
	        return this;
	    }
		
		/**
	     * Return selection string for current internal state.
	     *
	     * @see #getSelectionArgs()
	     */
	    public String GetSelection() {
	        return mSelection.ToString();
	    }
	
	    /**
	     * Return selection arguments for current internal state.
	     *
	     * @see #getSelection()
	     */
	    public String[] GetSelectionArgs() {
			return mSelectionArgs.ToArray();
	    }
	
	    private void MapColumns(String[] columns) {
	        for (int i = 0; i < columns.Length; i++) {
	            try {
					string target = mProjectionMap[columns[i]];
					columns[i] = target;
				} catch {
				  // Do nothing, it doesn't map	
				}
//				
//				String target = mProjectionMap[columns[i]];
//	            if (target != null) {
//	                columns[i] = target;
//	            }
	        }
	    }
		
		 /**
	     * Execute query using the current internal state as {@code WHERE} clause.
	     */
	    public ICursor Query(SQLiteDatabase db, String[] columns, String orderBy) {
	        return Query(db, columns, null, null, orderBy, null);
	    }
	
	    /**
	     * Execute query using the current internal state as {@code WHERE} clause.
	     */
	    public ICursor Query(SQLiteDatabase db, String[] columns, String groupBy, String having, String orderBy, String limit) {
	        AssertTable();
	        if (columns != null) MapColumns(columns);
	        if (LOGV) Log.Verbose(TAG, "query(columns=" + string.Join(", ", columns) + ") " + this);
	        return db.Query(mTable, columns, GetSelection(), GetSelectionArgs(), groupBy, having, orderBy, limit);
	    }
	
	    /**
	     * Execute update using the current internal state as {@code WHERE} clause.
	     */
	    public int Update(SQLiteDatabase db, ContentValues values) {
	        AssertTable();
	        if (LOGV) Log.Verbose(TAG, "update() " + this);
	        return db.Update(mTable, values, GetSelection(), GetSelectionArgs());
	    }
	
	    /**
	     * Execute delete using the current internal state as {@code WHERE} clause.
	     */
	    public int Delete(SQLiteDatabase db) {
	        AssertTable();
	        if (LOGV) Log.Verbose(TAG, "delete() " + this);
	        return db.Delete(mTable, GetSelection(), GetSelectionArgs());
	    }
		
		public override string ToString ()
		{
			return "SelectionBuilder[table=" + mTable + ", selection=" + GetSelection() + ", selectionArgs=" + string.Join(", ", GetSelectionArgs()) + "]";
		}
	}
}

