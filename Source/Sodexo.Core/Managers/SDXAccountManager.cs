using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXAccountManager : SDXAccountService
	{
		public List<AccountModel> AccountList { get; set;}

		public SDXAccountManager ()
		{

		}

		public async Task<List<AccountModel>> LoadAccountList(int itemsToReturn = 25, bool isIncludeUsers = true, bool isIncludeLocation = true, bool isIncludeOutlets = true)
		{
			AccountList = await GetAccountList (itemsToReturn, isIncludeUsers, isIncludeLocation, isIncludeOutlets);

			return AccountList;
		}

		public async Task<AccountModel> LoadAccount(string locationID, bool isIncludeUsers = true, bool isIncludeLocation = true, bool isIncludeOutlets = true)
		{
			return await GetAccount (locationID, isIncludeUsers, isIncludeLocation, isIncludeOutlets);;
		}

		public async Task<AccountModel> AddNewAccount (AccountModel account)
		{
			AccountModel newAccount = await AddAccount (account);

			return newAccount;
		}

		public async Task <string> DeleteOffer (string locationId, string outletId, string offerId)
		{
			return await DoDeleteOffer (locationId, outletId, offerId);
		}

		public async Task <AccountModel> UpdateAccount (string locationId, string consumerTypeId, string rowVersion)
		{
			return await DoUpdateAccount (locationId, consumerTypeId, rowVersion);
		}

		public async Task <OutletModel> UpdateOutlet (string locationId, OutletModel outlet)
		{
			return await DoUpdateOutlet (locationId, outlet);
		}

		public async Task <OutletModel> AddOutlet (string locationId, OutletModel outlet)
		{
			return await DoAddOutlet (locationId, outlet);
		}

		public async Task <OfferModel> AddOffer (string locationId, string outletId, string offerName, string offerCategoryId)
		{
			return await DoAddOffer (locationId, outletId, offerName, offerCategoryId);
		}

		public async Task <OfferResponseModel> SetPlanogram (string locationId, int outletId, int offerId, int decisionTreeNodeId)
		{
			return await DoSetPlanogram (locationId, outletId, offerId, decisionTreeNodeId);
		}

		public async Task <string> DeleteOutlet (string locationId, int outletId)
		{
			return await DoDeleteOutlet (locationId, outletId);
		}

		public async Task <string> DeleteAccount (string locationId)
		{
			return await DoDeleteAccount (locationId);
		}

		public async Task <string> SendPlanogramViaEmail (int planogramVersionId, string email)
		{
			return await DoSendPlanogramViaEmail (planogramVersionId, email);
		}
	}
}

