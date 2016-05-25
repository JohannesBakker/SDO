using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXAddOutletTableDataSource : UITableViewDataSource
	{
		List<OutletModel> outlets;
		String outletLabel;

		public SDXAddOutletTableDataSource (List<OutletModel> outlets, String outletLabel)
		{
			this.outlets = outlets;
			this.outletLabel = outletLabel;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return outlets.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell viewcell = tableView.DequeueReusableCell ("SDXAddOutletCell");
			SDXAddOutletCell cell = tableView.DequeueReusableCell ("SDXAddOutletCell") as SDXAddOutletCell;
			if (cell == null)
				cell = new SDXAddOutletCell ();

			cell.Update (indexPath.Row, outletLabel);
			//if (outlets != null && outlets.Count > 0 && indexPath.Row < outlets.Count)
			//	cell.FillContents(outlets.ElementAt(indexPath.Row));

			return cell;
		}
	}
}

