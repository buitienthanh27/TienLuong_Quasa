using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace API_Sample.Application.ExtensionMethods
{
	public static class ControllerBaseExtensionMethod
	{
		public static string GetAccessTokenByHeaderAuthorization(this ControllerBase controllerBase)
		{
			try
			{
				var authorization = controllerBase.Request.Headers[HeaderNames.Authorization];
				AuthenticationHeaderValue.TryParse(authorization, out var accessToken);
				return accessToken.Parameter;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}
