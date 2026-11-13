using AppuntaMed.Application.Interfaces.DataStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence.UnitOfWork
{
	public class UnitOfWorkEFCore(AppuntaMedDbContext context) : IUnitOfWork
	{
		public async Task Commit()
		{
			await context.SaveChangesAsync();
		}

		public Task Rollback()
		{
			return Task.CompletedTask;
		}
	}
}
