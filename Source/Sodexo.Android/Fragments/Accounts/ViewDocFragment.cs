
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;
using Android.Media;
using Android.Webkit;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;
using Android.Net.Http;
using System.Net;
using System.IO;
using Environment = Android.OS.Environment;

namespace Sodexo.Android
{
	public class ViewDocFragment : SDXBaseFragment
	{
		View view;

		public string DocUrl;
		public StorageContentModel StorageContent;
        public string PdfFileName;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.ViewDoc, container, false);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			Console.WriteLine ("StorageContent.StorageContentUrl = " + StorageContent.StorageContentUrl);
			Console.WriteLine ("StorageContent.ProcessedPhotoBaseUrl = " + StorageContent.ProcessedPhotoBaseUrl);

			DocUrl = StorageContent.StorageContentUrl;

			if (DocUrl != null && DocUrl != "") {

				var webview = view.FindViewById (Resource.Id.viewdoc_webview) as WebView;
				webview.Settings.JavaScriptEnabled = true;
				webview.Settings.BuiltInZoomControls = true;
				webview.Settings.SetSupportZoom (true);
				webview.Settings.AllowFileAccess = true;
				webview.Settings.AllowContentAccess = true;
				webview.Settings.LoadWithOverviewMode = true;
				webview.Settings.UseWideViewPort = true;
				webview.Settings.DefaultZoom = WebSettings.ZoomDensity.Far;
				webview.SetWebViewClient (new HelloWebViewClient ());
				webview.SetWebChromeClient (new WebChromeClient ());

				if (DocUrl.EndsWith(".bmp") || DocUrl.EndsWith(".jpg") || DocUrl.EndsWith(".png")|| DocUrl.EndsWith(".gif"))
				{
					DocUrl = StorageContent.ProcessedPhotoBaseUrl + "?w=1500";
					webview.LoadUrl (DocUrl);
				}
				else
				{

					DocUrl = "http://docs.google.com/viewer?url=" + DocUrl;
					webview.LoadUrl (DocUrl);

					//apply pdf solution here
					//DownloadPDFFile(DocUrl);  //note... if this is ever enabled... you need to make sure it is actually a PDF and not a DOC or XLS or something odd
				}
			}
		}

        private void DownloadPDFFile(String uri)
        {
            ShowLoading();

            PdfFileName = uri.Substring(uri.LastIndexOf('/') + 1);
            if (PdfFileName.Length <= 0)
                PdfFileName = uri;

            var webClient = new WebClient();
            Uri uriPDF = new Uri(uri);

            webClient.DownloadDataAsync(uriPDF);
            webClient.DownloadDataCompleted += (s, e) =>
            {
                var bytes = e.Result;
                if (e.Cancelled)
                    HideLoading();
                else
                {
                    String localPath = System.IO.Path.Combine(Environment.ExternalStorageDirectory.Path + "/Download/Sodexo_PDF/", PdfFileName);
                    Directory.CreateDirectory(Environment.ExternalStorageDirectory.Path + "/Download/Sodexo_PDF/");
                    File.WriteAllBytes(localPath, bytes);

                    DownloadCompleted(localPath);
                }
            };
        }

        private void DownloadCompleted(String localPDFPath)
        {
            HideLoading();

            //global::Android.Net.Uri uri = global::Android.Net.Uri.Parse("file://" + localPDFPath);
            Java.IO.File file = new Java.IO.File(localPDFPath);
            if (file.Exists())
            {
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(global::Android.Net.Uri.FromFile(file), "application/pdf");
                intent.SetFlags(ActivityFlags.ClearTop);

                try
                {
                    StartActivity(intent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
	}

	public class HelloWebViewClient : WebViewClient
	{
		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			view.LoadUrl (url);
			return true;
		}

		public override void OnReceivedSslError(WebView view, SslErrorHandler handler,SslError error)
		{
			handler.Proceed();
		}
	}
}

