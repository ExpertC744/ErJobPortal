using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class TPOController : Controller
    {
        private readonly AccountRepository _accountRepository;

        public TPOController(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // =====================================================
        // TPO DASHBOARD
        // =====================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account"
                );
            }

            return View();
        }


        // =====================================================
        // VIEW TPO DETAILS
        // =====================================================

        [HttpGet]
        public IActionResult ViewTPODetails()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            TPORegistration? tpo =
                _accountRepository.GetTPODetails(tpoId.Value);

            if (tpo == null)
            {
                TempData["Error"] = "TPO details not found.";
                return RedirectToAction("Dashboard");
            }

            return View(tpo);
        }

        [HttpGet]
        public IActionResult AddSubTPO()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account"
                );
            }

            return View();
        }

        // =====================================================
        // CREATE FEEDBACK - STATIC UI
        // =====================================================

        [HttpGet]
        public IActionResult CreateFeedback()
        {
            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account"
                );
            }

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName")
                ?? "TPO";

            ViewData["Title"] = "Create Feedback";

            return View();
        }

    }
}