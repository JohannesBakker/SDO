﻿
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public partial class SDXAddOutletCell : UITableViewCell
	{
		public int ConsumerID = 1;
		public int AnnualSalesID = 1;
		public int LocalCompetitonID = 1;

		public SDXAddOutletCell (IntPtr handle) : base (handle)
		{

		}

		public SDXAddOutletCell () : base ()
		{

		}

		partial void OnReturnKey_Pressed (SDXTextField sender)
		{
			UITextField tf = sender as UITextField; 
			tf.ResignFirstResponder ();
		}

		public void Update (int index)
		{
			NumberLB.Text = (index + 1).ToString ("00");

			ConsumerBtn.Tag = 100 + index;
			AnnualSalesBtn.Tag = 200 + index;
			LocalCompetitionBtn.Tag = 300 + index;
		}

		public bool IsFilled()
		{
			for (int i = 1; i <= 4; i++) {
				var tf = ContentView.ViewWithTag (i) as UITextField;
				if (tf.Text == "")
					return false;
			}
			return true;
		}

		public void FillContents (OutletModel outlet)
		{
			NameTF.Text = outlet.Name;
			ConsumerTF.Text = outlet.ConsumersOnSiteRange.Description;
			AnnualSalesTF.Text = outlet.AnnualSalesRange.Description;
			LocalCompetitionTF.Text = outlet.LocalCompetitionType.Description;

			DeleteBtn.Hidden = false;

			ConsumerID = outlet.ConsumersOnSiteRangeId;
			AnnualSalesID = outlet.AnnualSalesRangeId;
			LocalCompetitonID = outlet.LocalCompetitionTypeId;
		}

		public OutletModel GetOutlet ()
		{
			OutletModel outlet = new OutletModel ();

			outlet.Name = NameTF.Text;
			outlet.ConsumersOnSiteRangeId = ConsumerID;
			outlet.AnnualSalesRangeId = AnnualSalesID;
			outlet.LocalCompetitionTypeId = LocalCompetitonID;

			return outlet;
		}
	}
}

