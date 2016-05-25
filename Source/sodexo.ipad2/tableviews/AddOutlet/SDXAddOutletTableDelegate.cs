using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXAddOutletTableDelegate : UITableViewDelegate
	{
		public SDXAddOutletTableDelegate (List<OutletModel> outlets)
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 230.0f;
		}
	}
}

