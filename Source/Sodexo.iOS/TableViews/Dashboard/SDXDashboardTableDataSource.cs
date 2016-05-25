using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXDashboardTableDataSource : UITableViewDataSource
	{
		public List <DashboardItemModel> DashboardItems = null;

		public SDXDashboardTableDataSource () : base ()
		{

		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return DashboardItems == null ? 0 : DashboardItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SDXDashboardCell") as SDXDashboardCell;
			if (cell == null) {
				cell = new SDXDashboardCell ();
			}

			cell.Update (DashboardItems [indexPath.Row]);

			return cell;
		}
	}
}

