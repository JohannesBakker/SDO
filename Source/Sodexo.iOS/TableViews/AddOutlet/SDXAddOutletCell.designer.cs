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
	[Register ("SDXAddOutletCell")]
	partial class SDXAddOutletCell
	{
		[Outlet]
		MonoTouch.UIKit.UIButton AnnualSalesBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField AnnualSalesTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ConsumerBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField ConsumerTF { get; set; }

		[Outlet]
		Sodexo.iOS.SDXButton DeleteBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton LocalCompetitionBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField LocalCompetitionTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField NameTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NumberLB { get; set; }

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (SDXTextField sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
