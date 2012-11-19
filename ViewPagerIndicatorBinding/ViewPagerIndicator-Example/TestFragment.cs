namespace ViewPagerIndicator_Example
{
    using System.Text;

    using Android.OS;
    using Android.Support.V4.App;
    using Android.Views;
    using Android.Widget;

    public class TestFragment : Fragment
    {
        private const string KEY_CONTENT = "TestFragment:Content";
        private string mContent = "???";

        public TestFragment()
        {
        }

        public TestFragment(string content)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 20; i++)
            {
                if (i != 19)
                {
                    builder.Append(content).Append(" ");
                }
                else
                {
                    builder.Append(content);
                }
            }
            mContent = builder.ToString();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if ((savedInstanceState != null) && savedInstanceState.ContainsKey(KEY_CONTENT))
            {
                mContent = savedInstanceState.GetString(KEY_CONTENT);
            }

            var text = new TextView(Activity);
            text.Gravity = GravityFlags.Center;
            text.Text = mContent;
            text.TextSize = (20 * Resources.DisplayMetrics.Density);
            text.SetPadding(20, 20, 20, 20);

            var layout = new LinearLayout(Activity);
            layout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
            layout.SetGravity(GravityFlags.Center);
            layout.AddView(text);

            return layout;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KEY_CONTENT, mContent);
        }
    }
}
