using System;
using Android.App;
using System.Threading;

namespace DemoService
{
    [Service]
    [IntentFilter(new String[]{"com.xamarin.DemoIntentService"})]
    public class DemoIntentService : IntentService
    {
        public DemoIntentService () : base("DemoIntentService")
        {
        }

        protected override void OnHandleIntent (Android.Content.Intent intent)
        {
            Console.WriteLine ("perform some long running work");

            Thread.Sleep (5000);

            Console.WriteLine ("work complete");
        }
    }
}

