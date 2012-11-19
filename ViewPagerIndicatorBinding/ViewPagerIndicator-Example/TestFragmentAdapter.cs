namespace ViewPagerIndicator_Example
{
    using Android.Support.V4.App;
    using JavaString = Java.Lang.String;

    public class TestFragmentAdapter : FragmentPagerAdapter
    {
        public static JavaString[] CONTENT = new[]
            {
                new JavaString("This"),
                new JavaString("Is"),
                new JavaString("A"),
                new JavaString("Test")
            };

        private int mCount;

        public TestFragmentAdapter(FragmentManager fm)
            : base(fm)
        {
            mCount = CONTENT.Length;
        }

        public override int Count { get { return mCount; } }

        public override Fragment GetItem(int position)
        {
            return new TestFragment(CONTENT [position % CONTENT.Length].ToString());
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return CONTENT [position % CONTENT.Length];
        }

        public void SetCount(int count)
        {
            if (count > 0 && count <= 10)
            {
                mCount = count;
                NotifyDataSetChanged();
            }
        }
    }
}
