namespace AppuntaMed.API.Utilities
{
	public static class HttpContextExtensions
	{
		public static void PaginationHeader(this HttpContext httpContext, int totalItems)
		{
			httpContext.Response.Headers.Append("total-items", totalItems.ToString());
		}
	}
}
