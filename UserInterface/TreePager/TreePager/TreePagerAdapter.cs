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
using Android.Support.V4.View;
using Java.Lang;

namespace TreePager
{
    class TreePagerAdapter : PagerAdapter
    {
        Context context;

        // Underlying data (tree catalog):
        public TreeCatalog treeCatalog;

        // Load the adapter with the tree catalog at construction time:
        public TreePagerAdapter (Context context, TreeCatalog treeCatalog)
        {
            this.context = context;
            this.treeCatalog = treeCatalog;
        }

        // Return the number of trees in the catalog:
        public override int Count
        {
            get { return treeCatalog.NumTrees; }
        }

        // Create the tree page for the given position:
        public override Java.Lang.Object InstantiateItem (View container, int position)
        {
            // Instantiate the ImageView and give it an image:
            var imageView = new ImageView(context);
            imageView.SetImageResource(treeCatalog[position].imageId);

            // Add the image to the ViewPager:
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.AddView(imageView);
            return imageView;
        }

        // Remove a tree page from the given position.
        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(view as View);
        }

        // Determine whether a page View is associated with the specific key object
        // returned from InstantiateItem (in this case, they are one in the same):
        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        // Display a caption for each Tree page in the PagerTitleStrip:
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(treeCatalog[position].caption);
        }
    }
}