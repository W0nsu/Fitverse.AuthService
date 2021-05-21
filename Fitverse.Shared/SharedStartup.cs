using System;
using System.Text;
using Fitverse.Shared.PipelineBehaviors;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace Fitverse.Shared
{
	public abstract class SharedStartup
	{
		private readonly IWebHostEnvironment _currentEnvironment;

		public SharedStartup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
		{
			Configuration = configuration;
			_currentEnvironment = currentEnvironment;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ContractResolver = new DefaultContractResolver();
			});

			services.AddCors(o => o.AddPolicy("DevPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));

			services.AddProblemDetails(options =>
			{
				options.IncludeExceptionDetails = (ctx, ex) => _currentEnvironment.IsDevelopment();
				//options.IncludeExceptionDetails = (ctx, ex) => false;

				options.Map<NullReferenceException>(exception => new ProblemDetails
				{
					Title = "NotFound",
					Status = StatusCodes.Status404NotFound,
					Detail = exception.Message
				});
				options.Map<ArgumentException>(exception => new ProblemDetails
				{
					Title = "Invalid parameter",
					Status = StatusCodes.Status403Forbidden,
					Detail = exception.Message
				});
				options.Map<ValidationException>(exception => new ProblemDetails
				{
					Title = "Invalid parameter",
					Status = StatusCodes.Status403Forbidden,
					Detail = exception.Message
				});
				options.Map<UnauthorizedAccessException>(exception => new ProblemDetails
				{
					Title = "Invalid credentials",
					Status = StatusCodes.Status401Unauthorized,
					Detail = exception.Message
				});
			});

			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseCors("DevPolicy");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitverse.MembersService v1"));
			}

			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitverse.MembersService v1"));

			//app.UseHttpsRedirection();

			app.UseAuthentication();
			
			app.UseRouting();

			app.UseProblemDetails();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}