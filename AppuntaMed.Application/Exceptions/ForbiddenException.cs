using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Exceptions
{
	public class ForbiddenException(string message) : Exception(message)
	{
	}
}
