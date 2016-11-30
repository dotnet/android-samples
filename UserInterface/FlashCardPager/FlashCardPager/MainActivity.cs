using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;

namespace FlashCardPager
{
    [Activity(Label = "FlashCardPager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the content view from the "Main" layout resource:
            SetContentView(Resource.Layout.Main);

            // Instantiate the deck of flash cards:
            FlashCardDeck flashCards = new FlashCardDeck();

            // Instantiate the adapter and pass in the deck of flash cards:
            FlashCardDeckAdapter adapter = new FlashCardDeckAdapter(SupportFragmentManager, flashCards);

            // Find the ViewPager and plug in the adapter:
            ViewPager pager = (ViewPager)FindViewById(Resource.Id.pager);
            pager.Adapter = adapter;
        }
    } 
}

