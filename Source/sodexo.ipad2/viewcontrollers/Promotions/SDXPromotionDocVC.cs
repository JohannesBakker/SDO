using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Sodexo.Ipad2
{
	partial class SDXPromotionDocVC : SDXBaseVC
	{
		public SDXPromotionDocVC (IntPtr handle) : base (handle)
		{
		}

		public string DocUrl;

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (DocUrl != null && DocUrl != "") {
				ShowLoading ("Loading...");
				WebView.LoadRequest (new NSUrlRequest (new NSUrl (DocUrl)));
				WebView.LoadFinished += (object sender, EventArgs e) => {
					HideLoading ();
				};
			}
		}
		#endregion

		partial void OnBackButtob_Pressed(UIButton sender)
		{
			NavigationController.PopViewControllerAnimated (true);
		}
	}
}
