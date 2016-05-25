
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sodexo.Ipad2
{
	public partial class SDXFullImageVC : SDXBaseVC
	{
		public UIImage Picture;

		public SDXFullImageVC (IntPtr handle) : base (handle)
		{

		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PictureIV.Image = Picture;
			ScrollView.MaximumZoomScale = 3f;
			ScrollView.MinimumZoomScale = 1f;
			ScrollView.ViewForZoomingInScrollView += (UIScrollView v) => {
				return PictureIV;
			};

			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer ();
			tapGesture.AddTarget (() => {
				DismissViewController (true, null);
			});
			PictureIV.AddGestureRecognizer (tapGesture);
		}
		#endregion

		#region BUTTON EVENTS
		partial void OnImageView_Pressed (MonoTouch.UIKit.UIButton sender)
		{
			this.View.RemoveFromSuperview();
		}
		#endregion
	}
}

