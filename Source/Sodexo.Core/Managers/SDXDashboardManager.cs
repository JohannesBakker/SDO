using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXDashboardManager : SDXDashboardService
	{
		public SDXDashboardManager () : base ()
		{
		}

		public async Task<List<DashboardItemModel>> LoadDashboard ()
		{
			return await DoLoadDashboard ();
		}
	}
}

