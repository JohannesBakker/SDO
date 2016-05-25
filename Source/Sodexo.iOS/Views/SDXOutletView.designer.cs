// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sodexo.iOS
{
	[Register ("SDXOutletView")]
	partial class SDXOutletView
	{
		[Outlet]
		Sodexo.iOS.SDXLabel NameLB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PictureIV { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PictureIV != null) {
				PictureIV.Dispose ();
				PictureIV = null;
			}

			if (NameLB != null) {
				NameLB.Dispose ();
				NameLB = null;
			}
		}
	}
}
