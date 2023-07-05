using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Repository.Abstract;
using Kutuphane.Repository.Shared.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Security.Cryptography.X509Certificates;

namespace Kutuphane.Web.Controllers
{	
	[Authorize]
    public class BookController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

		public BookController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
			//List<Book> book = _db.Books.Include(c=> c.Category).Include(x=> x.Authors).Include(p=>p.Publishers).ToList<Book>();

			//List<Book> list = _bookRepo.GetAll().Select(x => new Book
			//{
			//    Id = x.Id,
			//    Name = x.Name,
			//    Publishers = x.Publishers,
			//    Authors= x.Authors,
			//    Description = x.Description,
			//    PublishDate = x.PublishDate,
            //    Price = x.Price,
            //    TotalPages = x.TotalPages
            //}).ToList<Book>();

            return Json(new { data = unitOfWork.Books.GetAll().ToList<Book>() });
        }

        [HttpPost]
        public IActionResult Create(Book book, List<Author> authors, List<Publisher> publishers)
        {
            if (book.Name != null)
            {
				unitOfWork.Books.Add(book);
                //_db.SaveChanges();
                Book eklenenKitap = unitOfWork.Books.GetFirstOrDefault(b => b.Id == book.Id);
                eklenenKitap.Authors = authors;
                eklenenKitap.Publishers = publishers;
				unitOfWork.Books.Update(eklenenKitap);
                // _db.SaveChanges();
                unitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            Book book = unitOfWork.Books.GetFirstOrDefault(x => x.Id == id);
            if (book != null)
            {
				unitOfWork.Books.Remove(book);
                //_db.SaveChanges();
                unitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult GetById(Guid id)
        {
            // Book book = _db.Books.Include(b=> b.Authors).Include(b=>b.Publishers).FirstOrDefault(b => b.Id == id);
            Book book = unitOfWork.Books.GetAll().Select(x => new Book
            {
                Id = x.Id,
                Name = x.Name,
                Publishers = x.Publishers,
                Authors = x.Authors,
                PublishDate = x.PublishDate,
                Description = x.Description,
                Price = x.Price,
                TotalPages = x.TotalPages
            }).First(); 
           if (book != null)
            {
                return Ok(book);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Update(Book book)
        {
            
            if (book.Name != null)
            {
				//Book asil = _db.Books.Include(b=> b.Authors).Include(b => b.Publishers).FirstOrDefault(b => b.Id == book.Id);

				Book asil = unitOfWork.Books.GetAll().Select(x => new Book
				{
					Id = x.Id,
					Name = x.Name,
					Publishers = x.Publishers,
					Authors = x.Authors,
					PublishDate = x.PublishDate,
					Description = x.Description,
					Price = x.Price,
					TotalPages = x.TotalPages
				}).First();

				asil.Authors = null;
                asil.Publishers = null;
				unitOfWork.Books.Update(asil);
                asil.Id = book.Id;
                asil.Price = book.Price;
                asil.TotalPages = book.TotalPages;
                asil.Isbn = book.Isbn;
                asil.PublishDate = book.PublishDate;
                asil.CategoryId = book.CategoryId;
                asil.Name = book.Name;
                asil.Description = book.Description;
                asil.Authors = book.Authors;
                asil.Publishers = book.Publishers;
				unitOfWork.Books.Update(asil);
                //_db.SaveChanges();
                unitOfWork.Save();
                return Ok();
			}
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult RemoveAuthorFromBook(Guid AuthorId, Guid BookId)
        {
            //Book book = _db.Books.Include(b => b.Authors).FirstOrDefault(b=>b.Id == BookId);//kitaplar ve yazarları joinledik. Db'de kitap id == DataTable'dan gelen BookId değerine eşit olan ilk değeri aldık book nesnesine atadık
            Book book = unitOfWork.Books.GetAll(b=> b.Id == BookId).First();
            
            Author author = book.Authors.FirstOrDefault(a => a.Id == AuthorId);
            book.Authors.Remove(author);
			unitOfWork.Books.Update(book);
            // _db.SaveChanges();
            unitOfWork.Save();
            return Ok();
        }

		[HttpPost]
		public IActionResult RemovePublisherFromBook(Guid PublisherId, Guid BookId)
		{
			//Book book = _db.Books.Include(b => b.Publishers).FirstOrDefault(b => b.Id == BookId);
			Book book = unitOfWork.Books.GetAll(b => b.Id == BookId).First();

			Publisher publisher = book.Publishers.FirstOrDefault(a => a.Id == PublisherId);
			
            book.Publishers.Remove(publisher);
			unitOfWork.Books.Update(book);
            //_db.SaveChanges();
            unitOfWork.Save();
			return Ok();
		}
        
        [HttpPost]
        public IActionResult AddAuthorToBook(Guid bookId, List<Guid> authors)
        {
            Book kitap = unitOfWork.Books.GetFirstOrDefault(b => b.Id == bookId);
            
            //List<Author> yazarlar = new List<Author>();
            //foreach(Guid authorId in authors)
            //{
            //    _db.Authors.FirstOrDefault(a => a.Id == authorId);
            //}

            List<Author> yazarlar = authors.Select(authorId => unitOfWork.Authors.GetFirstOrDefault(a => a.Id == authorId)).ToList();

            kitap.Authors = yazarlar;
            unitOfWork.Books.Update(kitap);
            //_db.SaveChanges();
            unitOfWork.Save();
            return Ok();
        }
        [HttpPost]
        public IActionResult AddPublisherToBook(Guid publisherId, List<Guid> publishers)
        {
            Book kitap = unitOfWork.Books.GetFirstOrDefault(b => b.Id == publisherId);
			List<Publisher> yayinevi = publishers.Select(publisherId => unitOfWork.Publisher.GetFirstOrDefault(p => p.Id == publisherId)).ToList();
			kitap.Publishers = yayinevi;
			unitOfWork.Books.Update(kitap);
            //_db.SaveChanges();
            unitOfWork.Save();
            return Ok();
        }

	}
}