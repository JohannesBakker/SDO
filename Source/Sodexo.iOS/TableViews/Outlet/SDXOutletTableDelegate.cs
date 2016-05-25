using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public class SDXOutletTableDelegate : UITableViewDelegate
	{
		public IEnumerable <OutletModel> Outlets;
		public List<bool> IsAddDetailAppearAry;

		public SDXOutletTableDelegate () : base()
		{

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			OutletModel outlet = Outlets.ElementAt (indexPath.Row);
			IEnumerable <OfferModel> offers = outlet.Offers;
			bool isWithPlanogramExist = false, isNoPlanogramExist = false;
			foreach (OfferModel offer in offers) {
				if (offer.OfferCategory.RequiresPlanogram)
					isWithPlanogramExist = true;
				else
					isNoPlanogramExist = true;
				if (isWithPlanogramExist && isNoPlanogramExist)
					break;
			}

			float fHeightAdditional = 0;
			if (isWithPlanogramExist)
				fHeightAdditional += 30;
			if (isNoPlanogramExist)
				fHeightAdditional += 30;

			if (!IsAddDetailAppearAry[indexPath.Row])
				return 70.0f + fHeightAdditional + Outlets.ElementAt (indexPath.Row).Offers.Count() * 50 + 50;
			else
				return 70.0f + fHeightAdditional + Outlets.ElementAt (indexPath.Row).Offers.Count() * 50 + 162;
		}
	}
}

