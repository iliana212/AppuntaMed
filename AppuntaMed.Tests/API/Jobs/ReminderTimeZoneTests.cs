using AppuntaMed.API.Jobs;
using AppuntaMed.Tests.Utilities;
using System.Text.Json;

namespace AppuntaMed.Tests.API.Jobs
{
	public class ReminderTimeZoneTests
	{
		private static readonly DateTime MidYear = new(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Resolve_BlankId_UsesMexicoCity(string? blank)
		{
			var zone = ReminderTimeZone.Resolve(blank);

			// Mexico City is UTC-6 all year (no DST since 2022); US Central would be UTC-5 in July.
			Assert.Equal(TimeSpan.FromHours(-6), zone.GetUtcOffset(MidYear));
		}

		[Fact]
		public void Resolve_IanaId_ReturnsThatZone()
		{
			var zone = ReminderTimeZone.Resolve("America/New_York");

			Assert.Equal(TimeSpan.FromHours(-4), zone.GetUtcOffset(MidYear));
		}

		[Fact]
        public void Resolve_IdWithSurroundingSpaces_IsTrimmed()
        {
			var zone = ReminderTimeZone.Resolve("  America/Mexico_City ");
	
			Assert.Equal(TimeSpan.FromHours(-6), zone.GetUtcOffset(MidYear));
	    }

		[Fact]
        public void Resolve_UnknownId_ThrowsWithAHelpfulMessage()
        {
			var ex = Assert.Throws<InvalidOperationException>(() => ReminderTimeZone.Resolve("Mars/Olympus_Mons"));
	
			Assert.Contains("Reminders:TimeZoneId", ex.Message);
			Assert.Contains("Mars/Olympus_Mons", ex.Message);
	    }

		[Fact]
        public void DefaultAppsettings_ContainsAValidTimeZone()
        {
	        using var document = JsonDocument.Parse(RepoFiles.Read(Path.Combine("AppuntaMed.API", "appsettings.json")));
			var configured = document.RootElement.GetProperty("Reminders").GetProperty("TimeZoneId").GetString();
	
			var exception = Record.Exception(() => ReminderTimeZone.Resolve(configured));
	
			Assert.Null(exception);
	    }
	}
}
