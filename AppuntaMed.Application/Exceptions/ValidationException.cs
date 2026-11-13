using FluentValidation.Results;

namespace AppuntaMed.Application.Exceptions
{
	public class ValidationExceptions : Exception
	{
		public List<string> ValidationErrors { get; set; } = [];

		public ValidationExceptions(string message)
		{
			ValidationErrors.Add(message);
		}

		public ValidationExceptions(ValidationResult validationResult)
		{
			foreach (var error in validationResult.Errors)
			{
				ValidationErrors.Add(error.ErrorMessage);
			}
		}
	}
}
