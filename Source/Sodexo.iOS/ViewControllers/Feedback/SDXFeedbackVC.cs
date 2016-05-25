
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.iOS
{
	public partial class SDXFeedbackVC : SDXBaseVC
	{
		private FeedbackTypeModel selectedFeedbackType;

		public SDXFeedbackVC (IntPtr handle) : base (handle)
		{
		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			if (UIScreen.MainScreen.Bounds.Height <= 480) {
				for (int i = 0; i < 6; i++) {
					var btn = View.ViewWithTag (i + 1) as UIButton;
					RectangleF frame = btn.Frame;
					frame.Y = 60 + (i / 2) * (110 + 1);
					frame.Height = 110;
					btn.Frame = frame;
				}
			}
		}

		async public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (3);

			await LoadReferenceData ();
		}
		#endregion

		#region Button Actions
		async partial void OnSubjectBtn_Pressed (SDXButton sender)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				await LoadReferenceData ();
				return;
			}

			var btn = sender as UIButton;

			if (manager.DataModel.FeedbackTypes.Count < btn.Tag)
				return;

			selectedFeedbackType = manager.DataModel.FeedbackTypes [btn.Tag - 1];

			if (btn.Tag == 1)
				PerformSegue ("SegueToPlanograms", this);
			else if (btn.Tag == 2)
				PerformSegue ("SegueToPromotions", this);
			else {
				PerformSegue ("SegueToLeaveFeedback", this);
			}
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToLeaveFeedback") {
				var vc = segue.DestinationViewController as SDXLeaveFeedbackVC;
				vc.FeedbackType = selectedFeedbackType;
			} else if (segue.Identifier == "SegueToPromotions") {
				var vc = segue.DestinationViewController as SDXPromotionsVC;
				vc.IsSelecting = true;
				vc.FeedbackType = selectedFeedbackType;
			} else if (segue.Identifier == "SegueToPlanograms") {
				var vc = segue.DestinationViewController as SDXPlanogramsVC;
				vc.FeedbackType = selectedFeedbackType;
			}
		}
		#endregion
	}
}

