using System;

namespace AuthService.Helpers
{
	public class LoginResult
	{
		public string Token { get; set; }
		public DateTime Expiry { get; set; }
	}
}