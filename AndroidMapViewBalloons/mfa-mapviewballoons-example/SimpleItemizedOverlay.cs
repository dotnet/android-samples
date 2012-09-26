namespace MapviewBalloons.Example
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Android.Content;
    using Android.GoogleMaps;
    using Android.Graphics.Drawables;
    using Android.Widget;

    using Com.Readystatesoftware.Mapviewballoons;

    using Java.Lang;

    public class SimpleItemizedOverlay : BalloonItemizedOverlay
    {
        private readonly Context _context;
        private readonly List<OverlayItem> _overlays = new List<OverlayItem>();

        public SimpleItemizedOverlay(Drawable defaultMarker, MapView mapView)
            : base(BoundCenter(defaultMarker), mapView)
        {
            _context = mapView.Context;
        }

        public void AddOverlay(OverlayItem overlayitem)
        {
            _overlays.Add(overlayitem);
            Populate();
        }

        public override int Size()
        {
            return _overlays.Count;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
        protected override Object CreateItem(int i)
        {
            return _overlays[i];
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
        protected override bool OnBalloonTap(int index, Object item)
        {
            Toast.MakeText(_context, "OnBallonTap for overlay index " + index, ToastLength.Long).Show();
            return true;
        }
    }
}
