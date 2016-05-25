
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXPlanogramsVC : SDXBaseVC
	{
		public FeedbackTypeModel FeedbackType;

		private List<OutletModel> filteredOutlets = new List<OutletModel> ();
		private OfferModel selectedOffer;

		public SDXPlanogramsVC (IntPtr handle) : base (handle)
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

			TableView.Delegate = new SDXPlanogramTableDelegate ();
			TableView.DataSource = new SDXPlanogramTableDataSource ();

			((SDXPlanogramTableDataSource)TableView.DataSource).OfferSelected = OnOfferSelected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (3);

			if (filteredOutlets.Count == 0)
				LoadMe ();
		}
		#endregion

		#region Event Handler
		private void OnOfferSelected (object sender, int row)
		{
			var btn = (UIButton) sender;
			selectedOffer = filteredOutlets [row].Offers [btn.Tag];
			PerformSegue ("SegueToLeaveFeedback", this);
		}
		#endregion

		#region Private Functions
		private async void LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null || manager.Me.Accounts == null) {
				ShowLoading ("Loading...");
				await manager.LoadMe ();
				HideLoading ();

				if (!manager.IsSuccessed) {
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
			}

			var me = manager.Me;

			foreach (var account in me.Accounts) {
				foreach (var outlet in account.Outlets) {
					for (int i = 0; i < outlet.Offers.Count; i++) {
						if (outlet.Offers [i].Responses.Count == 0) {
							outlet.Offers.RemoveAt (i);
							i--;
						}
					}
					if (outlet.Offers.Count > 0)
						filteredOutlets.Add (outlet);
				}
			}

			((SDXPlanogramTableDataSource)TableView.DataSource).Outlets = filteredOutlets;
			((SDXPlanogramTableDelegate)TableView.Delegate).Outlets = filteredOutlets;
			TableView.ReloadData ();

			if (filteredOutlets.Count == 0) {
				var alert = new UIAlertView ("No Planograms", "You did not set up any planograms yet, Click OK to go to planograms section and create one, or Cancel to select different feedback subject", null, "OK", "Cancel");
				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					if(e.ButtonIndex==0){
						var vc = Storyboard.InstantiateViewController ("SDXAccountsVC") as SDXAccountsVC;
						NavigationController.PushViewController (vc, true);
					} else {
						NavigationController.PopViewControllerAnimated(true);
					}
				};

				alert.Show ();
			}

		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToLeaveFeedback") {
				var vc = segue.DestinationViewController as SDXLeaveFeedbackVC;
				vc.FeedbackType = FeedbackType;
				vc.Offer = selectedOffer;
			}
		}
		#endregion
	}
}

