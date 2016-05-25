using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	partial class SDXPricesCell : UITableViewCell
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
			NameLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 20);
			NameLB.TextColor = UIColor.FromRGB(80, 80, 79);

			FamilyLB.Text = price.UnitOfMeasure + " " + price.ProductFamily;
			FamilyLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 13);
			FamilyLB.TextColor = UIColor.FromRGB (80, 80, 79);

			OptimumPriceLB.Text = "$ " + price.OptimumPrice.ToString ();
			OptimumPriceLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 24);
			OptimumPriceLB.TextColor = UIColor.FromRGB (138, 138, 138);
		}
	}
}
