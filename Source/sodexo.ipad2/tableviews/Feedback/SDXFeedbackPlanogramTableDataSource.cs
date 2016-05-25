using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXFeedbackPlanogramTableDataSource : UITableViewDataSource
	{
		public List <OfferModel> Offers;

		public SDXFeedbackPlanogramTableDataSource () : base ()
		{

		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Offers == null ? 0 : Offers.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SDXFeedbackPlanogramCell") as SDXFeedbackPlanogramCell;
			if (cell == null)
				cell = new SDXFeedbackPlanogramCell ();

			cell.Update (Offers [indexPath.Row]);

			return cell;
		}
	}
}

