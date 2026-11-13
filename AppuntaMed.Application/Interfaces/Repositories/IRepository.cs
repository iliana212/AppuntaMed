using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IRepository<T> where T : class
	{
		Task<T?> GetById(Guid id);
		Task<IEnumerable<T>> GetAll();
		Task<int> CountAll();
		Task<T> Add(T item);
		Task Update(T item);
		Task Delete(T item);
	}
}
