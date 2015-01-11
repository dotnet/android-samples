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

namespace BuiltInExpandableViews {

    // Represents a type of produce ("vegetables", "fruits", "herbs"):
    public class Produce {

        // Produce type such as vegetables, fruit, herbs
        public string Type { get; set; }

        // List of produce items for that type, such as bananas, apricots, plums, apples
        public ProduceItem[] ProduceItems { get; set; }
    }

    // Represents a produce item ("bananas", "carrots", etc.) within a type of produce:
    public class ProduceItem
    {
        // Name of produce item ("Bananas", "Carrots", etc.)
        public string Name { get; set; }

        // How many units of this produce item are in stock:
        public int Count { get; set; }
    }
}
