// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Sodexo.Ipad2
{
	[Register ("SDXOfferView")]
	partial class SDXOfferView
	{
		[Outlet]
		Sodexo.Ipad2.SDXLabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavigationBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel StatusLB { get; set; }

		[Action ("OnNavigationBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNavigationBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
