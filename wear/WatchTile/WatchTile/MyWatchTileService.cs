using Androidx.Wear.Tiles;
using AndroidX.Wear.Tiles.Additions;
using Google.Common.Util.Concurrent;

namespace WatchTile
{
    public class MyWatchTileService : TileService
    {
        /// <summary>
        /// Version 1
        /// </summary>
        private const string ResourcesVersion = "1";

        /// <summary>
        /// On tile request callback
        /// </summary>
        /// <param name="p0">Tile request</param>
        /// <returns></returns>
        protected override IListenableFuture OnTileRequest
            (RequestBuilders.TileRequest p0)
        {
            // Create textview
            var text = new LayoutElementBuilders.Text.Builder();
            {
                text.SetText("I ❤ Xamarin!");
            }

            // Create layout
            var layout = new LayoutElementBuilders
                .Layout.Builder();
            {
                layout.SetRoot(text.Build());
            }

            // Create tile timeline
            var timeline = new TimelineBuilders.Timeline.Builder();
            {
                var entry = new TimelineBuilders.TimelineEntry.Builder();
                {
                    entry.SetLayout(layout.Build());
                }
                timeline.AddTimelineEntry(entry.Build());
            }

            // Create tile
            var tile = new TileBuilders.Tile.Builder();
            {
                tile.SetTimeline(timeline.Build());
                tile.SetResourcesVersion(ResourcesVersion);
                return Futures.ImmediateFuture(tile.Build());
            }
        }

        /// <summary>
        /// On resource request callback
        /// </summary>
        /// <param name="p0">Resources request</param>
        /// <returns></returns>
        protected override IListenableFuture OnResourcesRequest
            (RequestBuilders.ResourcesRequest p0)
        {
            var resource = new ResourceBuilders.Resources.Builder();
            {
                resource.SetVersion(ResourcesVersion);
                return Futures.ImmediateFuture(resource.Build());
            }
        }
    }
}
