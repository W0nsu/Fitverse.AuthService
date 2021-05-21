using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using Fitverse.Client.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;

namespace AuthService.Helpers
{
	public class Authenticator : IAuthenticator
	{
		private readonly AuthContext _dbContext;
		private readonly IConfiguration _configuration;

		public Authenticator(AuthContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
		}
		
		public async Task<bool> Authenticate(User user, CancellationToken cancellationToken)
		{
			var userEntity = await _dbContext
				.Users
				.FirstOrDefaultAsync(x => x.Username == user.Username, cancellationToken);

			if (userEntity is not null)
			{
				return ComparePasswordToHash(user.Password, userEntity.Password);
			}

			throw new UnauthorizedAccessException($"Invalid credentials");
		}

		public string HashPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
			var hash = pbkdf2.GetBytes(20);
            
			var hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);
            
			var passwordHash = Convert.ToBase64String(hashBytes);

			return passwordHash;
		}

		public bool ComparePasswordToHash(string password, string hashedPassword)
		{
			var hashBytes = Convert.FromBase64String(hashedPassword);
			var salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
			var hash = pbkdf2.GetBytes(20);
			for (var i=0; i < 20; i++)
				if (hashBytes[i + 16] != hash[i])
					return false;

			return true;
		}
		
		public AuthenticatedUserModel GenerateJsonWebToken(User user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Issuer"],
				claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: credentials
			);
			
			var loginResult = new AuthenticatedUserModel()
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
				UserName = user.Username
			};
				
			return loginResult;
		}
	}
}