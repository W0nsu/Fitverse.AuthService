using AuthService.Helpers;
using AuthService.Models;
using Fitverse.Client.Models;

namespace AuthService.Interfaces
{
	public interface IAuthenticator
	{
        public string HashPassword(string password);
        public bool ComparePasswordToHash(string password, string hashedPassword);
        public AuthenticatedUserModel GenerateJsonWebToken(User user);
	}
}