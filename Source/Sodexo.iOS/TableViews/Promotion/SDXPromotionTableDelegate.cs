using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXPromotionTableDelegate : UITableViewDelegate
	{
		public List <PromotionModel> Promotions;

		public EventHandler<int> RowSelectedEvent;

		public SDXPromotionTableDelegate () : base ()
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = Promotions [indexPath.Row];

			var cell = tableView.DequeueReusableCell ("SDXPromotionCell") as SDXPromotionCell;

			return cell.GetHeightOfCell (item);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowSelectedEvent (tableView, indexPath.Row);
		}
	}
}

