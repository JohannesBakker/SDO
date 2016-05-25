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
	[Register ("SDXPlanogramCell")]
	partial class SDXPlanogramCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView MainView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PictureIV { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MainView != null) {
				MainView.Dispose ();
				MainView = null;
			}
			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}
			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}
		}
	}
}
