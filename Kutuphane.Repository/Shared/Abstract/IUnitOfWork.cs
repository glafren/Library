using Kutuphane.Models;
using Kutuphane.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Repository.Shared.Abstract
{
	public interface IUnitOfWork
	{
		IRepository<Author> Authors { get; }
		IRepository<Publisher> Publisher { get; }
		IBookRepository Books { get; } // içerisinde kendine özel fonksiyon olduğu için.
		IRepository<Category> Categories { get; }
		IRepository<AppUser> AppUsers { get; }

		void Save();
	}
}
