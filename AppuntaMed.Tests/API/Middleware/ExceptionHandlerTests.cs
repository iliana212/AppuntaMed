using AppuntaMed.API.Middleware;
using AppuntaMed.Application.Exceptions;
using AppuntaMed.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using System.Text.Json;

namespace AppuntaMed.Tests.API.Middleware
{	
	public class ExceptionHandlerTests
	{
		private readonly ExceptionHandler _handler = new(NullLogger<ExceptionHandler>.Instance);

		public static TheoryData<Exception, int> ExpectedStatusCodes => new()
		{
			{ new NotFoundException("missing"), 404 },
			{ new ForbiddenException("nope"), 403 },
			{ new ValidationExceptions("invalid"), 400 },
			{ new BusinessRuleException("rule"), 400 },
			{ new MediatorException("handler not found"), 500 },
			{ new InvalidOperationException("boom"), 500 }
		};

		private static async Task<(int Status, JsonElement Body)> Handle(ExceptionHandler handler, Exception exception)
		{
			var context = new DefaultHttpContext();
			context.Request.Path = "/api/test";
			context.Response.Body = new MemoryStream();

			var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

			Assert.True(handled);

			context.Response.Body.Position = 0;
			using var document = await JsonDocument.ParseAsync(context.Response.Body);

			return (context.Response.StatusCode, document.RootElement.Clone());
		}

		[Theory]
		[MemberData(nameof(ExpectedStatusCodes))]
		public async Task TryHandleAsync_MapsExceptionToExpectedStatusCode(Exception exception, int expectedStatus)
		{
			var (status, body) = await Handle(_handler, exception);

			Assert.Equal(expectedStatus, status);
			Assert.Equal(expectedStatus, body.GetProperty("status").GetInt32());
			Assert.Equal("/api/test", body.GetProperty("instance").GetString());
		}

		[Fact]
		public async Task TryHandleAsync_ValidationException_ReturnsEveryError()
		{
			var validation = new ValidationExceptions("first error");
			validation.ValidationErrors.Add("second error");

			var (_, body) = await Handle(_handler, validation);

			var errors = body.GetProperty("errors").EnumerateArray().Select(x => x.GetString()).ToList();
			Assert.Equal(new[] { "first error", "second error" }, errors);
		}

		[Fact]
		public async Task TryHandleAsync_UnexpectedException_DoesNotLeakInternalDetails()
		{
			var (status, body) = await Handle(_handler, new InvalidOperationException("connection string: Server=secret"));

			Assert.Equal(500, status);
			Assert.DoesNotContain("secret", body.GetProperty("detail").GetString());
		}

		[Fact]
		public async Task TryHandleAsync_ExceptionWrappedByReflection_IsUnwrapped()
		{
			var wrapped = new TargetInvocationException(new NotFoundException("Doctor not found"));

			var (status, body) = await Handle(_handler, wrapped);

			Assert.Equal(404, status);
			Assert.Equal("Doctor not found", body.GetProperty("detail").GetString());
		}
	}
}
