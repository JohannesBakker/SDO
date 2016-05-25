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
	[Register ("SDXOutletCell")]
	partial class SDXOutletCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView AddDetailView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AddNewBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView AddNewView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton DetailAddBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton DetailCancelBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton EditBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView TableView { get; set; }

		[Action ("OnOfferCategoryBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnOfferCategoryBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AddDetailView != null) {
				AddDetailView.Dispose ();
				AddDetailView = null;
			}
			if (AddNewBtn != null) {
				AddNewBtn.Dispose ();
				AddNewBtn = null;
			}
			if (AddNewView != null) {
				AddNewView.Dispose ();
				AddNewView = null;
			}
			if (DetailAddBtn != null) {
				DetailAddBtn.Dispose ();
				DetailAddBtn = null;
			}
			if (DetailCancelBtn != null) {
				DetailCancelBtn.Dispose ();
				DetailCancelBtn = null;
			}
			if (EditBtn != null) {
				EditBtn.Dispose ();
				EditBtn = null;
			}
			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}
			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}
			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}
		}
	}
}
