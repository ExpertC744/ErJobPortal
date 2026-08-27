using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace ErJobPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;

        public AccountController(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Candidate Registration
        [HttpGet]
        public IActionResult CandidateRegister()
        {
            return View();
        }

        // ==========================================
        // GET DEPARTMENTS
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateDepartments()
        {
            var departments =
                _accountRepository.GetDepartments();

            return Json(departments);
        }


        // ==========================================
        // GET BRANCHES BY DEPARTMENT
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateBranches(int departmentId)
        {
            var branches =
                _accountRepository.GetBranches(departmentId);

            return Json(branches);
        }


        // ==========================================
        // COLLEGE
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateColleges()
        {
            var colleges =
                _accountRepository.GetColleges();

            return Json(colleges);
        }


        // ==========================================
        // COLLEGE CODE BY COLLEGE
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateCollegeCodes(int collegeId)
        {
            var codes =
                _accountRepository.GetCollegeCodes(collegeId);

            return Json(codes);
        }



        // ==============================
        // CANDIDATE REGISTER - POST
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateRegister(CandidateRegister model)
        {
            // STATIC OTP FOR NOW
            //if (model.sOTP != "123456")
            //{
            //    ModelState.AddModelError(
            //        "sOTP",
            //        "Invalid OTP. Please enter 123456."
            //    );
            //}

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int result = _accountRepository.Register(model);

                if (result > 0)
                {
                    TempData["Success"] =
                        "Registration successful. Please login.";

                    return RedirectToAction("CandidateLogin");
                }

                ModelState.AddModelError(
                    "",
                    "Registration failed."
                );

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(model);
            }
        }



        // Organization Registration
        [HttpGet]
        public IActionResult OrganizationRegister()
        {
            OrganizationRegister model = new OrganizationRegister();
            return View();
        }

        // ==========================================
        // GET DEPARTMENTS BY COLLEGE
        // ==========================================

        [HttpGet]
        public IActionResult GetDepartments()
        {
            var departments =
                _accountRepository.GetDepartments();

            return Json(departments);
        }

        [HttpGet]
        public IActionResult GetBranches(int departmentId)
        {
            var branches =
                _accountRepository.GetBranches(departmentId);

            return Json(branches);
        }

        #region "College"

        [HttpGet]
        public IActionResult GetColleges()
        {
            var colleges = _accountRepository.GetColleges();

            return Json(colleges);
        }

        #endregion


        #region "College Code"

        [HttpGet]
        public IActionResult GetCollegeCodes(int collegeId)
        {
            var codes = _accountRepository.GetCollegeCodes(collegeId);

            return Json(codes);
        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationRegister(OrganizationRegister model)
        {
            try
            {
                // STATIC OTP FOR NOW
                if (model.sOTP != "123456")
                {
                    ModelState.AddModelError(
                        "sOTP",
                        "Invalid OTP. Please enter 123456."
                    );
                }

                // Check validation errors
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState)
                    {
                        foreach (var error in item.Value.Errors)
                        {
                            Console.WriteLine(
                                $"Field: {item.Key}, Error: {error.ErrorMessage}"
                            );
                        }
                    }

                    return View(model);
                }

                int result = _accountRepository.RegisterOrganization(model);

                if (result > 0)
                {
                    TempData["Success"] =
                        "Organization Registration Successfully.";

                    return RedirectToAction("OrganizationRegister");
                }

                TempData["Error"] =
                    "Organization Registration Failed.";

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Organization Register Error:");
                Console.WriteLine(ex.ToString());

                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult OrganizationLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OrganizationLogin(OrganizationLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OrganizationUser? user = _accountRepository.LoginOrganization(model);

            if (user != null)
            {
                HttpContext.Session.SetInt32("OrgID", user.nID);
                HttpContext.Session.SetString("OrgName", user.sOrgName ?? "");
                HttpContext.Session.SetString("OrgEmail", user.sEmail ?? "");
                return RedirectToAction("Dashboard", "Organization");
            }

            TempData["Error"] = "Invalid Email or Password";
            return View(model);
        }

        // candidate login

        [HttpGet]
        public IActionResult CandidateLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CandidateLogin(CandidateLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CandidateUser? user =
                _accountRepository.LoginCandidate(model);

            if (user != null)
            {
                HttpContext.Session.SetInt32(
                    "CandidateID",
                    user.nID);

                HttpContext.Session.SetString(
                    "CandidateName",
                    (user.sFName + " " + user.sLName).Trim());

                HttpContext.Session.SetString(
                    "CandidateEmail",
                    user.sEmail ?? "");

                return RedirectToAction(
                    "Dashboard",
                    "Candidate");
            }

            TempData["Error"] =
                "Invalid Email or Password";

            return View(model);
        }

        // ================= LOGOUT =================
        [HttpGet]
        public IActionResult Logout()
        {
            // Remove login session
            HttpContext.Session.Clear();

            // Remove session cookie
            Response.Cookies.Delete(".AspNetCore.Session");

            // Prevent cached pages
            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Go to Home/Index
            return RedirectToAction("Index", "Home");
        }
    }
}