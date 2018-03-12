using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;

namespace AutofillFramework
{
    [Activity(Label = "WelcomeActivity")]
    [Register("com.xamarin.AutofillFramework.WelcomeActivity")]
    public class WelcomeActivity : AppCompatActivity
    {
        public TextView countdownText;

        public static Intent GetStartActivityIntent(Context context)
        {
            return new Intent(context, typeof(WelcomeActivity));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.welcome_activity);
            countdownText = (TextView) FindViewById(Resource.Id.countdownText);
            new MyCountDownTimer(5000, 1000) {that = this}.Start();
        }

        public class MyCountDownTimer : CountDownTimer
        {
            public WelcomeActivity that;

            public MyCountDownTimer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public MyCountDownTimer(long millisInFuture, long countDownInterval) : base(millisInFuture,
                countDownInterval)
            {
            }

            public override void OnFinish()
            {
                if (!that.IsFinishing)
                {
                    that.Finish();
                }
            }

            public override void OnTick(long millisUntilFinished)
            {
                int secondsRemaining = Java.Lang.Math.ToIntExact(millisUntilFinished / 1000);
                that.countdownText.Text =
                    that.Resources.GetQuantityString(Resource.Plurals.welcome_page_countdown, secondsRemaining,
                        secondsRemaining);
            }
        }
    }
}