using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
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

			float h = 0;
			if (item.Photo != null) {
				h += item.PhotoId != null ? (float)(item.PhotoId * 300 / 1000.0f) : 0;
			}

			h += 3 + 60 + 5;
			float textH = Util.GetHeightOfString (item.Description, 220.0f, UIFont.FromName (Constants.KARLA_REGULAR, 12)) + 10;
			h += textH + 5;

			return h + 10;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RowSelectedEvent (tableView, indexPath.Row);
		}
	}
}

