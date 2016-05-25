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
	[Register ("SDXFeedbackPlanogramCell")]
	partial class SDXFeedbackPlanogramCell
	{
		[Outlet]
		UIView MainView { get; set; }

		[Outlet]
		UIImageView PictureIV { get; set; }

		[Outlet]
		SDXLabel plainLb { get; set; }

		[Outlet]
		SDXLabel StatusLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView arrow { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel OfferNameLb { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (arrow != null) {
				arrow.Dispose ();
				arrow = null;
			}
			if (OfferNameLb != null) {
				OfferNameLb.Dispose ();
				OfferNameLb = null;
			}
		}
	}
}
