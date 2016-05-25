using System;
using System.Net;
using System.IO;
using System.Text;

using Sodexo.RetailActivation.Portable.Helpers;

namespace Sodexo.Core
{
	public class JHImageManager
	{
		char[] pathSeparatorChars = {'/', ':'};

		public EventHandler<byte[]> LoadCompleted;

		public JHImageManager ()
		{
			
		}

		#region Public Functions
		public void LoadImageAsync (string originPhotoUrl, int width, int height)
		{
			string fileName = GetFileName (originPhotoUrl);
			string localPath = GetFullPath (fileName);
			localPath = localPath.Substring (0, localPath.Length - 4) + "-" + width.ToString () + "-" + height.ToString () + localPath.Substring (localPath.Length - 4);
			var processedPhotoUri = GetProcessedPhotoUri (originPhotoUrl, width, height);
			if (File.Exists (localPath)) {
				var bytes = File.ReadAllBytes (localPath);
				LoadCompleted (processedPhotoUri, bytes);
			} else {
				DownloadImage (processedPhotoUri, localPath);
			}
		}

		public void WriteImageToFile (string fileName, byte[] bytes, int width, int height)
		{
			string localPath = GetFullPath (fileName);
			localPath = localPath.Substring (0, localPath.Length - 4) + "-" + width.ToString () + "-" + height.ToString () + localPath.Substring (localPath.Length - 4);
			File.WriteAllBytes (localPath, bytes);
		}
		#endregion

		#region Private Functions
		private void DownloadImage (string url, string localPath)
		{
			var webClient = new WebClient ();

			webClient.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) => {
				var bytes = e.Result;
				if (bytes == null) {
					if (e.Error != null)
						Console.WriteLine ("Image Download Failed: " + e.Error.ToString());
					LoadCompleted (url, bytes);
					return;
				}

				File.WriteAllBytes (localPath, bytes);

				LoadCompleted (url, bytes);
			};

			webClient.DownloadDataAsync (new Uri (url));
		}

		private string GetProcessedPhotoUri (string originPhotoUrl, int w, int h)
		{
			w = Math.Abs (w); h = Math.Abs (h);
			var uri = originPhotoUrl + "?autorotate=true";
			if (w != 0 && h != 0) {
				uri += "&w=" + w.ToString () + "&h=" + h.ToString () + "&mode=crop";
			} else if (w != 0) {
				uri += "&w=" + w.ToString ();
			} else if (h != 0) {
				uri += "&h=" + h.ToString ();
			}
			return uri;
		}

		private string GetFileName (string url)
		{
			int num = url.LastIndexOfAny (pathSeparatorChars);

			if (num < 0) {
				return url;
			}
			return url.Substring (num + 1);
		}

		private string GetFullPath (string fileName)
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return Path.Combine (documentsPath, fileName);
		}
		#endregion
	}
}

