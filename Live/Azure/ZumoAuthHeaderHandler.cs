using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Live
{
	public class ZumoAuthHeaderHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(AccountService.Instance.AuthenticationToken))
			{
				throw new InvalidOperationException("User is not currently logged in");
			}

			request.Headers.Add("X-ZUMO-AUTH", AccountService.Instance.AuthenticationToken);

			return base.SendAsync(request, cancellationToken);
		}
	}
}

