using Kutuphane.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Repository.Shared.Abstract
{
	public interface IRepository<T> where T : BaseModel
	{
		IQueryable<T> GetAll(); //gelen dataya tekrar sorgulamak yapabilmek için  Iqueryable tipinde tanımladık.
		IQueryable<T> GetAll(Expression<Func<T, bool>> filter);

		void Add(T entity);
		void Remove(T entity);
		void Update(T entity);

		void AddRange(IEnumerable<T> entities);
		void RemoveRange(IEnumerable<T> entities);
		void UpdateRange(IEnumerable<T> entities);

		T GetFirstOrDefault(Expression<Func<T,bool>> filter);
	}
}
