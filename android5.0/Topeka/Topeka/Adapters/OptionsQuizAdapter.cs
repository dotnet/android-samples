using System;
using Android.Widget;
using Android.Views;
using System.Text;
using Android.Content;

namespace Topeka.Adapters
{
	public class OptionsQuizAdapter : BaseAdapter
	{
		readonly string[] options;
		readonly int layoutId;
		readonly string[] alphabet;

		public OptionsQuizAdapter (string[] options, int layoutId)
		{
			this.options = options;
			this.layoutId = layoutId;
			alphabet = null;
		}

		public OptionsQuizAdapter (string[] options, int layoutId, Context context, bool withPrefix)
		{
			this.options = options;
			this.layoutId = layoutId;
			if (withPrefix) {
				alphabet = context.Resources.GetStringArray (Resource.Array.alphabet);
			} else {
				alphabet = null;
			}
		}

		public override int Count {
			get {
				return options.Length;
			}
		}
        
        public override long GetItemId(int position)
        {
            return position;
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override Java.Lang.Object GetItem (int position)
		{
			return options [position];
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null) {
				var inflater = LayoutInflater.From (parent.Context);
				convertView = inflater.Inflate (layoutId, parent, false);
			}
			var text = GetText (position);
			((TextView)convertView).Text = text;
			return convertView;
		}

		string GetText (int position)
		{
			string text;
			if (alphabet == null) {
				text = GetItem (position).ToString();
			} else {
				text = GetPrefix (position) + GetItem (position);
			}
			return text;
		}

		string GetPrefix (int position)
		{
			var length = alphabet.Length;
			if (position >= length || 0 > position) {
				throw new InvalidOperationException ("Only positions between 0 and " + length + " are supported");
			}
			StringBuilder prefix;
			if (position < length) {
				prefix = new StringBuilder (alphabet [position]);
			} else {
				int tmpPosition = position % length;
				prefix = new StringBuilder (tmpPosition);
				prefix.Append (GetPrefix (position - tmpPosition));
			}
			prefix.Append (". ");
			return prefix.ToString ();
		}
	}
}

