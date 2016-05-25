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
	[Register ("SDXAccountDetailVC")]
	partial class SDXAccountDetailVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView AccountInfoView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AddressLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LocationIDLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LocationNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Action ("OnAccountEditBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAccountEditBtn_Pressed (UIButton sender);

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
