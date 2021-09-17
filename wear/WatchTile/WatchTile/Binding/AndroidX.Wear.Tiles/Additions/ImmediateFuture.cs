using Java.Lang;
using Java.Util.Concurrent;
using Google.Common.Util.Concurrent;

namespace AndroidX.Wear.Tiles.Additions
{
    public static class Futures
    {
        public static ImmediateFuture ImmediateFuture(Java.Lang.Object value)
            => new ImmediateFuture(value);
    }

    public class ImmediateFuture : Java.Lang.Object, IListenableFuture
    {
        public bool IsDone => true;

        public bool IsCancelled => false;

        private Java.Lang.Object _obj;

        public ImmediateFuture(Java.Lang.Object o)
            => _obj = o;

        public void AddListener(IRunnable p0, IExecutor p1)
            => p1.Execute(p0);

        public bool Cancel(bool mayInterruptIfRunning) => false;

        public Java.Lang.Object Get() => _obj;

        public Java.Lang.Object Get(long timeout, TimeUnit unit) => Get();
    }
}
