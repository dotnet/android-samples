using System;

namespace AutofillService.Data
{
	public interface IDataCallback<T>
	{
		void OnLoaded(T obj);

		void OnDataNotAvailable(String msg, params object[] pObjects);
	}
}