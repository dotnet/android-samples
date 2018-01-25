using System.Collections.Generic;

namespace ViewModel
{
	public class ItemViewModel : Android.Arch.Lifecycle.ViewModel
	{
		List<Item> Items;

		public List<Item> GetItems()
		{
			if (Items == null)
				Items = LoadItems();
			return Items;
		}

		List<Item> LoadItems()
		{
			Items = new List<Item>();
			for (var i = 0; i < 10; i++)
			{
				Items.Add(new Item { Name = "Item " + i});
			}
			return Items;
		}
	}
}
