using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Repository.Shared.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Unicode;

namespace Kutuphane.Web.Controllers
{
	public class UserController : Controller
	{
	private readonly IUnitOfWork unitOfWork;

		public UserController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		[Authorize(Roles = "Admin,User")]
        public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		[Authorize(Roles = "Admin, User")] // rolü admin olanlar create fonksiyonunu kullanabilir.
		public IActionResult Create(AppUser user)
		{
			Random rnd = new Random();
			user.Password = rnd.Next(100, 10000).ToString(); //yeni kullanıcı oluşturulduğu zaman random 100 ve 10000 arasında bir şifre oluştursun.
			if(user.UserName != null){
				unitOfWork.AppUsers.Add(user);
				unitOfWork.Save();
				return Ok();
			}
			else{
				return BadRequest();
			}
		}

		[HttpDelete]
		[Authorize(Roles = "Admin")]
		public IActionResult Delete(Guid id)
		{
			AppUser user = unitOfWork.AppUsers.GetFirstOrDefault(u => u.Id == id);
			if(user.UserName != null ){
				unitOfWork.AppUsers.Remove(user);
				unitOfWork.Save();
				return Ok();
			}
			else{
				return BadRequest();
			}
		}

		[HttpPost]
		public IActionResult GetById(Guid id)
		{
			AppUser user = unitOfWork.AppUsers.GetFirstOrDefault(u => u.Id == id);
			if(user.UserName != null){
				return Json(user);
			}
			else{
				return BadRequest();
			}
				
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Update(AppUser user) 
		{
			if(user.UserName != null){
				AppUser asil = unitOfWork.AppUsers.GetFirstOrDefault(u => u.Id == user.Id);
				asil.Name = user.Name;
				asil.UserName = user.UserName;
				asil.Password = user.Password;
				asil.isAdmin = user.isAdmin;
				asil.DateModified = DateTime.Now;
				unitOfWork.AppUsers.Update(asil);
				unitOfWork.Save();
			}
			return Ok();
		}
		public IActionResult GetAll() 
		{
			List<AppUser> users = unitOfWork.AppUsers.GetAll().ToList<AppUser>();
            return Json(new { data = users });
        }
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(AppUser user)
		{
			AppUser appuser = unitOfWork.AppUsers.GetFirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
			if (appuser != null) 
			{
				List<Claim> claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.Name,appuser.UserName));
                claims.Add(new Claim(ClaimTypes.GivenName, appuser.Name));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, appuser.Id.ToString()));
				if (appuser.isAdmin)
				{
					claims.Add(new Claim(ClaimTypes.Role, "Admin"));
				}
				else 
				{
					claims.Add(new Claim(ClaimTypes.Role, "User"));
				}
				var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
				
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
				return RedirectToAction("Index", "Home");
			}

			else
			{
				return View();
			}
        }
		public async Task<IActionResult> Logout()
		{
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}

	}
}
