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
	[Register ("SDXAddOutletVC")]
	partial class SDXAddOutletVC
	{
		[Outlet]
		MonoTouch.UIKit.UIButton AddPhotoBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView AddView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView BottomView { get; set; }

		[Outlet]
		Sodexo.iOS.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView HeaderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NextBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NumberLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PhotosView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Action ("OnAddBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddBtn_Pressed (UIButton sender);

		[Action ("OnAddPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddPhotoBtn_Pressed (UIButton sender);

		[Action ("OnCellAnnualSalesInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAnnualSalesInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellConsumersInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellConsumersInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellDropBtn_Pressed (UIButton sender);

		[Action ("OnDeleteOutletBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDeleteOutletBtn_Pressed (SDXButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
