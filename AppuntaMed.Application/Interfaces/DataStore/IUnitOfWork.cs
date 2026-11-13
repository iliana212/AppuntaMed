using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.DataStore
{
	public interface IUnitOfWork
	{
		Task Commit();
		Task Rollback();
	}
}
