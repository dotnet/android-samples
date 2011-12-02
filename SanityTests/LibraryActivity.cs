using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
#if __ANDROID_8__
using Android.App.Backup;
#endif
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.SanityTests
{
    [Activity(Label = "Library Activity")]
    public class LibraryActivity : Activity
    {
        public LibraryActivity ()
        {
        }

        public LibraryActivity (IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
        }
    }

#if __ANDROID_8__
    public class MyBackupAgent : BackupAgent {
      [Preserve]
      public MyBackupAgent ()
      {
      }

      public override void OnBackup (ParcelFileDescriptor oldState, BackupDataOutput data, ParcelFileDescriptor newState)
      {
          Log.Info ("*jonp*", "Asked to perform a backup...");
      }

      public override void OnRestore (BackupDataInput data, int appVersionCode, ParcelFileDescriptor newState)
      {
          Log.Info ("*jonp*", "Asked to restore a backup...");
      }
    }
#endif

#region BNC_679599
    public interface IA {
        void Do ();
    }

    public class A : IA {

        public void Do ()
        {
        }
    }

    public interface IB {
        void Do ();
    }

    public class B<TA> : IB
        where TA : IA, new ()
    {
        public void Do ()
        {
        }
    }
#endregion
}
