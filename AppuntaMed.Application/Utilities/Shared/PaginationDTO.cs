using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Utilities.Shared
{
	public class PaginationDTO<T>
	{
		public List<T> Items { get; set; } = [];
		public int Total { get; set; }
	}
}
