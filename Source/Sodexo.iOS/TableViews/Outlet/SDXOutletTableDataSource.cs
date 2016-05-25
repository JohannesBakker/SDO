using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXOutletTableDataSource : UITableViewDataSource
	{
		public IEnumerable <OutletModel> Outlets;
		public List<bool> IsAddDetailAppearAry;
		public EventHandler<OfferModel> OfferRowSelectedEvent;

		public SDXOutletTableDataSource () : base()
		{

		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Outlets != null ? Outlets.Count() : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string CellIdentifier = "SDXOutletCell";

			var cell = tableView.DequeueReusableCell (CellIdentifier) as SDXOutletCell;
			if (cell == null)
				cell = new SDXOutletCell ();

			cell.OfferRowSelectedEvent = OfferRowSelectedEvent;
			cell.Update (indexPath.Row, Outlets.ElementAt (indexPath.Row), IsAddDetailAppearAry[indexPath.Row]);

			return cell;
		}
	}
}

