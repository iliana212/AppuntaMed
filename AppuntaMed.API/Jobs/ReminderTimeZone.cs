namespace AppuntaMed.API.Jobs
{
	public static class ReminderTimeZone
	{
		public const string DefaultId = "America/Mexico_City";

		/// <summary>
		/// IANA ids ("America/Mexico_City") work on Linux/Docker and, with ICU, on Windows too.
		/// An invalid id stops the application at startup with a clear message instead of failing at 8:00 am.
		/// </summary>
		public static TimeZoneInfo Resolve(string? timeZoneId)
		{
			var id = string.IsNullOrWhiteSpace(timeZoneId) ? DefaultId : timeZoneId.Trim();

			try
			{
				return TimeZoneInfo.FindSystemTimeZoneById(id);
			}
			catch (Exception ex) when(ex is TimeZoneNotFoundException or InvalidTimeZoneException)
			{
				throw new InvalidOperationException(
					$"The time zone '{id}' set in Reminders:TimeZoneId was not found. " +
					"Use an IANA id such as 'America/Mexico_City' (the container needs tzdata).", ex);
			}
        }
	}
}
