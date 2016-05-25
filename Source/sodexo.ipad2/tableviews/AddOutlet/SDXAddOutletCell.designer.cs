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
	[Register ("SDXAddOutletCell")]
	partial class SDXAddOutletCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AnnualSalesBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXTextField AnnualSalesTF { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ConsumerBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXTextField ConsumerTF { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton DeleteBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton LocalCompetitionBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXTextField LocalCompetitionTF { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXTextField NameTF { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NumberLB { get; set; }

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (SDXTextField sender);

		void ReleaseDesignerOutlets ()
		{
			if (AnnualSalesBtn != null) {
				AnnualSalesBtn.Dispose ();
				AnnualSalesBtn = null;
			}
			if (AnnualSalesTF != null) {
				AnnualSalesTF.Dispose ();
				AnnualSalesTF = null;
			}
			if (ConsumerBtn != null) {
				ConsumerBtn.Dispose ();
				ConsumerBtn = null;
			}
			if (ConsumerTF != null) {
				ConsumerTF.Dispose ();
				ConsumerTF = null;
			}
			if (DeleteBtn != null) {
				DeleteBtn.Dispose ();
				DeleteBtn = null;
			}
			if (LocalCompetitionBtn != null) {
				LocalCompetitionBtn.Dispose ();
				LocalCompetitionBtn = null;
			}
			if (LocalCompetitionTF != null) {
				LocalCompetitionTF.Dispose ();
				LocalCompetitionTF = null;
			}
			if (NameTF != null) {
				NameTF.Dispose ();
				NameTF = null;
			}
			if (NumberLB != null) {
				NumberLB.Dispose ();
				NumberLB = null;
			}
		}
	}
}
