using Android.OS;
using Android.Support.Annotation;
using Java.Lang;
using Java.Util.Concurrent;

namespace AutofillService
{
	/**
	 * Global executor pools for the whole application.
	 * <p>
	 * Grouping tasks like this avoids the effects of task starvation (e.g. disk reads don't wait behind
	 * webservice requests).
	 */
	public class AppExecutors
	{

		private static int THREAD_COUNT = 3;

		public IExecutor diskIO;

		public IExecutor networkIO;

		public IExecutor mainThread;

		[VisibleForTesting]
		AppExecutors(IExecutor diskIO, IExecutor networkIO, IExecutor mainThread)
		{
			this.diskIO = diskIO;
			this.networkIO = networkIO;
			this.mainThread = mainThread;
		}

		public AppExecutors() : this(new DiskIoThreadExecutor(), Executors.NewFixedThreadPool(THREAD_COUNT), new MainThreadExecutor())
		{
		}

		class MainThreadExecutor : Java.Lang.Object, IExecutor
		{

			private Handler mainThreadHandler = new Handler(Looper.MainLooper);


			public void Execute(IRunnable command)
			{
				mainThreadHandler.Post(command);
			}
		}

		/**
		 * Executor that runs a task on a new background thread.
		 */
		class DiskIoThreadExecutor : Java.Lang.Object, IExecutor
		{
			private IExecutor mDiskIO;

			public DiskIoThreadExecutor()
			{
				mDiskIO = Executors.NewSingleThreadExecutor();
			}

			public void Execute(IRunnable command)
			{
				mDiskIO.Execute(command);
			}
		}
	}
}