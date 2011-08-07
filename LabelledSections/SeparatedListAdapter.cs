using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.LabelledSections
{
	class SeparatedListAdapter : BaseAdapter
	{
		public SeparatedListAdapter (Context context)
		{
			headers = new ArrayAdapter<string> (context, Resource.Layout.ListHeader);
		}
		
		Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter> ();
		ArrayAdapter<string> headers;
		const int TypeSectionHeader = 0;
		
		public void AddSection (string section, IAdapter adapter)
		{
			headers.Add (section);
			sections.Add (section, adapter);
		}
		
		public override Java.Lang.Object GetItem (int position)
		{
			int op = position;
			foreach (var section in sections.Keys) {
				var adapter = sections [section];
				int size = adapter.Count + 1;
				if (position == 0)
					return section;
				if (position < size)
					return adapter.GetItem (position - 1);
				position -= size;
			}
			return null;
		}
		
		public override int Count {
			get {
				return sections.Values.Sum (adapter => adapter.Count + 1);
			}
		}
		
		public override int ViewTypeCount {
			get {
				return 1 + sections.Values.Sum (adapter => adapter.ViewTypeCount);
			}
		}

		public override int GetItemViewType (int position)
		{
			int type = 1;
			foreach (var section in sections.Keys) {
				var adapter = sections [section];
				int size = adapter.Count + 1;

				// check if position inside this section
				if (position == 0)
					return TypeSectionHeader;
				if (position < size)
					return type + adapter.GetItemViewType (position - 1);

				// otherwise jump into next section
				position -= size;
				type += adapter.ViewTypeCount;
			}
			return -1;
		}

		public override bool AreAllItemsEnabled ()
		{
			return false;
		}

		public override bool IsEnabled (int position)
		{
			return (GetItemViewType (position) != TypeSectionHeader);
		}

		public override View GetView(int position, View convertView, ViewGroup parent) {
			int sectionnum = 0;
			foreach (var section in sections.Keys) {
				var adapter = sections [section];
				int size = adapter.Count + 1;

				// check if position inside this section
				if (position == 0)
					return headers.GetView (sectionnum, convertView, parent);
				if (position < size)
					return adapter.GetView (position - 1, convertView, parent);

				// otherwise jump into next section
				position -= size;
				sectionnum++;
			}
			return null;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
	}
}

#if false
    public class SeparatedListAdapter extends BaseAdapter {
 
        public final Map<String, Adapter> sections = new LinkedHashMap<String, Adapter>();
        public final ArrayAdapter<String> headers;
        public final static int TYPE_SECTION_HEADER = 0;
 
        public SeparatedListAdapter(Context context) {
            headers = new ArrayAdapter<String>(context, R.layout.list_header);
        }
 
        public void addSection(String section, Adapter adapter) {
            this.headers.add(section);
            this.sections.put(section, adapter);
        }
 
        public Object getItem(int position) {
            for (Object section : this.sections.keySet()) {
                Adapter adapter = sections.get(section);
                int size = adapter.getCount() + 1;
 
                // check if position inside this section
                if (position == 0)
                    return section;
                if (position < size)
                    return adapter.getItem(position - 1);
 
                // otherwise jump into next section
                position -= size;
            }
            return null;
        }
 
        public int getCount() {
            // total together all sections, plus one for each section header
            int total = 0;
            for (Adapter adapter : this.sections.values())
                total += adapter.getCount() + 1;
            return total;
        }
 
        public int getViewTypeCount() {
            // assume that headers count as one, then total all sections
            int total = 1;
            for (Adapter adapter : this.sections.values())
                total += adapter.getViewTypeCount();
            return total;
        }
 
        public int getItemViewType(int position) {
            int type = 1;
            for (Object section : this.sections.keySet()) {
                Adapter adapter = sections.get(section);
                int size = adapter.getCount() + 1;
 
                // check if position inside this section
                if (position == 0)
                    return TYPE_SECTION_HEADER;
                if (position < size)
                    return type + adapter.getItemViewType(position - 1);
 
                // otherwise jump into next section
                position -= size;
                type += adapter.getViewTypeCount();
            }
            return -1;
        }
 
        public boolean areAllItemsSelectable() {
            return false;
        }
 
        public boolean isEnabled(int position) {
            return (getItemViewType(position) != TYPE_SECTION_HEADER);
        }
 
        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            int sectionnum = 0;
            for (Object section : this.sections.keySet()) {
                Adapter adapter = sections.get(section);
                int size = adapter.getCount() + 1;
 
                // check if position inside this section
                if (position == 0)
                    return headers.getView(sectionnum, convertView, parent);
                if (position < size)
                    return adapter.getView(position - 1, convertView, parent);
 
                // otherwise jump into next section
                position -= size;
                sectionnum++;
            }
            return null;
        }
 
        @Override
        public long getItemId(int position) {
            return position;
        }
 
    }
#endif
