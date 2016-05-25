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
	[Register ("SDXEditAccountVC")]
	partial class SDXEditAccountVC
	{
		[Outlet]
		Sodexo.Ipad2.SDXLabel Address1LB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel Address2LB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel BusinessSegmentLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel CityLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel ConsumerTypeLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXButton DeleteAccountBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXButton LookupBtn { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXButton NextBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel StateLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel TitleLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView UnitContentView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel UnitNameLB { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField UnitNumberTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView UnitNumberView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXLabel ZipLB { get; set; }

		[Action ("OnConsumerTypeBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnConsumerTypeBtn_Pressed (UIButton sender);

		[Action ("OnDeleteAccountBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDeleteAccountBtn_Pressed (SDXButton sender);

		[Action ("OnHideDropView:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropView (UIButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (SDXTextField sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
