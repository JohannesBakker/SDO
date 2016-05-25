using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
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

			var h = Util.DashboardCellHeight (item);
			h += 10;
			//Console.WriteLine ("---------------  cell height : "+h.ToString ()+" "+item.Title);
			return h;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowSelectedEvent (tableView, indexPath.Row);
		}
	}
}

