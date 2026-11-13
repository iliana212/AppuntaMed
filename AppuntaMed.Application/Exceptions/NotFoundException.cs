using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Exceptions
{
	public class NotFoundException(string message) : Exception(message)
	{
	}
}
