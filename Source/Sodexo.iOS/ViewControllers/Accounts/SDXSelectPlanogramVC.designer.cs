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

namespace Sodexo.iOS
{
	[Register ("SDXSelectPlanogramVC")]
	partial class SDXSelectPlanogramVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView ActionsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AddressLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton BackBtn { get; set; }

		[Outlet]
		Sodexo.iOS.SDXButton BackToDecisionTreeBtn { get; set; }

		[Outlet]
		Sodexo.iOS.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LocationIdLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NextBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OfferNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OfferPictureIV { get; set; }

		[Outlet]
		Sodexo.iOS.SDXLabel OfferStatusLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OutletNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OutletPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlanogramAvatarIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlanogramDetailsLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlanogramNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlanogramPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlanogramTitleLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PlanogramView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		SDXButton SetAsPlanogramBtn { get; set; }

		[Action ("OnBackBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnBackBtn_Pressed (SDXButton sender);

		[Action ("OnBackToDecisionTreeBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnBackToDecisionTreeBtn_Pressed (SDXButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		[Action ("OnSetAsPlanogramBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSetAsPlanogramBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
