using System.Threading.Tasks;
using AuthService.Commands;
using AuthService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly IMediator _mediator;

		public AuthController(IMediator mediator)
		{
			_mediator = mediator;
		}
		
		[HttpPost("fitverse-admin-add")]
		public async Task<IActionResult> RegisterAsync([FromBody] UserDto user)
		{
			var command = new RegisterCommand(user);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<IActionResult> LoginAsync([FromBody] UserDto user)
		{
			var command = new LoginCommand(user);
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
}