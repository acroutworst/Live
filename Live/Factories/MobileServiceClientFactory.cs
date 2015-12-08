using System;
using System.Net.Http;
using Microsoft.WindowsAzure.MobileServices;

namespace Live
{
	public static class MobileServiceClientFactory
	{
		public static MobileServiceClient CreateClient()
		{
			return new MobileServiceClient (Keys.ApplicationURL, Keys.ApplicationKey);
		}

		public static MobileServiceClient CreateClient(params HttpMessageHandler[] handlers)
		{
			return new MobileServiceClient (Keys.ApplicationURL, Keys.ApplicationKey, handlers);
		}
	}
}

