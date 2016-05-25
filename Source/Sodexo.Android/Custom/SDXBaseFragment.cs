using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.InputMethods;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Android
{
	public class SDXBaseFragment : Fragment
	{
		public int PopCount = 1;
        public static bool bPopFlag = false;

		View view;
		GlobalLayoutListener layoutListner;

		protected EventHandler <Bitmap> PhotoSelectedEvent = null;

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();

			if (view != null && layoutListner != null)
				view.ViewTreeObserver.RemoveGlobalOnLayoutListener (layoutListner);
		}

		#region Layout Functions
		protected void LayoutView (View view)
		{
			this.view = view;

			layoutListner = new GlobalLayoutListener (Activity, view);
			ViewTreeObserver vto = view.ViewTreeObserver;
			vto.AddOnGlobalLayoutListener (layoutListner);
		}
		#endregion

		#region Header
		protected void SetHeaderTitle (string title, int index)
		{
			((MainActivity)Activity).HeaderTitleTv.Text = title;
			SetHeaderBackground (index);
		}

		protected void HideAllHeaderButtons ()
		{
			((MainActivity)Activity).HideAllHeaderButtons ();
		}

		protected void AddBackBtn (int index)
		{
			((MainActivity)Activity).BackBtn.Visibility = ViewStates.Visible;
		}

		protected void SetHeaderBackground (int index)
		{
			((MainActivity)Activity).SetHeaderBg (index);
			((MainActivity)Activity).HighlightMenuItem (index);
		}
		#endregion

		#region Push, Pop
		protected void PushFragment (Fragment fragment, string fragmentName)
		{
			((MainActivity)Activity).PushFragment (fragment, fragmentName);
		}

		public void PopFragment ()
		{
			((MainActivity)Activity).PopFragment (PopCount);
		}
		#endregion

		#region Keyboard
		public void HideKeyboard (EditText et)
		{
			var inputManager = (InputMethodManager) Activity.GetSystemService (Activity.InputMethodService);
			inputManager.HideSoftInputFromWindow (et.WindowToken, HideSoftInputFlags.None);
		}
		#endregion

		#region Reference Data
		async protected Task <bool> LoadReferenceData ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel != null)
				return true;

			Util.ShowLoading (Activity);
			await manager.LoadReferenceData ();
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return false;
			}
			return true;
		}
		#endregion

		#region Photo
		protected void PickPhoto ()
		{
			((MainActivity)Activity).PickPhoto ();
		}

		public void SetPhoto (Bitmap bitmap)
		{
			if (PhotoSelectedEvent != null)
				PhotoSelectedEvent (null, bitmap);
		}

		async protected void DoUploadPhoto (Bitmap bmp, string fileName, string modelName, string modelId, int maxImgSize, EventHandler<PhotoModel> OnCompleted)
		{
			Util.ShowLoading (Activity);

			SDXImageManager manager = TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, modelName, modelId);
			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				OnCompleted (this, null);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromBitmap (bmp);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);
			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				OnCompleted (this, null);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);
			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				OnCompleted (this, null);
				return;
			}

			if (maxImgSize > 0) {
				using (Bitmap scaledBmp = Util.GetScaledBitmap (bmp, maxImgSize)) {
					var imgMgr = new JHImageManager ();
					byte[] scaledBytes = Util.GetByteArrayFromBitmap (scaledBmp);
					imgMgr.WriteImageToFile (sasRequest.FileName, scaledBytes, maxImgSize, 0);
				}

				Util.HideLoading ();
				Util.ShowAlert ("Successfully uploaded.", Activity);
			} else {
				// Upload Feedback Image
				Util.HideLoading ();
			}

			OnCompleted (this, photo);
		}
		#endregion

		#region Other
		protected void ShowErrorMessage (string errorMsg)
		{
			if (errorMsg.Contains (Sodexo.Core.Constants.ERROR_MSG_NEED_REAUTH)) {
				var builder = new AlertDialog.Builder (Activity);
				builder.SetMessage (errorMsg);
				builder.SetPositiveButton ("OK", (object sender, DialogClickEventArgs e) => {
					((MainActivity)Activity).LogOut ();
				});
				builder.Create ().Show ();
			} else {
				ShowAlert (errorMsg);
			}
		}

		protected void ShowLoading ()
		{
			Util.ShowLoading (Activity);
		}

		protected void HideLoading ()
		{
			Util.HideLoading ();
		}

		protected void ShowAlert (string msg)
		{
			Util.ShowAlert (msg, Activity);
		}
		#endregion
	}
}

