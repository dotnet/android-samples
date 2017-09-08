using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;

namespace AutofillFramework.app
{
	public class MyAutofillCallback : AutofillManager.AutofillCallback
	{
		bool AutofillReceived = false;

		public override void OnAutofillEvent(View view, AutofillEventType eventType)
		{
			base.OnAutofillEvent(view, eventType);
			if (view is AutoCompleteTextView)
			{
				switch (eventType)
				{
					case AutofillEventType.InputUnavailable:
					// no break on purpose
					case AutofillEventType.InputHidden:
						if (!AutofillReceived)
						{
							((AutoCompleteTextView)view).ShowDropDown();
						}
						break;
					case AutofillEventType.InputShown:
						AutofillReceived = true;
						((AutoCompleteTextView)view).Adapter = null;
						break;
					default:
						Log.Debug(CommonUtil.Tag, "Unexpected callback: " + eventType);
						break;
				}
			}
		}
	}
}
