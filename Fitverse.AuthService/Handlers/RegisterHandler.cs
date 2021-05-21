using System;
using System.Threading;
using System.Threading.Tasks;
using AuthService.Commands;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Helpers;
using AuthService.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Handlers
{
	public class RegisterHandler : IRequestHandler<RegisterCommand, UserDto>
	{
		private readonly IConfiguration _configuration;
		private readonly IServiceProvider _provider;

		public RegisterHandler(IConfiguration configuration, IServiceProvider provider)
		{
			_configuration = configuration;
			_provider = provider;
		}
		
		public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
		{
			var userEntity = request.User.Adapt<User>();

			using var scope = _provider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
			
			var isLoginAlreadyTaken = await dbContext
				.Users
				.AnyAsync(m => m.Username == userEntity.Username, cancellationToken);

			if (isLoginAlreadyTaken)
				throw new ArgumentException($"Login: [{request.User.Username}] already taken");
			
			var authenticator = new Authenticator(dbContext, _configuration);

			userEntity.Password = authenticator.HashPassword(userEntity.Password);
			
			_ = await dbContext.Users.AddAsync(userEntity, cancellationToken);
			_ = await dbContext.SaveChangesAsync(cancellationToken);

			return request.User;
		}
	}
}