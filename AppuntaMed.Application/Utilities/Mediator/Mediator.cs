using AppuntaMed.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace AppuntaMed.Application.Utilities.Mediator
{
	public class Mediator(IServiceProvider serviceProvider) : IMediator
	{
		public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
		{
			await RunValidations(request);

			var useCaseType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
			var useCase = serviceProvider.GetService(useCaseType);

			if (useCase is null)
				throw new MediatorException($"Handler not found: {request.GetType().Name}");

			var method = useCaseType.GetMethod("Handle")!;

			return await (Task<TResponse>)method.Invoke(useCase, [request])!;
		}

		public async Task Send(IRequest request)
		{
			await RunValidations(request);

			var useCaseType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
			var useCase = serviceProvider.GetService(useCaseType);

			if (useCase is null)
				throw new MediatorException($"Handler not found: {request.GetType().Name}");

			var method = useCaseType.GetMethod("Handle")!;

			await (Task)method.Invoke(useCase, [request])!;
		}

		private async Task RunValidations(object request)
		{
			var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
			var validator = serviceProvider.GetService(validatorType);

			if (validator is not null)
			{
				var validateMethod = validatorType.GetMethod("ValidateAsync");
				var validateTask = (Task)validateMethod!.Invoke(validator, [request, CancellationToken.None])!;

				await validateTask.ConfigureAwait(false);

				var result = validateTask.GetType().GetProperty("Result");
				var validationResult = (ValidationResult)result!.GetValue(validateTask)!;

				if (!validationResult.IsValid)
					throw new ValidationExceptions(validationResult);
			}
		}
	}
}
