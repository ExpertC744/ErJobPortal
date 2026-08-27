using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

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

        public IActionResult EditProfile()
        {

            return View();
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
        public IActionResult UpdateAddress(CandidateProfileViewModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account"
                );
            }

            _repo.UpdateAddress(
                candidateId.Value,
                model.Profile.CountryID,
                model.Profile.StateID,
                model.Profile.CityID,
                model.Profile.Pincode
            );
            TempData["Success"] = "Address updated successfully.";
            return RedirectToAction("Profile");
        }
        // =====================================================
        // UPDATE EDUCATION
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEducation([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateInternshipPreference([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateInternshipPreference(model);

            TempData["Success"] =
                "Internship / Fellowship Preference updated successfully.";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLanguages([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateReferences([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateAchievements([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateLinks([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateSkills([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateDocuments([Bind(Prefix = "Profile")] CandidateProfileModel model)
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
        public IActionResult UpdateInternshipPreference([Bind(Prefix = "Profile")] CandidateProfileViewModel model)
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



        // =========================================================
        // CANDIDATE CREATE FEEDBACK - GET
        // =========================================================
        [HttpGet]
        public IActionResult CreateFeedback()
        {
            // =====================================================
            // GET CANDIDATE ID
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            CandidateFeedbackViewModel feedback = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT
                nID,
                Que1,
                Que2,
                Que3,
                Que4,
                Que5,
                nBit,
                nSABit
            FROM tblSATRFeedback
            WHERE nID = 1
              AND ISNULL(nBit, 1) = 1";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            feedback = new CandidateFeedbackViewModel
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] != DBNull.Value
                                    ? dr["Que1"].ToString()
                                    : "",

                                Que2 = dr["Que2"] != DBNull.Value
                                    ? dr["Que2"].ToString()
                                    : "",

                                Que3 = dr["Que3"] != DBNull.Value
                                    ? dr["Que3"].ToString()
                                    : "",

                                Que4 = dr["Que4"] != DBNull.Value
                                    ? dr["Que4"].ToString()
                                    : "",

                                Que5 = dr["Que5"] != DBNull.Value
                                    ? dr["Que5"].ToString()
                                    : "",

                                sQue1 = "",
                                sQue2 = "",
                                sQue3 = "",
                                sQue4 = "",
                                sQue5 = "",

                                nCandidateID = candidateId.Value,

                                nBit = dr["nBit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nBit"])
                                    : true,

                                nSABit = dr["nSABit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nSABit"])
                                    : true
                            };
                        }
                    }
                }
            }

            if (feedback == null)
            {
                return NotFound("Feedback questions not found.");
            }

            return View(feedback);
        }



        // =========================================================
        // CANDIDATE FEEDBACK - CREATE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateFeedback(
            CandidateFeedbackViewModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                TempData["Error"] =
                    "Candidate session expired. Please login again.";

                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrEmpty(model.sQue1))
                ModelState.AddModelError(
                    "sQue1",
                    "Please select an answer.");

            if (string.IsNullOrEmpty(model.sQue2))
                ModelState.AddModelError(
                    "sQue2",
                    "Please select a rating.");

            if (string.IsNullOrEmpty(model.sQue3))
                ModelState.AddModelError(
                    "sQue3",
                    "Please select an answer.");

            if (string.IsNullOrEmpty(model.sQue4))
                ModelState.AddModelError(
                    "sQue4",
                    "Please select an emoji.");

            if (!ModelState.IsValid)
            {
                // IMPORTANT:
                // model is now CandidateFeedbackViewModel,
                // same type required by the View.
                return View(model);
            }

            // =====================================================
            // QUESTION 1
            // =====================================================

            int sQue1;

            if (model.sQue1 == "True")
            {
                sQue1 = 1;
            }
            else if (model.sQue1 == "False")
            {
                sQue1 = 0;
            }
            else
            {
                ModelState.AddModelError(
                    "sQue1",
                    "Invalid answer.");

                return View(model);
            }

            // =====================================================
            // QUESTION 2
            // =====================================================

            if (!int.TryParse(
                model.sQue2,
                out int sQue2))
            {
                ModelState.AddModelError(
                    "sQue2",
                    "Invalid rating.");

                return View(model);
            }

            // =====================================================
            // QUESTION 3
            // =====================================================

            int sQue3;

            if (model.sQue3 == "Yes")
            {
                sQue3 = 1;
            }
            else if (model.sQue3 == "No")
            {
                sQue3 = 0;
            }
            else
            {
                ModelState.AddModelError(
                    "sQue3",
                    "Invalid answer.");

                return View(model);
            }

            // =====================================================
            // QUESTION 4
            // =====================================================

            if (!int.TryParse(
                model.sQue4,
                out int sQue4))
            {
                ModelState.AddModelError(
                    "sQue4",
                    "Invalid emoji rating.");

                return View(model);
            }

            // =====================================================
            // QUESTION 5
            // =====================================================

            string sQue5 =
                model.sQue5 ?? "";

            // =====================================================
            // DATABASE
            // =====================================================

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO tblCandidateFeedback
            (
                sQue1,
                sQue2,
                sQue3,
                sQue4,
                sQue5,
                nAdminID,
                RegDate,
                ModDate,
                nBit,
                nSABit
            )
            VALUES
            (
                @sQue1,
                @sQue2,
                @sQue3,
                @sQue4,
                @sQue5,
                @nAdminID,
                GETDATE(),
                GETDATE(),
                1,
                0
            )";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@sQue1",
                        SqlDbType.Int).Value = sQue1;

                    cmd.Parameters.Add(
                        "@sQue2",
                        SqlDbType.Int).Value = sQue2;

                    cmd.Parameters.Add(
                        "@sQue3",
                        SqlDbType.Int).Value = sQue3;

                    cmd.Parameters.Add(
                        "@sQue4",
                        SqlDbType.Int).Value = sQue4;

                    cmd.Parameters.Add(
                        "@sQue5",
                        SqlDbType.NVarChar,
                        200).Value =
                            string.IsNullOrWhiteSpace(sQue5)
                            ? DBNull.Value
                            : sQue5;

                    // Candidate ID
                    cmd.Parameters.Add(
                        "@nAdminID",
                        SqlDbType.Int).Value =
                            candidateId.Value;

                    con.Open();

                    int rows =
                        cmd.ExecuteNonQuery();

                    if (rows <= 0)
                    {
                        TempData["Error"] =
                            "Feedback was not saved.";

                        return View(model);
                    }
                }
            }

            TempData["Success"] =
                "Feedback submitted successfully.";

            return RedirectToAction(
                "CreateFeedback");
        }
    }
}