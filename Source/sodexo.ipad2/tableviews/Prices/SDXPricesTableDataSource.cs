using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPricesTableDataSource : UITableViewDataSource
	{
		public List<ProductPriceModel> Prices;

		public SDXPricesTableDataSource () : base ()
		{
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Prices != null ? Prices.Count : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SDXPricesCell") as SDXPricesCell;
			if (cell == null)
				cell = new SDXPricesCell ();

			cell.Update (Prices [indexPath.Row]);

			return cell;
		}
	}
}

