using System;
using System.Threading;
using System.Threading.Tasks;
using AuthService.Commands;
using AuthService.Data;
using AuthService.Helpers;
using AuthService.Models;
using Fitverse.Client.Models;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Handlers
{
	public class LoginHandler : IRequestHandler<LoginCommand, AuthenticatedUserModel>
	{
		private readonly IConfiguration _configuration;
		private readonly IServiceProvider _provider;

		public LoginHandler(IConfiguration configuration, IServiceProvider provider)
		{
			_configuration = configuration;
			_provider = provider;
		}
		
		public async Task<AuthenticatedUserModel> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			var user = request.User.Adapt<User>();
			
			using var scope = _provider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
			
			var authenticator = new Authenticator(dbContext, _configuration);

			var isUserAuthenticated = await authenticator.Authenticate(user, cancellationToken);

			if (!isUserAuthenticated)
				throw new UnauthorizedAccessException($"Invalid credentials");
			
			var loginResult = authenticator.GenerateJsonWebToken(user);
			return loginResult;
		}
	}
}