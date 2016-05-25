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
	[Register ("SDXAddAccountVC")]
	partial class SDXAddAccountVC
	{
		[Outlet]
		MonoTouch.UIKit.UITextField AccountNameTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField AccountNumberTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField Address1TF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField Address2TF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField BusinessSegmentTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField CityTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField ConsumerTypeTF { get; set; }

		[Outlet]
		Sodexo.iOS.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NextBtn { get; set; }

		[Outlet]
		Sodexo.iOS.SDXScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField StateTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField ZipTF { get; set; }

		[Action ("OnDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDropBtn_Pressed (UIButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (SDXTextField sender);

		[Action ("OnSearchBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSearchBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
