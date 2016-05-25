using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXAddOutletTableDataSource : UITableViewDataSource
	{
		List<OutletModel> outlets;

		public SDXAddOutletTableDataSource (List<OutletModel> outlets)
		{
			this.outlets = outlets;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return outlets.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			SDXAddOutletCell cell = tableView.DequeueReusableCell ("SDXAddOutletCell") as SDXAddOutletCell;
			if (cell == null)
				cell = new SDXAddOutletCell ();

			cell.Update (indexPath.Row);

			return cell;
		}
	}
}

