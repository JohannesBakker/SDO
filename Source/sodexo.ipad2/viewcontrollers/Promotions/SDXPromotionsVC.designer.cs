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
	[Register ("SDXPromotionsVC")]
	partial class SDXPromotionsVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView FilterSubView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView FilterView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField KeywordTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView TableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel AcceptLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton AcceptNoBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView AcceptView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton AcceptYesBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ActionsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AddPhotoBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel AddPhotoLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CategoryIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ContentView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel DateLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel DescriptionLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton FilterBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel MessageLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView MessageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView PhotosView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView PromotionView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView StatusIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel StatusLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView StatusView { get; set; }

		[Action ("FilterMenuBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void FilterMenuBtn_Pressed (UIButton sender);

		[Action ("OnAcceptBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAcceptBtn_Pressed (SDXButton sender);

		[Action ("OnAddPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddPhotoBtn_Pressed (UIButton sender);

		[Action ("OnAddToCalendarBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddToCalendarBtn_Pressed (SDXButton sender);

		[Action ("OnApplyBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnApplyBtn_Pressed (SDXButton sender);

		[Action ("OnCancelBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCancelBtn_Pressed (SDXButton sender);

		[Action ("OnDeclineBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDeclineBtn_Pressed (SDXButton sender);

		[Action ("OnReadDocBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReadDocBtn_Pressed (SDXButton sender);

		[Action ("OnReturnKey_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReturnKey_Pressed (UITextField sender);

		[Action ("OnSendFeedbackBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSendFeedbackBtn_Pressed (SDXButton sender);

		[Action ("OnSendToMyMailBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSendToMyMailBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AcceptLB != null) {
				AcceptLB.Dispose ();
				AcceptLB = null;
			}
			if (AcceptNoBtn != null) {
				AcceptNoBtn.Dispose ();
				AcceptNoBtn = null;
			}
			if (AcceptView != null) {
				AcceptView.Dispose ();
				AcceptView = null;
			}
			if (AcceptYesBtn != null) {
				AcceptYesBtn.Dispose ();
				AcceptYesBtn = null;
			}
			if (ActionsView != null) {
				ActionsView.Dispose ();
				ActionsView = null;
			}
			if (AddPhotoBtn != null) {
				AddPhotoBtn.Dispose ();
				AddPhotoBtn = null;
			}
			if (AddPhotoLB != null) {
				AddPhotoLB.Dispose ();
				AddPhotoLB = null;
			}
			if (CategoryIV != null) {
				CategoryIV.Dispose ();
				CategoryIV = null;
			}
			if (ContentView != null) {
				ContentView.Dispose ();
				ContentView = null;
			}
			if (DateLB != null) {
				DateLB.Dispose ();
				DateLB = null;
			}
			if (DescriptionLB != null) {
				DescriptionLB.Dispose ();
				DescriptionLB = null;
			}
			if (FilterBtn != null) {
				FilterBtn.Dispose ();
				FilterBtn = null;
			}
			if (MessageLB != null) {
				MessageLB.Dispose ();
				MessageLB = null;
			}
			if (MessageView != null) {
				MessageView.Dispose ();
				MessageView = null;
			}
			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}
			if (PhotosView != null) {
				PhotosView.Dispose ();
				PhotosView = null;
			}
			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}
			if (PromotionView != null) {
				PromotionView.Dispose ();
				PromotionView = null;
			}
			if (StatusIV != null) {
				StatusIV.Dispose ();
				StatusIV = null;
			}
			if (StatusLB != null) {
				StatusLB.Dispose ();
				StatusLB = null;
			}
			if (StatusView != null) {
				StatusView.Dispose ();
				StatusView = null;
			}
		}
	}
}
