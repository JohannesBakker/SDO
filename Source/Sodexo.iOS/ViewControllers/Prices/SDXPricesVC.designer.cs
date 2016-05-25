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
	[Register ("SDXPricesVC")]
	partial class SDXPricesVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView AdvancedView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView BasicView { get; set; }

		[Outlet]
		Sodexo.iOS.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField KeywordTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SearchBtn { get; set; }

		[Action ("OnDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDropBtn_Pressed (UIButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (SDXTextField sender);

		[Action ("OnSearchBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSearchBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
