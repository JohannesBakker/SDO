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
	[Register ("SDXFullImageVC")]
	partial class SDXFullImageVC
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Action ("OnImageView_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnImageView_Pressed (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
