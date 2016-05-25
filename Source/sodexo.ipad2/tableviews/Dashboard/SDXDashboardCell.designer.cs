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
	[Register ("SDXDashboardCell")]
	partial class SDXDashboardCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel DateLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView EmptyProgressIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView FullProgressView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView HeaderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView IndicatorIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView MainView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView MainContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel ProgressValueLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ProgressValueView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ProgressView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView SymbolIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView FullProgressIV { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (FullProgressIV != null) {
				FullProgressIV.Dispose ();
				FullProgressIV = null;
			}
		}
	}
}
