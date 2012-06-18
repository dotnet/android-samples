using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoIO
{
	public class ReflectionUtils
	{
		public static object TryInvoke(object target, string methodName, params Object[] args) {
	        Type[] argTypes = new Type[args.Length];
	        for (int i = 0; i < args.Length; i++) {
	            argTypes[i] = args[i].GetType();
	        }
	
	        return TryInvoke(target, methodName, argTypes, args);
	    }
	
	    public static object TryInvoke(object target, string methodName, Type[] argTypes, params Object[] args) {
	        try {
	            return target.GetType().GetMethod(methodName, argTypes).Invoke(target, args);
	        } catch {
			}
	
	        return null;
	    }
		
		public static T CallWithDefault<T> (object target, string methodName, T defaultValue)
		{
		   try {
		       return (T) target.GetType ().GetMethod (methodName).Invoke (target, null);
		   } catch {
		   }
			
			return defaultValue;
		}

	}
}

