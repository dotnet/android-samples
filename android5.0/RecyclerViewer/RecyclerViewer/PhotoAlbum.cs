using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace RecyclerViewer
{
    // Photo: contains image resource ID and caption:
    public class Photo
    {
        public Photo(int id, string caption)
        {
            PhotoID = id;
            Caption = caption;
        }

        // Return the ID of the photo:
        public int PhotoID { get; }

        // Return the Caption of the photo:
        public string Caption { get; }
    }

    // Photo album: holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:

        static Photo[] mBuiltInPhotos = {
            new Photo ( Resource.Drawable.buckingham_guards,
                        "Buckingham Palace" ),
            new Photo ( Resource.Drawable.la_tour_eiffel,
                        "The Eiffel Tower" ),
            new Photo ( Resource.Drawable.louvre_1,
                        "The Louvre" ),
            new Photo ( Resource.Drawable.before_mobile_phones,
                        "Before mobile phones" ),
            new Photo ( Resource.Drawable.big_ben_1,
                        "Big Ben skyline" ),
            new Photo ( Resource.Drawable.big_ben_2,
                        "Big Ben from below" ),
            new Photo ( Resource.Drawable.london_eye,
                        "The London Eye" ),
            new Photo ( Resource.Drawable.eurostar,
                        "Eurostar Train" ),
            new Photo ( Resource.Drawable.arc_de_triomphe,
                        "Arc de Triomphe" ),
            new Photo ( Resource.Drawable.louvre_2,
                        "Inside the Louvre" ),
            new Photo ( Resource.Drawable.versailles_fountains,
                        "Versailles fountains" ),
            new Photo ( Resource.Drawable.modest_accomodations,
                        "Modest accomodations" ),
            new Photo ( Resource.Drawable.notre_dame,
                        "Notre Dame" ),
            new Photo ( Resource.Drawable.inside_notre_dame,
                        "Inside Notre Dame" ),
            new Photo ( Resource.Drawable.seine_river,
                        "The Seine" ),
            new Photo ( Resource.Drawable.rue_cler,
                        "Rue Cler" ),
            new Photo ( Resource.Drawable.champ_elysees,
                        "The Avenue des Champs-Elysees" ),
            new Photo ( Resource.Drawable.seine_barge,
                        "Seine barge" ),
            new Photo ( Resource.Drawable.versailles_gates,
                        "Gates of Versailles" ),
            new Photo ( Resource.Drawable.edinburgh_castle_2,
                        "Edinburgh Castle" ),
            new Photo ( Resource.Drawable.edinburgh_castle_1,
                        "Edinburgh Castle up close" ),
            new Photo ( Resource.Drawable.old_meets_new,
                        "Old meets new" ),
            new Photo ( Resource.Drawable.edinburgh_from_on_high,
                        "Edinburgh from on high" ),
            new Photo ( Resource.Drawable.edinburgh_station,
                        "Edinburgh station" ),
            new Photo ( Resource.Drawable.scott_monument,
                        "Scott Monument" ),
            new Photo ( Resource.Drawable.view_from_holyrood_park,
                        "View from Holyrood Park" ),
            new Photo ( Resource.Drawable.tower_of_london,
                        "Outside the Tower of London" ),
            new Photo ( Resource.Drawable.tower_visitors,
                        "Tower of London visitors" ),
            new Photo ( Resource.Drawable.one_o_clock_gun,
                        "One O'Clock Gun" ),
            new Photo ( Resource.Drawable.victoria_albert,
                        "Victoria and Albert Museum" ),
            new Photo ( Resource.Drawable.royal_mile,
                        "The Royal Mile" ),
            new Photo ( Resource.Drawable.museum_and_castle,
                        "Edinburgh Museum and Castle" ),
            new Photo ( Resource.Drawable.portcullis_gate,
                        "Portcullis Gate" ),
            new Photo ( Resource.Drawable.to_notre_dame,
                        "Left or right?" ),
            new Photo ( Resource.Drawable.pompidou_centre,
                        "Pompidou Centre" ),
            new Photo ( Resource.Drawable.heres_lookin_at_ya,
                        "Here's Lookin' at Ya!" ),
            };

        // Array of photos that make up the album:
        private Photo[] mPhotos;

        // Random number generator for shuffling the photos:
        Random mRandom;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoAlbum()
        {
            mPhotos = mBuiltInPhotos;
            mRandom = new Random();
        }

        // Return the number of photos in the photo album:
        public int NumPhotos
        {
            get { return mPhotos.Length; }
        }

        // Indexer (read only) for accessing a photo:
        public Photo this[int i]
        {
            get { return mPhotos[i]; }
        }

        // Pick a random photo and swap it with the top:
        public int RandomSwap()
        {
            // Save the photo at the top:
            Photo tmpPhoto = mPhotos[0];

            // Generate a next random index between 1 and 
            // Length (noninclusive):
            int rnd = mRandom.Next(1, mPhotos.Length);

            // Exchange top photo with randomly-chosen photo:
            mPhotos[0] = mPhotos[rnd];
            mPhotos[rnd] = tmpPhoto;

            // Return the index of which photo was swapped with the top:
            return rnd;
        }

        // Shuffle the order of the photos:
        public void Shuffle()
        {
            // Use the Fisher-Yates shuffle algorithm:
            for (int idx = 0; idx < mPhotos.Length; ++idx)
            {
                // Save the photo at idx:
                Photo tmpPhoto = mPhotos[idx];

                // Generate a next random index between idx (inclusive) and 
                // Length (noninclusive):
                int rnd = mRandom.Next(idx, mPhotos.Length);

                // Exchange photo at idx with randomly-chosen (later) photo:
                mPhotos[idx] = mPhotos[rnd];
                mPhotos[rnd] = tmpPhoto;
            }
        }
    }
}
