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
	[Register ("SDXAccountsVC")]
	partial class SDXAccountsVC
	{
		[Outlet]
		Sodexo.Ipad2.SDXLabel AddressOneLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel AddressTwoLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel BusinessSegLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel CityLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel ConsumerTypeLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXButton LookupBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LookupView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXButton NextBtn { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXDropTableView OutletDropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OutletDropView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OutletView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel StateLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel TitleOneLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleTwoLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel UnitNameLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField UnitNumberTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView UnitView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel ZipLB { get; set; }

		[Action ("OnDeleteOutletBtn_Pressed:")]
		partial void OnDeleteOutletBtn_Pressed (Sodexo.Ipad2.SDXButton sender);

		[Action ("AddBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void AddBtn_Pressed (UIButton sender);

		[Action ("OnCellAnnualSalesInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAnnualSalesInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellConsumersInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellConsumersInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellDropBtn_Pressed (UIButton sender);

		[Action ("OnConsumerBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnConsumerBtn_Pressed (UIButton sender);

		[Action ("OnFinishSetupBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnFinishSetupBtn_Pressed (SDXButton sender);

		[Action ("OnHideDropView:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropView (UIButton sender);

		[Action ("OnHideOutletDropTableView:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideOutletDropTableView (UIButton sender);

		[Action ("OnLookupAccountBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnLookupAccountBtn_Pressed (SDXButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
