using AppuntaMed.API.Utilities;
using AppuntaMed.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AppuntaMed.Tests.API.Utilities
{
	/// <summary>
	/// A permission without a registered policy makes the endpoint fail at runtime (500).
	/// </summary>
	public class AuthorizationExtensionsTests
	{
		private static ServiceProvider BuildProvider()
		{
			var services = new ServiceCollection();
			services.AddLogging();
			services.AddAppAuthorization();

			return services.BuildServiceProvider();
		}

		[Fact]
        public async Task AddAppAuthorization_RegistersOnePolicyPerDeclaredPermission()
        {
			using var provider = BuildProvider();
			var policies = provider.GetRequiredService<IAuthorizationPolicyProvider>();
	
	        foreach (var permission in Permissions.All)
		    {
				var policy = await policies.GetPolicyAsync(permission);		
				Assert.True(policy is not null, $"No authorization policy registered for '{permission}'");
		    }
	    }

		[Fact]
        public async Task AddAppAuthorization_ProfessionReadPolicy_RequiresThatPermission()
        {
			using var provider = BuildProvider();
			var policies = provider.GetRequiredService<IAuthorizationPolicyProvider>();
			var policy = await policies.GetPolicyAsync(Permissions.ProfessionRead);
	
			Assert.NotNull(policy);
			Assert.Single(policy!.Requirements);
	    }
    }
}