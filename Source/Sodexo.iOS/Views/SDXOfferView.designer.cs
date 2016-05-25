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
	[Register ("SDXOfferView")]
	partial class SDXOfferView
	{
		[Outlet]
		Sodexo.iOS.SDXLabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavigationBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		Sodexo.iOS.SDXLabel StatusLB { get; set; }

		[Action ("OnNavigationBtn_Pressed:")]
		partial void OnNavigationBtn_Pressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}

			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}

			if (StatusLB != null) {
				StatusLB.Dispose ();
				StatusLB = null;
			}

			if (NavigationBtn != null) {
				NavigationBtn.Dispose ();
				NavigationBtn = null;
			}
		}
	}
}
