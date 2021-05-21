using System;
using AuthService.Data;
using AuthService.Helpers;
using AuthService.Interfaces;
using Fitverse.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AuthService
{
	public class AuthServiceStartup : SharedStartup
	{
		public AuthServiceStartup(IConfiguration configuration, IWebHostEnvironment environment) : base(
			configuration,
			environment)
		{
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				//services.AddCors();

				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Fitverse.AuthService",
					Version = "v1",
					Description = "ASP.NET Core Web API for Fitverse, complex fitness management solution",
					Contact = new OpenApiContact
					{
						Name = "Paweł Wąsowski",
						Email = "pwasowski@edu.cdv.pl",
						Url = new Uri("https://www.linkedin.com/in/pawelwasowski/")
					}
				});
			});

			base.ConfigureServices(services);
			
			services.AddScoped<IAuthenticator, Authenticator>();
			
			services.AddDbContext<AuthContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("AzureDb")));

			services.AddMediatR(typeof(AuthServiceStartup));

			services.AddValidatorsFromAssembly(typeof(AuthServiceStartup).Assembly);
		}
	}
}