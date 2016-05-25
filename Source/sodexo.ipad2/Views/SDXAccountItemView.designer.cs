// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sodexo.Ipad2
{
	[Register ("SDXAccountItemView")]
	partial class SDXAccountItemView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton BtnEdit { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView BtnEditImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton BtnSelect { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imgStatus { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAccouontNumber { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAddress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLocation { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblNumber { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewRoot { get; set; }

		[Action ("OnAccountBtn_Pressed:")]
		partial void OnAccountBtn_Pressed (MonoTouch.UIKit.UIButton sender);

		[Action ("OnAccountEditBtnPressed:")]
		partial void OnAccountEditBtnPressed (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BtnEdit != null) {
				BtnEdit.Dispose ();
				BtnEdit = null;
			}

			if (BtnSelect != null) {
				BtnSelect.Dispose ();
				BtnSelect = null;
			}

			if (imgStatus != null) {
				imgStatus.Dispose ();
				imgStatus = null;
			}

			if (lblAccouontNumber != null) {
				lblAccouontNumber.Dispose ();
				lblAccouontNumber = null;
			}

			if (lblAddress != null) {
				lblAddress.Dispose ();
				lblAddress = null;
			}

			if (lblLocation != null) {
				lblLocation.Dispose ();
				lblLocation = null;
			}

			if (lblNumber != null) {
				lblNumber.Dispose ();
				lblNumber = null;
			}

			if (viewRoot != null) {
				viewRoot.Dispose ();
				viewRoot = null;
			}

			if (BtnEditImage != null) {
				BtnEditImage.Dispose ();
				BtnEditImage = null;
			}
		}
	}
}
