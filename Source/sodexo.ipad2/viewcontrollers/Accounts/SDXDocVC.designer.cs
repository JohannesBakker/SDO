// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Sodexo.Ipad2
{
	[Register ("SDXDocVC")]
	partial class SDXDocVC
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView WebView { get; set; }

		[Action ("OnBackButtob_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnBackButtob_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}
