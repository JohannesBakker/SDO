using System;
using System.Collections.Generic;

namespace Sodexo.Core
{
	public class SDXAuthManager
	{
		private Dictionary<string,string> fedAuthTokens;
		public DateTime AuthenticationTime { get; set; }
		public bool IsAuthenticated { get; set; }

		public SDXAuthManager ()
		{
			IsAuthenticated = false;
		}

		public void SetSecurityToken(Dictionary<string,string> tokens)
		{
			fedAuthTokens = tokens;
		}

		public Dictionary<string,string> GetSecurityToken()
		{
			return fedAuthTokens;
		}

		public void Logout()
		{
			fedAuthTokens = new Dictionary<string, string> ();
			IsAuthenticated = false;
			AuthenticationTime = DateTime.MinValue;
		}
	}
}

