using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Utilities.Mediator;
using FluentValidation;
using NSubstitute;

namespace AppuntaMed.Tests.Application.Utilities.Mediator
{
	public class MediatorTests
	{
		public class FakeRequest : IRequest<string> 
		{
			public required string Name { get; set; }
		}

		public class FakeHandler : IRequestHandler<FakeRequest, string> {
			public Task<string> Handle(FakeRequest request)
			{
				return Task.FromResult("correct answer");
			}
		}

		public class FakeRequestValidator : AbstractValidator<FakeRequest>
		{
			public FakeRequestValidator()
			{
				RuleFor(x => x.Name).NotEmpty();
			}
		}

		[Fact]
		public async Task Send_CallMethodHandler()
		{
			var request = new FakeRequest { Name = "Name A"};
			var useCaseMock = Substitute.For<IRequestHandler<FakeRequest, string>>();
			var serviceProvider = Substitute.For<IServiceProvider>();

			serviceProvider.GetService(typeof(IRequestHandler<FakeRequest, string>)).Returns(useCaseMock);

			var mediator = new AppuntaMed.Application.Utilities.Mediator.Mediator(serviceProvider);
			var result = await mediator.Send(request);

			await useCaseMock.Received(1).Handle(request);


		}

		[Fact]
		public async Task Send_NoHandlerRegistred_ThrowException()
		{
			var request = new FakeRequest { Name = "Name A" };
			var useCaseMock = Substitute.For<IRequestHandler<FakeRequest, string>>();
			var serviceProvider = Substitute.For<IServiceProvider>();

			var mediator = new AppuntaMed.Application.Utilities.Mediator.Mediator(serviceProvider);

			await Assert.ThrowsAsync<MediatorException>(() => mediator.Send(request));
		}

		[Fact]
		public async Task Send_InvalidCommand_ThrowsException()
		{
			var request = new FakeRequest { Name = "" };
			var serviceProvider = Substitute.For<IServiceProvider>();
			var validator = new FakeRequestValidator();

			serviceProvider.GetService(typeof(IValidator<FakeRequest>))
				.Returns(validator);

			var mediator = new AppuntaMed.Application.Utilities.Mediator.Mediator(serviceProvider);

			await Assert.ThrowsAsync<ValidationExceptions>(() => mediator.Send(request));

		}
	}
}
 