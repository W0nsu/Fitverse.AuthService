using AuthService.Dtos;
using Fitverse.Client.Models;
using MediatR;

namespace AuthService.Commands
{
	public class LoginCommand : IRequest<AuthenticatedUserModel>
	{
		public LoginCommand(UserDto user)
		{
			User = user;
		}

		public UserDto User { get; }
	}
}