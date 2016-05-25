
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public partial class SDXPricesCell : UITableViewCell
	{

		public SDXPricesCell (IntPtr handle) : base (handle)
		{
		}

		public SDXPricesCell () : base ()
		{
			
		}

		public void Update (ProductPriceModel price)
		{
			NameLB.Text = price.ProductName;
			FamilyLB.Text = price.UnitOfMeasure + " " + price.ProductFamily;
			OptimumPriceLB.Text = "$ " + price.OptimumPrice.ToString ();
		}
	}
}

