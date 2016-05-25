using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXUserManager : SDXUserService
	{
		public UserModel Me = null;

		public SDXUserManager () : base ()
		{

		}

		public async Task<UserModel> LoadUser (string domainUniqueIdentifier, bool isIncludeAccounts = true)
		{
			return await GetUser (domainUniqueIdentifier, isIncludeAccounts);
		}

		public async Task<UserModel> LoadMe (bool isIncludedAccounts = true)
		{
			Me = await GetUser ("me", isIncludedAccounts);

			return Me;
		}

		public async Task<string> AddMeToAccount (string locationId)
		{
			return await AddUserToAccount ("me", locationId);
		}
	}
}

