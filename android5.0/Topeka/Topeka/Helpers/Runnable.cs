using System;
using Java.Lang;

namespace Topeka.Helpers
{
    public class Runnable : Java.Lang.Object, IRunnable
    {
        public EventHandler RunAction { get; set; }
        public void Run()
        {
            if (RunAction != null)
                RunAction(this, new EventArgs());
        }
    }
}
