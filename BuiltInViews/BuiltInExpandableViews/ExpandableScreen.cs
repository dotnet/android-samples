using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace BuiltInExpandableViews
{
    [Activity(Label = "BuiltInExpandableViews", MainLauncher = true, Icon = "@drawable/icon")]
    public class ExpandableScreen : ExpandableListActivity
    {
        // A list of produce objects; each is a container for "vegetables", "fruits", or "herbs":
        List<Produce> tableItems = new List<Produce>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Populate the list view table with groups ("vegetables", etc.) and children ("carrots", etc.):
            tableItems.Add(new Produce() { Type = "Vegetables", 
                                           ProduceItems = new ProduceItem[] 
                                           { 
                                               new ProduceItem { Name="Squash", Count = 33 },
                                               new ProduceItem { Name="Zucchini", Count = 6 },
                                               new ProduceItem { Name="Carrots", Count = 20 }
                                           }});
            tableItems.Add(new Produce() { Type = "Fruit", 
                                           ProduceItems = new ProduceItem[] 
                                           { 
                                               new ProduceItem { Name="Apricots", Count = 42 },
                                               new ProduceItem { Name="Bananas", Count = 29 },
                                               new ProduceItem { Name="Plums", Count = 9 },
                                               new ProduceItem { Name="Apples", Count = 91 }
                                           }});
            tableItems.Add(new Produce() { Type = "Herbs", 
                                           ProduceItems = new ProduceItem[] 
                                           { 
                                               new ProduceItem { Name="Basil", Count = 10 },
                                               new ProduceItem { Name="Rosemary", Count = 12 },
                                               new ProduceItem { Name="Thyme", Count = 9 }
                                           }});

            // Wire in the adapter for this expandable list activity:
            var adapter = new ExpandableScreenAdapter(this, tableItems);
            SetListAdapter(adapter);
        }
    }
}

