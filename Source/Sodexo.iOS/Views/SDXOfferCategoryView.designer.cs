// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sodexo.iOS
{
	[Register ("SDXOfferCategoryView")]
	partial class SDXOfferCategoryView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton Button { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Action ("OnBtn_Pressed:")]
		partial void OnBtn_Pressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}

			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}

			if (Button != null) {
				Button.Dispose ();
				Button = null;
			}
		}
	}
}
