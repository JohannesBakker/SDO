using System;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXLocationManager : SDXLocationService
	{
		public SDXLocationManager () : base ()
		{
		}

		public async Task <LocationModel> LoadLocation (string locationID)
		{
			return await GetLocation (locationID);
		}
	}
}

