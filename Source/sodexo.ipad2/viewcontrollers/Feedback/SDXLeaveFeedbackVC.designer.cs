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
	[Register ("SDXLeaveFeedbackVC")]
	partial class SDXLeaveFeedbackVC
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AttachedPictureIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton AttachPhotoBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton CancelBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel CommentLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView CommentTV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ContentView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel QuestionLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView ScrollView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXButton SendFeedbackBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		SDXLabel TitleLB { get; set; }

		[Action ("CancelButton_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void CancelButton_Pressed (SDXButton sender);

		[Action ("OnAttachPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAttachPhotoBtn_Pressed (SDXButton sender);

		[Action ("OnSendFeedbackBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSendFeedbackBtn_Pressed (SDXButton sender);

		[Action ("OnStarBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnStarBtn_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AttachedPictureIV != null) {
				AttachedPictureIV.Dispose ();
				AttachedPictureIV = null;
			}
			if (AttachPhotoBtn != null) {
				AttachPhotoBtn.Dispose ();
				AttachPhotoBtn = null;
			}
			if (CancelBtn != null) {
				CancelBtn.Dispose ();
				CancelBtn = null;
			}
			if (CommentLB != null) {
				CommentLB.Dispose ();
				CommentLB = null;
			}
			if (CommentTV != null) {
				CommentTV.Dispose ();
				CommentTV = null;
			}
			if (ContentView != null) {
				ContentView.Dispose ();
				ContentView = null;
			}
			if (QuestionLB != null) {
				QuestionLB.Dispose ();
				QuestionLB = null;
			}
			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
			if (SendFeedbackBtn != null) {
				SendFeedbackBtn.Dispose ();
				SendFeedbackBtn = null;
			}
			if (TitleLB != null) {
				TitleLB.Dispose ();
				TitleLB = null;
			}
		}
	}
}
