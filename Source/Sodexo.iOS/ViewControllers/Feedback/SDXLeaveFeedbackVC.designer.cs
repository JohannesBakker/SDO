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
	[Register ("SDXLeaveFeedbackVC")]
	partial class SDXLeaveFeedbackVC
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView AttachedPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AttachPhotoBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CommentLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView CommentTV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ContentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OfferNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OfferPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OfferStatusLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OfferView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PromotionCategoryIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PromotionContentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PromotionDateLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PromotionDescLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PromotionNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PromotionPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PromotionStatusIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PromotionView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel QuestionLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TitleView { get; set; }

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
		}
	}
}
