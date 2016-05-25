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
	[Register ("SDXSelectPlanogramVC")]
	partial class SDXSelectPlanogramVC
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ActionsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton BackBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton BackToDecisionTreeBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXDropTableView DropTableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView DropView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton NextBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PlanogramAvatarIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel PlanogramDetailsLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel PlanogramNameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PlanogramPictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel PlanogramTitleLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView PlanogramView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView ScrollView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
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
			if (ActionsView != null) {
				ActionsView.Dispose ();
				ActionsView = null;
			}
			if (BackBtn != null) {
				BackBtn.Dispose ();
				BackBtn = null;
			}
			if (BackToDecisionTreeBtn != null) {
				BackToDecisionTreeBtn.Dispose ();
				BackToDecisionTreeBtn = null;
			}
			if (DropTableView != null) {
				DropTableView.Dispose ();
				DropTableView = null;
			}
			if (DropView != null) {
				DropView.Dispose ();
				DropView = null;
			}
			if (NextBtn != null) {
				NextBtn.Dispose ();
				NextBtn = null;
			}
			if (PlanogramAvatarIV != null) {
				PlanogramAvatarIV.Dispose ();
				PlanogramAvatarIV = null;
			}
			if (PlanogramDetailsLB != null) {
				PlanogramDetailsLB.Dispose ();
				PlanogramDetailsLB = null;
			}
			if (PlanogramNameLB != null) {
				PlanogramNameLB.Dispose ();
				PlanogramNameLB = null;
			}
			if (PlanogramPictureIV != null) {
				PlanogramPictureIV.Dispose ();
				PlanogramPictureIV = null;
			}
			if (PlanogramTitleLB != null) {
				PlanogramTitleLB.Dispose ();
				PlanogramTitleLB = null;
			}
			if (PlanogramView != null) {
				PlanogramView.Dispose ();
				PlanogramView = null;
			}
			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
			if (SetAsPlanogramBtn != null) {
				SetAsPlanogramBtn.Dispose ();
				SetAsPlanogramBtn = null;
			}
		}
	}
}
