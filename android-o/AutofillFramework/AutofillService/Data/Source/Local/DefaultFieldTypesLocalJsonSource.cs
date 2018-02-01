using System;
using System.Collections.Generic;
using System.IO;
using Android.Content.Res;
using AutofillService.Model;
using GoogleGson;
using GoogleGson.Reflect;
using Java.IO;


namespace AutofillService.Data.Source.Local
{
	public class DefaultFieldTypesLocalJsonSource : IDefaultFieldTypesSource
	{

		private static DefaultFieldTypesLocalJsonSource sInstance;

		private static Resources mResources;
		private static Gson mGson;

		private DefaultFieldTypesLocalJsonSource(Resources resources, Gson gson)
		{
			mResources = resources;
			mGson = gson;
		}

		public static DefaultFieldTypesLocalJsonSource GetInstance(Resources resources, Gson gson)
		{
			if (sInstance == null)
			{
				sInstance = new DefaultFieldTypesLocalJsonSource(resources, gson);
			}
			return sInstance;
		}

		public List<DefaultFieldTypeWithHints> GetDefaultFieldTypes()
		{
			var type = Java.Lang.Class.FromType(typeof(List<DefaultFieldTypeWithHints>));
			Type fieldTypeListType = TypeToken.Get(type).GetType();
			Stream inputStream = mResources.OpenRawResource(Resource.Raw.default_field_types);
			List<DefaultFieldTypeWithHints> fieldTypes = null;

			using (Reader reader = new BufferedReader(new InputStreamReader(inputStream, "UTF-8")))
			{
				try
				{
					fieldTypes = mGson.FromJson(reader, type);
				}
				catch (Java.IO.IOException ex)
				{
					Util.Loge(ex, "Exception during deserialization of FieldTypes.");
				}
			}
			return fieldTypes;
		}
	}
}