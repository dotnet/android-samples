using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Uri = Android.Net.Uri;

namespace MonoIO
{
	class NotifyingAsyncQueryHandler : AsyncQueryHandler
	{
		private WeakReference mListener;
		
		/**
	     * Interface to listen for completed query operations.
	     */
	    public interface AsyncQueryListener {
	        void OnQueryComplete(int token, Java.Lang.Object cookie, ICursor cursor);
	    }
		
		public NotifyingAsyncQueryHandler(ContentResolver resolver, AsyncQueryListener listener) : base(resolver)
		{
	        SetQueryListener(listener);
	    }
		
		/**
	     * Assign the given {@link AsyncQueryListener} to receive query events from
	     * asynchronous calls. Will replace any existing listener.
	     */
	    public void SetQueryListener(AsyncQueryListener listener) 
		{
	        mListener = new WeakReference(listener);
	    }
		
		/**
	     * Clear any {@link AsyncQueryListener} set through
	     * {@link #setQueryListener(AsyncQueryListener)}
	     */
	    public void ClearQueryListener() {
	        mListener = null;
	    }
		
		/**
	     * Begin an asynchronous query with the given arguments. When finished,
	     * {@link AsyncQueryListener#onQueryComplete(int, Object, Cursor)} is
	     * called if a valid {@link AsyncQueryListener} is present.
	     */
	    public void StartQuery(Uri uri, String[] projection) {
	        StartQuery(-1, null, uri, projection, null, null, null);
	    }
		
		/**
	     * Begin an asynchronous query with the given arguments. When finished,
	     * {@link AsyncQueryListener#onQueryComplete(int, Object, Cursor)} is called
	     * if a valid {@link AsyncQueryListener} is present.
	     *
	     * @param token Unique identifier passed through to
	     *            {@link AsyncQueryListener#onQueryComplete(int, Object, Cursor)}
	     */
	    public void StartQuery(int token, Uri uri, String[] projection) {
	        StartQuery(token, null, uri, projection, null, null, null);
	    }
	
	    /**
	     * Begin an asynchronous query with the given arguments. When finished,
	     * {@link AsyncQueryListener#onQueryComplete(int, Object, Cursor)} is called
	     * if a valid {@link AsyncQueryListener} is present.
	     */
	    public void StartQuery(Uri uri, String[] projection, String sortOrder) {
	        StartQuery(-1, null, uri, projection, null, null, sortOrder);
	    }
	
	    /**
	     * Begin an asynchronous query with the given arguments. When finished,
	     * {@link AsyncQueryListener#onQueryComplete(int, Object, Cursor)} is called
	     * if a valid {@link AsyncQueryListener} is present.
	     */
	    public void StartQuery(Uri uri, String[] projection, String selection, String[] selectionArgs, String orderBy) {
	        StartQuery(-1, null, uri, projection, selection, selectionArgs, orderBy);
	    }
	
	    /**
	     * Begin an asynchronous update with the given arguments.
	     */
	    public void StartUpdate(Uri uri, ContentValues values) {
	        StartUpdate(-1, null, uri, values, null, null);
	    }
	
	    public void StartInsert(Uri uri, ContentValues values) {
	        StartInsert(-1, null, uri, values);
	    }
	
	    public void StartDelete(Uri uri) {
	        StartDelete(-1, null, uri, null, null);
	    }
		
		protected override void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			var listener = mListener == null ? null : mListener.Target as AsyncQueryListener;
	        if (listener != null) {
	            listener.OnQueryComplete(token, cookie, cursor);
	        } else if (cursor != null) {
	            cursor.Close();
	        }
		}
	}
}

