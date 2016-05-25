using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPricesTableDelegate : UITableViewDelegate
	{
		public SDXPricesTableDelegate () : base ()
		{
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 70.0f;
		}
	}
}

