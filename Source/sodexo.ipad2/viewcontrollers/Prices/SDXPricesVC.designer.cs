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
	[Register ("SDXPricesVC")]
	partial class SDXPricesVC
	{
		[Outlet]
		SDXDropTableView DropTableView { get; set; }

		[Outlet]
		UIView AdvancedView { get; set; }

		[Outlet]
		UIView BasicView { get; set; }

		[Outlet]
		UIButton competitionButton { get; set; }

		[Outlet]
		UITextField competitionSelection { get; set; }

		[Outlet]
		UIButton consumerButton { get; set; }

		[Outlet]
		UITextField consumerSelection { get; set; }

		[Outlet]
		UIView DropView { get; set; }

		[Outlet]
		UIButton pricingButton { get; set; }

		[Outlet]
		UITextField pricingSelection { get; set; }

		[Outlet]
		UIButton segmentButton { get; set; }

		[Outlet]
		UITextField segmentSelection { get; set; }

		[Outlet]
		UIButton stateButton { get; set; }

		[Outlet]
		UITextField stateSelection { get; set; }

		[Outlet]
		UITableView PriceTableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton advancedButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AdvancedSearchBT { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton basicButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView BasicScrollView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField BasicSearchField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView BasicViewContainer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton FilterBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView FilterView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel messageLb { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView messageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView PriceSelectionView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView pricesView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField SearchTF { get; set; }

		[Action ("FilterMenuBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void FilterMenuBtn_Pressed (UIButton sender);

		[Action ("OnAdvancedBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAdvancedBtn_Pressed (UIButton sender);

		[Action ("OnApplyBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnApplyBtn_Pressed (SDXButton sender);

		[Action ("OnBasicBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnBasicBtn_Pressed (UIButton sender);

		[Action ("OnCancelBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCancelBtn_Pressed (SDXButton sender);

		[Action ("OnDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDropBtn_Pressed (UIButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (UITextField sender);

		[Action ("OnSearchBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSearchBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (advancedButton != null) {
				advancedButton.Dispose ();
				advancedButton = null;
			}
			if (AdvancedSearchBT != null) {
				AdvancedSearchBT.Dispose ();
				AdvancedSearchBT = null;
			}
			if (basicButton != null) {
				basicButton.Dispose ();
				basicButton = null;
			}
			if (BasicScrollView != null) {
				BasicScrollView.Dispose ();
				BasicScrollView = null;
			}
			if (BasicSearchField != null) {
				BasicSearchField.Dispose ();
				BasicSearchField = null;
			}
			if (BasicViewContainer != null) {
				BasicViewContainer.Dispose ();
				BasicViewContainer = null;
			}
			if (FilterBtn != null) {
				FilterBtn.Dispose ();
				FilterBtn = null;
			}
			if (FilterView != null) {
				FilterView.Dispose ();
				FilterView = null;
			}
			if (messageLb != null) {
				messageLb.Dispose ();
				messageLb = null;
			}
			if (messageView != null) {
				messageView.Dispose ();
				messageView = null;
			}
			if (PriceSelectionView != null) {
				PriceSelectionView.Dispose ();
				PriceSelectionView = null;
			}
			if (pricesView != null) {
				pricesView.Dispose ();
				pricesView = null;
			}
			if (SearchTF != null) {
				SearchTF.Dispose ();
				SearchTF = null;
			}
		}
	}
}
