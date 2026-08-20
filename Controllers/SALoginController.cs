using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;
        public SALoginController(AccountRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(SALoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _repository.Login(model.sEmail, model.sPassword);

            if (user != null)
            { 
                HttpContext.Session.SetString("SAID", user.nID.ToString());
                HttpContext.Session.SetString("SAName", user.sFName);
                HttpContext.Session.SetString("SARole", user.sRole);

                return RedirectToAction("Dashboard", "SuperAdmin");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View(model);
        }
    }
}
