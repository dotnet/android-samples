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

namespace TreePager
{
    // TreePage: contains image resource ID and caption for a tree:
    public class TreePage
    {
        // Image ID for this tree image:
        public int imageId;

        // Caption text for this image:
        public string caption;

        // Returns the ID of the image:
        public int ImageID { get { return imageId; } }

        // Returns the caption text for the image:
        public string Caption { get { return caption; } }
    }

    // Tree catalog: holds image resource IDs and caption text:
    public class TreeCatalog
    {
        // Built-in tree catalog (could be replaced with a database)
        static TreePage[] treeBuiltInCatalog = {
            new TreePage { imageId = Resource.Drawable.larch,
                           caption = "No.1: The Larch" },
            new TreePage { imageId = Resource.Drawable.maple,
                           caption = "No.2: Maple" },
            new TreePage { imageId = Resource.Drawable.birch,
                           caption = "No.3: Birch" },
            new TreePage { imageId = Resource.Drawable.coconut,
                           caption = "No.4: Coconut" },
            new TreePage { imageId = Resource.Drawable.oak,
                           caption = "No.5: Oak" },
            new TreePage { imageId = Resource.Drawable.fir,
                           caption = "No.6: Fir" },
            new TreePage { imageId = Resource.Drawable.pine,
                           caption = "No.7: Pine" },
            new TreePage { imageId = Resource.Drawable.elm,
                           caption = "No.8: Elm" },
        };

        // Array of tree pages that make up the catalog:
        private TreePage[] treePages;

        // Create an instance copy of the built-in tree catalog:
        public TreeCatalog () { treePages = treeBuiltInCatalog; }

        // Indexer (read only) for accessing a tree page:
        public TreePage this[int i] { get { return treePages[i];  } }

        // Returns the number of tree pages in the catalog:
        public int NumTrees {  get { return treePages.Length; } }
    }
}