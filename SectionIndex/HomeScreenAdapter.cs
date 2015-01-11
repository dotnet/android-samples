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

namespace SectionIndex {
    public class HomeScreenAdapter : BaseAdapter<string>, ISectionIndexer {
        string[] items;
        Activity context;
        public HomeScreenAdapter(Activity context, string[] items) : base() {
            this.context = context;
            this.items = items;

            alphaIndex = new Dictionary<string, int>();
            for (int i = 0; i < items.Length; i++) {
                var key = items[i][0].ToString();
                if (!alphaIndex.ContainsKey(key)) 
                    alphaIndex.Add(key, i);
            }
            sections = new string[alphaIndex.Keys.Count];
            alphaIndex.Keys.CopyTo(sections, 0);
            sectionsObjects = new Java.Lang.Object[sections.Length];
            for (int i = 0; i < sections.Length; i++) {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override string this[int position] {   
            get { return items[position]; } 
        }
        public override int Count {
            get { return items.Length; } 
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position];
            return view;
        }

        string[] sections;
        Java.Lang.Object[] sectionsObjects;
        Dictionary<string, int> alphaIndex;
        // -- ISectionIndexer --
        public int GetPositionForSection(int section)
        {
            return alphaIndex[sections[section]];
        }
			
		public int GetSectionForPosition(int position)
		{      // this method isn't called in this example, but code is provided for completeness
			int prevSection = 0;

			for (int i = 0; i < sections.Length; i++)
			{
				if (GetPositionForSection(i) > position)
				{
					break;
				}

				prevSection = i;
			}

			return prevSection;
		}


        public Java.Lang.Object[] GetSections()
        {
            return sectionsObjects;
        }
    }
}