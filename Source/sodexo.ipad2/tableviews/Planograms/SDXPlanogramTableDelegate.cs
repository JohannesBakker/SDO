using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPlanogramTableDelegate : UITableViewDelegate
	{
		public List<OutletModel> Outlets;
		public EventHandler<int> RowSelectedEvent;

		public SDXPlanogramTableDelegate () : base ()
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 65 + Outlets [indexPath.Row].Offers.Count * 50 + 10;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowSelectedEvent (tableView, indexPath.Row);
		}
	}
}

