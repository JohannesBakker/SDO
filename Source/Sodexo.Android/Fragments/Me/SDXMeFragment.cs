
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

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;


namespace Sodexo.Android
{
	public class SDXMeFragment : SDXBaseFragment
	{
		public bool IsFromDashboard = false;

		private UserModel me;

		View view;
		ImageView avatarIv;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHeaderTitle ("Me", 5);
			HideAllHeaderButtons ();
			if (IsFromDashboard)
				AddBackBtn (5);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Me, container, false);
			LayoutView (view);

			GetInstance ();

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			LoadMe ();
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			var uploadBtn = view.FindViewById (Resource.Id.me_upload_btn) as Button;
			uploadBtn.Click += UploadBtn_OnClicked;
			var logoutBtn = view.FindViewById (Resource.Id.me_logout_btn) as Button;
			logoutBtn.Click += LogoutBtn_OnClicked;
		}

		private async void LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null) {
				Util.ShowLoading (Activity);
				me = await manager.LoadMe (false);

				if (!manager.IsSuccessed) {
					Util.HideLoading ();
					Util.ShowAlert (manager.ErrorMessage, Activity);
					return;
				}
			}
			me = manager.Me;

			FillContents ();
		}

		private void FillContents ()
		{
			var nameTv = view.FindViewById (Resource.Id.me_name_tv) as TextView;
			var titleTv = view.FindViewById (Resource.Id.me_title_tv) as TextView;
			avatarIv = view.FindViewById (Resource.Id.me_avatar_iv) as ImageView;

			nameTv.Text = me.FirstName + " " + me.LastName;
			titleTv.Text = me.Title;

			if (me.Photos == null || me.Photos.Count == 0) {
				Util.HideLoading ();
				return;
			}

			PhotoModel photo = me.Photos [me.Photos.Count - 1];
			Util.ShowLoading (Activity);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.LoadCompleted += (object sender, byte[] bytes) => {
				Activity.RunOnUiThread (delegate {
					Util.HideLoading ();
					if (bytes == null)
						return;
					using (Bitmap bitmap = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
						if (bitmap != null)
							avatarIv.SetImageBitmap (bitmap);
					}
				});
			};

			imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, Math.Max (avatarIv.LayoutParameters.Width, avatarIv.LayoutParameters.Height), 0);
		}

		private void OnPhotoSelected (object sender, Bitmap bitmap)
		{
			if (bitmap == null)
				return;

			Console.WriteLine ("Picked Image Size : (" + bitmap.Width + "," + bitmap.Height + ")");
			avatarIv.SetImageBitmap (bitmap);
			(new Handler ()).PostDelayed (new Java.Lang.Runnable (() => {
				Activity.RunOnUiThread (() => {
					UploadAvatar (bitmap);
				});
			}), 500);
		}
		
		private void UploadAvatar (Bitmap bitmap)
		{
			string fileName = me.LoginName.ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			int maxImgSz = Math.Max (avatarIv.LayoutParameters.Width, avatarIv.LayoutParameters.Height);
			DoUploadPhoto (bitmap, fileName, "User", me.UserId.ToString (), maxImgSz, (object sender, PhotoModel photo) => {
				if (photo == null)
					return;
				me.Photos.Add (photo);
			});
		}
		#endregion

		#region Button Actions
		private void LogoutBtn_OnClicked (object sender, EventArgs args)
		{
			((MainActivity)Activity).LogOut ();
		}

		private void UploadBtn_OnClicked (object sender, EventArgs args)
		{
			if (PhotoSelectedEvent == null)
				PhotoSelectedEvent = OnPhotoSelected;
			PickPhoto ();
		}
		#endregion
	}
}

