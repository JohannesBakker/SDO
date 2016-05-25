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
	[Register ("SDXPromotionCell")]
	partial class SDXPromotionCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CategoryIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel DateLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel DescriptionLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView HeaderBg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView MainView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView statusIV { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CategoryIV != null) {
				CategoryIV.Dispose ();
				CategoryIV = null;
			}
			if (DateLB != null) {
				DateLB.Dispose ();
				DateLB = null;
			}
			if (DescriptionLB != null) {
				DescriptionLB.Dispose ();
				DescriptionLB = null;
			}
			if (HeaderBg != null) {
				HeaderBg.Dispose ();
				HeaderBg = null;
			}
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
			if (statusIV != null) {
				statusIV.Dispose ();
				statusIV = null;
			}
		}
	}
}
