using AppuntaMed.Application.Exceptions;
using AppuntaMed.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AppuntaMed.API.Middleware
{
	public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			var error = exception is TargetInvocationException { InnerException: { } inner } ? inner : exception;

			var problemDetails = error switch
			{
				NotFoundException => Create(StatusCodes.Status404NotFound, "Resource not found", error.Message),
				ForbiddenException => Create(StatusCodes.Status403Forbidden, "Forbidden", error.Message),
				ValidationExceptions validation => CreateValidation(validation),
				BusinessRuleException => Create(StatusCodes.Status400BadRequest, "Business rule violation", error.Message),
				_ => Create(StatusCodes.Status500InternalServerError, "Server failure", "An unexpected error occurred on the server")
			};

			problemDetails.Instance = httpContext.Request.Path;

			if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
				logger.LogError(error, "Unhandled exception: {Message}", error.Message);
			           else
				logger.LogWarning("Request rejected ({Status}): {Message}", problemDetails.Status, error.Message);
			
			httpContext.Response.StatusCode = problemDetails.Status!.Value;
			
			await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null,
			contentType: "application/problem+json", cancellationToken);

			return true;
		}

		private static ProblemDetails Create(int status, string title, string detail)
		{
			return new ProblemDetails { Status = status, Title = title, Detail = detail };
		}

		private static ProblemDetails CreateValidation(ValidationExceptions validation)
		{
			var problem = Create(StatusCodes.Status400BadRequest, "Validation failed", "One or more validation errors occurred");
			problem.Extensions["errors"] = validation.ValidationErrors;
			return problem;
		}
	}
}
