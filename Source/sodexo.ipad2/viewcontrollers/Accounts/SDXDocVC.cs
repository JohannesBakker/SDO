
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sodexo.Ipad2
{
	public partial class SDXDocVC : SDXBaseVC
	{
		public string DocUrl;

		public SDXDocVC (IntPtr handle) : base (handle)
		{

		}

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
			//NavigationController.PopViewControllerAnimated (true);
			this.View.RemoveFromSuperview();
		}
	}
}

