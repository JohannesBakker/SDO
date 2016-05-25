using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPromotionTableDataSource : UITableViewDataSource
	{
		public List <PromotionModel> Promotions;

		public SDXPromotionTableDataSource () : base ()
		{

		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return Promotions == null ? 0 : Promotions.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("SDXPromotionCell") as SDXPromotionCell;
			if (cell == null)
				cell = new SDXPromotionCell ();

			cell.Update (Promotions [indexPath.Row]);

			return cell;
		}
	}
}

