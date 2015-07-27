using System;
using System.Text;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace Topeka.Adapters
{
	public class OptionsQuizAdapter : BaseAdapter
	{
		string[] options;
		int layoutId;
		string[] alphabet;
		
		public override bool HasStableIds => true;

		public override int Count =>  options.Length;

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
			alphabet = withPrefix ? context.Resources.GetStringArray (Resource.Array.alphabet) : null;
		}

		public override long GetItemId (int position)
		{
			return position;
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

			((TextView)convertView).Text = GetText (position);
			return convertView;
		}

		string GetText (int position)
		{
			return (alphabet == null) ? GetItem (position).ToString () : GetPrefix (position) + GetItem (position);
		}

		string GetPrefix (int position)
		{
			var length = alphabet.Length;
			if (position >= length || 0 > position)
				throw new InvalidOperationException ("Only positions between 0 and " + length + " are supported");

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

