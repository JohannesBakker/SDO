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
	[Register ("SDXDecisionTreeRadioView")]
	partial class SDXDecisionTreeRadioView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton Button { get; set; }

		[Outlet]
		Sodexo.iOS.SDXLabel DescLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView RadioIV { get; set; }

		[Action ("OnRadioBtn_Pressed:")]
		partial void OnRadioBtn_Pressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Button != null) {
				Button.Dispose ();
				Button = null;
			}

			if (DescLB != null) {
				DescLB.Dispose ();
				DescLB = null;
			}

			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}

			if (RadioIV != null) {
				RadioIV.Dispose ();
				RadioIV = null;
			}
		}
	}
}
