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
	[Register ("SDXPromotionDetailVC")]
	partial class SDXPromotionDetailVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView AcceptView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ActionsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AddPhotoBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AddPhotoLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView CategoryIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ContentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DateLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PhotosView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView StatusIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StatusLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView StatusView { get; set; }

		[Action ("OnAcceptBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAcceptBtn_Pressed (SDXButton sender);

		[Action ("OnAddPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddPhotoBtn_Pressed (UIButton sender);

		[Action ("OnAddToCalendarBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddToCalendarBtn_Pressed (SDXButton sender);

		[Action ("OnDeclineBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDeclineBtn_Pressed (SDXButton sender);

		[Action ("OnReadDocBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnReadDocBtn_Pressed (SDXButton sender);

		[Action ("OnSendFeedbackBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSendFeedbackBtn_Pressed (SDXButton sender);

		[Action ("OnSendToMyMailBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnSendToMyMailBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
