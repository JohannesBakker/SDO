using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;
using System.Drawing;

namespace Sodexo.iOS
{
	public class SDXOfferTableViewDelegate : UITableViewDelegate
	{
		UIView headerViewForWith, headerViewForNo;
		private List <OfferModel> withPlanograms;
		private List <OfferModel> noPlanograms;

		public EventHandler<OfferModel> RowSelectedEvent;

		public SDXOfferTableViewDelegate () : base ()
		{
			Console.WriteLine ("SDXOfferTableViewDelegate");
			headerViewForWith = new UIView (new RectangleF (0, 0, 300, 30));
			headerViewForWith.BackgroundColor = UIColor.White;
			UIImageView iv = new UIImageView (new RectangleF(10, 7, 280, 16));
			iv.ContentMode = UIViewContentMode.ScaleAspectFit;
			iv.Image = UIImage.FromBundle ("text_with_planogram.png");
			headerViewForWith.Add (iv);

			headerViewForNo = new UIView (new RectangleF (0, 0, 300, 30));
			headerViewForNo.BackgroundColor = UIColor.White;
			UIImageView ivNo = new UIImageView (new RectangleF(10, 7, 280, 16));
			ivNo.ContentMode = UIViewContentMode.ScaleAspectFit;
			ivNo.Image = UIImage.FromBundle ("text_no_planogram.png");
			headerViewForNo.Add (ivNo);
		}

		public void SetOffers (List <OfferModel> offersWith, List <OfferModel> offersNo)
		{
			withPlanograms = offersWith;
			noPlanograms = offersNo;
		}

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			return 30.0f;
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 50.0f;
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			if (section == 0 && withPlanograms.Count != 0) {
				return headerViewForWith;
			} else
				return headerViewForNo;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);

			OfferModel offer;
			if (indexPath.Section == 0 && withPlanograms.Count != 0) {
				offer = withPlanograms [indexPath.Row];
			} else {
				offer = noPlanograms [indexPath.Row];
			}

			if (RowSelectedEvent != null)
				RowSelectedEvent (tableView, offer);
		}
	}
}

