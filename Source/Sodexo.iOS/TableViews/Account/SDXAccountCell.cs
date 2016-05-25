
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public partial class SDXAccountCell : UITableViewCell
	{
		private const int OutletViewTagBase = 20;

		public SDXAccountCell (IntPtr handle) : base (handle)
		{
			
		}

		public SDXAccountCell (NSString cellId) : base (UITableViewCellStyle.Default, cellId)
		{

		}
	 	
		public void UpdateCell (int num, AccountModel account)
		{
			for (int i = OutletViewTagBase;; i++) {
				UIView view = ContentView.ViewWithTag (i);
				if (view != null)
					view.RemoveFromSuperview ();
				else
					break;
			}

			NumberLB.Text = (num + 1).ToString ("00");
			NavigateBtn.Tag = Constants.AccountCellNavigateBtnTagBase + num;

			if (account == null) {
				SeparatorIV.Hidden = true;

				Util.MoveViewToY (AccountInfoView, 10);
				NumberLB.Enabled = false;
				AddressLB.Hidden = true;
				LocationIDLB.Hidden = true;
				PlusIV.Hidden = false;
				ArrowIV.Hidden = true;

				NameLB.Text = "ADD NEW";

				return;
			}

			SeparatorIV.Hidden = false;

			Util.MoveViewToY (AccountInfoView, SeparatorIV.Frame.Y + SeparatorIV.Frame.Height);
			NumberLB.Enabled = true;
			AddressLB.Hidden = false;
			LocationIDLB.Hidden = false;
			PlusIV.Hidden = true;
			ArrowIV.Hidden = false;

			NameLB.Text = account.Location.LocationName;
			AddressLB.Text = account.Location.LocationCity + " " + account.Location.LocationAddress1;
			LocationIDLB.Text = account.LocationId.ToString ();

			int yPos = 92 + 1;

			IEnumerable<OutletModel> outlets = account.Outlets;
			for (int i = 0; i < outlets.Count(); i++) 
			{
				OutletModel outlet = outlets.ElementAt (i);

				var view = SDXOutletView.Create ();
				view.Frame = new RectangleF (10, yPos, view.Frame.Width, view.Frame.Height);
				view.Update (outlet);

				view.Tag = i + OutletViewTagBase;
				ContentView.AddSubview (view);

				yPos += (int)view.Frame.Height + 1;
			}
		}
	}
}

