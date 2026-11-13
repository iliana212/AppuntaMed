using AppuntaMed.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppuntaMed.Persistence.Repositories
{
	public class Repository<T>(AppuntaMedDbContext context) : IRepository<T> where T : class
	{
		public Task<T> Add(T item)
		{
			context.Add(item);
			return Task.FromResult(item);
		}

		public async Task<int> CountAll()
		{
			return await context.Set<T>().CountAsync();
		}

		public Task Delete(T item)
		{
			context.Remove(item);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			return await context.Set<T>().ToListAsync();
		}

		public async Task<T?> GetById(Guid id)
		{
			return await context.Set<T>().FindAsync(id);
		}

		public Task Update(T item)
		{
			context.Update(item);
			return Task.CompletedTask;
		}
	}
}
