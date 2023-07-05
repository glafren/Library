using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Repository.Abstract;
using Kutuphane.Repository.Shared.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.Web.Controllers
{
    public class PublisherController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

		public PublisherController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            List<Publisher> publishers =unitOfWork.Publisher.GetAll().ToList<Publisher> ();
            return Json(new { data = publishers });
        }

		public IActionResult GetPublisherIdAndName()
		{
			var publisher = unitOfWork.Publisher.GetAll().Select(c => new {
				Id = c.Id,
				Name = c.Name
			}).ToList();
			return Json(publisher);
		}

		[HttpPost]
        public IActionResult Create(Publisher publisher)
        {
            if(publisher.Name != null)
            {
                //_db.Publishers.Add(publisher);
                //_db.SaveChanges();
                unitOfWork.Publisher.Add(publisher);
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
			//Publisher publisher = _db.Publishers.FirstOrDefault(x => x.Id == id);
			Publisher publisher = unitOfWork.Publisher.GetFirstOrDefault(x => x.Id == id);
			if (publisher != null)
            {
                //_db.Publishers.Remove(publisher);
                //_db.SaveChanges();
                unitOfWork.Publisher.Remove(publisher);
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
            //Publisher publisher =_db.Publishers.FirstOrDefault(p => p.Id == id);
			Publisher publisher = unitOfWork.Publisher.GetFirstOrDefault(p => p.Id == id);
			if (publisher != null)
            {
                return Ok(publisher);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Update(Publisher publisher)
        {
            if(publisher.Name != null)
            {
				//Publisher asil = _db.Publishers.FirstOrDefault(p => p.Id == publisher.Id);
				Publisher asil = unitOfWork.Publisher.GetFirstOrDefault(p => p.Id == publisher.Id);
				asil.Description = publisher.Description;
				asil.Name = publisher.Name;
				asil.DateModified = DateTime.Now;
				//_db.Publishers.Update(asil);
				//_db.SaveChanges();
                unitOfWork.Publisher.Update(publisher);
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
