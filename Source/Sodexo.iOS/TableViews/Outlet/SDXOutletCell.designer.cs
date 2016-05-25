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
	[Register ("SDXOutletCell")]
	partial class SDXOutletCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView AddDetailView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AddNewBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView AddNewView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DetailAddBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DetailCancelBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EditBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Action ("OnOfferCategoryBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnOfferCategoryBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
