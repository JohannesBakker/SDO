using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Sodexo.Core;
using TinyIoC;
using System.Drawing;

namespace Sodexo.Ipad2
{
	public class JHImagePickHelper
	{
		private UIViewController viewController;

		public EventHandler <UIImage> PhotoPicked;

		public JHImagePickHelper (UIViewController viewController)
		{
			this.viewController = viewController;
		}

		public void PickPhoto ()
		{
			var actionSheet = new UIActionSheet ("");
			actionSheet.AddButton ("Take a Photo");
			actionSheet.AddButton ("Select Existing One");
			actionSheet.AddButton ("Cancel");
			actionSheet.CancelButtonIndex = 2;
			actionSheet.Clicked += delegate(object s, UIButtonEventArgs e) {
				Console.WriteLine ("Button " + e.ButtonIndex.ToString () + " clicked");
				if (e.ButtonIndex == 0) {
					TakePhoto ();
				} else if (e.ButtonIndex == 1) {
					SelectPhoto ();
				}
			};
			actionSheet.ShowInView (viewController.View);
		}

		#region Private Functions
		private void TakePhoto ()
		{
			if (UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera)) {
//				UIImagePickerController picker = new UIImagePickerController ();
//				picker.SourceType = UIImagePickerControllerSourceType.Camera;
//				picker.Delegate = new SDXImagePickerDelegate (PhotoPicked, true);
//				viewController.PresentViewController (picker, true, null);

				NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationStartPhotoTaking, null);

				UIImagePickerController picker = new UIImagePickerController();
				picker.SourceType = UIImagePickerControllerSourceType.Camera;
				picker.Delegate = new SDXImagePickerDelegate (PhotoPicked, false);

				picker.View.Frame = new RectangleF (0, 100, 1024, 568);//viewController.View.Bounds;
				picker.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

				this.viewController.AddChildViewController (picker);
				picker.DidMoveToParentViewController (this.viewController);
				viewController.View.AddSubview (picker.View);

			} else {
				new UIAlertView ("Retail Ranger", "Camera is not available.", null, "OK", null).Show ();
			}
		}

		private void SelectPhoto ()
		{
			/*UIImagePickerController picker = new UIImagePickerController ();
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			picker.Delegate = new SDXImagePickerDelegate (PhotoPicked);

			Func<UINavigationController,UIInterfaceOrientation> orientation = navigator => UIInterfaceOrientation.LandscapeLeft;
			Func<UINavigationController,UIInterfaceOrientationMask> orientationMask = navigator => UIInterfaceOrientationMask.Landscape;
			picker.GetPreferredInterfaceOrientation = orientation;
			picker.SupportedInterfaceOrientations = orientationMask;

			picker.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
			viewController.PresentViewController (picker, true, null);*/

			UIImagePickerController picker = new UIImagePickerController();
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			picker.Delegate = new SDXImagePickerDelegate (PhotoPicked, false);

			picker.View.Frame = viewController.View.Bounds;
			picker.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			this.viewController.AddChildViewController (picker);
			picker.DidMoveToParentViewController (this.viewController);
			viewController.View.AddSubview (picker.View);
		}


		#endregion
	}

	#region ImagePickerDelegate
	public class SDXImagePickerDelegate : UIImagePickerControllerDelegate
	{
		private EventHandler<UIImage> photoPicked;
		private bool bCamera;

		public SDXImagePickerDelegate (EventHandler<UIImage> photoPicked, bool bCamera)
		{
			this.photoPicked = photoPicked;
			this.bCamera = bCamera;
		}

		public override void Canceled (UIImagePickerController picker)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationEndPhotoTaking, null);

			photoPicked (picker, null);
			picker.DismissViewController (true, null);

			//if (bCamera == false) {
				picker.WillMoveToParentViewController(null);
				picker.View.RemoveFromSuperview();
				picker.RemoveFromParentViewController();
			//}
		}

		public override UIInterfaceOrientation GetPreferredInterfaceOrientation (UINavigationController navigationController)
		{
			return UIInterfaceOrientation.LandscapeLeft;
		}

		public override UIInterfaceOrientationMask SupportedInterfaceOrientations (UINavigationController navigationController)
		{
			return UIInterfaceOrientationMask.Landscape;
		}
		public override void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationEndPhotoTaking, null);

			var fixedImage = Util.GetOrientationFixedImage (image);
			var manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			var maxDimension = (float)manager.MaxPhotoUploadDimension;
			var scaledImage = Util.GetScaledImage (fixedImage, new SizeF (maxDimension, maxDimension));
			photoPicked (picker, scaledImage);
			picker.DismissViewController (true, null);

			//if (bCamera == false) {
				picker.WillMoveToParentViewController(null);
				picker.View.RemoveFromSuperview();
				picker.RemoveFromParentViewController();
			//}
		}
	}
	#endregion
}

