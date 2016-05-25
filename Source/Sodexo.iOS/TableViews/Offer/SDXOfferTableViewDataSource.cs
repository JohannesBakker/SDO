using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXOfferTableViewDataSource : UITableViewDataSource
	{
		private List <OfferModel> withPlanograms, noPlanograms;

		public SDXOfferTableViewDataSource () : base ()
		{
			withPlanograms = new List<OfferModel> ();
			noPlanograms = new List<OfferModel> ();
		}

		public void SetOffers (List <OfferModel> offersWith, List <OfferModel> offersNo)
		{
			withPlanograms = offersWith;
			noPlanograms = offersNo;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			if (withPlanograms.Count == 0 && noPlanograms.Count == 0) {
				return 0;
			} else if (withPlanograms.Count == 0 || noPlanograms.Count == 0) {
				return 1;
			}
			return 2;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			if (section == 0 && withPlanograms.Count != 0)
				return withPlanograms.Count;
			else
				return noPlanograms.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			SDXOfferCell cell = tableView.DequeueReusableCell ("SDXOfferCell") as SDXOfferCell;

			if (indexPath.Section == 0 && withPlanograms.Count != 0)
				cell.Update (withPlanograms.ElementAt (indexPath.Row), false);
			else
				cell.Update (noPlanograms.ElementAt (indexPath.Row), false);

			return cell;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle != UITableViewCellEditingStyle.Delete) {
				return;
			}

			OfferModel offer;
			if (indexPath.Section == 0 && withPlanograms.Count != 0)
				offer = withPlanograms.ElementAt (indexPath.Row);
			else
				offer = noPlanograms.ElementAt (indexPath.Row);

			NSMutableDictionary dict = new NSMutableDictionary ();
			dict.SetValueForKey ((NSString)(offer.OutletId.ToString ()), (NSString)"outlet_id");
			dict.SetValueForKey ((NSString)(offer.OfferId.ToString ()), (NSString)"offer_id");
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationDeleteOffer, null, dict);
		}
	}
}

