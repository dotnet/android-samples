using Android.OS;
using Android.Views;
using Android.Widget;

namespace Analytics
{
	public class ImageFragment : Android.Support.V4.App.Fragment
	{
		const string ARG_PATTERN = "pattern";
		int resId;

		public static ImageFragment Create (int resId)
		{
			var fragment = new ImageFragment ();
			var args = new Bundle ();
			args.PutInt (ARG_PATTERN, resId);
			fragment.Arguments = args;
			return fragment;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			if (Arguments != null)
				resId = Arguments.GetInt (ARG_PATTERN);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.fragment_main, null);
			var imageView = view.FindViewById<ImageView> (Resource.Id.imageView);
			imageView.SetImageResource (resId);
			return view;
		}
	}
}

