using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXPriceManager : SDXPriceService
	{
		public SDXPriceManager ()
		{
		}

		public async Task <List<ProductPriceModel>> LoadPricesAdvance (int divisonId, int pricingBandId, int localCompetitionTypeId, int consumerTypeId, int offerCategoryId, string searchText)
		{
			return await DoLoadPricesAdvance (divisonId, pricingBandId, localCompetitionTypeId, consumerTypeId, offerCategoryId, searchText);
		}

		public async Task <List<ProductPriceModel>> LoadPricesBasic (int outletId, string searchText)
		{
			return await DoLoadPricesBasic (outletId, searchText);
		}
	}
}

