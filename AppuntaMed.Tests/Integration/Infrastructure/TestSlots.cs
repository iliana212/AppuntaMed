using System.Globalization;

namespace AppuntaMed.Tests.Integration.Infrastructure
{
	public static class TestSlots
	{
		private static readonly DateTime Origin = DateTime.UtcNow.Date.AddDays(60);
		private static int _counter;

		/// <summary>Slots start 3 hours apart. Some tests book the interval right AFTER their slot and some use 60-minute slots</summary>
		public static (DateTime Start, DateTime End) Next(int minutes = 30)
		{
			var start = Origin.AddHours(3 * Interlocked.Increment(ref _counter));

			return (start, start.AddMinutes(minutes));
		}

       /// <summary>
       /// Query strings are sent WITHOUT a "Z": MVC would convert a UTC "Z" date to the machine's local time
       /// and the filter would shift by the time zone of whoever runs the tests.
       /// </summary>
       public static string Query(DateTime value) => value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }
}
