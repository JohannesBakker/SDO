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
	[Register ("SDXLookupAccountVC")]
	partial class SDXLookupAccountVC
	{
		[Outlet]
		Sodexo.Ipad2.SDXButton LookupBtn { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField UnitNumberTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ViewMain { get; set; }

		[Action ("OnLookupAccountBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnLookupAccountBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
