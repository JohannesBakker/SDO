using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXDashboardTableDelegate : UITableViewDelegate
	{
		public List <DashboardItemModel> DashboardItems = null;
		public EventHandler <int> RowSelectedEvent;

		public SDXDashboardTableDelegate () : base ()
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = DashboardItems [indexPath.Row];

			SDXDashboardCell cell = tableView.DequeueReusableCell ("SDXDashboardCell") as SDXDashboardCell;

			return cell.GetHeightOfCell (item);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowSelectedEvent (tableView, indexPath.Row);
		}
	}
}

