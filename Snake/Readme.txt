This is a literal translation of Snake from:
http://developer.android.com/resources/samples/Snake/index.html

This currently dies due to:

SnakeView is declared in layout/snake_layout.xml as:

<mono.samples.snake.SnakeView

  android:id="@+id/snake"

  android:layout_width="match_parent"
  android:layout_height="match_parent"
  tileSize="24" />

In Snake.cs, we try to instatiate this class via:

SnakeView mSnakeView = (SnakeView)FindViewById (R.id.snake);

However, this tries to call a constuctor that takes an IntPtr, instead of
the constructors defined in the class:

public SnakeView (Context context, AttributeSet attrs) :
    base (context, attrs)
{
  InitSnakeView ();
}

public SnakeView (Context context, AttributeSet attrs, int defStyle) :
    base (context, attrs, defStyle)
{
  InitSnakeView ();
}

Thus, it fails with:

W/ActivityManager(   59): Launch timeout has expired, giving up wake lock!
W/ActivityManager(   59): Activity idle timeout for HistoryRecord{44fb6d58 mono.samples.snake/.Snake}
E/Mono.Android(  863): System.MissingMethodException: No constructor found for Mono.Samples.Snake.SnakeView::.ctor(System.IntPtr)
E/Mono.Android(  863):   at System.Activator.CreateInstance (System.Type type, BindingFlags bindingAttr, System.Reflection.Binder binder, System.Object[] args, System.Globalization.CultureInfo culture, System.Object[] activationAttributes) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at System.Activator.CreateInstance (System.Type type, System.Object[] args, System.Object[] activationAttributes) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at System.Activator.CreateInstance (System.Type type, System.Object[] args) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at Android.Runtime.TypeManager.CreateInstance (IntPtr handle) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at Java.Lang.Object.GetObject (IntPtr handle) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at Android.App.Activity.FindViewById (Int32 id) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at Mono.Samples.Snake.Snake.OnCreate (Android.OS.Bundle savedInstanceState) [0x00000] in <filename unknown>:0 
E/Mono.Android(  863):   at Android.App.Act
D/AndroidRuntime(  863): Shutting down VM
W/dalvikvm(  863): threadid=1: thread exiting with uncaught exception (group=0x4001d800)
E/AndroidRuntime(  863): FATAL EXCEPTION: main
E/AndroidRuntime(  863): java.lang.NoSuchMethodError: No constructor found for Mono.Samples.Snake.SnakeView::.ctor(System.IntPtr)
E/AndroidRuntime(  863): 	at mono.samples.snake.Snake.n_onCreate(Native Method)
E/AndroidRuntime(  863): 	at mono.samples.snake.Snake.onCreate(Snake.java:21)
E/AndroidRuntime(  863): 	at android.app.Instrumentation.callActivityOnCreate(Instrumentation.java:1047)
E/AndroidRuntime(  863): 	at android.app.ActivityThread.performLaunchActivity(ActivityThread.java:2627)
E/AndroidRuntime(  863): 	at android.app.ActivityThread.handleLaunchActivity(ActivityThread.java:2679)
E/AndroidRuntime(  863): 	at android.app.ActivityThread.access$2300(ActivityThread.java:125)
E/AndroidRuntime(  863): 	at android.app.ActivityThread$H.handleMessage(ActivityThread.java:2033)
E/AndroidRuntime(  863): 	at android.os.Handler.dispatchMessage(Handler.java:99)
E/AndroidRuntime(  863): 	at android.os.Looper.loop(Looper.java:123)
E/AndroidRuntime(  863): 	at android.app.ActivityThread.main(ActivityThread.java:4627)
E/AndroidRuntime(  863): 	at java.lang.reflect.Method.invokeNative(Native Method)
E/AndroidRuntime(  863): 	at java.lang.reflect.Method.invoke(Method.java:521)
E/AndroidRuntime(  863): 	at com.android.internal.os.ZygoteInit$MethodAndArgsCaller.run(ZygoteInit.java:868)
E/AndroidRuntime(  863): 	at com.android.internal.os.ZygoteInit.main(ZygoteInit.java:626)
E/AndroidRuntime(  863): 	at dalvik.system.NativeStart.main(Native Method)
W/ActivityManager(   59):   Force finishing activity mono.samples.snake/.Snake
W/ActivityManager(   59): Activity pause timeout for HistoryRecord{44fb6d58 mono.samples.snake/.Snake}