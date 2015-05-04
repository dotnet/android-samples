using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace ActivitySceneTransitionBasic.ImageLoader
{

	class FileCache
	{

		public static string SaveLocation;
		public static async Task<string> Download(string url)
		{
			if (string.IsNullOrEmpty (SaveLocation))
				throw new Exception ("Save location is required");
			var fileName = md5 (url);

			return await Download (url, fileName);
		}

		static object locker = new object ();
		public static async Task<string> Download(string url, string fileName)
		{
			try{
				var path = Path.Combine (SaveLocation, fileName);
				if (File.Exists (path))
					return path;

				await GetDownload(url,path);

				return path;
			}
			catch(Exception ex) {
				Console.WriteLine (ex);
				return  "";
			}
		}

		static Dictionary<string,Task> downloadTasks = new Dictionary<string, Task> ();
		static Task GetDownload(string url, string fileName)
		{
			lock (locker) {
				Task task;
				if (downloadTasks.TryGetValue (fileName, out task))
					return task;
				var client = new WebClient ();
				downloadTasks.Add (fileName, task = client.DownloadFileTaskAsync (url, fileName));
				return task;

			}
		}
		static void removeTask(string fileName)
		{
			lock (locker) {
				downloadTasks.Remove (fileName);
			}
		}


		static MD5CryptoServiceProvider checksum = new MD5CryptoServiceProvider ();
		static int hex (int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v-10;
		}

		static string md5 (string input)
		{
			var bytes = checksum.ComputeHash (Encoding.UTF8.GetBytes (input));
			var ret = new char [32];
			for (int i = 0; i < 16; i++){
				ret [i*2] = (char)hex (bytes [i] >> 4);
				ret [i*2+1] = (char)hex (bytes [i] & 0xf);
			}
			return new string (ret);
		}
	}
}