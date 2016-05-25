using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXAccountTableDataSource : UITableViewDataSource
	{
		public IList <AccountModel> Accounts { get; set; }

		public SDXAccountTableDataSource (SDXAccountsVC accountsVC)
		{

		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Accounts != null ? Accounts.Count + 1 : 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NSString AccountCellIdentifier = (NSString)"SDXAccountCell";
			SDXAccountCell cell = tableView.DequeueReusableCell (AccountCellIdentifier) as SDXAccountCell;

			if (cell == null) {
				cell = new SDXAccountCell (AccountCellIdentifier);
			}

			AccountModel account = null;
			if (Accounts != null && indexPath.Row < Accounts.Count)
				account = Accounts.ElementAt (indexPath.Row);

			cell.UpdateCell (indexPath.Row, account);

			return cell;
		}
	}
}

