using System;

namespace StockService
{
	public class Stock
	{
		public Stock ()
		{
		}

		public string Symbol { get; set; }

		public float LastPrice { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Stock: Symbol={0}, LastPrice={1}]", Symbol, LastPrice);
		}
	}
}

