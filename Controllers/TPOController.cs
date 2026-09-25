using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class TPOController : Controller
    {
        // =====================================================
        // TPO DASHBOARD
        // =====================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            // Check whether TPO is logged in
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account"
                );
            }

            // TPO is logged in
            return View();
        }
    }
}