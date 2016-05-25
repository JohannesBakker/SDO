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
	[Register ("SDXOfferCell")]
	partial class SDXOfferCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel PlanogramLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel StatusLB { get; set; }

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
			if (PlanogramLB != null) {
				PlanogramLB.Dispose ();
				PlanogramLB = null;
			}
			if (StatusLB != null) {
				StatusLB.Dispose ();
				StatusLB = null;
			}
		}
	}
}
