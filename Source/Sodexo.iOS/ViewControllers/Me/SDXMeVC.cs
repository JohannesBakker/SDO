
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXMeVC : SDXBaseVC
	{
		public bool IsFromDashboard = false;

		private UserModel me;

		public SDXMeVC (IntPtr handle) : base (handle)
		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.

			LoadMe ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (IsFromDashboard)
				AddBackButton (5);
			else
				SetHeaderBackground (5);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		#region Private Functions
		private async void LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null) {
				ShowLoading ("Loading...");
				me = await manager.LoadMe (false);

				if (!manager.IsSuccessed) {
					HideLoading ();
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
			}
			me = manager.Me;

			FillContents ();
		}

		private void FillContents ()
		{
			NameLB.Text = me.FirstName + " " + me.LastName;
			TitleLB.Text = me.Title;

			if (me.Photos == null || me.Photos.Count == 0) {
				HideLoading ();
				return;
			}

			PhotoModel photo = me.Photos [me.Photos.Count - 1];
			ShowLoading ("Loading...");
			JHImageManager imgManager = new JHImageManager ();
			imgManager.LoadCompleted += (object sender, byte[] bytes) => {
				InvokeOnMainThread (delegate {
					HideLoading ();
					AvatarIV.Alpha = 0.5f;
					AvatarIV.Image = Util.GetImageFromByteArray (bytes);
					UIView.Animate (0.5f, () => {
						AvatarIV.Alpha = 1;
					});
				});
			};

			SizeF size = new SizeF (AvatarIV.Frame.Width * 2, AvatarIV.Frame.Height * 2);
			imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int)size.Width, (int)size.Height);
		}

		private void SetPhoto (UIImage image)
		{
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());
			AvatarIV.Image = image;
			AvatarIV.Alpha = 0.5f;
			UIView.Animate (0.3f, ()=>{
				AvatarIV.Alpha = 1;
			});
			PerformSelector (new MonoTouch.ObjCRuntime.Selector ("UploadAvatar"), null, 0.5f);
		}

		[Export ("UploadAvatar")]
		private async void UploadAvatar ()
		{
			ShowLoading ("Uploading...");

			SDXImageManager manager = TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			string fileName = me.LoginName.ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, "User", me.UserId.ToString ());

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (AvatarIV.Image);

			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			me.Photos.Add (photo);

			SizeF scaledSize = new SizeF (AvatarIV.Frame.Width * 2, AvatarIV.Frame.Height * 2);
			UIImage scaledImg = Util.GetScaledImage (AvatarIV.Image, scaledSize);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.WriteImageToFile (sasRequest.FileName, Util.GetByteArrayFromImage(scaledImg), (int)scaledSize.Width, (int)scaledSize.Height);
			HideLoading ();
			new UIAlertView ("Retail Ranger", "Successfully uploaded.", null, "OK", null).Show ();
		}
		#endregion

		#region Button Actions
		partial void OnLogoutBtn_Pressed (SDXButton sender)
		{
			Logout ();
		}

		partial void OnUploadBtn_Pressed (SDXButton sender)
		{
			JHImagePickHelper helper = new JHImagePickHelper (this);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}
		#endregion
	}
}

