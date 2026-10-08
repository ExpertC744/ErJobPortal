using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

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


        private string? GetCandidateCode()
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
                return null;

            var candidate = _repository.GetCandidateRegistrationDetails(candidateId.Value);

            if (candidate == null || candidate.Value.DOB == null || candidate.Value.RegDate == null)
            {
                return null;
            }

            string candidateCode = "CD" + candidate.Value.DOB.Value.ToString("ddMMyy") + candidate.Value.RegDate.Value.ToString("MMdd") + candidate.Value.CandidateID.ToString("D2");

            return candidateCode;
        }

        // shrirang 01/10/26
        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard(string? id)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // =========================================================
            // GET CANDIDATE REGISTRATION DETAILS
            // =========================================================

            var candidate = _repository.GetCandidateRegistrationDetails(candidateId.Value);

            if (candidate == null)
            {
                return NotFound("Candidate registration not found.");
            }

            // =========================================================
            // CHECK DOB AND REGISTRATION DATE
            // =========================================================

            if (candidate.Value.DOB == null ||
                candidate.Value.RegDate == null)
            {
                return BadRequest("Candidate DOB or Registration Date is missing.");
            }

            // =========================================================
            // CREATE CANDIDATE CODE
            // =========================================================
            //
            // CD
            // + DOB ddMMyy
            // + Registration Date MMdd
            // + Candidate ID 2 digits
            //
            // Example:
            // DOB      = 08/05/1999
            // RegDate  = 26/07/2026
            // nID      = 1
            //
            // CD080599072601
            // =========================================================

            string candidateCode = "CD" +
                candidate.Value.DOB.Value.ToString("ddMMyy") + 
                candidate.Value.CandidateID.ToString("D2");

            // =========================================================
            // IF NORMAL URL IS OPENED
            // REDIRECT TO CODE URL
            // =========================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Dashboard", "Candidate", new { id = candidateCode });
            }

            // =========================================================
            // OPTIONAL: CHECK THAT URL CODE BELONGS TO LOGGED-IN
            // CANDIDATE
            // =========================================================

            if (id != candidateCode)
            {
                return NotFound();
            }

            // =========================================================
            // EXISTING DASHBOARD CODE
            // =========================================================

            ViewBag.CandidateID = candidateId;

            ViewBag.CandidateName = HttpContext.Session.GetString("CandidateName");

            ViewBag.CandidateEmail = HttpContext.Session.GetString("CandidateEmail");

            ViewBag.CandidateCode = candidateCode;

            // =========================================================
            // CHECK WHETHER CANDIDATE HAS ENTERED ANY PROFILE DATA
            // =========================================================

            CandidateProfileModel? candidateProfile = _repo.GetProfile(candidateId.Value);
            ViewBag.HasCandidateProfile = HasCandidateProfileData(candidateProfile);
            //end
            // =========================================================
            // ORGANIZATION COUNT
            // =========================================================
            int organizationRegistrationCount = 0;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT COUNT(nID) FROM tblOrgRegistration;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    organizationRegistrationCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // =========================================================
            // ORGANIZATION LIST
            // =========================================================

            List<OrganizationUser> organizations = _repository.GetAllOrganizationList();
            ViewBag.OrganizationRegistrationCount = organizationRegistrationCount;
            ViewBag.Organizations = organizations;

            // =========================================================
            // MATCHING INTERNSHIP POSTS
            // =========================================================
            List<OrgPostM> internshipPosts = GetMatchingJobsForCandidate(candidateId.Value);
            ViewBag.InternshipPosts = internshipPosts;

            // =========================================================
            // TRAINEE APPLIED POSTS
            // =========================================================
            List<OrgPostM> traineeAppliedPosts = TraineeApplyToPostList(candidateId.Value);
            ViewBag.TraineeAppliedPosts = traineeAppliedPosts;



            List<OrgPostM> matchingJobs = GetMatchingJobsForCandidate(candidateId.Value);


            /// =========================================================
            // CHART DATA
            // =========================================================

            SAChartsViewModel chartModel = new SAChartsViewModel();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand("SP_GetSACharts", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        // =====================================================
                        // RESULT SET 1 - CANDIDATE DATA
                        // =====================================================

                        while (reader.Read())
                        {
                            chartModel.CandidateData.Add(
                                new SAChartData
                                {
                                    Year = Convert.ToInt32(reader["Year"]),
                                    Month = Convert.ToInt32(reader["Month"]),
                                    TotalCount = Convert.ToInt32(reader["TotalCount"])
                                });
                        }

                        // =====================================================
                        // RESULT SET 2 - INTERNSHIP POST DATA
                        // =====================================================

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                chartModel.InternshipPostData.Add(
                                    new SAChartData
                                    {
                                        Year = Convert.ToInt32(reader["Year"]),
                                        Month = Convert.ToInt32(reader["Month"]),
                                        TotalCount = Convert.ToInt32(reader["TotalCount"])
                               });
                            }
                        }
                    }
                }
            }


            // =========================================================
            // MONTHLY SUITABLE OPENINGS
            // =========================================================

            int currentYear = DateTime.Now.Year;

            int[] suitableOpeningsMonthly = new int[12];

            foreach (var job in matchingJobs)
            {
                if (!job.dStartDate.HasValue)
                    continue;

                DateTime startDate = job.dStartDate.Value;

                if (startDate.Year != currentYear)
                    continue;

                int monthIndex = startDate.Month - 1;

                int requiredTrainees = job.nRequiredTrainees;

                if (requiredTrainees < 0)
                    requiredTrainees = 0;

                if (monthIndex >= 0 && monthIndex < 12)
                {
                    suitableOpeningsMonthly[monthIndex] +=
                        requiredTrainees;
                }
            }


            // =========================================================
            // SEND CHART DATA TO DASHBOARD VIEW
            // =========================================================

            ViewBag.ChartYear = currentYear;

            ViewBag.SuitableOpeningsMonthly =
                suitableOpeningsMonthly;

            return View(chartModel);
        }

        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        // =============================================================
        // GET MATCHING INTERNSHIP POSTS FOR CANDIDATE
        // =============================================================
        //[HttpGet]
        //private List<OrgPostM> GetMatchingJobsForCandidate(int candidateId)
        //{
        //    List<OrgPostM> posts = new List<OrgPostM>();
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("GetMatchingJobsForCandidate", con))
        //        {
        //            // =====================================================
        //            // STORED PROCEDURE
        //            // =====================================================

        //            cmd.CommandType = CommandType.StoredProcedure;

        //            // =====================================================
        //            // CANDIDATE ID
        //            // =====================================================

        //            cmd.Parameters.Add("@CandidateID", SqlDbType.Int).Value = candidateId;
        //            con.Open();
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    OrgPostM post = new OrgPostM();

        //                    // =================================================
        //                    // POST ID
        //                    // SP: fj.nID AS PostID
        //                    // =================================================

        //                    post.nID = dr["PostID"] != DBNull.Value ? Convert.ToInt32(dr["PostID"]) : 0;

        //                    // =================================================
        //                    // ORGANIZATION ID
        //                    // SP: fj.nOrgID AS OrgID
        //                    // =================================================

        //                    post.nOrgID = dr["OrgID"] != DBNull.Value ? Convert.ToInt32(dr["OrgID"]) : 0;
        //                    post.sName = dr["OrganizationName"] != DBNull.Value ? dr["OrganizationName"].ToString()! : "";

        //                    // =================================================
        //                    // POSITION ID
        //                    // SP: fj.nPositionID AS PositionID
        //                    // =================================================

        //                    post.nPositionID =
        //                        dr["PositionID"] != DBNull.Value
        //                        ? Convert.ToInt32(
        //                            dr["PositionID"])
        //                        : 0;


        //                    // =================================================
        //                    // POSITION NAME
        //                    // SP: fj.PositionName
        //                    // =================================================

        //                    post.sPositionName =
        //                        dr["PositionName"] != DBNull.Value
        //                        ? dr["PositionName"]
        //                            .ToString()!
        //                        : "";


        //                    // =================================================
        //                    // COUNTRY
        //                    // SP: fj.sCountryName AS Country
        //                    // =================================================

        //                    post.sCountryName =
        //                        dr["Country"] != DBNull.Value
        //                        ? dr["Country"]
        //                            .ToString()!
        //                        : "";


        //                    // =================================================
        //                    // STATE
        //                    // SP: fj.sStateName AS State
        //                    // =================================================

        //                    post.sStateName =
        //                        dr["State"] != DBNull.Value
        //                        ? dr["State"]
        //                            .ToString()!
        //                        : "";


        //                    // =================================================
        //                    // CITY
        //                    // SP: fj.nCityName AS City
        //                    // =================================================

        //                    post.nCityName =
        //                        dr["City"] != DBNull.Value
        //                        ? dr["City"]
        //                            .ToString()!
        //                        : "";


        //                    // =================================================
        //                    // FELLOWSHIP TYPE ID
        //                    // SP: InternshipFellowshipType
        //                    // =================================================

        //                    // =============================================================
        //                    // FELLOWSHIP TYPE ID
        //                    // =============================================================

        //                    post.nInternshipFellowshipTypeID =
        //                        dr["InternshipFellowshipTypeID"] != DBNull.Value
        //                        ? Convert.ToInt32(
        //                            dr["InternshipFellowshipTypeID"])
        //                        : 0;


        //                    // =============================================================
        //                    // FELLOWSHIP TYPE NAME
        //                    // =============================================================

        //                    post.sInternshipFellowshipTypeName = dr["InternshipFellowshipType"] != DBNull.Value ? dr["InternshipFellowshipType"].ToString()! : "";
        //                    // =================================================
        //                    // FACILITIES
        //                    // SP: fj.sFacilities
        //                    // =================================================

        //                    post.sFacilities =
        //                        dr["sFacilities"] != DBNull.Value
        //                        ? dr["sFacilities"]
        //                            .ToString()!
        //                        : "";


        //                    // =================================================
        //                    // APPLICATION STATUS
        //                    // SP: ISNULL(jas.CApply, 0) AS CApply
        //                    // =================================================

        //                    post.IsApplied = dr["CApply"] != DBNull.Value && Convert.ToInt32(dr["CApply"]) == 1;

        //                    // =================================================
        //                    // ADD TO LIST
        //                    // =================================================

        //                    posts.Add(post);
        //                }
        //            }
        //        }
        //    }

        //    return posts;
        //}

        [HttpGet]
        private List<OrgPostM> GetMatchingJobsForCandidate(int candidateId)
        {
            List<OrgPostM> posts = new List<OrgPostM>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetMatchingJobsForCandidate", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CandidateID", SqlDbType.Int).Value = candidateId;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        OrgPostM post = new OrgPostM();

                        // =============================================
                        // POST
                        // =============================================

                        post.nID =
                            dr["PostID"] != DBNull.Value
                            ? Convert.ToInt32(dr["PostID"])
                            : 0;


                        // =============================================
                        // ORGANIZATION
                        // =============================================

                        post.nOrgID =
                            dr["OrgID"] != DBNull.Value
                            ? Convert.ToInt32(dr["OrgID"])
                            : 0;

                        post.sName =
                            dr["OrganizationName"] != DBNull.Value
                            ? dr["OrganizationName"].ToString()!
                            : "";

                        // =============================================
                        // POSITION
                        // =============================================

                        post.nPositionID =
                            dr["PositionID"] != DBNull.Value
                            ? Convert.ToInt32(dr["PositionID"])
                            : 0;

                        post.sPositionName =
                            dr["PositionName"] != DBNull.Value
                            ? dr["PositionName"].ToString()!
                            : "";

                        // =============================================
                        // LOCATION
                        // =============================================

                        post.sCountryName =
                            dr["Country"] != DBNull.Value
                            ? dr["Country"].ToString()!
                            : "";

                        post.sStateName =
                            dr["State"] != DBNull.Value
                            ? dr["State"].ToString()!
                            : "";

                        post.nCityName =
                            dr["City"] != DBNull.Value
                            ? dr["City"].ToString()!
                            : "";

                        // =============================================
                        // INTERNSHIP / FELLOWSHIP TYPE
                        // =============================================

                        post.nInternshipFellowshipTypeID =
                            dr["InternshipFellowshipTypeID"] != DBNull.Value
                            ? Convert.ToInt32(
                                dr["InternshipFellowshipTypeID"])
                            : 0;

                        post.sInternshipFellowshipTypeName =
                            dr["InternshipFellowshipType"] != DBNull.Value
                            ? dr["InternshipFellowshipType"].ToString()!
                            : "";
                        // =============================================
                        // REQUIRED TRAINEES
                        // =============================================

                        post.nRequiredTrainees =
                            dr["nRequiredTrainees"] != DBNull.Value
                            ? Convert.ToInt32(dr["nRequiredTrainees"])
                            : 0;


                        // =============================================
                        // START DATE
                        // =============================================

                        post.dStartDate =
                            dr["dStartDate"] != DBNull.Value
                            ? Convert.ToDateTime(dr["dStartDate"])
                            : (DateTime?)null;
                        // =============================================
                        // FACILITIES
                        // =============================================

                        post.sFacilities =
                            dr["sFacilities"] != DBNull.Value
                            ? dr["sFacilities"].ToString()!
                            : "";

                        // =============================================
                        // APPLICATION STATUS
                        // =============================================

                        post.IsApplied =
                            dr["CApply"] != DBNull.Value &&
                            Convert.ToInt32(dr["CApply"]) == 1;

                        // =============================================
                        // ADD
                        // =============================================

                        posts.Add(post);
                    }
                }
            }

            return posts;
        }


        [Route("Candidate/OrgList/{id?}")]
        public IActionResult OrgList(string? id)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            string? candidateCode = GetCandidateCode();
            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound("Candidate code could not be generated.");
            }
            // If URL does not contain candidate code, add it
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("OrgList", "Candidate", new { id = candidateCode });
            }

            // Prevent another/wrong candidate code
            if (id != candidateCode)
            {
                return NotFound();
            }

            List<OrganizationUser> organization = _repository.GetAllOrganizationList();

            ViewBag.CandidateCode = candidateCode;
            ViewBag.CandidateID = candidateId.Value;

            return View(organization);
        }

        [HttpGet]
        [Route("Candidate/EditProfile/{id?}")]
        public IActionResult EditProfile(string? id)
        {
            // =========================================================
            // GET CANDIDATE ID FROM SESSION
            // =========================================================
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // =========================================================
            // GET CANDIDATE CODE
            // =========================================================
            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =========================================================
            // IF URL DOES NOT HAVE ID
            // REDIRECT TO CODE URL
            // =========================================================
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "EditProfile",
                    "Candidate",
                    new { id = candidateCode });
            }

            // =========================================================
            // VALIDATE URL ID
            // =========================================================
            if (id != candidateCode)
            {
                return NotFound();
            }

            // =========================================================
            // SEND CODE TO VIEW / LAYOUT
            // =========================================================
            ViewBag.CandidateCode = candidateCode;
            ViewBag.CandidateID = candidateId.Value;

            return View();
        }

        // =========================================================
        // GET PROFILE
        // =========================================================
        [HttpGet]
        public IActionResult Profile(string id)
        {
            // =========================================================
            // GET CANDIDATE ID FROM SESSION
            // =========================================================
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =========================================================
            // GET CANDIDATE CODE
            // =========================================================
            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =========================================================
            // IF URL DOES NOT CONTAIN ID
            // REDIRECT TO CODE URL
            // =========================================================
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "Profile",
                    "Candidate",
                    new { id = candidateCode });
            }

            // =========================================================
            // VALIDATE CANDIDATE CODE
            // =========================================================
            if (id != candidateCode)
            {
                return NotFound();
            }

            // =========================================================
            // GET PROFILE
            // =========================================================
            CandidateProfileModel? profile =
                _repo.GetProfile(candidateId.Value);

            if (profile == null)
            {
                profile = new CandidateProfileModel
                {
                    CandidateID = candidateId.Value
                };
            }

            // =========================================================
            // LOAD DROPDOWNS
            // =========================================================
            CandidateProfileViewModel vm =
                LoadProfileDropdowns(profile);

            // =========================================================
            // SEND CODE TO VIEW / LAYOUT
            // =========================================================
            ViewBag.CandidateCode = candidateCode;

            return View(vm);
        }

        // shrirang 30-09-26
        // =========================================================
        // LOAD ALL DROPDOWNS
        // =========================================================

        private CandidateProfileViewModel LoadProfileDropdowns(CandidateProfileModel profile)
        {
            return new CandidateProfileViewModel
            {
                Profile = profile,

                // =====================================================
                // EDUCATION
                // =====================================================

                Divisions = _repo.GetDivisions(),

                Streams = _repo.GetStreams(),

                GraduationStatuses =
                    _repo.GetGraduationStatuses(),


                // =====================================================
                // INTERNSHIP / FELLOWSHIP
                // =====================================================

                InternshipFellowshipType =
                    _repo.GetInternshipFellowshipType(),

                InternshipTitles =
                    _repo.GetInternshipTitles(),

                InternshipDurations =
                    _repo.GetInternshipDurations(),

                InternshipStatuses =
                    _repo.GetInternshipStatuses(),


                // =====================================================
                // REFERENCES
                // =====================================================

                Relationships =
                    _repo.GetRelationships(),


                // =====================================================
                // SKILLS
                // =====================================================

                MedicalSkills =
                    _repo.GetMedicalSkills(),

                TechnicalSkills =
                    _repo.GetTechnicalSkills(),

                NonTechnicalSkills =
                    _repo.GetNonTechnicalSkills()
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

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public IActionResult UpdateInternshipPreference(
        //[Bind(Prefix = "Profile")] CandidateProfileModel model)
        //    {
        //        int? candidateId =
        //            HttpContext.Session.GetInt32("CandidateID");

        //        if (candidateId == null)
        //        {
        //            return RedirectToAction(
        //                "CandidateLogin",
        //                "Account"
        //            );
        //        }

        //        model.CandidateID = candidateId.Value;

        //        _repo.UpdateInternshipPreference(model);

        //        TempData["Success"] =
        //            "Internship / Fellowship Preference updated successfully.";

        //        return RedirectToAction("Profile");
        //    }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateLanguages([Bind(Prefix = "Profile")] CandidateProfileModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    model.CandidateID = candidateId.Value;

        //    _repo.UpdateLanguages(model);

        //    TempData["Success"] = "Languages updated successfully.";

        //    return RedirectToAction("Profile");
        //}


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

        // shrirang 24/09/26
        // =========================================================
        // UPDATE OBJECTIVE
        // =========================================================

        // =========================================================
        // UPDATE CAREER OBJECTIVE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateObjective([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            // =====================================================
            // CHECK LOGIN
            // =====================================================

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // ALWAYS USE LOGGED-IN CANDIDATE ID
            // Do not trust CandidateID coming from the browser.
            // =====================================================

            model.CandidateID = candidateId.Value;

            // =====================================================
            // VALIDATION
            // =====================================================

            if (!string.IsNullOrWhiteSpace(model.Objective) &&
                model.Objective.Trim().Length > 1000)
            {
                TempData["Error"] =
                    "Career Objective cannot exceed 1000 characters.";

                return RedirectToAction("Profile");
            }

            // =====================================================
            // UPDATE DATABASE
            // =====================================================

            _repo.UpdateObjective(model);

            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================

            TempData["Success"] =
                "Career objective updated successfully.";

            // =====================================================
            // REDIRECT TO PROFILE
            // Profile() will call GetProfile() again and load
            // the newly saved Objective.
            // =====================================================

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateDocuments([Bind(Prefix = "Profile")] CandidateProfileModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    model.CandidateID = candidateId.Value;

        //    _repo.UpdateDocuments(model);

        //    TempData["Success"] = "Documents and personal details updated successfully.";

        //    return RedirectToAction("Profile");
        //}

        //// =========================================================
        //// UPDATE INTERNSHIP / FELLOWSHIP PREFERENCE
        //// =========================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateInternshipPreference([Bind(Prefix = "Profile")] CandidateProfileViewModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    // Never trust CandidateID from form
        //    model.Profile.CandidateID = candidateId.Value;

        //    _repo.UpdateInternshipPreference(model.Profile);

        //    TempData["Success"] =
        //        "Internship Preference Updated Successfully.";

        //    return RedirectToAction("Profile");
        //}

        // =========================================================
        // SEARCH JOBS
        // =========================================================
        [HttpGet]
        [Route("Candidate/SearchJobs/{id?}")]
        public IActionResult SearchJobs(string? id)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound("Candidate code could not be generated.");
            }

            // If ID is missing, add it to URL
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "SearchJobs",
                    "Candidate",
                    new { id = candidateCode });
            }

            // Validate ID
            if (id != candidateCode)
            {
                return NotFound();
            }

            ViewBag.CandidateCode = candidateCode;
            ViewBag.CandidateID = candidateId.Value;

            return View();
        }


        // =========================================================
        // MY APPLICATIONS
        // =========================================================
        [HttpGet]
        [Route("Candidate/MyApplications/{id?}")]
        public IActionResult MyApplications(string? id)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound("Candidate code could not be generated.");
            }

            // If ID is missing, add it to URL
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "MyApplications",
                    "Candidate",
                    new { id = candidateCode });
            }

            // Validate ID
            if (id != candidateCode)
            {
                return NotFound();
            }

            ViewBag.CandidateCode = candidateCode;
            ViewBag.CandidateID = candidateId.Value;

            return View();
        }


        // =========================================================
        // CHANGE PASSWORD
        // =========================================================
        [HttpGet]
        [Route("Candidate/ChangePassword/{id?}")]
        public IActionResult ChangePassword(string? id)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound("Candidate code could not be generated.");
            }

            // If ID is missing, add it to URL
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "ChangePassword",
                    "Candidate",
                    new { id = candidateCode });
            }

            // Validate ID
            if (id != candidateCode)
            {
                return NotFound();
            }

            ViewBag.CandidateCode = candidateCode;
            ViewBag.CandidateID = candidateId.Value;

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
        public IActionResult CreateFeedback(CandidateFeedbackViewModel model)
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

        // shrirang 27/08/26

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateDocuments([Bind(Prefix = "Profile")] CandidateProfileModel model, IFormFile? ResumeFile, IFormFile? PhotoFile, IFormFile? SignatureFile)
        //{
        //    int? candidateId =
        //        HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null || candidateId <= 0)
        //    {
        //        return RedirectToAction(
        //            "CandidateLogin",
        //            "Account");
        //    }

        //    model.CandidateID = candidateId.Value;

        //    // Get existing profile
        //    CandidateProfileModel? existingProfile =
        //        _repo.GetProfile(candidateId.Value);

        //    if (existingProfile != null)
        //    {
        //        // Keep old files if user does not select a new file
        //        model.sResume = existingProfile.sResume;
        //        model.sPhoto = existingProfile.sPhoto;
        //        model.sSignature = existingProfile.sSignature;
        //    }

        //    // Upload folder
        //    string uploadFolder = Path.Combine(
        //        Directory.GetCurrentDirectory(),
        //        "wwwroot",
        //        "uploads",
        //        "candidates");

        //    if (!Directory.Exists(uploadFolder))
        //    {
        //        Directory.CreateDirectory(uploadFolder);
        //    }


        //    // =====================================================
        //    // RESUME
        //    // =====================================================

        //    if (ResumeFile != null && ResumeFile.Length > 0)
        //    {
        //        string extension =
        //            Path.GetExtension(ResumeFile.FileName)
        //                .ToLowerInvariant();

        //        if (extension != ".pdf" &&
        //            extension != ".doc" &&
        //            extension != ".docx")
        //        {
        //            TempData["Error"] =
        //                "Resume must be PDF, DOC or DOCX.";

        //            return RedirectToAction("Profile");
        //        }

        //        string fileName =
        //            $"{candidateId}_Resume{extension}";

        //        string filePath =
        //            Path.Combine(uploadFolder, fileName);

        //        using (FileStream stream =
        //               new FileStream(filePath, FileMode.Create))
        //        {
        //            ResumeFile.CopyTo(stream);
        //        }

        //        model.sResume =
        //            $"/uploads/candidates/{fileName}";
        //    }


        //    // =====================================================
        //    // PHOTO
        //    // =====================================================

        //    if (PhotoFile != null && PhotoFile.Length > 0)
        //    {
        //        string extension =
        //            Path.GetExtension(PhotoFile.FileName)
        //                .ToLowerInvariant();

        //        if (extension != ".jpg" &&
        //            extension != ".jpeg" &&
        //            extension != ".png")
        //        {
        //            TempData["Error"] =
        //                "Photo must be JPG, JPEG or PNG.";

        //            return RedirectToAction("Profile");
        //        }

        //        string fileName =
        //            $"{candidateId}_Photo{extension}";

        //        string filePath =
        //            Path.Combine(uploadFolder, fileName);

        //        using (FileStream stream =
        //               new FileStream(filePath, FileMode.Create))
        //        {
        //            PhotoFile.CopyTo(stream);
        //        }

        //        model.sPhoto =
        //            $"/uploads/candidates/{fileName}";
        //    }


        //    // =====================================================
        //    // SIGNATURE
        //    // =====================================================

        //    if (SignatureFile != null && SignatureFile.Length > 0)
        //    {
        //        string extension =
        //            Path.GetExtension(SignatureFile.FileName)
        //                .ToLowerInvariant();

        //        if (extension != ".jpg" &&
        //            extension != ".jpeg" &&
        //            extension != ".png")
        //        {
        //            TempData["Error"] =
        //                "Signature must be JPG, JPEG or PNG.";

        //            return RedirectToAction("Profile");
        //        }

        //        string fileName =
        //            $"{candidateId}_Signature{extension}";

        //        string filePath =
        //            Path.Combine(uploadFolder, fileName);

        //        using (FileStream stream =
        //               new FileStream(filePath, FileMode.Create))
        //        {
        //            SignatureFile.CopyTo(stream);
        //        }

        //        model.sSignature =
        //            $"/uploads/candidates/{fileName}";
        //    }


        //    // =====================================================
        //    // UPDATE EVERYTHING TOGETHER
        //    // =====================================================

        //    _repo.UpdateDocuments(model);

        //    TempData["Success"] =
        //        "Personal details updated successfully.";

        //    return RedirectToAction("Profile");
        //}

        // =========================================================
        // UPDATE INTERNSHIP DETAILS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInternshipDetails([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }


            // =====================================================
            // NEVER TRUST CANDIDATE ID FROM FORM
            // =====================================================

            model.CandidateID =
                candidateId.Value;


            // =====================================================
            // UPDATE INTERNSHIP DETAILS
            // =====================================================

            _repo.UpdateInternshipDetails(model);


            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================

            TempData["Success"] =
                "Internship details updated successfully.";


            // =====================================================
            // REDIRECT
            // =====================================================

            return RedirectToAction("Profile");
        }

        // =========================================================
        // UPDATE LANGUAGES
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLanguages([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }


            // =====================================================
            // NEVER TRUST CANDIDATE ID FROM FORM
            // =====================================================

            model.CandidateID = candidateId.Value;


            // =====================================================
            // UPDATE LANGUAGES
            // =====================================================

            _repo.UpdateLanguages(model);


            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================

            TempData["Success"] =
                "Languages updated successfully.";


            // =====================================================
            // REDIRECT
            // =====================================================

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE SOCIAL LINKS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSocialLinks([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            // Get Candidate ID from session
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            // Check login
            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // Never trust CandidateID coming from the form
            model.CandidateID = candidateId.Value;

            // Update GitHub and LinkedIn
            _repo.UpdateSocialLinks(model);

            // Success message
            TempData["Success"] = "Social links updated successfully.";

            // Return to Profile
            return RedirectToAction("Profile");
        }

        // =========================================================
        // VIEW PROFILE (READ-ONLY RESUME VIEW) RADHIKA
        // =========================================================
        // shrirang 19/09/26

        // =========================================================
        // VIEW PROFILE
        // =========================================================
        // shrirang 02/10/26
        // =========================================================
        // VIEW PROFILE
        // Opens candidate's selected/default resume template
        // =========================================================

        [HttpGet]
        [ResponseCache(
            NoStore = true,
            Location = ResponseCacheLocation.None)]
        public IActionResult ViewProfile()
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // REDIRECT TO RESUME CONTROLLER
            // ResumeController.Resume() will read
            // Resume_Profile and open the correct template.
            // =====================================================

            return RedirectToAction(
                "Resume",
                "Resume",
                new
                {
                    id = candidateId.Value
                });
        }

        [HttpGet]
        [Route("Candidate/InternshipDetails/{id?}")]
        public IActionResult InternshipDetails(string? id)
        {
            // id = CD080900080701

            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // =========================================================
            // GET CANDIDATE REGISTRATION DETAILS
            // =========================================================

            var candidate = _repository.GetCandidateRegistrationDetails(candidateId.Value);

            if (candidate == null)
            {
                return NotFound("Candidate registration not found.");
            }

            List<OrgPostM> internshipPosts =
                GetMatchingJobsForCandidate(candidateId.Value);

            ViewBag.InternshipPosts = internshipPosts;

            // =========================================================
            // PASS ID TO VIEW
            // =========================================================

            ViewBag.InternshipId = id;

            return View();
        }

        // shrirang 14/09/26
        // shrirang 12/09/26


        // =========================================================
        // UPDATE INTERNSHIP / FELLOWSHIP PREFERENCE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInternshipPreference([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // NEVER TRUST CANDIDATE ID FROM FORM
            // =====================================================

            model.CandidateID = candidateId.Value;

            // =====================================================
            // VALIDATE INTERNSHIP / FELLOWSHIP TYPE
            // =====================================================

            if (model.Internship_FellowshipType == null)
            {
                ModelState.AddModelError(
                    "Internship_FellowshipType",
                    "Please select Internship / Fellowship Type.");

                // Reload dropdowns because the view needs them
                CandidateProfileViewModel vm =
                    LoadProfileDropdowns(model);

                return View("Profile", vm);
            }

            // =====================================================
            // UPDATE DATABASE
            // =====================================================

            _repo.UpdateInternshipPreference(model);

            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================

            TempData["Success"] =
                "Internship / Fellowship Preference updated successfully.";

            // =====================================================
            // REDIRECT
            // =====================================================

            return RedirectToAction("Profile");
        }

        // =========================================================
        // UPDATE DOCUMENTS / PERSONAL DETAILS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDocuments(
            [Bind(Prefix = "Profile")] CandidateProfileModel model,
            IFormFile? ResumeFile,
            IFormFile? PhotoFile,
            IFormFile? SignatureFile)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // Never trust CandidateID from browser
            model.CandidateID = candidateId.Value;

            // =====================================================
            // GET EXISTING PROFILE
            // =====================================================

            CandidateProfileModel? existingProfile =
                _repo.GetProfile(candidateId.Value);

            if (existingProfile != null)
            {
                // Preserve existing documents
                // if no new file is selected.

                if (string.IsNullOrEmpty(model.sResume))
                {
                    model.sResume = existingProfile.sResume;
                }

                if (string.IsNullOrEmpty(model.sPhoto))
                {
                    model.sPhoto = existingProfile.sPhoto;
                }

                if (string.IsNullOrEmpty(model.sSignature))
                {
                    model.sSignature = existingProfile.sSignature;
                }
            }

            // =====================================================
            // UPLOAD DIRECTORY
            // =====================================================

            string uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "candidates");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // =====================================================
            // RESUME
            // =====================================================

            if (ResumeFile != null &&
                ResumeFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(
                        ResumeFile.FileName)
                        .ToLowerInvariant();

                if (extension != ".pdf" &&
                    extension != ".doc" &&
                    extension != ".docx")
                {
                    TempData["Error"] =
                        "Resume must be PDF, DOC or DOCX.";

                    return RedirectToAction("Profile");
                }

                string fileName =
                    $"{candidateId.Value}_Resume{extension}";

                string filePath =
                    Path.Combine(
                        uploadFolder,
                        fileName);

                using (FileStream stream =
                       new FileStream(
                           filePath,
                           FileMode.Create))
                {
                    ResumeFile.CopyTo(stream);
                }

                model.sResume =
                    $"/uploads/candidates/{fileName}";
            }

            // =====================================================
            // PHOTO
            // =====================================================

            if (PhotoFile != null &&
                PhotoFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(
                        PhotoFile.FileName)
                        .ToLowerInvariant();

                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png")
                {
                    TempData["Error"] =
                        "Photo must be JPG, JPEG or PNG.";

                    return RedirectToAction("Profile");
                }

                string fileName =
                    $"{candidateId.Value}_Photo{extension}";

                string filePath =
                    Path.Combine(
                        uploadFolder,
                        fileName);

                using (FileStream stream =
                       new FileStream(
                           filePath,
                           FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                model.sPhoto =
                    $"/uploads/candidates/{fileName}";
            }

            // =====================================================
            // SIGNATURE
            // =====================================================

            if (SignatureFile != null &&
                SignatureFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(
                        SignatureFile.FileName)
                        .ToLowerInvariant();

                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png")
                {
                    TempData["Error"] =
                        "Signature must be JPG, JPEG or PNG.";

                    return RedirectToAction("Profile");
                }

                string fileName =
                    $"{candidateId.Value}_Signature{extension}";

                string filePath =
                    Path.Combine(
                        uploadFolder,
                        fileName);

                using (FileStream stream =
                       new FileStream(
                           filePath,
                           FileMode.Create))
                {
                    SignatureFile.CopyTo(stream);
                }

                model.sSignature =
                    $"/uploads/candidates/{fileName}";
            }

            // =====================================================
            // DEBUG / VALIDATION
            // =====================================================

            // At this point model should contain:
            //
            // model.CandidateID
            // model.sResume
            // model.sPhoto
            // model.sSignature
            // model.sDivyang
            // model.sHobbies

            // =====================================================
            // UPDATE DATABASE
            // =====================================================

            _repo.UpdateDocuments(model);

            TempData["Success"] =
                "Documents and personal details updated successfully.";

            return RedirectToAction("Profile");
        }

        public void UpdateDocuments(CandidateProfileModel model)
        {
            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE tblCandidateProfile
            SET
                sResume = @sResume,
                sPhoto = @sPhoto,
                sSignature = @sSignature,
                sDivyang = @sDivyang,
                sHobbies = @sHobbies,
                ModDate = GETDATE()
            WHERE CandidateID = @CandidateID";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int).Value =
                            model.CandidateID;

                    cmd.Parameters.Add(
                        "@sResume",
                        SqlDbType.NVarChar,
                        500).Value =
                            (object?)model.sResume ??
                            DBNull.Value;

                    cmd.Parameters.Add(
                        "@sPhoto",
                        SqlDbType.NVarChar,
                        500).Value =
                            (object?)model.sPhoto ??
                            DBNull.Value;

                    cmd.Parameters.Add(
                        "@sSignature",
                        SqlDbType.NVarChar,
                        500).Value =
                            (object?)model.sSignature ??
                            DBNull.Value;

                    cmd.Parameters.Add(
                        "@sDivyang",
                        SqlDbType.NVarChar,
                        50).Value =
                            (object?)model.sDivyang ??
                            DBNull.Value;

                    cmd.Parameters.Add(
                        "@sHobbies",
                        SqlDbType.NVarChar,
                        -1).Value =
                            (object?)model.sHobbies ??
                            DBNull.Value;

                    con.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        throw new Exception(
                            "No candidate record found for CandidateID: "
                            + model.CandidateID);
                    }
                }
            }
        }
        // shrirang 03/10/26
        // =========================================================
        // MY INTERNSHIP radhika24-09
        // =========================================================
        // =========================================================
        // MY INTERNSHIP
        // =========================================================

        [HttpGet]
        public IActionResult MyInternship()
        {
            // =====================================================
            // CHECK CANDIDATE SESSION
            // =====================================================

            int? candidateID =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateID == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account"
                );
            }

            // =====================================================
            // GET CANDIDATE CODE
            // =====================================================

            string candidateCode =
                HttpContext.Session.GetString("CandidateCode")
                ?? "";

            // =====================================================
            // LIST
            // =====================================================

            List<OrgPostM> internships =
                new List<OrgPostM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_CandidateAppliedToPost",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int
                    ).Value = candidateID.Value;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM item =
                                new OrgPostM();

                            // =================================================
                            // POST ID
                            // =================================================

                            item.nID =
                                dr["PostID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        dr["PostID"]);

                            // =================================================
                            // ORGANIZATION ID
                            // =================================================

                            item.nOrgID =
                                dr["OrgID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        dr["OrgID"]);

                            // =================================================
                            // ORGANIZATION NAME
                            // =================================================

                            item.sName =
                                dr["OrganizationName"] == DBNull.Value
                                    ? ""
                                    : dr["OrganizationName"].ToString();

                            // =================================================
                            // POSITION
                            // =================================================

                            item.sPositionName =
                                dr["Position"] == DBNull.Value
                                    ? ""
                                    : dr["Position"].ToString();

                            // =================================================
                            // COUNTRY
                            // =================================================

                            item.sCountryName =
                                dr["Country"] == DBNull.Value
                                    ? ""
                                    : dr["Country"].ToString();

                            // =================================================
                            // STATE
                            // =================================================

                            item.sStateName =
                                dr["State"] == DBNull.Value
                                    ? ""
                                    : dr["State"].ToString();

                            // =================================================
                            // CITY
                            // =================================================

                            item.nCityName =
                                dr["City"] == DBNull.Value
                                    ? ""
                                    : dr["City"].ToString();

                            // =================================================
                            // INTERNSHIP / FELLOWSHIP
                            // =================================================

                            item.sInternshipFellowshipTypeName =
                                dr["InternshipFellowshipType"]
                                    == DBNull.Value
                                        ? ""
                                        : dr[
                                            "InternshipFellowshipType"
                                          ].ToString();

                            // =================================================
                            // FACILITIES
                            // =================================================

                            item.sFacilities =
                                dr["Facilities"] == DBNull.Value
                                    ? ""
                                    : dr["Facilities"].ToString();

                            // =================================================
                            // APPLIED
                            // =================================================

                            item.IsApplied = true;

                            internships.Add(item);
                        }
                    }
                }
            }

            // =====================================================
            // SEND DATA TO VIEW
            // =====================================================

            ViewBag.CandidateCode = candidateCode;

            return View(internships);
        }


        // =====================================================
        // CHECK WHETHER COLUMN EXISTS
        // =====================================================

        private bool HasColumn(
            SqlDataReader reader,
            string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(
                    reader.GetName(i),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // =========================================================
        // ORGANIZATION INTERESTED
        // =========================================================
        [HttpGet]
        [Route("Candidate/OrganizationInterested/{id?}")]
        public IActionResult OrganizationInterested(string? id)
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE ID
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // GET CANDIDATE CODE
            // =====================================================

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =====================================================
            // IF URL DOES NOT HAVE CANDIDATE CODE
            // REDIRECT TO CODE URL
            // =====================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "OrganizationInterested",
                    "Candidate",
                    new
                    {
                        id = candidateCode
                    });
            }

            // =====================================================
            // VALIDATE CANDIDATE CODE
            // =====================================================

            if (!string.Equals(
                    id,
                    candidateCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            // =====================================================
            // ORGANIZATION LIST
            // =====================================================

            List<OrganizationInterestedViewModel> organizations =
                new List<OrganizationInterestedViewModel>();

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            // =====================================================
            // CALL STORED PROCEDURE
            // =====================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_TraineeOrganizationInterested",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    // =============================================
                    // LOGGED-IN CANDIDATE ID
                    // =============================================

                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int).Value =
                            candidateId.Value;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            organizations.Add(
                                new OrganizationInterestedViewModel
                                {
                                    // =================================
                                    // ID
                                    // =================================

                                    ID =
                                        dr["OrganizationID"] != DBNull.Value
                                        ? Convert.ToInt32(
                                            dr["OrganizationID"])
                                        : 0,

                                    // =================================
                                    // ORGANIZATION NAME
                                    // =================================

                                    OrganizationName =
                                        dr["OrganizationName"] != DBNull.Value
                                        ? dr["OrganizationName"].ToString()
                                        : "",

                                    // =================================
                                    // ORGANIZATION URL
                                    // =================================

                                    OrganizationUrl =
                                        dr["OrganizationUrl"] != DBNull.Value
                                        ? dr["OrganizationUrl"].ToString()
                                        : "",

                                    // =================================
                                    // CONTACT PERSON
                                    // =================================

                                    ContactPersonName =
                                        dr["ContactPersonName"] != DBNull.Value
                                        ? dr["ContactPersonName"].ToString()
                                        : "",

                                    // =================================
                                    // DESIGNATION
                                    // =================================

                                    Designation =
                                        dr["Designation"] != DBNull.Value
                                        ? dr["Designation"].ToString()
                                        : "",

                                    // =================================
                                    // APPLY STATUS
                                    // =================================

                                    ApplyStatus =
                                        dr["ApplyStatus"] != DBNull.Value
                                        ? dr["ApplyStatus"].ToString()
                                        : "-"
                                });
                        }
                    }
                }
            }

            // =====================================================
            // VIEW BAG
            // =====================================================

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateID =
                candidateId.Value;

            ViewBag.CandidateName =
                HttpContext.Session.GetString(
                    "CandidateName");

            ViewBag.CandidateEmail =
                HttpContext.Session.GetString(
                    "CandidateEmail");

            // =====================================================
            // RETURN VIEW
            // =====================================================

            return View(organizations);
        }
        //  [HttpGet]
        //  [Route("Candidate/OrganizationInterested/{id?}")]
        //  public IActionResult OrganizationInterested(string? id)
        //  {
        //      // =====================================================
        //      // GET LOGGED-IN CANDIDATE ID
        //      // =====================================================

        //      int? candidateId =
        //          HttpContext.Session.GetInt32("CandidateID");

        //      if (candidateId == null || candidateId <= 0)
        //      {
        //          return RedirectToAction(
        //              "CandidateLogin",
        //              "Account");
        //      }

        //      // =====================================================
        //      // GET CANDIDATE CODE
        //      // =====================================================

        //      string? candidateCode = GetCandidateCode();

        //      if (string.IsNullOrEmpty(candidateCode))
        //      {
        //          return NotFound(
        //              "Candidate code could not be generated.");
        //      }

        //      // =====================================================
        //      // IF URL DOES NOT HAVE CANDIDATE CODE
        //      // REDIRECT TO CODE URL
        //      // =====================================================

        //      if (string.IsNullOrEmpty(id))
        //      {
        //          return RedirectToAction(
        //              "OrganizationInterested",
        //              "Candidate",
        //              new
        //              {
        //                  id = candidateCode
        //              });
        //      }

        //      // =====================================================
        //      // VALIDATE CANDIDATE CODE
        //      // =====================================================

        //      if (id != candidateCode)
        //      {
        //          return NotFound();
        //      }

        //      // =====================================================
        //      // GET ORGANIZATION DATA
        //      // =====================================================

        //      List<OrganizationInterestedViewModel> organizations =
        //          new List<OrganizationInterestedViewModel>();

        //      string connectionString =
        //          _configuration.GetConnectionString(
        //              "DefaultConnection");

        //      using (SqlConnection con =
        //             new SqlConnection(connectionString))
        //      {
        //          string query = @"
        //SELECT
        //    nID,
        //    sOrgName,
        //    sOrgUrl,
        //    sName,
        //    sDesignation
        //FROM tblOrgRegistration
        //ORDER BY nID DESC";

        //          using (SqlCommand cmd =
        //                 new SqlCommand(query, con))
        //          {
        //              con.Open();

        //              using (SqlDataReader dr =
        //                     cmd.ExecuteReader())
        //              {
        //                  while (dr.Read())
        //                  {
        //                      organizations.Add(
        //                          new OrganizationInterestedViewModel
        //                          {
        //                              ID =
        //                                  dr["nID"] != DBNull.Value
        //                                  ? Convert.ToInt32(dr["nID"])
        //                                  : 0,

        //                              OrganizationName =
        //                                  dr["sOrgName"] != DBNull.Value
        //                                  ? dr["sOrgName"].ToString()
        //                                  : "",

        //                              OrganizationUrl =
        //                                  dr["sOrgUrl"] != DBNull.Value
        //                                  ? dr["sOrgUrl"].ToString()
        //                                  : "",

        //                              ContactPersonName =
        //                                  dr["sName"] != DBNull.Value
        //                                  ? dr["sName"].ToString()
        //                                  : "",

        //                              Designation =
        //                                  dr["sDesignation"] != DBNull.Value
        //                                  ? dr["sDesignation"].ToString()
        //                                  : "",

        //                              ApplyStatus = "Interested"
        //                          });
        //                  }
        //              }
        //          }
        //      }

        //      // =====================================================
        //      // SEND DATA TO VIEW
        //      // =====================================================

        //      ViewBag.CandidateCode = candidateCode;
        //      ViewBag.CandidateID = candidateId.Value;

        //      ViewBag.CandidateName =
        //          HttpContext.Session.GetString(
        //              "CandidateName");

        //      ViewBag.CandidateEmail =
        //          HttpContext.Session.GetString(
        //              "CandidateEmail");

        //      return View(organizations);
        //  }


        // =========================================================
        // TRAINEE SELECTION
        // =========================================================
        [HttpGet]
        [Route("Candidate/TraineeStatus/{id?}")]
        public IActionResult TraineeStatus(string? id)
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // GET CANDIDATE CODE
            // =====================================================

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =====================================================
            // IF ID IS MISSING
            // REDIRECT TO CODE URL
            // =====================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "TraineeStatus",
                    "Candidate",
                    new
                    {
                        id = candidateCode
                    });
            }

            // =====================================================
            // VALIDATE CANDIDATE CODE
            // =====================================================

            if (!string.Equals(
                    id,
                    candidateCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            // =====================================================
            // LOAD TRAINEE FINAL SELECTION DATA
            // =====================================================

            List<TraineeSelectionViewModel> selectionList =
                new List<TraineeSelectionViewModel>();

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_TraineeFinalsectionstatusTR",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    // =============================================
                    // PASS LOGGED-IN CANDIDATE ID
                    // =============================================

                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int).Value =
                            candidateId.Value;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TraineeSelectionViewModel item =
                                new TraineeSelectionViewModel();

                            // =====================================
                            // ID
                            // =====================================

                            item.ID =
                                dr["nID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nID"])
                                : 0;

                            // =====================================
                            // ORGANIZATION
                            // =====================================

                            item.OrganizationName =
                                dr["OrganizationName"] != DBNull.Value
                                ? dr["OrganizationName"].ToString()
                                : "";

                            // =====================================
                            // POSITION
                            // =====================================

                            item.Position =
                                dr["PositionName"] != DBNull.Value
                                ? dr["PositionName"].ToString()
                                : "";

                            // =====================================
                            // FINAL STATUS
                            // =====================================

                            item.FinalStatus =
                                dr["FinalStatus"] != DBNull.Value
                                ? dr["FinalStatus"].ToString()
                                : "";

                            // =====================================
                            // REGISTRATION DATE
                            // =====================================

                            item.RegistrationDate =
                                dr["RegDate"] != DBNull.Value
                                ? Convert.ToDateTime(
                                    dr["RegDate"])
                                : null;

                            selectionList.Add(item);
                        }
                    }
                }
            }

            // =====================================================
            // VIEW BAG
            // =====================================================

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateID =
                candidateId.Value;

            // =====================================================
            // RETURN VIEW
            // =====================================================

            return View(selectionList);
        }
        //[HttpGet]
        //[Route("Candidate/TraineeStatus/{id?}")]
        //public IActionResult TraineeStatus(string? id)
        //{
        //    // =====================================================
        //    // GET LOGGED-IN CANDIDATE
        //    // =====================================================

        //    int? candidateId =
        //        HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null || candidateId <= 0)
        //    {
        //        return RedirectToAction(
        //            "CandidateLogin",
        //            "Account");
        //    }

        //    // =====================================================
        //    // GET CANDIDATE CODE
        //    // =====================================================

        //    string? candidateCode = GetCandidateCode();

        //    if (string.IsNullOrEmpty(candidateCode))
        //    {
        //        return NotFound(
        //            "Candidate code could not be generated.");
        //    }

        //    // =====================================================
        //    // IF ID IS MISSING
        //    // REDIRECT TO CODE URL
        //    // =====================================================

        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return RedirectToAction(
        //            "TraineeStatus",
        //            "Candidate",
        //            new { id = candidateCode });
        //    }

        //    // =====================================================
        //    // VALIDATE CANDIDATE CODE
        //    // =====================================================

        //    if (id != candidateCode)
        //    {
        //        return NotFound();
        //    }

        //    // =====================================================
        //    // LOAD TRAINEE SELECTION DATA
        //    // =====================================================

        //    List<TraineeSelectionViewModel> selectionList =
        //        new List<TraineeSelectionViewModel>();

        //    string connectionString =
        //        _configuration.GetConnectionString(
        //            "DefaultConnection");

        //    //    using (SqlConnection con =
        //    //           new SqlConnection(connectionString))
        //    //    {
        //    //        string query = @"
        //    //    SELECT
        //    //        nID,
        //    //        sOrgName,
        //    //        sPosition,
        //    //        sFinalStatus,
        //    //        RegDate
        //    //    FROM tblTraineeSelection
        //    //    WHERE nCandidateID = @CandidateID
        //    //    ORDER BY RegDate DESC;
        //    //";

        //    //        using (SqlCommand cmd =
        //    //               new SqlCommand(query, con))
        //    //        {
        //    //            cmd.Parameters.Add(
        //    //                "@CandidateID",
        //    //                SqlDbType.Int).Value =
        //    //                    candidateId.Value;

        //    //            con.Open();

        //    //            using (SqlDataReader dr =
        //    //                   cmd.ExecuteReader())
        //    //            {
        //    //                while (dr.Read())
        //    //                {
        //    //                    selectionList.Add(
        //    //                        new TraineeSelectionViewModel
        //    //                        {
        //    //                            ID =
        //    //                                dr["nID"] != DBNull.Value
        //    //                                ? Convert.ToInt32(dr["nID"])
        //    //                                : 0,

        //    //                            OrganizationName =
        //    //                                dr["sOrgName"] != DBNull.Value
        //    //                                ? dr["sOrgName"].ToString()
        //    //                                : "",

        //    //                            Position =
        //    //                                dr["sPosition"] != DBNull.Value
        //    //                                ? dr["sPosition"].ToString()
        //    //                                : "",

        //    //                            FinalStatus =
        //    //                                dr["sFinalStatus"] != DBNull.Value
        //    //                                ? dr["sFinalStatus"].ToString()
        //    //                                : "",

        //    //                            RegistrationDate =
        //    //                                dr["RegDate"] != DBNull.Value
        //    //                                ? Convert.ToDateTime(dr["RegDate"])
        //    //                                : null
        //    //                        });
        //    //                }
        //    //            }
        //    //        }
        //    //    }

        //    // =====================================================
        //    // VIEW BAG
        //    // =====================================================

        //    ViewBag.CandidateCode = candidateCode;
        //    ViewBag.CandidateID = candidateId.Value;

        //    return View(selectionList);
        //}

        // =========================================================
        // CHARTS
        // =========================================================

        //khushi 02-10
        [HttpGet]
        [Route("Candidate/Charts/{id?}")]
        public IActionResult Charts(string? id)
        {
            // =========================================================
            // CHECK CANDIDATE SESSION
            // =========================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }


            // =========================================================
            // GET CANDIDATE CODE
            // =========================================================

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }


            // =========================================================
            // IF ID IS MISSING FROM URL
            // =========================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "Charts",
                    "Candidate",
                    new
                    {
                        id = candidateCode
                    });
            }


            // =========================================================
            // VALIDATE CANDIDATE CODE
            // =========================================================

            if (id != candidateCode)
            {
                return NotFound();
            }


            // =========================================================
            // GET MATCHING / SUITABLE JOBS
            // =========================================================

            List<OrgPostM> matchingJobs =
                GetMatchingJobsForCandidate(candidateId.Value);


            // =========================================================
            // MONTHLY SUITABLE OPENINGS
            // =========================================================

            int currentYear = DateTime.Now.Year;

            int[] suitableOpeningsMonthly =
                new int[12];


            foreach (var job in matchingJobs)
            {
                if (!job.dStartDate.HasValue)
                    continue;


                DateTime startDate =
                    job.dStartDate.Value;


                // Only current year's jobs
                if (startDate.Year != currentYear)
                    continue;


                int monthIndex =
                    startDate.Month - 1;


                int requiredTrainees =
                    job.nRequiredTrainees;


                if (requiredTrainees < 0)
                    requiredTrainees = 0;


                suitableOpeningsMonthly[monthIndex] +=
                    requiredTrainees;
            }


            // =========================================================
            // GET INTERNSHIP POST DATA
            // FROM SP_GetSACharts
            // =========================================================

            SAChartsViewModel model =
                new SAChartsViewModel();


            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");


            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetSACharts",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;


                    con.Open();


                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        // =================================================
                        // 1. CANDIDATE DATA
                        // =================================================

                        while (reader.Read())
                        {
                            model.CandidateData.Add(
                                new SAChartData
                                {
                                    Year =
                                        Convert.ToInt32(
                                            reader["Year"]),

                                    Month =
                                        Convert.ToInt32(
                                            reader["Month"]),

                                    TotalCount =
                                        Convert.ToInt32(
                                            reader["TotalCount"])
                                });
                        }


                        // =================================================
                        // 2. INTERNSHIP POST DATA
                        // =================================================

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.InternshipPostData.Add(
                                    new SAChartData
                                    {
                                        Year =
                                            Convert.ToInt32(
                                                reader["Year"]),

                                        Month =
                                            Convert.ToInt32(
                                                reader["Month"]),

                                        TotalCount =
                                            Convert.ToInt32(
                                                reader["TotalCount"])
                                    });
                            }
                        }
                    }
                }
            }


            // =========================================================
            // SEND CANDIDATE DATA TO VIEW
            // =========================================================

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateID =
                candidateId.Value;

            ViewBag.ChartYear =
                currentYear;

            ViewBag.SuitableOpeningsMonthly =
                suitableOpeningsMonthly;


            // =========================================================
            // RETURN SAME VIEW WITH MODEL
            // =========================================================

            return View(model);
        }



        // shrirang 29/09/26
        // =========================================================
        // GET CAREER OBJECTIVE DRAFTS
        // =========================================================

        [HttpGet]
        public IActionResult GetCareerObjectives(int genderID)
        {
            List<SAObjectiveM> objectives = new List<SAObjectiveM>();

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection cn =
                       new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("SP_GetObjective", cn))
                    {
                        cmd.CommandType =
                            CommandType.StoredProcedure;

                        cn.Open();

                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isEnabled =
                                    reader["nBit"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["nBit"]);

                                if (!isEnabled)
                                {
                                    continue;
                                }

                                int objectiveGenderID =
                                    reader["nGenderID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["nGenderID"])
                                    : 0;

                                // =========================================
                                // FILTER BY SELECTED GENDER
                                // =========================================

                                if (objectiveGenderID != genderID)
                                {
                                    continue;
                                }

                                objectives.Add(new SAObjectiveM
                                {
                                    nID =
                                        Convert.ToInt32(reader["nID"]),

                                    nGenderID =
                                        objectiveGenderID,

                                    sObjective =
                                        reader["sObjective"]?.ToString()
                                        ?? string.Empty,

                                    dCreatedDate =
                                        reader["dCreatedDate"] != DBNull.Value
                                        ? Convert.ToDateTime(
                                            reader["dCreatedDate"])
                                        : DateTime.MinValue,

                                    nBit = isEnabled,

                                    nSABit =
                                        reader["nSABit"] != DBNull.Value &&
                                        Convert.ToBoolean(
                                            reader["nSABit"])
                                });
                            }
                        }
                    }
                }

                objectives =
                    objectives
                        .OrderBy(x => x.nID)
                        .ToList();

                return Json(new
                {
                    success = true,

                    objectives = objectives.Select(x => new
                    {
                        x.nID,
                        x.nGenderID,
                        x.sObjective
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // CUSTOM RESUME BUILDER
        // Supports:
        // /Candidate/customResumeBuilder/2
        //
        // Also supports:
        // /Candidate/customResumeBuilder/CD080599072601
        // =========================================================

        [HttpGet]
        [Route("Candidate/customResumeBuilder/{id?}")]
        [ResponseCache(
            NoStore = true,
            Location = ResponseCacheLocation.None)]
        public IActionResult CustomResumeBuilder(string? id)
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE ID
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // GET CANDIDATE CODE
            // =====================================================

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrEmpty(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =====================================================
            // IF NO ID IS PASSED
            // REDIRECT TO CANDIDATE CODE URL
            // =====================================================

            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(
                    "CustomResumeBuilder",
                    "Candidate",
                    new
                    {
                        id = candidateCode
                    });
            }

            // =====================================================
            // VALIDATE URL
            //
            // The URL can contain:
            //
            // 1. Candidate ID
            //    /customResumeBuilder/2
            //
            // 2. Candidate Code
            //    /customResumeBuilder/CD080599072601
            //
            // But it MUST belong to the logged-in candidate.
            // =====================================================

            bool validCandidate = false;

            // -----------------------------------------------------
            // CASE 1: NUMERIC CANDIDATE ID
            // -----------------------------------------------------

            if (int.TryParse(id, out int urlCandidateId))
            {
                if (urlCandidateId == candidateId.Value)
                {
                    validCandidate = true;
                }
            }

            // -----------------------------------------------------
            // CASE 2: CANDIDATE CODE
            // -----------------------------------------------------

            if (!validCandidate &&
                string.Equals(
                    id,
                    candidateCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                validCandidate = true;
            }

            // =====================================================
            // WRONG CANDIDATE
            // =====================================================

            if (!validCandidate)
            {
                return NotFound();
            }

            // =====================================================
            // GET PROFILE
            // =====================================================

            CandidateProfileModel? profile =
                _repo.GetProfile(candidateId.Value);

            // =====================================================
            // IF PROFILE DOES NOT EXIST
            // =====================================================

            if (profile == null)
            {
                profile = new CandidateProfileModel
                {
                    CandidateID = candidateId.Value
                };
            }

            // =====================================================
            // LOAD RESUME VIEW MODEL
            // =====================================================

            ResumeViewModel vm =
     new ResumeViewModel
     {
         Profile = profile,

         Relationships =
             _repo.GetRelationships(),

         Streams =
             _repo.GetStreams(),

         Divisions =
             _repo.GetDivisions(),

         InternshipFellowshipType =
             _repo.GetInternshipFellowshipType(),

         InternshipTitles =
             _repo.GetInternshipTitles(),

         InternshipDurations =
             _repo.GetInternshipDurations(),

         InternshipStatuses =
             _repo.GetInternshipStatuses()
     };

            // =====================================================
            // VIEW BAG
            // =====================================================

            ViewBag.CandidateID =
                candidateId.Value;

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateName =
                HttpContext.Session.GetString(
                    "CandidateName");

            ViewBag.CandidateEmail =
                HttpContext.Session.GetString(
                    "CandidateEmail");

            // =====================================================
            // OPEN VIEW
            // =====================================================

            return View(
                "~/Views/Candidate/customResumeBuilder.cshtml",
                vm);
        }

        //radhika29-09
        [HttpGet]
        public IActionResult InternshipPostDetails()
        {
            int candidateId = Convert.ToInt32(HttpContext.Session.GetString("CandidateID"));
            List<OrgPostM> posts = GetMatchingJobsForCandidate(candidateId);
            ViewBag.InternshipPosts = posts;

            return View();
        }

        // shrirang 01/10/26
        private bool HasCandidateProfileData(CandidateProfileModel? profile)
        {
            if (profile == null)
                return false;

            // CandidateID is generated by the system.
            // It should not count as profile data.
            var ignoredProperties = new HashSet<string>
{
    nameof(CandidateProfileModel.CandidateID)
};

            var properties =
                typeof(CandidateProfileModel).GetProperties();

            foreach (var property in properties)
            {
                if (ignoredProperties.Contains(property.Name))
                    continue;

                object? value =
                    property.GetValue(profile);

                // NULL = no data
                if (value == null)
                    continue;

                // STRING fields
                if (value is string stringValue)
                {
                    if (!string.IsNullOrWhiteSpace(stringValue))
                    {
                        return true;
                    }

                    continue;
                }

                // Nullable<T>
                Type propertyType =
                    Nullable.GetUnderlyingType(property.PropertyType)
                    ?? property.PropertyType;

                // Value type
                if (propertyType.IsValueType)
                {
                    object? defaultValue =
                        Activator.CreateInstance(propertyType);

                    if (!Equals(value, defaultValue))
                    {
                        return true;
                    }

                    continue;
                }

                // Other reference types
                return true;
            }

            return false;
        }

        //radhika29-09
        //[HttpGet]
        //public IActionResult InternshipPostDetails()
        //{
        //    return View();
        //}


        #region "Apply for the Post"

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyToPost(int postId, int orgId)
        {
            // ==========================================
            // CHECK CANDIDATE LOGIN
            // ==========================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // ==========================================
            // GET CANDIDATE REGISTRATION DETAILS
            // ==========================================

            var candidate =
                _repository.GetCandidateRegistrationDetails(
                    candidateId.Value);

            if (candidate == null ||
                candidate.Value.DOB == null ||
                candidate.Value.RegDate == null)
            {
                return BadRequest(
                    "Candidate information is missing.");
            }

            // ==========================================
            // CREATE CANDIDATE CODE
            // ==========================================

            string candidateCode = "CD" +
                candidate.Value.DOB.Value.ToString("ddMMyy") +
                candidate.Value.RegDate.Value.ToString("MMdd") +
                candidate.Value.CandidateID.ToString("D2");

            // ==========================================
            // APPLY
            // ==========================================

            int result =
                _repo.ApplyForPost(
                    orgId,
                    candidateId.Value,
                    postId);

            // ==========================================
            // SUCCESS
            // ==========================================

            if (result == 1)
            {
                TempData["ApplyMessage"] =
                    "Application submitted successfully.";

                TempData["ApplyMessageType"] =
                    "success";
            }
            else
            {
                TempData["ApplyMessage"] =
                    "You have already applied for this post.";

                TempData["ApplyMessageType"] =
                    "info";
            }

            // ==========================================
            // REDIRECT SAME DASHBOARD PAGE
            // ==========================================

            return RedirectToAction(
                "Dashboard",
                "Candidate",
                new { id = candidateCode });
        }

        #endregion

        #region "TR Apply to Post List Called"

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult TraineeApplied()
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            // Candidate registration details
            var candidate = _repository.GetCandidateRegistrationDetails(candidateId.Value);

            if (candidate == null)
            {
                return NotFound("Candidate registration not found.");
            }

            if (candidate.Value.DOB == null || candidate.Value.RegDate == null)
            {
                return BadRequest(
                    "Candidate DOB or Registration Date is missing.");
            }

            // Candidate Code
            string candidateCode = "CD" +
                candidate.Value.DOB.Value.ToString("ddMMyy") +
                candidate.Value.RegDate.Value.ToString("MMdd") +
                candidate.Value.CandidateID.ToString("D2");

            ViewBag.CandidateID = candidateId.Value;
            ViewBag.CandidateName = HttpContext.Session.GetString("CandidateName");
            ViewBag.CandidateEmail = HttpContext.Session.GetString("CandidateEmail");
            ViewBag.CandidateCode = candidateCode;

            // =========================================================
            // GET APPLIED POSTS
            // =========================================================

            List<OrgPostM> traineeAppliedPosts = TraineeApplyToPostList(candidateId.Value);

            ViewBag.TraineeAppliedPosts = traineeAppliedPosts;

            return View();
        }
        // =============================================================
        // GET TRAINEE APPLIED POSTS
        // =============================================================
        [HttpGet]
        private List<OrgPostM> TraineeApplyToPostList(int candidateId)
        {
            List<OrgPostM> appliedPosts = new List<OrgPostM>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CandidateAppliedToPost", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // =====================================================
                    // CANDIDATE ID
                    // =====================================================
                    cmd.Parameters.Add("@CandidateID", SqlDbType.Int).Value = candidateId;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM post = new OrgPostM();

                            // =================================================
                            // POST ID
                            // =================================================
                            post.nID =
                                dr["PostID"] != DBNull.Value
                                ? Convert.ToInt32(dr["PostID"])
                                : 0;

                            // =================================================
                            // ORGANIZATION ID
                            // =================================================
                            post.nOrgID =
                                dr["OrgD"] != DBNull.Value
                                ? Convert.ToInt32(dr["OrgD"])
                                : 0;

                            // =================================================
                            // ORGANIZATION NAME
                            // SP: org.sOrgName AS OrganizationName
                            // =================================================
                            post.sName =
                                dr["OrganizationName"] != DBNull.Value
                                ? dr["OrganizationName"].ToString()!
                                : "";

                            // =================================================
                            // POSITION NAME
                            // SP: pp.sName AS Position
                            // =================================================
                            post.sPositionName =
                                dr["Position"] != DBNull.Value
                                ? dr["Position"].ToString()!
                                : "";

                            // =================================================
                            // COUNTRY
                            // SP: np.sCountryName AS CountryName
                            // =================================================
                            post.sCountryName =
                                dr["CountryName"] != DBNull.Value
                                ? dr["CountryName"].ToString()!
                                : "";

                            // =================================================
                            // STATE
                            // SP: np.sStateName AS StateName
                            // =================================================
                            post.sStateName =
                                dr["StateName"] != DBNull.Value
                                ? dr["StateName"].ToString()!
                                : "";

                            // =================================================
                            // CITY
                            // SP: np.nCityName AS CityName
                            // =================================================
                            post.nCityName =
                                dr["CityName"] != DBNull.Value
                                ? dr["CityName"].ToString()!
                                : "";

                            // =================================================
                            // INTERNSHIP / FELLOWSHIP TYPE
                            // SP: sf.sName AS StipendFees
                            // =================================================
                            post.sInternshipFellowshipTypeName =
                                dr["StipendFees"] != DBNull.Value
                                ? dr["StipendFees"].ToString()!
                                : "";

                            // =================================================
                            // FACILITIES
                            // SP: np.sFacilities AS FacilityAvailable
                            // =================================================
                            post.sFacilities =
                                dr["FacilityAvailable"] != DBNull.Value
                                ? dr["FacilityAvailable"].ToString()!
                                : "";

                            // =================================================
                            // APPLICATION STATUS
                            // SP: fs.CApply
                            // =================================================
                            post.IsApplied =
                                dr["CApply"] != DBNull.Value &&
                                Convert.ToInt32(dr["CApply"]) == 1;

                            // =================================================
                            // ADD TO LIST
                            // =================================================
                            appliedPosts.Add(post);
                        }
                    }
                }
            }

            return appliedPosts;
        }

        #endregion


        #region "Chart1"
        private void PrepareSuitableOpeningChart(int candidateId, CandidateDashboardViewModel model)
        {
            // Get matching/suitable jobs for trainee
            List<OrgPostM> matchingJobs = GetMatchingJobsForCandidate(candidateId);

            // Initialize all 12 months with 0
            string[] months =
            {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

            int[] monthlyOpenings = new int[12];

            foreach (var job in matchingJobs)
            {
                // Make sure date exists
                if (job.dStartDate == null)
                    continue;

                DateTime startDate = job.dStartDate.Value;

                int requiredTrainees = job.nRequiredTrainees;

                if (requiredTrainees < 0)
                    requiredTrainees = 0;

                int monthIndex = startDate.Month - 1;

                monthlyOpenings[monthIndex] += requiredTrainees;
            }

            model.SuitableOpeningMonths =
                months.ToList();

            model.SuitableOpeningCounts =
                monthlyOpenings.ToList();
        }
        #endregion




        // shrirang 02/10/26

        // =========================================================
        // UPDATE RESUME TEMPLATE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateResumeTemplate(
            int candidateID,
            string resumeTemplate)
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE
            // =====================================================

            int? sessionCandidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (sessionCandidateId == null ||
                sessionCandidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // NEVER TRUST CANDIDATE ID FROM FORM
            // =====================================================

            candidateID = sessionCandidateId.Value;

            // =====================================================
            // VALIDATE RESUME TEMPLATE
            // =====================================================

            if (string.IsNullOrWhiteSpace(resumeTemplate))
            {
                TempData["Error"] =
                    "Please select a resume profile.";

                return RedirectToAction("Profile");
            }

            // =====================================================
            // CONVERT r1 / r2 / r3 ... TO INTEGER
            // =====================================================

            if (!resumeTemplate.StartsWith("r",
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Invalid resume profile selected.";

                return RedirectToAction("Profile");
            }

            string numberPart =
                resumeTemplate.Substring(1);

            if (!int.TryParse(
                numberPart,
                out int resumeProfile))
            {
                TempData["Error"] =
                    "Invalid resume profile selected.";

                return RedirectToAction("Profile");
            }

            // =====================================================
            // DEFAULT VALIDATION
            // =====================================================

            if (resumeProfile <= 0)
            {
                TempData["Error"] =
                    "Invalid resume profile selected.";

                return RedirectToAction("Profile");
            }

            // =====================================================
            // SAVE SELECTED RESUME PROFILE
            // =====================================================

            _repo.UpdateResumeProfile(
                candidateID,
                resumeProfile);

            // =====================================================
            // SUCCESS
            // =====================================================

            TempData["Success"] =
                $"Resume Profile {resumeProfile} selected successfully.";

            return RedirectToAction("Profile");
        }

        // shrirang 02/10/26

        // =============================================================
        // GET ALL ACTIVE ORGANIZATION POSTS FOR CANDIDATE
        // =============================================================

        [HttpGet]
        private List<OrgPostM> GetAllOrganizationPostsForCandidate(int candidateId)
        {
            List<OrgPostM> posts = new List<OrgPostM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
SELECT
    p.nID AS PostID,
    p.nOrgID AS OrgID,

    org.sOrgName AS OrganizationName,

    pp.sName AS PositionName,

    p.sCountryName AS Country,
    p.sStateName AS State,
    p.nCityName AS City,

    sf.sName AS InternshipFellowshipType,

    p.sFacilities,

    CASE
        WHEN ISNULL(jas.CApply, 0) = 1
            THEN 1
        ELSE 0
    END AS CApply

FROM tblOrgPost p

LEFT JOIN tblOrgRegistration org
    ON org.nID = p.nOrgID

LEFT JOIN tblPosition pp
    ON pp.nID = p.nPositionID

LEFT JOIN tblStipendFees sf
    ON sf.nID = p.nInternshipFellowshipTypeID

LEFT JOIN tblJobApplicationStatus jas
    ON jas.nPostID = p.nID
    AND jas.nCandidateID = @CandidateID

WHERE
    ISNULL(p.nBit, 1) = 1
    AND ISNULL(p.nSABit, 1) = 1

ORDER BY
    p.nID DESC;";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int
                    ).Value = candidateId;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM post = new OrgPostM();

                            post.nID =
                                dr["PostID"] != DBNull.Value
                                ? Convert.ToInt32(dr["PostID"])
                                : 0;

                            post.nOrgID =
                                dr["OrgID"] != DBNull.Value
                                ? Convert.ToInt32(dr["OrgID"])
                                : 0;

                            post.sName =
                                dr["OrganizationName"] != DBNull.Value
                                ? dr["OrganizationName"].ToString()!
                                : "";

                            post.sPositionName =
                                dr["PositionName"] != DBNull.Value
                                ? dr["PositionName"].ToString()!
                                : "";

                            post.sCountryName =
                                dr["Country"] != DBNull.Value
                                ? dr["Country"].ToString()!
                                : "";

                            post.sStateName =
                                dr["State"] != DBNull.Value
                                ? dr["State"].ToString()!
                                : "";

                            post.nCityName =
                                dr["City"] != DBNull.Value
                                ? dr["City"].ToString()!
                                : "";

                            post.sInternshipFellowshipTypeName =
                                dr["InternshipFellowshipType"] != DBNull.Value
                                ? dr["InternshipFellowshipType"].ToString()!
                                : "";

                            post.sFacilities =
                                dr["sFacilities"] != DBNull.Value
                                ? dr["sFacilities"].ToString()!
                                : "";

                            post.IsApplied =
                                dr["CApply"] != DBNull.Value &&
                                Convert.ToInt32(dr["CApply"]) == 1;

                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }


        // shrirang 03/10/26

        // =========================================================
        // VIEW APPLIED ORGANIZATION POST
        // Candidate can only view a post for which he/she has applied
        // =========================================================

        [HttpGet]
        [Route("Candidate/ViewAppliedOrgPost/{id}")]
        public IActionResult ViewAppliedOrgPost(int id)
        {
            // =====================================================
            // GET LOGGED-IN CANDIDATE
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // GET CANDIDATE CODE
            // =====================================================

            string? candidateCode =
                GetCandidateCode();

            if (string.IsNullOrWhiteSpace(candidateCode))
            {
                return NotFound(
                    "Candidate code could not be generated.");
            }

            // =====================================================
            // GET ONLY APPLIED POSTS
            // =====================================================

            List<OrgPostM> appliedPosts =
                TraineeApplyToPostList(candidateId.Value);

            // =====================================================
            // FIND REQUESTED POST
            // =====================================================

            OrgPostM? appliedPost =
                appliedPosts.FirstOrDefault(
                    x => x.nID == id && x.IsApplied);

            // =====================================================
            // SECURITY CHECK
            // Candidate has not applied for this post
            // =====================================================

            if (appliedPost == null)
            {
                return NotFound(
                    "This internship post was not applied by the logged-in candidate.");
            }

            // =====================================================
            // GET ORGANIZATION PROFILE
            // =====================================================

            OrgProfile? organizationProfile = null;

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")!;

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetOrganizationProfile",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value =
                            appliedPost.nOrgID;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            organizationProfile =
                                new OrgProfile
                                {
                                    nID =
                                        dr["nID"] != DBNull.Value
                                            ? Convert.ToInt32(
                                                dr["nID"])
                                            : 0,

                                    nOrgID =
                                        dr["nOrgID"] != DBNull.Value
                                            ? Convert.ToInt32(
                                                dr["nOrgID"])
                                            : appliedPost.nOrgID,

                                    sOrganizationName =
                                        dr["sName"] != DBNull.Value
                                            ? dr["sName"].ToString()
                                            : "",

                                    sOrganizationEmail =
                                        dr["sEmail"] != DBNull.Value
                                            ? dr["sEmail"].ToString()
                                            : "",

                                    sMobile =
                                        dr["sMobile"] != DBNull.Value
                                            ? dr["sMobile"].ToString()
                                            : "",

                                    sDesignation =
                                        dr["sDesignation"] != DBNull.Value
                                            ? dr["sDesignation"].ToString()
                                            : "",

                                    dDateOfBirth =
                                        dr["dDateOfBirth"] != DBNull.Value
                                            ? Convert.ToDateTime(
                                                dr["dDateOfBirth"])
                                            : null,

                                    sCompanyLogo =
                                        dr["sCompanyLogo"] != DBNull.Value
                                            ? dr["sCompanyLogo"].ToString()
                                            : "",

                                    sCompanyAddress =
                                        dr["sCompanyAddress"] != DBNull.Value
                                            ? dr["sCompanyAddress"].ToString()
                                            : "",

                                    nEstablishmentYear =
                                        dr["nEstablishmentYear"] != DBNull.Value
                                            ? Convert.ToInt32(
                                                dr["nEstablishmentYear"])
                                            : 0,

                                    sGSTNo =
                                        dr["sGSTNo"] != DBNull.Value
                                            ? dr["sGSTNo"].ToString()
                                            : "",

                                    sCINNo =
                                        dr["sCINNo"] != DBNull.Value
                                            ? dr["sCINNo"].ToString()
                                            : "",

                                    nEmployeeStrength =
                                        dr["nEmployeeStrength"] != DBNull.Value
                                            ? dr["nEmployeeStrength"].ToString()
                                            : "",

                                    dCreatedDate =
                                        dr["dCreatedDate"] != DBNull.Value
                                            ? Convert.ToDateTime(
                                                dr["dCreatedDate"])
                                            : DateTime.MinValue,

                                    dModifiedDate =
                                        dr["dModifiedDate"] != DBNull.Value
                                            ? Convert.ToDateTime(
                                                dr["dModifiedDate"])
                                            : null,

                                    nBit =
                                        dr["nBit"] != DBNull.Value &&
                                        Convert.ToBoolean(
                                            dr["nBit"]),

                                    nSABit =
                                        dr["nSABit"] != DBNull.Value
                                            ? Convert.ToBoolean(
                                                dr["nSABit"])
                                            : null
                                };
                        }
                    }
                }
            }

            // =====================================================
            // ORGANIZATION PROFILE NOT FOUND
            // =====================================================

            if (organizationProfile == null)
            {
                return NotFound(
                    "Organization profile not found.");
            }

            // =====================================================
            // VIEW BAG
            // =====================================================

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateID =
                candidateId.Value;

            ViewBag.CandidateName =
                HttpContext.Session.GetString(
                    "CandidateName");

            ViewBag.CandidateEmail =
                HttpContext.Session.GetString(
                    "CandidateEmail");

            ViewBag.PostID =
                appliedPost.nID;

            ViewBag.OrgID =
                appliedPost.nOrgID;

            // =====================================================
            // RETURN VIEW
            // =====================================================

            ViewAppliedOrgPostViewModel model =
                new ViewAppliedOrgPostViewModel
                {
                    Post = appliedPost,
                    Organization = organizationProfile
                };

            return View(model);
        }

        // shrirang 03/10/26

        // =========================================================
        // VIEW MY ORGANIZATION POST
        // Candidate can view complete details of selected internship
        // =========================================================

        [HttpGet]
        [Route("Candidate/ViewMyOrgPost/{id}")]
        public IActionResult ViewMyOrgPost(int id)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            string? candidateCode = GetCandidateCode();

            if (string.IsNullOrWhiteSpace(candidateCode))
            {
                return NotFound("Candidate code could not be generated.");
            }

            // Get selected post
            OrgPostM? selectedPost =
                GetInternshipPostByID(id);

            if (selectedPost == null)
            {
                return NotFound(
                    $"Internship post not found. Post ID = {id}");
            }

            // Get posts for navigation
            List<OrgPostM> allPosts =
                GetAllInternshipPosts();

            // Make sure current post is always included
            if (!allPosts.Any(x => x.nID == selectedPost.nID))
            {
                allPosts.Add(selectedPost);
            }

            // IMPORTANT: sort by Post ID
            allPosts =
                allPosts
                    .OrderBy(x => x.nID)
                    .ToList();

            int currentIndex =
                allPosts.FindIndex(
                    x => x.nID == selectedPost.nID);

            if (currentIndex < 0)
            {
                return NotFound(
                    $"Post ID {id} could not be added to the navigation list.");
            }

            OrgPostM? previousPost = null;
            OrgPostM? nextPost = null;

            if (currentIndex > 0)
            {
                previousPost =
                    allPosts[currentIndex - 1];
            }

            if (currentIndex < allPosts.Count - 1)
            {
                nextPost =
                    allPosts[currentIndex + 1];
            }

            ViewBag.CandidateCode =
                candidateCode;

            ViewBag.CandidateID =
                candidateId.Value;

            ViewBag.CandidateName =
                HttpContext.Session.GetString("CandidateName");

            ViewBag.CandidateEmail =
                HttpContext.Session.GetString("CandidateEmail");

            ViewBag.CurrentPostID =
                selectedPost.nID;

            ViewBag.PreviousPostID =
                previousPost?.nID;

            ViewBag.NextPostID =
                nextPost?.nID;

            ViewBag.HasPrevious =
                previousPost != null;

            ViewBag.HasNext =
                nextPost != null;

            return View(selectedPost);
        }
        private OrgPostM? GetInternshipPostByID(int id)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")!;

            int orgId = 0;

            // ============================================================
            // STEP 1: GET ORGANIZATION ID FROM tblPost
            // ============================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(@"
             SELECT nOrgID
             FROM tblPost
             WHERE nID = @nID
         ", con))
                {
                    cmd.Parameters.Add(
                        "@nID",
                        SqlDbType.Int).Value = id;

                    con.Open();

                    object? result =
                        cmd.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return null;
                    }

                    orgId = Convert.ToInt32(result);
                }
            }

            if (orgId <= 0)
            {
                return null;
            }

            // ============================================================
            // STEP 2:
            // GET POST DETAILS FROM SP_GetOrganizationPostList
            // ============================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetOrganizationPostList",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value = orgId;

                    con.Open();

                    using SqlDataReader dr =
                        cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        int postId =
                            dr["nID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nID"])
                                : 0;

                        if (postId != id)
                        {
                            continue;
                        }

                        OrgPostM post =
                            new OrgPostM();

                        post.nID = postId;

                        post.nOrgID =
                            dr["nOrgID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nOrgID"])
                                : orgId;

                        // ====================================================
                        // POSITION
                        // ====================================================

                        post.nPositionID =
                            dr["nPositionID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nPositionID"])
                                : 0;

                        post.sPositionName =
                            dr["sPositionName"] != DBNull.Value
                                ? dr["sPositionName"].ToString()
                                : "";

                        // ====================================================
                        // REQUIRED TRAINEES
                        // ====================================================

                        post.nRequiredTrainees =
                            dr["nRequiredTrainees"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nRequiredTrainees"])
                                : 0;

                        // ====================================================
                        // GENDER
                        // ====================================================

                        post.nGenderID =
                            dr["nGenderID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nGenderID"])
                                : 0;

                        post.sGenderName =
                            dr["sGenderName"] != DBNull.Value
                                ? dr["sGenderName"].ToString()
                                : "";

                        // ====================================================
                        // QUALIFICATION
                        // ====================================================

                        post.nMinimumQualificationID =
                            dr["nMinimumQualificationID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nMinimumQualificationID"])
                                : 0;

                        post.sMinimumQualificationName =
                            dr["sMinimumQualificationName"] != DBNull.Value
                                ? dr["sMinimumQualificationName"].ToString()
                                : "";

                        // ====================================================
                        // LOCATION
                        // ====================================================

                        post.sCountryName =
                            dr["sCountryName"] != DBNull.Value
                                ? dr["sCountryName"].ToString()
                                : "";

                        post.sStateName =
                            dr["sStateName"] != DBNull.Value
                                ? dr["sStateName"].ToString()
                                : "";

                        post.nCityName =
                            dr["nCityName"] != DBNull.Value
                                ? dr["nCityName"].ToString()
                                : "";

                        // ====================================================
                        // WORKING DETAILS
                        // ====================================================

                        post.sWorkingHours =
                            dr["sWorkingHours"] != DBNull.Value
                                ? dr["sWorkingHours"].ToString()
                                : "";

                        post.sWorkingShift =
                            dr["sWorkingShift"] != DBNull.Value
                                ? dr["sWorkingShift"].ToString()
                                : "";

                        // ====================================================
                        // INTERNSHIP TYPE
                        // ====================================================

                        post.nInternshipTypeID =
                            dr["nInternshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nInternshipTypeID"])
                                : 0;

                        post.sInternshipTypeName =
                            dr["sInternshipTypeName"] != DBNull.Value
                                ? dr["sInternshipTypeName"].ToString()
                                : "";

                        // ====================================================
                        // INTERNSHIP / FELLOWSHIP TYPE
                        // ====================================================

                        post.nInternshipFellowshipTypeID =
                            dr["nInternshipFellowshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nInternshipFellowshipTypeID"])
                                : 0;

                        post.sInternshipFellowshipTypeName =
                            dr["sInternshipFellowshipTypeName"] != DBNull.Value
                                ? dr["sInternshipFellowshipTypeName"].ToString()
                                : "";

                        // ====================================================
                        // CHARGES
                        // ====================================================

                        post.sTotalCharges =
                            dr["sTotalCharges"] != DBNull.Value
                                ? Convert.ToDecimal(
                                    dr["sTotalCharges"])
                                : null;

                        post.sCurrency =
                            dr["sCurrency"] != DBNull.Value
                                ? dr["sCurrency"].ToString()
                                : "";

                        // ====================================================
                        // TRAINING
                        // ====================================================

                        post.nTrainingInvolvedID =
                            dr["nTrainingInvolvedID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nTrainingInvolvedID"])
                                : 0;

                        post.sTrainingInvolvedName =
                            dr["sTrainingInvolvedName"] != DBNull.Value
                                ? dr["sTrainingInvolvedName"].ToString()
                                : "";

                        // ====================================================
                        // DURATION
                        // ====================================================

                        post.nInternshipDurationID =
                            dr["nInternshipDurationID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nInternshipDurationID"])
                                : 0;

                        post.sInternshipDurationName =
                            dr["sInternshipDurationName"] != DBNull.Value
                                ? dr["sInternshipDurationName"].ToString()
                                : "";

                        // ====================================================
                        // DATES
                        // ====================================================

                        post.dStartDate =
                            dr["dStartDate"] != DBNull.Value
                                ? Convert.ToDateTime(
                                    dr["dStartDate"])
                                : DateTime.MinValue;

                        post.dCompletionDate =
                            dr["dCompletionDate"] != DBNull.Value
                                ? Convert.ToDateTime(
                                    dr["dCompletionDate"])
                                : DateTime.MinValue;

                        // ====================================================
                        // INTERNSHIP MODE
                        // ====================================================

                        post.nInternshipModeID =
                            dr["nInternshipModeID"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["nInternshipModeID"])
                                : 0;

                        post.sInternshipModeName =
                            dr["sInternshipModeName"] != DBNull.Value
                                ? dr["sInternshipModeName"].ToString()
                                : "";

                        // ====================================================
                        // OTHER DETAILS
                        // ====================================================

                        post.sDivyang =
                            dr["sDivyang"] != DBNull.Value
                                ? dr["sDivyang"].ToString()
                                : "";

                        post.sLanguageKnown =
                            dr["sLanguageKnown"] != DBNull.Value
                                ? dr["sLanguageKnown"].ToString()
                                : "";

                        post.sWorkingDays =
                            dr["sWorkingDays"] != DBNull.Value
                                ? dr["sWorkingDays"].ToString()
                                : "";

                        post.sFacilities =
                            dr["sFacilities"] != DBNull.Value
                                ? dr["sFacilities"].ToString()
                                : "";

                        // ====================================================
                        // SKILLS
                        // ====================================================

                        // ============================================================
                        // SKILLS
                        // ============================================================

                        // Medical Skills
                        post.sMedicalSkills =
                            dr["sMedicalSkills"] != DBNull.Value
                                ? dr["sMedicalSkills"].ToString()
                                : "";

                        // Technical Skills
                        string technicalSkillIds =
                            dr["sTechnicalSkills"] != DBNull.Value
                                ? dr["sTechnicalSkills"].ToString() ?? ""
                                : "";

                        post.sTechnicalSkills =
                            GetTechnicalSkillNames(technicalSkillIds);

                        // Non-Technical Skills
                        post.sNonTechnicalSkills =
                            dr["sNonTechnicalSkills"] != DBNull.Value
                                ? dr["sNonTechnicalSkills"].ToString()
                                : "";

                        // ====================================================
                        // STEP 3:
                        // GET ORGANIZATION NAME FROM EXISTING PROFILE SP
                        // ====================================================

                        post.sName =
                            GetOrganizationNameByID(orgId);

                        return post;
                    }
                }
            }

            return null;
        }

        private string GetTechnicalSkillNames(string? skillIds)
        {
            if (string.IsNullOrWhiteSpace(skillIds))
                return "";

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")!;

            List<string> skillNames =
                new List<string>();

            string[] ids =
                skillIds.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries);

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                foreach (string id in ids)
                {
                    if (!int.TryParse(
                            id.Trim(),
                            out int skillId))
                    {
                        continue;
                    }

                    using (SqlCommand cmd =
                           new SqlCommand(@"
         SELECT sTechnicalSkillName
         FROM tblTechnicalSkills
         WHERE nTechnicalSkillID = @nTechnicalSkillID
     ", con))
                    {
                        cmd.Parameters.Add(
                            "@nTechnicalSkillID",
                            SqlDbType.Int).Value = skillId;

                        object? result =
                            cmd.ExecuteScalar();

                        if (result != null &&
                            result != DBNull.Value)
                        {
                            string name =
                                result.ToString()!.Trim();

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                skillNames.Add(name);
                            }
                        }
                    }
                }
            }

            return string.Join(", ", skillNames);
        }

        private string GetOrganizationNameByID(int orgId)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetOrganizationProfile",
                           con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value = orgId;

                    con.Open();

                    using SqlDataReader dr =
                        cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // Your existing profile SP uses sName
                        // for organization name.
                        if (dr["sName"] != DBNull.Value)
                        {
                            return dr["sName"].ToString() ?? "";
                        }
                    }
                }
            }

            return "";
        }

        private List<OrgPostM> GetAllInternshipPosts()
        {
            List<OrgPostM> posts =
                new List<OrgPostM>();

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")!;

            using SqlConnection con =
                new SqlConnection(connectionString);

            using SqlCommand cmd =
                new SqlCommand(@"
     SELECT
         P.nID,
         P.nOrgID,
         P.nPositionID,
         P.nRequiredTrainees,
         P.nGenderID,
         P.nMinimumQualificationID,

         P.sCountryName,
         P.sStateName,
         P.nCityName,

         P.sWorkingHours,
         P.nInternshipTypeID,
         P.sWorkingShift,
         P.nInternshipFellowshipTypeID,

         P.sTotalCharges,
         P.sCurrency,

         P.nTrainingInvolvedID,
         P.nInternshipDurationID,

         P.dStartDate,
         P.dCompletionDate,

         P.nInternshipModeID,

         P.sDivyang,
         P.sLanguageKnown,
         P.sWorkingDays,
         P.sFacilities,

         P.sMedicalSkills,
         P.sTechnicalSkills,
         P.sNonTechnicalSkills

     FROM tblPost P

     WHERE ISNULL(P.nBit, 0) = 0

     ORDER BY P.nID
 ", con);

            con.Open();

            using SqlDataReader dr =
                cmd.ExecuteReader();

            while (dr.Read())
            {
                OrgPostM post =
                    new OrgPostM();

                // =====================================================
                // BASIC
                // =====================================================

                post.nID =
                    dr["nID"] != DBNull.Value
                        ? Convert.ToInt32(dr["nID"])
                        : 0;

                post.nOrgID =
                    dr["nOrgID"] != DBNull.Value
                        ? Convert.ToInt32(dr["nOrgID"])
                        : 0;


                // =====================================================
                // POSITION
                // =====================================================

                post.nPositionID =
                    dr["nPositionID"] != DBNull.Value
                        ? Convert.ToInt32(dr["nPositionID"])
                        : 0;


                // =====================================================
                // REQUIRED TRAINEES
                // =====================================================

                post.nRequiredTrainees =
                    dr["nRequiredTrainees"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nRequiredTrainees"])
                        : 0;


                // =====================================================
                // GENDER
                // =====================================================

                post.nGenderID =
                    dr["nGenderID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nGenderID"])
                        : 0;


                // =====================================================
                // QUALIFICATION
                // =====================================================

                post.nMinimumQualificationID =
                    dr["nMinimumQualificationID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nMinimumQualificationID"])
                        : 0;


                // =====================================================
                // LOCATION
                // =====================================================

                post.sCountryName =
                    dr["sCountryName"] != DBNull.Value
                        ? dr["sCountryName"].ToString()
                        : "";

                post.sStateName =
                    dr["sStateName"] != DBNull.Value
                        ? dr["sStateName"].ToString()
                        : "";

                post.nCityName =
                    dr["nCityName"] != DBNull.Value
                        ? dr["nCityName"].ToString()
                        : "";


                // =====================================================
                // WORKING DETAILS
                // =====================================================

                post.sWorkingHours =
                    dr["sWorkingHours"] != DBNull.Value
                        ? dr["sWorkingHours"].ToString()
                        : "";

                post.sWorkingShift =
                    dr["sWorkingShift"] != DBNull.Value
                        ? dr["sWorkingShift"].ToString()
                        : "";

                post.sWorkingDays =
                    dr["sWorkingDays"] != DBNull.Value
                        ? dr["sWorkingDays"].ToString()
                        : "";


                // =====================================================
                // INTERNSHIP TYPE
                // =====================================================

                post.nInternshipTypeID =
                    dr["nInternshipTypeID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nInternshipTypeID"])
                        : 0;


                // =====================================================
                // INTERNSHIP / FELLOWSHIP TYPE
                // =====================================================

                post.nInternshipFellowshipTypeID =
                    dr["nInternshipFellowshipTypeID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nInternshipFellowshipTypeID"])
                        : 0;


                // =====================================================
                // CHARGES
                // =====================================================

                post.sTotalCharges =
                    dr["sTotalCharges"] != DBNull.Value
                        ? Convert.ToDecimal(
                            dr["sTotalCharges"])
                        : null;

                post.sCurrency =
                    dr["sCurrency"] != DBNull.Value
                        ? dr["sCurrency"].ToString()
                        : "";


                // =====================================================
                // TRAINING
                // =====================================================

                post.nTrainingInvolvedID =
                    dr["nTrainingInvolvedID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nTrainingInvolvedID"])
                        : 0;


                // =====================================================
                // DURATION
                // =====================================================

                post.nInternshipDurationID =
                    dr["nInternshipDurationID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nInternshipDurationID"])
                        : 0;


                // =====================================================
                // DATES
                // =====================================================

                post.dStartDate =
                    dr["dStartDate"] != DBNull.Value
                        ? Convert.ToDateTime(
                            dr["dStartDate"])
                        : DateTime.MinValue;

                post.dCompletionDate =
                    dr["dCompletionDate"] != DBNull.Value
                        ? Convert.ToDateTime(
                            dr["dCompletionDate"])
                        : DateTime.MinValue;


                // =====================================================
                // INTERNSHIP MODE
                // =====================================================

                post.nInternshipModeID =
                    dr["nInternshipModeID"] != DBNull.Value
                        ? Convert.ToInt32(
                            dr["nInternshipModeID"])
                        : 0;


                // =====================================================
                // OTHER DETAILS
                // =====================================================

                post.sDivyang =
                    dr["sDivyang"] != DBNull.Value
                        ? dr["sDivyang"].ToString()
                        : "";

                post.sLanguageKnown =
                    dr["sLanguageKnown"] != DBNull.Value
                        ? dr["sLanguageKnown"].ToString()
                        : "";

                post.sFacilities =
                    dr["sFacilities"] != DBNull.Value
                        ? dr["sFacilities"].ToString()
                        : "";


                // =====================================================
                // SKILLS
                // =====================================================

                post.sMedicalSkills =
                    dr["sMedicalSkills"] != DBNull.Value
                        ? dr["sMedicalSkills"].ToString()
                        : "";

                post.sTechnicalSkills =
                    dr["sTechnicalSkills"] != DBNull.Value
                        ? dr["sTechnicalSkills"].ToString()
                        : "";

                post.sNonTechnicalSkills =
                    dr["sNonTechnicalSkills"] != DBNull.Value
                        ? dr["sNonTechnicalSkills"].ToString()
                        : "";


                // =====================================================
                // ADD
                // =====================================================

                posts.Add(post);
            }

            return posts;
        }

        [HttpGet]
        [Route("Candidate/ExtraPost/{id?}")]
        public IActionResult ExtraPost(string? id)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =====================================================
            // GET MATCHING EXTRA POSTS
            // =====================================================

            var extraPosts =
                new List<OrgPostM>();

            using (SqlConnection con =
                new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd =
                    new SqlCommand(
                        "GetMatchingJobsForCandidateExtraPost",
                        con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@CandidateID",
                        candidateId.Value);

                    con.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new OrgPostM();

                            // =================================================
                            // BASIC POST INFORMATION
                            // =================================================

                            post.nID =
                                reader["PostID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(reader["PostID"]);

                            post.nOrgID =
                                reader["OrgID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(reader["OrgID"]);

                            post.sName =
                                reader["OrganizationName"] == DBNull.Value
                                    ? ""
                                    : reader["OrganizationName"].ToString();

                            post.sPositionName =
                                reader["PositionName"] == DBNull.Value
                                    ? ""
                                    : reader["PositionName"].ToString();

                            // =================================================
                            // LOCATION
                            // =================================================

                            post.sCountryName =
                                reader["Country"] == DBNull.Value
                                    ? ""
                                    : reader["Country"].ToString();

                            post.sStateName =
                                reader["State"] == DBNull.Value
                                    ? ""
                                    : reader["State"].ToString();

                            post.nCityName =
                                reader["City"] == DBNull.Value
                                    ? ""
                                    : reader["City"].ToString();

                            // =================================================
                            // INTERNSHIP / FELLOWSHIP
                            // =================================================

                            post.sInternshipFellowshipTypeName =
                                reader["InternshipFellowshipType"] == DBNull.Value
                                    ? ""
                                    : reader["InternshipFellowshipType"].ToString();

                            // =================================================
                            // FACILITIES
                            // =================================================

                            post.sFacilities =
                                reader["sFacilities"] == DBNull.Value
                                    ? ""
                                    : reader["sFacilities"].ToString();

                            // =================================================
                            // APPLICATION STATUS
                            // =================================================

                            post.IsApplied =
                                reader["CApply"] != DBNull.Value &&
                                Convert.ToInt32(reader["CApply"]) == 1;

                            // =================================================
                            // OTHER DETAILS
                            // =================================================

                            if (reader["dStartDate"] != DBNull.Value)
                            {
                                post.dStartDate =
                                    Convert.ToDateTime(
                                        reader["dStartDate"]);
                            }

                            if (reader["dCompletionDate"] != DBNull.Value)
                            {
                                post.dCompletionDate =
                                    Convert.ToDateTime(
                                        reader["dCompletionDate"]);
                            }

                            post.nRequiredTrainees =
                                reader["nRequiredTrainees"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        reader["nRequiredTrainees"]);

                            post.sWorkingHours =
                                reader["sWorkingHours"] == DBNull.Value
                                    ? ""
                                    : reader["sWorkingHours"].ToString();

                            post.sWorkingShift =
                                reader["sWorkingShift"] == DBNull.Value
                                    ? ""
                                    : reader["sWorkingShift"].ToString();

                            post.sCurrency =
                                reader["sCurrency"] == DBNull.Value
                                    ? ""
                                    : reader["sCurrency"].ToString();

                            post.sTotalCharges =
     reader["sTotalCharges"] == DBNull.Value
         ? (decimal?)null
         : Convert.ToDecimal(reader["sTotalCharges"]);

                            post.sMedicalSkills =
                                reader["JobMedicalSkills"] == DBNull.Value
                                    ? ""
                                    : reader["JobMedicalSkills"].ToString();

                            post.sTechnicalSkills =
                                reader["JobTechnicalSkills"] == DBNull.Value
                                    ? ""
                                    : reader["JobTechnicalSkills"].ToString();

                            post.sNonTechnicalSkills =
                                reader["JobNonTechnicalSkills"] == DBNull.Value
                                    ? ""
                                    : reader["JobNonTechnicalSkills"].ToString();

                            post.sDivyang =
                                reader["sDivyang"] == DBNull.Value
                                    ? ""
                                    : reader["sDivyang"].ToString();

                            post.sLanguageKnown =
                                reader["sLanguageKnown"] == DBNull.Value
                                    ? ""
                                    : reader["sLanguageKnown"].ToString();

                            post.sWorkingDays =
                                reader["sWorkingDays"] == DBNull.Value
                                    ? ""
                                    : reader["sWorkingDays"].ToString();

                            extraPosts.Add(post);
                        }
                    }
                }
            }

            // =====================================================
            // SEND DATA TO VIEW
            // =====================================================

            ViewBag.InternshipPosts = extraPosts;

            ViewBag.candidateCode = id;



            // =========================================================
            // PASS ID TO VIEW
            // =========================================================

            ViewBag.InternshipId = id;

            return View();
        }

        //radhika07-10

        [HttpGet]
        public IActionResult TotalOpening()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TopSearch()
        {
            return View();
        }
    }
}
