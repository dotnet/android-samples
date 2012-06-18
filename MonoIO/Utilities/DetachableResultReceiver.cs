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
using Android.Util;

namespace MonoIO
{
	/**
	 * Proxy {@link ResultReceiver} that offers a listener interface that can be
	 * detached. Useful for when sending callbacks to a {@link Service} where a
	 * listening {@link Activity} can be swapped out during configuration changes.
	 */
	public class DetachableResultReceiver : ResultReceiver {
	    private static String TAG = "DetachableResultReceiver";
	
	    public Receiver mReceiver;
	
	    public DetachableResultReceiver(Handler handler) : base(handler)
		{    
		}
	
	    public void ClearReceiver() {
	        mReceiver = null;
	    }
	
	    public void SetReceiver(Receiver receiver) {
	        mReceiver = receiver;
	    }
	
	    public interface Receiver 
		{
	         void OnReceiveResult(int resultCode, Bundle resultData);
	    }
	
		protected override void OnReceiveResult (int resultCode, Bundle resultData)
		{	
			if (mReceiver != null) {
	            mReceiver.OnReceiveResult(resultCode, resultData);
	        } else {
	            Log.Warn(TAG, "Dropping result on floor for code " + resultCode + ": " + resultData.ToString());
	        }
		}
	}
}

