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
	[Register ("SDXPricesResultVC")]
	partial class SDXPricesResultVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView FilterSearchView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView FilterView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NoResultLb { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField SearchTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Action ("OnApplyBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnApplyBtn_Pressed (SDXButton sender);

		[Action ("OnCancelBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCancelBtn_Pressed (SDXButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (UITextField sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}