using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;
		private readonly IWebHostEnvironment _environment;

		public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
		{
			_configuration = configuration;
			_environment = environment;
			_httpClient = httpClientFactory.CreateClient();
		}

		[HttpPost]
		public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
		{
			var authority = _configuration["Authentication:Authority"] ?? "http://localhost:8080/realms/AppuntaMed";
			var tokenEndpoint = $"{authority}/protocol/openid-connect/token";

			var keycloakParameters = new Dictionary<string, string>
			{
				{ "client_id", "appuntamed-api" },
				{ "grant_type", "password" },
				{ "username", request.User},
				{ "password", request.Psswd }
			};

			var content = new FormUrlEncodedContent(keycloakParameters);
			var response = await _httpClient.PostAsync(tokenEndpoint, content);

			if (!response.IsSuccessStatusCode)
			{
				return Unauthorized(new { mensaje = "No se pudo generar el token." });
			}

			var jsonResponse = await response.Content.ReadAsStringAsync();
			
			return Content(jsonResponse, "application/json");
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var authority = _configuration["Authentication:Authority"] ?? "http://localhost:8080/realms/AppuntaMed";
			var tokenEndpoint = $"{authority}/protocol/openid-connect/token";

			var keycloakParameters = new Dictionary<string, string>
			{
				{ "client_id", "appuntamed-api" },
				{ "grant_type", "refresh_token" },
				{ "refresh_token", request.RefreshToken }
			};

			var content = new FormUrlEncodedContent(keycloakParameters);
			var response = await _httpClient.PostAsync(tokenEndpoint, content);

			if (!response.IsSuccessStatusCode)
			{
				return Unauthorized(new { mensaje = "El refresh token es inválido o ha expirado." });
			}

			var jsonResponse = await response.Content.ReadAsStringAsync();

			return Content(jsonResponse, "application/json");
		}

		[HttpGet("claims")]
		[Authorize]
		public IActionResult GetClaims()
		{
			if (!_environment.IsDevelopment())
               return NotFound();

			var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
			return Ok(claims);
		}
	}

	public record TokenRequest(string User, string Psswd);
	public record RefreshTokenRequest(string RefreshToken);	
}
