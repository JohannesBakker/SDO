using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	partial class SDXFeedbackVC : SDXBaseVC
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
			if (dashboardItem != null ) {
				pushToFeedback();
			}

			foreach (UIView sub in View) {
				if (sub is UIButton) {
					((UIButton)sub).Font = UIFont.FromName (Constants.OSWALD_LIGHT, 30);
				}
			}
		}

		async public void pushToFeedback()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				await LoadReferenceData ();
			}

			if (manager.DataModel.FeedbackTypes.Count > 0) {
				selectedFeedbackType = manager.DataModel.FeedbackTypes [(int)dashboardItem.FeedbackTypeId];
				//DoSegue ((int)dashboardItem.FeedbackTypeId);
				//vc.ModelId = item.ModelId;
			}
		}
		async public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (3);
			SetSectionName ("Feedback");
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

			DoSegue(btn.Tag);
		}

		private void DoSegue(int num)
		{
			if (num == 1)
				PerformSegue ("SegueToPlanogramsFeedback", this);
			else if (num == 2)
				PerformSegue ("SegueToPromotionsFeedback", this);
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
			} else if (segue.Identifier == "SegueToPromotionsFeedback") {
				var vc = segue.DestinationViewController as SDXPromotionsFeedbackVC;
				vc.FeedbackType = selectedFeedbackType;
			} else if (segue.Identifier == "SegueToPlanogramsFeedback") {
				var vc = segue.DestinationViewController as SDXPlanogramsFeedbackVC;
				vc.FeedbackType = selectedFeedbackType;
			}
		}
		#endregion
	}
}
