using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPlanogramTableDataSource : UITableViewDataSource
	{
		public List<OutletModel> Outlets;
		public EventHandler<int> OfferSelected;

		public SDXPlanogramTableDataSource ()
		{

		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Outlets == null ? 0 : Outlets.Count ();
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SDXPlanogramCell") as SDXPlanogramCell;

			if (cell == null)
				cell = new SDXPlanogramCell ();

			cell.OfferSelected = OfferSelected;
			cell.Update (Outlets [indexPath.Row], indexPath.Row);

			return cell;
		}
	}
}

