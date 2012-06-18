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
using Android.Preferences;
using System.Threading.Tasks;

namespace MonoIO
{
	public class EulaHelper
	{
		public static bool HasAcceptedEula(Context context) 
		{
	        var sp = PreferenceManager.GetDefaultSharedPreferences(context);
	        return sp.GetBoolean("accepted_eula", false);
		}
		
		protected static void SetAcceptedEula(Context context) 
		{
			Task.Factory.StartNew(() => {
	            var sp = PreferenceManager.GetDefaultSharedPreferences(context);
				sp.Edit().PutBoolean("accepted_eula", true).Commit();
			});
	    }
		
		/**
	     * Show End User License Agreement.
	     *
	     * @param accepted True IFF user has accepted license already, which means it can be dismissed.
	     *                 If the user hasn't accepted, then the EULA must be accepted or the program
	     *                 exits.
	     * @param activity Activity started from.
	     */
		public static void ShowEula(bool accepted, Activity activity)
		{
			AlertDialog.Builder eula = new AlertDialog.Builder(activity)
				.SetTitle(Resource.String.eula_title)
				.SetIcon(Android.Resource.Drawable.IcDialogInfo)
				.SetMessage(Resource.String.eula_text)
				.SetCancelable(accepted);
			
			if (accepted) 
			{
				eula.SetPositiveButton(Android.Resource.String.Ok, (object dialog, DialogClickEventArgs which) => {
					(dialog as IDialogInterface).Dismiss();	
				});
			} else {
				eula.SetPositiveButton(Resource.String.accept,(object dialog, DialogClickEventArgs which) => {
					SetAcceptedEula(activity);
					(dialog as IDialogInterface).Dismiss();
				})
				.SetNegativeButton(Resource.String.decline, (object dialog, DialogClickEventArgs which) => { 
					(dialog as IDialogInterface).Cancel();
					activity.Finish();
				});
			}
			eula.Show();
		}
	}
}

