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
	[Register ("SDXEditOutletVC")]
	partial class SDXEditOutletVC
	{
		[Outlet]
		Sodexo.Ipad2.SDXTextField AnnualTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton CancelBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ChangePhotoBtn { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField ConsumerTF { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXDropTableView DropTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView DropView { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField LocalCompetitionTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OutletImage { get; set; }

		[Outlet]
		Sodexo.Ipad2.SDXTextField OutletNameTF { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PhotosView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SaveBtn { get; set; }

		[Action ("OnAddPhotoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAddPhotoBtn_Pressed (UIButton sender);

		[Action ("OnCancelBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCancelBtn_Pressed (SDXButton sender);

		[Action ("OnCellAnnualSalesInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellAnnualSalesInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellConsumersInfoBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellConsumersInfoBtn_Pressed (UIButton sender);

		[Action ("OnCellDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnCellDropBtn_Pressed (UIButton sender);

		[Action ("OnDeleteOutletBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnDeleteOutletBtn_Pressed (SDXButton sender);

		[Action ("OnHideDropBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnHideDropBtn_Pressed (UIButton sender);

		[Action ("OnNextBtn_Pressed:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnNextBtn_Pressed (SDXButton sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
