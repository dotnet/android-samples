using System;
using Android.OS;
using Android.Util;
using Android.Widget;
using MessengerCore;

namespace MessengerService
{
	/// <summary>
	/// This Handler is used by the Service to process all requests that come in via a Messenger.
	/// </summary>
	public class TimestampRequestHandler : Handler
	{
		static string TAG = typeof(TimestampRequestHandler).FullName;
		WeakReference<TimestampService> serviceRef;

		public TimestampRequestHandler(TimestampService service)
		{
			serviceRef = new WeakReference<TimestampService>(service);
		}

		TimestampService Service
		{
			get
			{
				TimestampService service;
				if (serviceRef.TryGetTarget(out service))
				{
					return service;
				}
				return null;
			}
		}

		void ReplyWithTimestampTo(Message msg)
		{
				Messenger messenger = msg.ReplyTo;
			if (messenger == null)
			{
				Log.Wtf(TAG, "No Messenger instance - can't reply!");
			}
			else
			{
				string timestamp = Service?.GetFormattedTimestamp() ?? "";
				Message responseMessage = BuildResponse(timestamp);

				try
				{
					messenger.Send(responseMessage);
					Log.Debug(TAG, $"Sent '{timestamp}' via a Message to the client.");
				}
				catch (RemoteException ex)
				{
					Log.Error(TAG, ex, $"There was a problem trying to send '${timestamp}' to the client.");
				}
			}
		}

		Message BuildResponse(string timestamp)
		{
			Bundle dataToReturn = new Bundle();
			dataToReturn.PutString(Constants.TIMESTAMP_RESPONSE_KEY, timestamp);

			Message responseMessage = Message.Obtain(null, Constants.GET_UTC_TIMESTAMP_RESPONSE);
			responseMessage.Data = dataToReturn;
			return responseMessage;
		}

		public override void HandleMessage(Message msg)
		{
			int messageType = msg.What;
			Log.Debug(TAG, $"Message type: {messageType}.");

			switch (messageType)
			{
				case Constants.SAY_HELLO_TO_TIMESTAMP_SERVICE:
					Log.Info(TAG, "Someone said hello!");
					Toast.MakeText(Service.ApplicationContext, "Hello to you too!", ToastLength.Short).Show();
					break;

				case Constants.GET_UTC_TIMESTAMP:
					Log.Info(TAG, "External request for a timestamp.");
					ReplyWithTimestampTo(msg);
					break;
				default:
					Log.Warn(TAG, $"Unknown messageType, ignoring the value {messageType}.");
					base.HandleMessage(msg);
					break;
			}
		}
	}

}
