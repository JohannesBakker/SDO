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
	[Register ("SDXAccountsDetailVC")]
	partial class SDXAccountDetailVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView AccountListView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView AccountScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ManagePanelView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OfferDetailView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView SelectPlanogramView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Action ("AddNewAccount_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void AddNewAccount_Pressed (UIButton sender);

		[Action ("AddNewOutlet_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void AddNewOutlet_Pressed (UIButton sender);

		[Action ("ManageAccount_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ManageAccount_Pressed (UIButton sender);

		[Action ("OnCellAddDetailAddBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAddDetailAddBtn_Pressed (SDXButton sender);

		[Action ("OnCellAddDetailCancelBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAddDetailCancelBtn_Pressed (SDXButton sender);

		[Action ("OnCellAddNewBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAddNewBtn_Pressed (UIButton sender);

		[Action ("OnCellOutletEditBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellOutletEditBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
