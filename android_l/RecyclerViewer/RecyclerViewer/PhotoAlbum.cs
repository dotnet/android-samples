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
        // Photo ID for this photo:
        public int mPhotoID;

        // Caption text for this photo:
        public string mCaption;

        // Return the ID of the photo:
        public int PhotoID 
        { 
            get { return mPhotoID; } 
        }

        // Return the Caption of the photo:
        public string Caption 
        { 
            get { return mCaption; } 
        }
    }

    // Photo album: holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:

        static Photo[] mBuiltInPhotos = {
            new Photo { mPhotoID = Resource.Drawable.buckingham_guards,
                        mCaption = "Buckingham Palace" },
            new Photo { mPhotoID = Resource.Drawable.la_tour_eiffel,
                        mCaption = "The Eiffel Tower" },
            new Photo { mPhotoID = Resource.Drawable.louvre_1,
                        mCaption = "The Louvre" },
            new Photo { mPhotoID = Resource.Drawable.before_mobile_phones,
                        mCaption = "Before mobile phones" },
            new Photo { mPhotoID = Resource.Drawable.big_ben_1,
                        mCaption = "Big Ben skyline" },
            new Photo { mPhotoID = Resource.Drawable.big_ben_2,
                        mCaption = "Big Ben from below" },
            new Photo { mPhotoID = Resource.Drawable.london_eye,
                        mCaption = "The London Eye" },
            new Photo { mPhotoID = Resource.Drawable.eurostar,
                        mCaption = "Eurostar Train" },
            new Photo { mPhotoID = Resource.Drawable.arc_de_triomphe,
                        mCaption = "Arc de Triomphe" },
            new Photo { mPhotoID = Resource.Drawable.louvre_2,
                        mCaption = "Inside the Louvre" },
            new Photo { mPhotoID = Resource.Drawable.versailles_fountains,
                        mCaption = "Versailles fountains" },
            new Photo { mPhotoID = Resource.Drawable.modest_accomodations,
                        mCaption = "Modest accomodations" },
            new Photo { mPhotoID = Resource.Drawable.notre_dame,
                        mCaption = "Notre Dame" },
            new Photo { mPhotoID = Resource.Drawable.inside_notre_dame,
                        mCaption = "Inside Notre Dame" },
            new Photo { mPhotoID = Resource.Drawable.seine_river,
                        mCaption = "The Seine" },
            new Photo { mPhotoID = Resource.Drawable.rue_cler,
                        mCaption = "Rue Cler" },
            new Photo { mPhotoID = Resource.Drawable.champ_elysees,
                        mCaption = "The Avenue des Champs-Elysees" },
            new Photo { mPhotoID = Resource.Drawable.seine_barge,
                        mCaption = "Seine barge" },
            new Photo { mPhotoID = Resource.Drawable.versailles_gates,
                        mCaption = "Gates of Versailles" },
            new Photo { mPhotoID = Resource.Drawable.edinburgh_castle_2,
                        mCaption = "Edinburgh Castle" },
            new Photo { mPhotoID = Resource.Drawable.edinburgh_castle_1,
                        mCaption = "Edinburgh Castle up close" },
            new Photo { mPhotoID = Resource.Drawable.old_meets_new,
                        mCaption = "Old meets new" },
            new Photo { mPhotoID = Resource.Drawable.edinburgh_from_on_high,
                        mCaption = "Edinburgh from on high" },
            new Photo { mPhotoID = Resource.Drawable.edinburgh_station,
                        mCaption = "Edinburgh station" },
            new Photo { mPhotoID = Resource.Drawable.scott_monument,
                        mCaption = "Scott Monument" },
            new Photo { mPhotoID = Resource.Drawable.view_from_holyrood_park,
                        mCaption = "View from Holyrood Park" },
            new Photo { mPhotoID = Resource.Drawable.tower_of_london,
                        mCaption = "Outside the Tower of London" },
            new Photo { mPhotoID = Resource.Drawable.tower_visitors,
                        mCaption = "Tower of London visitors" },
            new Photo { mPhotoID = Resource.Drawable.one_o_clock_gun,
                        mCaption = "One O'Clock Gun" },
            new Photo { mPhotoID = Resource.Drawable.victoria_albert,
                        mCaption = "Victoria and Albert Museum" },
            new Photo { mPhotoID = Resource.Drawable.royal_mile,
                        mCaption = "The Royal Mile" },
            new Photo { mPhotoID = Resource.Drawable.museum_and_castle,
                        mCaption = "Edinburgh Museum and Castle" },
            new Photo { mPhotoID = Resource.Drawable.portcullis_gate,
                        mCaption = "Portcullis Gate" },
            new Photo { mPhotoID = Resource.Drawable.to_notre_dame,
                        mCaption = "Left or right?" },
            new Photo { mPhotoID = Resource.Drawable.pompidou_centre,
                        mCaption = "Pompidou Centre" },
            new Photo { mPhotoID = Resource.Drawable.heres_lookin_at_ya,
                        mCaption = "Here's Lookin' at Ya!" },
            };

        // Array of photos that make up the album:
        private Photo[] mPhotos;

        // Random number generator for shuffling the photos:
        Random mRandom;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoAlbum ()
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
        public void Shuffle ()
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
