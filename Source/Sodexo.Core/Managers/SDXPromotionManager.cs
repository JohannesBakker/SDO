using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXPromotionManager : SDXPromotionService
	{
		public SDXPromotionManager () : base ()
		{

		}

		public async Task<List<PromotionModel>> LoadPromotions (bool isIncludeLocalActivations = true, bool isIncludePromotionCategories = true)
		{
			return await DoLoadPromotions (isIncludeLocalActivations, isIncludePromotionCategories);
		}

		public async Task<PromotionModel> LoadPromotion (int promotionId, bool isIncludeLocalActivations = true, bool isIncludePromotionCategories = true)
		{
			return await DoLoadPromotion (promotionId, isIncludeLocalActivations, isIncludePromotionCategories);
		}

		public async Task <PromotionResponseModel> AcceptPromotion (int promotionId, string domainUniqueIdentifier = "me")
		{
			return await DoAcceptPromotion (promotionId, domainUniqueIdentifier);
		}

		public async Task <PromotionResponseModel> DeclinePromotion (int promotionId, string domainUniqueIdentifier = "me")
		{
			return await DoDeclinePromotion (promotionId, domainUniqueIdentifier);
		}

		public async Task <string> SendPromotionViaEmail (int promotionId, string email)
		{
			return await DoSendPromotionViaEmail (promotionId, email);
		}
	}
}

