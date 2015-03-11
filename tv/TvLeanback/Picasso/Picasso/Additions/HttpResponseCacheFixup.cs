using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;

namespace Squareup.OkHttp
{
    public partial class HttpResponseCache
    {
        static IntPtr id_get_Ljava_net_URI_Ljava_lang_String_Ljava_util_Map_;
        // Metadata.xml XPath method reference: path="/api/package[@name='com.squareup.okhttp']/class[@name='HttpResponseCache']/method[@name='get' and count(parameter)=3 and parameter[1][@type='java.net.URI'] and parameter[2][@type='java.lang.String'] and parameter[3][@type='java.util.Map']]"
        [Register("get", "(Ljava/net/URI;Ljava/lang/String;Ljava/util/Map;)Ljava/net/CacheResponse;", "")]
        public override global::Java.Net.CacheResponse Get(global::Java.Net.URI p0, string p1, global::System.Collections.Generic.IDictionary<string, global::System.Collections.Generic.IList<string>> p2)
        {
            if (id_get_Ljava_net_URI_Ljava_lang_String_Ljava_util_Map_ == IntPtr.Zero)
                id_get_Ljava_net_URI_Ljava_lang_String_Ljava_util_Map_ = JNIEnv.GetMethodID(class_ref, "get", "(Ljava/net/URI;Ljava/lang/String;Ljava/util/Map;)Ljava/net/CacheResponse;");
            IntPtr native_p1 = JNIEnv.NewString(p1);
            IntPtr native_p2 = global::Android.Runtime.JavaDictionary<string, global::System.Collections.Generic.IList<string>>.ToLocalJniHandle(p2);
            global::Java.Net.CacheResponse __ret = global::Java.Lang.Object.GetObject<global::Java.Net.CacheResponse>(JNIEnv.CallObjectMethod(Handle, id_get_Ljava_net_URI_Ljava_lang_String_Ljava_util_Map_, new JValue(p0), new JValue(native_p1), new JValue(Java.Interop.JavaObjectExtensions.ToInteroperableCollection(p2))), JniHandleOwnership.TransferLocalRef);
            JNIEnv.DeleteLocalRef(native_p1);
            JNIEnv.DeleteLocalRef(native_p2);
            return __ret;
        }
    }
}
