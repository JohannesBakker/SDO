using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPriceOutletTableDataSource : UITableViewDataSource
	{
		public SDXPriceOutletTableDataSource ()
		{
		}
		public List<OutletModel> Outlets;


		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Outlets != null ? Outlets.Count : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("SDXpriceOutletCell");

			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, "SDXpriceOutletCell");
				cell.TextLabel.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 14);
			}

			cell.TextLabel.Text = Outlets.ElementAt (indexPath.Row).Name;

			return cell;
		}
	}
}

