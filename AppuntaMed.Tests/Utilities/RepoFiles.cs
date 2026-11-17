namespace AppuntaMed.Tests.Utilities
{
	public static class RepoFiles
	{
		public static string Read(string relativePath) => File.ReadAllText(PathOf(relativePath));

		public static string PathOf(string relativePath)
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);

			while (directory is not null)
			{
				var candidate = Path.Combine(directory.FullName, relativePath);

				if (File.Exists(candidate))
					return candidate;

				directory = directory.Parent;
			}

			throw new FileNotFoundException($"'{relativePath}' was not found above '{AppContext.BaseDirectory}'");
		}
	}
}
