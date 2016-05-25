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
	[Register ("SDXMeVC")]
	partial class SDXMeVC
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AvatarIV { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NameLB { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView SideView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel TitleLB { get; set; }

		[Action ("OnLogoutBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnLogoutBtn_Pressed (SDXButton sender);

		[Action ("OnUploadBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnUploadBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AvatarIV != null) {
				AvatarIV.Dispose ();
				AvatarIV = null;
			}
			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}
			if (SideView != null) {
				SideView.Dispose ();
				SideView = null;
			}
			if (TitleLB != null) {
				TitleLB.Dispose ();
				TitleLB = null;
			}
		}
	}
}
