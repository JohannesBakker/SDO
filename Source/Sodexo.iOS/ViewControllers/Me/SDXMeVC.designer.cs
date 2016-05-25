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
	[Register ("SDXMeVC")]
	partial class SDXMeVC
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView AvatarIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLB { get; set; }

		[Action ("OnLogoutBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnLogoutBtn_Pressed (SDXButton sender);

		[Action ("OnUploadBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnUploadBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
