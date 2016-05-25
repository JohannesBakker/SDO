using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Sodexo.Core;
using TinyIoC;
using System.Drawing;

namespace Sodexo.iOS
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
				UIImagePickerController picker = new UIImagePickerController ();
				picker.SourceType = UIImagePickerControllerSourceType.Camera;
				picker.Delegate = new SDXImagePickerDelegate (PhotoPicked);
				viewController.PresentViewController (picker, true, null);
			} else {
				new UIAlertView ("Retail Ranger", "Camera is not available.", null, "OK", null).Show ();
			}
		}

		private void SelectPhoto ()
		{
			UIImagePickerController picker = new UIImagePickerController ();
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			picker.Delegate = new SDXImagePickerDelegate (PhotoPicked);
			viewController.PresentViewController (picker, true, null);
		}
		#endregion
	}

	#region ImagePickerDelegate
	public class SDXImagePickerDelegate : UIImagePickerControllerDelegate
	{
		private EventHandler<UIImage> photoPicked;

		public SDXImagePickerDelegate (EventHandler<UIImage> photoPicked)
		{
			this.photoPicked = photoPicked;
		}

		public override void Canceled (UIImagePickerController picker)
		{
			photoPicked (picker, null);
			picker.DismissViewController (true, null);
		}

		public override void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
		{
			//var fixedImage = Util.GetOrientationFixedImage (image);
			var manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			var maxDimension = (float)manager.MaxPhotoUploadDimension;
			var scaledImage = Util.GetScaledImage (image, new SizeF (maxDimension, maxDimension));  // this will also correct photo orientation
			photoPicked (picker, scaledImage);
			picker.DismissViewController (true, null);
		}
	}
	#endregion
}

