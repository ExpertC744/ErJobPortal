using ErJobPortal.Models;
using ErJobPortal.Repositories;
using JobPortalTrainee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JobPortalTrainee.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public OrganizationController(
    IConfiguration configuration,
    AccountRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }


        // =========================================================
        // DASHBOARD
        // =========================================================


        [HttpGet]
        [ResponseCache(
      NoStore = true,
      Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard()
        {
            int? orgId = HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction("OrganizationLogin", "Account");
            }

            ViewBag.OrgID = orgId.Value;

            ViewBag.OrgName = HttpContext.Session.GetString("OrgName");

            ViewBag.OrgEmail = HttpContext.Session.GetString("OrgEmail");
            int traineeRegistrationCount = 0;
            int organizationRegistrationCount = 0;

            // =========================================================
            // CHART DATA
            // =========================================================

            int internshipEligibleCount = 0;

            List<int> monthlyCandidateRegistrations =
                Enumerable.Repeat(0, 12).ToList();



            string connectionString =
      _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // =========================================================
                // TOTAL TRAINEES + TOTAL ORGANIZATIONS
                // =========================================================

                string query = @"
        SELECT
            (SELECT COUNT(nID)
             FROM tblCandidateRegister) AS CandidateCount,

            (SELECT COUNT(nID)
             FROM tblOrgRegistration) AS OrganizationCount;
    ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            traineeRegistrationCount =
                                Convert.ToInt32(dr["CandidateCount"]);

                            organizationRegistrationCount =
                                Convert.ToInt32(dr["OrganizationCount"]);
                        }
                    }
                }


                // =========================================================
                // ELIGIBLE TRAINEES FOR INTERNSHIP
                // =========================================================
                //
                // IMPORTANT:
                // Change the WHERE condition according to your actual
                // internship eligibility column/logic.
                //
                // Example:
                // WHERE nBit = 1
                //
                // If every registered trainee is eligible, simply use:
                // SELECT COUNT(nID) FROM tblCandidateRegister
                //
                // =========================================================

                string eligibleQuery = @"
        SELECT COUNT(nID)
        FROM tblCandidateRegister
        WHERE ISNULL(nBit, 1) = 1;
    ";

                using (SqlCommand cmd = new SqlCommand(eligibleQuery, con))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        internshipEligibleCount =
                            Convert.ToInt32(result);
                    }
                }


                // =========================================================
                // MONTHLY TRAINEE REGISTRATION
                // CURRENT YEAR
                // =========================================================

                string monthlyQuery = @"
        SELECT
            MONTH(RegDate) AS RegistrationMonth,
            COUNT(nID) AS RegistrationCount
        FROM tblCandidateRegister
        WHERE YEAR(RegDate) = YEAR(GETDATE())
        GROUP BY MONTH(RegDate)
        ORDER BY MONTH(RegDate);
    ";

                using (SqlCommand cmd =
                       new SqlCommand(monthlyQuery, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int month =
                                Convert.ToInt32(
                                    dr["RegistrationMonth"]);

                            int count =
                                Convert.ToInt32(
                                    dr["RegistrationCount"]);

                            if (month >= 1 && month <= 12)
                            {
                                monthlyCandidateRegistrations[month - 1] =
                                    count;
                            }
                        }
                    }
                }
            }
            // Get complete trainee list
            List<SATraineeListM> trainees =
                _repository.GetAllTrainees();

            List<OrganizationUser> organizations =
                _repository.GetAllOrganizationList();

            ViewBag.TraineeRegistrationCount =
                traineeRegistrationCount;

            ViewBag.OrganizationRegistrationCount =
                organizationRegistrationCount;

            ViewBag.Trainees =
                trainees;

            ViewBag.Organizations =
                organizations;


            // =========================================================
            // CHART DATA
            // =========================================================

            ViewBag.InternshipEligibleCount =
                internshipEligibleCount;

            ViewBag.MonthlyCandidateRegistrations =
                monthlyCandidateRegistrations;


            return View();


        }


        public IActionResult CreatePost()
        {
            return View();
        }

        // ==========================================
        // CANDIDATE LIST
        // ==========================================
        [HttpGet]
        public IActionResult TraineeList()
        {
            List<SATraineeListM> trainees = _repository.GetSATraineeList();
            return View(trainees);
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
        // EDIT PROFILE - GET
        // =========================================================

        [HttpGet]
        public IActionResult EditProfile()
        {
            int? orgId = HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction(
                    "OrganizationLogin",
                    "Account");
            }


            OrgProfile model = new OrgProfile
            {
                nOrgID = orgId.Value
            };


            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")!;


            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetOrganizationProfile",
                           cn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;


                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value =
                        orgId.Value;


                    cn.Open();


                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {


                            model.nID =
                                dr["nID"] != DBNull.Value
                                    ? Convert.ToInt32(
                                        dr["nID"])
                                    : 0;


                            model.nOrgID =
                                dr["nOrgID"] != DBNull.Value
                                    ? Convert.ToInt32(
                                        dr["nOrgID"])
                                    : orgId.Value;



                            // Database column = sName
                            // Model property = sOrganizationName

                            model.sOrganizationName =
                                dr["sName"] != DBNull.Value
                                    ? dr["sName"].ToString()!
                                    : string.Empty;


                            // Database column = sEmail
                            // Model property = sOrganizationEmail

                            model.sOrganizationEmail =
                                dr["sEmail"] != DBNull.Value
                                    ? dr["sEmail"].ToString()!
                                    : string.Empty;


                            model.sMobile =
                                dr["sMobile"] != DBNull.Value
                                    ? dr["sMobile"].ToString()!
                                    : string.Empty;


                            model.sDesignation =
                                dr["sDesignation"] != DBNull.Value
                                    ? dr["sDesignation"].ToString()!
                                    : string.Empty;


                            model.dDateOfBirth =
                                dr["dDateOfBirth"] != DBNull.Value
                                    ? Convert.ToDateTime(
                                        dr["dDateOfBirth"])
                                    : null;


                            model.sCompanyLogo =
                                dr["sCompanyLogo"] != DBNull.Value
                                    ? dr["sCompanyLogo"].ToString()!
                                    : string.Empty;


                            model.sCompanyAddress =
                                dr["sCompanyAddress"] != DBNull.Value
                                    ? dr["sCompanyAddress"].ToString()!
                                    : string.Empty;


                            model.nEstablishmentYear =
                                dr["nEstablishmentYear"] != DBNull.Value
                                    ? Convert.ToInt32(
                                        dr["nEstablishmentYear"])
                                    : 0;


                            model.sGSTNo =
                                dr["sGSTNo"] != DBNull.Value
                                    ? dr["sGSTNo"].ToString()!
                                    : string.Empty;


                            model.sCINNo =
                                dr["sCINNo"] != DBNull.Value
                                    ? dr["sCINNo"].ToString()!
                                    : string.Empty;


                            model.nEmployeeStrength =
                                dr["nEmployeeStrength"] != DBNull.Value
                                    ? dr["nEmployeeStrength"].ToString()!
                                    : string.Empty;


                            model.dCreatedDate =
                                dr["dCreatedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(
                                        dr["dCreatedDate"])
                                    : DateTime.MinValue;


                            model.dModifiedDate =
                                dr["dModifiedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(
                                        dr["dModifiedDate"])
                                    : null;


                            model.nBit =
                                dr["nBit"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    dr["nBit"]);


                            model.nSABit =
                                dr["nSABit"] != DBNull.Value
                                    ? Convert.ToBoolean(
                                        dr["nSABit"])
                                    : null;
                        }
                        else
                        {


                            model.nOrgID =
                                orgId.Value;
                        }
                    }
                }
            }


            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(OrgProfile model, IFormFile? CompanyLogo)
        {
            int? orgId = HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction("OrganizationLogin", "Account");
            }
            model.nOrgID = orgId.Value;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            string logoPath = model.sCompanyLogo ?? string.Empty;

            if (CompanyLogo != null && CompanyLogo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "organization");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string extension = Path.GetExtension(CompanyLogo.FileName).ToLowerInvariant();

                string[] allowedExtensions =
                {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("CompanyLogo", "Only JPG, JPEG, PNG and WEBP files are allowed.");
                    return View(model);
                }

                if (CompanyLogo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("CompanyLogo", "Company logo size cannot exceed 5 MB.");
                    return View(model);
                }


                string fileName = Path.GetFileName(CompanyLogo.FileName);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    CompanyLogo.CopyTo(stream);
                }


                logoPath = fileName;
            }


            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddOrganizationProfile", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value =
                        model.nOrgID;


                    cmd.Parameters.Add(
                        "@dDateOfBirth",
                        SqlDbType.DateTime).Value =
                        model.dDateOfBirth.HasValue
                            ? model.dDateOfBirth.Value
                            : DBNull.Value;


                    cmd.Parameters.Add(
                        "@sCompanyLogo",
                        SqlDbType.NVarChar,
                        500).Value =
                        string.IsNullOrWhiteSpace(
                            logoPath)
                            ? DBNull.Value
                            : logoPath;



                    cmd.Parameters.Add(
                        "@sCompanyAddress",
                        SqlDbType.NVarChar,
                        -1).Value =
                        string.IsNullOrWhiteSpace(
                            model.sCompanyAddress)
                            ? DBNull.Value
                            : model.sCompanyAddress;


                    cmd.Parameters.Add(
                        "@nEstablishmentYear",
                        SqlDbType.Int).Value =
                        model.nEstablishmentYear;



                    cmd.Parameters.Add(
                        "@sGSTNo",
                        SqlDbType.NVarChar,
                        50).Value =
                        string.IsNullOrWhiteSpace(
                            model.sGSTNo)
                            ? DBNull.Value
                            : model.sGSTNo;



                    cmd.Parameters.Add(
                        "@sCINNo",
                        SqlDbType.NVarChar,
                        50).Value =
                        string.IsNullOrWhiteSpace(
                            model.sCINNo)
                            ? DBNull.Value
                            : model.sCINNo;

                    cmd.Parameters.Add("@nEmployeeStrength", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(model.nEmployeeStrength) ? DBNull.Value : model.nEmployeeStrength;
                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Organization profile updated successfully.";
            return RedirectToAction("EditProfile");
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
            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================================
        // NO CACHE
        // =========================================================

        private void SetNoCacheHeaders()
        {
            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";
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

            int? OrgID =
                HttpContext.Session.GetInt32("OrgID");

            if (OrgID == null || OrgID <= 0)
            {
                return RedirectToAction("CreateFeedback", "Organization");
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
            FROM tblSAOrgFeedback
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

                                nOrgID = OrgID.Value,

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

            int? OrgID =
                HttpContext.Session.GetInt32("OrgID");

            if (OrgID == null || OrgID <= 0)
            {
                TempData["Error"] =
    "Organization session expired. Please login again.";

                return RedirectToAction("CreateFeedback", "Organization");
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
            INSERT INTO tblOrgFeedback
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
                            OrgID.Value;

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

            TempData["Success"] = "Feedback submitted successfully.";

            return RedirectToAction(
                "CreateFeedback",
                "Organization");
        }

    }
}