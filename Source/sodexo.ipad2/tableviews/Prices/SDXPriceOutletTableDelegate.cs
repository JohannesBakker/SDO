using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public class SDXPriceOutletTableDelegate : UITableViewDelegate
	{
		public EventHandler <int> RowSelectedEvent;

		public SDXPriceOutletTableDelegate ()
		{
		}
	}
}

