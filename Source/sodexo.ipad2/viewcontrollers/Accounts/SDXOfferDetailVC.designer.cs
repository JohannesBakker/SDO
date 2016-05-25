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
	[Register ("SDXOfferDetailVC")]
	partial class SDXOfferDetailVC
	{
		[Outlet]
		MonoTouch.UIKit.UIView ActionsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AddPhotoBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AddPhotoLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ContentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OfferNameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OfferPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OfferStatusLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PhotosView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlanogramDetailsLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlanogramPictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlanogramStatusIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlanogramStatusLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView StatusView { get; set; }

		[Action ("OnAddPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddPhotoBtn_Pressed (UIButton sender);

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
