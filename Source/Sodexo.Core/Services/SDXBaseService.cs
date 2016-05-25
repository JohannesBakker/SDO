using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Diagnostics;
using TinyIoC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ModernHttpClient;
using MonoTouch.Foundation;

namespace Sodexo.Core
{
	public class SDXBaseService
	{
		public bool IsSuccessed = false;
		public string ErrorMessage = "";

		private const int MaxNumberOfAttempts = 3;

		public SDXBaseService ()
		{

		}

		private HttpClient GetHttpClient()
		{
			var nativeMessageHandler = new NativeMessageHandler ();
			var client = new HttpClient (nativeMessageHandler);

			SDXAuthManager authManager = TinyIoCContainer.Current.Resolve <SDXAuthManager> ();
			var fedAuthTokens = authManager.GetSecurityToken ();
			var cookievalue = string.Empty;
			foreach (var authtoken in fedAuthTokens)
			{
				cookievalue += string.Format ("{0}={1}; ", authtoken.Key, authtoken.Value);
			}
			client.DefaultRequestHeaders.Add("Cookie",cookievalue);

			var versionName = string.Empty;
			#if __IOS__
			try
			{
				versionName = string.Format("{0}-{1}", NSBundle.MainBundle.InfoDictionary ["CFBundleDisplayName"], NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"]);
			} catch (Exception)
			{
				versionName = "iOS";
			}

			#else
			try
			{
				var context = Android.SodexoApp.Context.ApplicationContext;
				var pkgInfo = context.PackageManager.GetPackageInfo (context.PackageName,0);
				versionName = pkgInfo.PackageName  + " - " + pkgInfo.VersionName + " - " + pkgInfo.VersionCode;

			} catch (Exception)
			{
				versionName = "Android";
			}
			#endif
			client.DefaultRequestHeaders.Add("x-application-platform", versionName);

			return client;
		}

		private async Task<string> Result (HttpResponseMessage response)
		{
			//response.EnsureSuccessStatusCode ();
			ErrorMessage = "";

			if (response == null) {
				IsSuccessed = false;
				ErrorMessage = "Request Cancelled, Please try again.";
				return "";
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created) {
				IsSuccessed = true;
			}  else {
				IsSuccessed = false;
			}

			var responseBody = "";

			if (response.Content != null) {
				responseBody = await response.Content.ReadAsStringAsync ();
			}

			if (response.Content == null 
				|| response.Content.Headers == null 
				|| (response.Content.Headers.ContentType != null && !response.Content.Headers.ContentType.ToString().Contains("application/json")) )
			{
				IsSuccessed = false;
				ErrorMessage = Constants.ERROR_MSG_NEED_REAUTH;
				return responseBody;
			}

			if (!IsSuccessed)
				ObtainErrorMessage (responseBody);

			return responseBody;
		}

		// Get Method Without Auth Info
		protected async Task<string> GetAsyncWithoutAuth (string uri)
		{
			using (var client = new HttpClient ()) {
				var timer = Stopwatch.StartNew ();
				HttpResponseMessage response = null;
				var numberOfAttempts = 0;
				do {
					try {
						Console.WriteLine ("starting GetAsyncWithoutAuth of '{0}'", uri);
						response = await client.GetAsync (uri);
						break;
					}  catch (Exception e) {
						Console.WriteLine ("Error in GetAsyncWithoutAuth : " + e.Message);
						numberOfAttempts ++;
					}
				}  while (numberOfAttempts < MaxNumberOfAttempts);
				var ms = timer.ElapsedMilliseconds;
				Console.WriteLine ("service timer ms:{0}", ms);

				return await Result (response);
			}
		}

		// Get Method
		protected async Task<string> GetAsync (string uri)
		{
            using (var client = GetHttpClient())
            {
                var timer = Stopwatch.StartNew();
                HttpResponseMessage response = null;
                try
                {
                    Console.WriteLine("starting GetAsync of '{0}'", uri);
                    response = await client.GetAsync(uri);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in GetAsync : " + e.Message);
                }
                var ms = timer.ElapsedMilliseconds;
                Console.WriteLine("service timer ms:{0}", ms);

                return await Result(response);
            }
		}

		// Delete Method
		protected async Task <string> DeleteAsync (string uri)
		{
			using (var client = GetHttpClient()) {
				var timer = Stopwatch.StartNew ();
				HttpResponseMessage response = null;
				try {
					Console.WriteLine ("starting DeleteAsync of '{0}'", uri);
					response = await client.DeleteAsync (uri);
				}  catch (Exception e) {
					Console.WriteLine ("Error in DeleteAsync : " + e.Message);
				}
				var ms = timer.ElapsedMilliseconds;
				Console.WriteLine ("service timer ms:{0}", ms);

				return await Result (response);
			}
		}

		// Post Method
		protected async Task<string> PostAsync (string uri, HttpContent content)
		{
			using (var client = GetHttpClient()) {
				var timer = Stopwatch.StartNew ();
				client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				HttpResponseMessage response = null;
				try {
					Console.WriteLine ("starting PostAsync of '{0}'", uri);
					response = await client.PostAsync (uri, content);
				}  catch (Exception e) {
					Console.WriteLine ("Error in PostAsync : " + e.Message);
				}
				var ms = timer.ElapsedMilliseconds;
				Console.WriteLine ("service timer ms:{0}", ms);

				return await Result (response);
			}
		}

		// Put Async Method
		protected async Task<string> PutAsync (string uri, HttpContent content)
		{
			using (var client = GetHttpClient()) {
				var timer = Stopwatch.StartNew ();
				client.DefaultRequestHeaders.Add ("x-ms-blob-type", "BlockBlob");
				HttpResponseMessage response = null;
				try {
					Console.WriteLine ("starting PutAsync of '{0}'", uri);
					response = await client.PutAsync(uri, content);
				}  catch (Exception e) {
					Console.WriteLine ("Error in PutAsync : " + e.Message);
				}
				var ms = timer.ElapsedMilliseconds;
				Console.WriteLine ("service timer ms:{0}", ms);

				return await Result (response);
			}
		}

		// Patch Async
		protected async Task<string> PatchAsync (string uri, HttpContent content)
		{
			using (var client = GetHttpClient()) {
				var timer = Stopwatch.StartNew ();
				client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				var method = new HttpMethod ("PATCH");
				var request = new HttpRequestMessage (method, uri) {
					Content = content
				} ;

				HttpResponseMessage response = null;
				try {
					Console.WriteLine ("starting PatchAsync of '{0}'", uri);
					response = await client.SendAsync (request);
				}  catch (Exception e) {
					Console.WriteLine ("Error in PatchAsync : " + e.Message);
				}
				var ms = timer.ElapsedMilliseconds;
				Console.WriteLine ("service timer ms:{0}", ms);

				return await Result (response);
			}
		}

		private void ObtainErrorMessage (string responseBody)
		{
			if (responseBody != "") {
				JObject errorJObj = null;
				try {
					errorJObj = JObject.Parse (responseBody);
					ErrorMessage = (string) errorJObj ["message"];
				}  catch (Exception ) {
					Console.WriteLine ("Parsing Error: " + responseBody);
					ErrorMessage = Constants.ERROR_MSG_RESOURCE_NOT_FOUND;
				}
			}  else {
				ErrorMessage = Constants.ERROR_MSG_BAD_REQUEST;
			}
		}

		protected void WriteResponseToConsole(string responseBody)
		{
			if (string.IsNullOrEmpty(responseBody))
			{
				return;
			}

			var manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			if (manager == null || !manager.IsAuthorizedLoaded)
			{
				return;
			}
			if (manager.DeserializeObjectsToConsole)
			{
				Console.WriteLine (JsonConvert.DeserializeObject (responseBody).ToString ());
			}

		}
	}
}