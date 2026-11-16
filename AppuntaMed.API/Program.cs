using AppuntaMed.API.Jobs;
using AppuntaMed.API.Middleware;
using AppuntaMed.API.Utilities;
using AppuntaMed.Application;
using AppuntaMed.Notifications;
using AppuntaMed.Persistence;
using AppuntaMed.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(seedDemoData: builder.Configuration.GetValue<bool>("Seeding:DemoData")); //Demo data is opt-in
builder.Services.AddNotificationServices();
builder.Services.AddSecurityServices();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHostedService<AppointmentReminderJob>();

builder.Services.AddOpenApi();

// --- CONFIGURACIÓN DE KEYCLOAK / JWT BEARER ---
var authority = builder.Configuration["Authentication:Authority"];
var requireHttps = builder.Configuration.GetValue<bool>("Authentication:RequireHttpsMetadata");
var audience = builder.Configuration["Authentication:Audience"];


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.MapInboundClaims = false;
		options.Authority = authority;
		options.Audience = audience;
		options.RequireHttpsMetadata = requireHttps;

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			NameClaimType = KeycloakClaims.Username,
			RoleClaimType = KeycloakClaims.Roles
		};		
	});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAppAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	app.MapScalarApiReference(options =>
	{
		options.WithTitle("AppuntaMed API").WithTheme(ScalarTheme.BluePlanet);			
	});
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();