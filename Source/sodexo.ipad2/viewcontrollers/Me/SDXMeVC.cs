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

namespace Sodexo.Ipad2
{
	partial class SDXMeVC : SDXBaseVC
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
			//this is added because on ios 7 there is a view size glitch which restarts the view dimensions to height = 1024 and width 768 - reasons so far unknows
			View.Frame = new RectangleF (0, 0, 1024, 594);

			Util.MoveViewToX (SideView, View.Frame.Size.Width + SideView.Frame.Size.Width);
			Util.MoveViewToY (SideView, 0);
			ShowLoading ("Loading...");

			NameLB.Text = "";
			TitleLB.Text = "";

			NameLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 50);
			TitleLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 24);
			LoadMe ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (5);
			SetSectionName ("Me");
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

				me = await manager.LoadMe (false);

				if (!manager.IsSuccessed) {
					HideLoading ();
					new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
					return;
				}
			}
			me = manager.Me;

			FillContents ();
		}

		private void FillContents ()
		{
			var fullname = me.FirstName + " " + me.LastName;
			fullname = fullname.ToUpper ();
			NameLB.Text = fullname;

			if (me.Title != null) {
				TitleLB.Text = me.Title.ToUpper ();
			}
			float nametH = Util.GetHeightOfString (NameLB.Text, NameLB.Frame.Width, NameLB.Font) + 10;

			RectangleF frame = NameLB.Frame;
			frame.Height = nametH;
			frame.Y = View.Frame.Height / 2 - nametH / 2;
			Console.WriteLine (frame.Y);
			NameLB.Frame = frame;
			//NameLB.Text = item.Text;

			float titletH = Util.GetHeightOfString (NameLB.Text, NameLB.Frame.Width, NameLB.Font) + 10;

			RectangleF tframe = TitleLB.Frame;
			tframe.Height = titletH;
			tframe.Y = frame.Height + frame.Y;
			TitleLB.Frame = tframe;


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

//			Util.MoveViewToY (SideView, 0);
//			Util.MoveViewToX (SideView, 1024);
//			SideView.Frame = new RectangleF (1024, 0, SideView.Frame.Size.Width, SideView.Frame.Size.Height);
			UIView.Animate (0.8f, ()=>{
				Util.MoveViewToX(SideView,View.Frame.Width-SideView.Frame.Width);
			});

//			Console.WriteLine ("-------X------"+SideView.Frame.X);
//			Console.WriteLine ("-------View.X------"+View.Frame.X);
//			Console.WriteLine ("-------View.Width------"+View.Frame.Width);
//			Console.WriteLine ("-------View.Height------"+View.Frame.Height);
//			Console.WriteLine ("-------Orientation-----"+this.InterfaceOrientation);

			SizeF size = new SizeF (AvatarIV.Frame.Width * 2, AvatarIV.Frame.Height * 2);
			imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int)size.Width, (int)size.Height);
			HideLoading ();
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
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (AvatarIV.Image);

			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				HideLoading ();
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				HideLoading ();
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			me.Photos.Add (photo);

			SizeF scaledSize = new SizeF (AvatarIV.Frame.Width * 2, AvatarIV.Frame.Height * 2);
			UIImage scaledImg = Util.GetScaledImage (AvatarIV.Image, scaledSize);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.WriteImageToFile (sasRequest.FileName, Util.GetByteArrayFromImage(scaledImg), (int)scaledSize.Width, (int)scaledSize.Height);
			HideLoading ();
			new UIAlertView ("Retail Ranger", "Successfully uploaded.", null, "OK", null).Show ();

			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationUserPhotoUpdated,null);
		}
		#endregion

		#region Button Actions
		partial void OnLogoutBtn_Pressed (SDXButton sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationLogout, null);
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
