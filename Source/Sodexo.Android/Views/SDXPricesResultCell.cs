using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Views.Animations;
using Android.Graphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Android
{
	public class SDXPricesResultCell : Java.Lang.Object
	{
		View view;

		TextView nameTv, familyTv, priceTv;

		public SDXPricesResultCell (Activity context, View view) : base ()
		{
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			nameTv = view.FindViewById (Resource.Id.pricesresult_name_tv) as TextView;
			familyTv = view.FindViewById (Resource.Id.pricesresult_family_tv) as TextView;
			priceTv = view.FindViewById (Resource.Id.pricesresult_optimum_price_tv) as TextView;
		}

		public void Update (ProductPriceModel price)
		{
			nameTv.Text = price.ProductName;
			familyTv.Text = price.UnitOfMeasure + price.ProductFamily;
			priceTv.Text = "$ " + price.OptimumPrice.ToString ();
		}
	}
}

