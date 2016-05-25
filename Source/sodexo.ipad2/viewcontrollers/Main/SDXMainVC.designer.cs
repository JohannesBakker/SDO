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
	[Register ("SDXMainVC")]
	partial class SDXMainVC
	{
		[Outlet]
		UIView MenuView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ContainerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel HeaderJobLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel HeaderNameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView HeaderUserBorder { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView HeaderUserImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView HeaderView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView MainHeadedBackground { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SectionHeader { get; set; }

		[Action ("OnMenuItem_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnMenuItem_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}
			if (HeaderJobLB != null) {
				HeaderJobLB.Dispose ();
				HeaderJobLB = null;
			}
			if (HeaderNameLB != null) {
				HeaderNameLB.Dispose ();
				HeaderNameLB = null;
			}
			if (HeaderUserBorder != null) {
				HeaderUserBorder.Dispose ();
				HeaderUserBorder = null;
			}
			if (HeaderUserImage != null) {
				HeaderUserImage.Dispose ();
				HeaderUserImage = null;
			}
			if (HeaderView != null) {
				HeaderView.Dispose ();
				HeaderView = null;
			}
			if (MainHeadedBackground != null) {
				MainHeadedBackground.Dispose ();
				MainHeadedBackground = null;
			}
			if (SectionHeader != null) {
				SectionHeader.Dispose ();
				SectionHeader = null;
			}
		}
	}
}
