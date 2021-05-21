using System.Diagnostics;
using AuthService.Dtos;
using AuthService.Models;
using MediatR;

namespace AuthService.Commands
{
	public class RegisterCommand : IRequest<UserDto>
	{
		public RegisterCommand(UserDto user)
		{
			User = user;
		}

		public UserDto User { get; }
	}
}