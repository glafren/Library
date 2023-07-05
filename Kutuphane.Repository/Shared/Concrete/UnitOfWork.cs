using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Repository.Abstract;
using Kutuphane.Repository.Concrete;
using Kutuphane.Repository.Shared.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Repository.Shared.Concrete
{
	public class UnitOfWork : IUnitOfWork
	{
		public IRepository<Author> Authors {get; private set;}

		public IRepository<Publisher> Publisher { get; private set; }

		public IBookRepository Books { get; private set; }

		public IRepository<Category> Categories { get; private set; }
		public IRepository<AppUser> AppUsers { get; private set; }

		private readonly ApplicationDbContext _db;

		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Authors = new Repository<Author>(db);
			Publisher = new Repository<Publisher>(db);
			Categories = new Repository<Category>(db);
			AppUsers = new Repository<AppUser>(db);
			Books = new BookRepository(db);
		}

		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
