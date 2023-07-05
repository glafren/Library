using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Repository.Abstract;
using Kutuphane.Repository.Shared.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.Web.Controllers
{
	public class AuthorController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		public AuthorController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GetAll()
		{
			List<Author> author = unitOfWork.Authors.GetAll().ToList<Author>();
			return Json(new { data = author });
		}

		public IActionResult GetAuthorIdAndName()
		{
			var author = unitOfWork.Authors.GetAll().Select(c => new {
				Id = c.Id,
				Name = c.Name
			}).ToList();
			return Json(author);

		}

		[HttpPost]
        public IActionResult Create(Author author)
        {
            if(author.Name != null)
            {
				//_db.Authors.Add(author);
				unitOfWork.Authors.Add(author);
				//_db.SaveChanges();
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
			//Author author = _db.Authors.FirstOrDefault(x => x.Id == id);
			Author author = unitOfWork.Authors.GetFirstOrDefault(c => c.Id == id);
			if (author != null)
			{
				//_db.Authors.Remove(author);
				unitOfWork.Authors.Remove(author);
				//_db.SaveChanges();
				unitOfWork.Save();
				return Ok();
			}
			else { return BadRequest(); }
		}

		[HttpPost]
		public IActionResult GetById(Guid id)
		{
			//Author author = _db.Authors.FirstOrDefault(a => a.Id == id);
			Author author = unitOfWork.Authors.GetFirstOrDefault(a => a.Id == id);
			if (author != null)
			{
				return Ok(author);
			}
			else
			{
				return BadRequest();
			}
		}
		[HttpPost]
		public IActionResult Update(Author author)
		{
			if (author.Name != null)
			{
				//Author asil = _db.Authors.FirstOrDefault(a => a.Id == author.Id);
				Author asil = unitOfWork.Authors.GetFirstOrDefault(a => a.Id == author.Id);
				asil.Description = author.Description;
				asil.Name = author.Name;
				asil.DateModified = DateTime.Now;
				//_db.Authors.Update(asil);
				//_db.SaveChanges();
				unitOfWork.Authors.Update(asil);
				unitOfWork.Save();
				return Ok();
			}
			else
			{
				return BadRequest();
			}
		}
	}
}
