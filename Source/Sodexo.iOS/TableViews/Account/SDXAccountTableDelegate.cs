using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXAccountTableDelegate : UITableViewDelegate
	{
		public IList <AccountModel> Accounts { get; set; }

		public SDXAccountTableDelegate (SDXAccountsVC accountsVC)
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (Accounts == null || indexPath.Row == Accounts.Count)
				return 100.0f;
			return 92.0f + (65 + 1) * Accounts.ElementAt(indexPath.Row).Outlets.Count ();
		}
	}
}

