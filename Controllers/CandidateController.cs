using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ErJobPortal.Controllers
{
    public class CandidateController : Controller
    {
        private readonly CandidateProfileRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly AccountRepository _repository;

        public CandidateController(IConfiguration configuration, AccountRepository repository, CandidateProfileRepository repo)
        {
            _configuration = configuration;
            _repository = repository;
            _repo = repo;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard()
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            ViewBag.CandidateID = candidateId;
            ViewBag.CandidateName = HttpContext.Session.GetString("CandidateName");
            ViewBag.CandidateEmail = HttpContext.Session.GetString("CandidateEmail");
            int organizationRegistrationCount = 0;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT (SELECT COUNT(nID)  FROM tblOrgRegistration) AS OrganizationCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            organizationRegistrationCount = Convert.ToInt32(dr["OrganizationCount"]);
                        }
                    }
                }

            }

            // Get complete trainee list
            List<OrganizationUser> organizations = _repository.GetAllOrganizationList();
            ViewBag.OrganizationRegistrationCount = organizationRegistrationCount;
            ViewBag.Organizations = organizations;
            return View();
        }


        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        [HttpGet]
        public IActionResult OrgList()
        {
            List<OrganizationUser> organization = _repository.GetAllOrganizationList();
            return View(organization);
        }


        // =========================================================
        // GET PROFILE
        // =========================================================

        [HttpGet]
        public IActionResult Profile()
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            CandidateProfileModel? profile = _repo.GetProfile(candidateId.Value);

            if (profile == null)
            {
                profile = new CandidateProfileModel
                {
                    CandidateID = candidateId.Value
                };
            }

            CandidateProfileViewModel vm = LoadProfileDropdowns(profile);

            return View(vm);
        }


        // =========================================================
        // LOAD ALL DROPDOWNS
        // =========================================================

        private CandidateProfileViewModel LoadProfileDropdowns(CandidateProfileModel profile)
        {
            return new CandidateProfileViewModel
            {
                Profile = profile,

                Divisions = _repo.GetDivisions(),
                Streams = _repo.GetStreams(),
                GraduationStatuses = _repo.GetGraduationStatuses(),
                InternshipFellowshipType = _repo.GetInternshipFellowshipType(),
                InternshipTitles = _repo.GetInternshipTitles(),
                InternshipDurations = _repo.GetInternshipDurations(),
                InternshipStatuses = _repo.GetInternshipStatuses(),
                Relationships = _repo.GetRelationships()
            };
        }


        // =====================================================
        // UPDATE ADDRESS
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            _repo.UpdateAddress(
                candidateId.Value,
                model.CountryID,
                model.StateID,
                model.CityID,
                model.Pincode
            );

            TempData["Success"] = "Address updated successfully.";

            return RedirectToAction("Profile");
        }



        // =====================================================
        // UPDATE EDUCATION
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEducation(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateEducation(model);

            TempData["Success"] = "Education updated successfully.";

            return RedirectToAction("Profile");
        }


        // =====================================================
        // UPDATE INTERNSHIP / FELLOWSHIP PREFERENCE
        // =====================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateInternshipPreference(CandidateProfileModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    _repo.UpdateInternshipPreference(
        //        candidateId.Value,
        //        model.Internship_FellowshipType,
        //        model.Preferred_Country,
        //        model.Preferred_State,
        //        model.Preferred_City
        //    );

        //    TempData["Success"] = "Internship / Fellowship Preference updated successfully.";

        //    return RedirectToAction("Profile");
        //}

        // =====================================================
        // UPDATE DOCUMENTS
        // =====================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateDocuments(CandidateProfileModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    _repo.UpdateDocuments(
        //        candidateId.Value,
        //        model.sResume,
        //        model.sPhoto,
        //        model.sSignature,
        //        model.sDivyang,
        //        model.sHobbies
        //    );

        //    TempData["Success"] = "Documents updated successfully.";

        //    return RedirectToAction("Profile");
        //}


        // =========================================================
        // UPDATE INTERNSHIP DETAILS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInternshipDetails(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateInternshipDetails(model);

            TempData["Success"] = "Internship details updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE LANGUAGES
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLanguages(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateLanguages(model);

            TempData["Success"] = "Languages updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE REFERENCES
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateReferences(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateReferences(model);

            TempData["Success"] = "References updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE ACHIEVEMENTS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAchievements(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateAchievements(model);

            TempData["Success"] = "Achievements updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE LINKS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLinks(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateLinks(model);

            TempData["Success"] = "Links updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE OBJECTIVE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateObjective(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateObjective(model);

            TempData["Success"] = "Career objective updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE SKILLS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSkills(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateSkills(model);

            TempData["Success"] = "Skills updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE DOCUMENTS / HOBBIES
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDocuments(CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateDocuments(model);

            TempData["Success"] = "Documents and personal details updated successfully.";

            return RedirectToAction("Profile");
        }

        // =========================================================
        // UPDATE INTERNSHIP / FELLOWSHIP PREFERENCE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInternshipPreference(CandidateProfileViewModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // Never trust CandidateID from form
            model.Profile.CandidateID = candidateId.Value;

            _repo.UpdateInternshipPreference(model.Profile);

            TempData["Success"] =
                "Internship Preference Updated Successfully.";

            return RedirectToAction("Profile");
        }

        // =========================================================
        // OTHER PAGES
        // =========================================================

        public IActionResult SearchJobs()
        {
            return View();
        }

        public IActionResult MyApplications()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            // Delete session cookie
            Response.Cookies.Delete(".AspNetCore.Session");
            // Prevent browser from caching the previous page
            SetNoCacheHeaders();
            // Go to Home page
            return RedirectToAction("Index", "Home");
        }


        // =========================================================
        // NO CACHE
        // =========================================================

        private void SetNoCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }
    }
}