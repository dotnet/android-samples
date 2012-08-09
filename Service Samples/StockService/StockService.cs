
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace StockService
{
	[Service]
	[IntentFilter(new String[]{"com.xamarin.StockService"})]
	public class StockService : IntentService
	{
		IBinder binder;
		List<Stock> stocks;
		public const string StocksUpdatedAction = "StocksUpdated";

		protected override void OnHandleIntent (Intent intent)
		{
			var stockSymbols = new List<string> (){"AMZN", "FB", "GOOG", "AAPL", "MSFT", "IBM"};

			stocks = UpdateStocks (stockSymbols);

			var stocksIntent = new Intent (StocksUpdatedAction); 

			SendOrderedBroadcast (stocksIntent, null);
		}

		public override IBinder OnBind (Intent intent)
		{
			binder = new StockServiceBinder (this);
			return binder;
		}

		public List<Stock> GetStocks ()
		{
			return stocks;
		}

		List<Stock> UpdateStocks (List<string> symbols)
		{
			List<Stock> results = null;

			string[] array = symbols.ToArray ();
			string symbolsString = String.Join ("%22%2C%22", array);

			string uri = String.Format (
                "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22{0}%22)%0A%09%09&diagnostics=false&format=json&env=http%3A%2F%2Fdatatables.org%2Falltables.env",
                symbolsString);

			var httpReq = (HttpWebRequest)HttpWebRequest.Create (new Uri (uri));

			try {
				using (HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse ()) {

					var response = httpRes.GetResponseStream ();
					var json = (JsonObject)JsonObject.Load (response);
            
					results = (from result in (JsonArray)json ["query"] ["results"] ["quote"]
                           let jResult = result as JsonObject 
                           select new Stock { Symbol = jResult["Symbol"], LastPrice = (float)jResult["LastTradePriceOnly"] }).ToList ();
				}
			} catch (Exception ex) {
				Log.Debug("StockService", "error connecting to web service: " + ex.Message);
			}

			return results;
		}

	}

	public class StockServiceBinder : Binder
	{
		StockService service;

		public StockServiceBinder (StockService service)
		{
			this.service = service;
		}

		public StockService GetStockService ()
		{
			return service;
		}
	}
}

