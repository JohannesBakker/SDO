using System;

namespace Sodexo.Core
{
	public static class Constants
	{
		public static string APIRootUrl = "";
		public static string APISubscriptionKey = "";
		public static string APIAuthenticationCookieDomain = "";
		public static string APIAuthenticationCookiePath = "";

		// Error Messages
		public const string ERROR_MSG_NEED_REAUTH			= "You need to login again.";
		public const string ERROR_MSG_RESOURCE_NOT_FOUND	= "Resource Not Found";
		public const string ERROR_MSG_BAD_REQUEST			= "Bad Request";
	}
}

