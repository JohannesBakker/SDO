using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXPlanogramTableDelegate : UITableViewDelegate
	{
		public List<OutletModel> Outlets;

		public SDXPlanogramTableDelegate () : base ()
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 65 + Outlets [indexPath.Row].Offers.Count * 50 + 10;
		}
	}
}

