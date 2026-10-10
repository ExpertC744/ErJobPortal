using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Net;


namespace ErJobPortal.Controllers
{
    public class TPOController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        private readonly TPOSchedulerRepository _schedulerRepository;

        public TPOController(
            AccountRepository accountRepository,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            TPOSchedulerRepository schedulerRepository)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _environment = environment;
            _schedulerRepository = schedulerRepository;
        }


        // =====================================================
        // TPO DASHBOARD
        // =====================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName")
                ?? "TPO";

            return View();
        }


        // =====================================================
        // VIEW TPO + SUB TPO DETAILS
        // =====================================================

        [HttpGet]
        public IActionResult ViewTPODetails()
        {
            // =================================================
            // CHECK LOGIN
            // =================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =================================================
            // GET ALL TPO + SUB TPO
            // =================================================

            List<TPORegistration> tpoList =
                _accountRepository.GetAllTPODetails();


            // =================================================
            // CHECK DATA
            // =================================================

            if (tpoList == null ||
                tpoList.Count == 0)
            {
                TempData["Error"] =
                    "TPO details not found.";

                return RedirectToAction(
                    "Dashboard");
            }


            // =================================================
            // PAGE INFORMATION
            // =================================================

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName")
                ?? "TPO";

            ViewData["Title"] =
                "TPO Details";


            // =================================================
            // SEND LIST TO VIEW
            // =================================================

            return View(tpoList);
        }


        // =====================================================
        // ADD SUB TPO - GET
        // =====================================================

        [HttpGet]
        public IActionResult AddSubTPO()
        {
            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName")
                ?? "TPO";

            ViewData["Title"] =
                "Add Sub TPO";

            return View();
        }


        // =====================================================
        // ADD SUB TPO - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSubTPO(
            string FullName,
            string MobileNo,
            string CollegeMailID,
            string CollegeEmployeeID,
            string CollegeName,
            string CollegeCode,
            string CollegeAddress,
            string OrganizationWebsiteURL,
            string Designation,
            string DepartmentName,
            string OTP,
            string Password,
            string ConfirmPassword,
            IFormFile SupportingDocument1,
            IFormFile SupportingDocument2,
            IFormFile ProfilePhoto)
        {
            // =================================================
            // CHECK LOGIN
            // =================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =================================================
            // BASIC VALIDATION
            // =================================================

            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(MobileNo) ||
                string.IsNullOrWhiteSpace(CollegeMailID) ||
                string.IsNullOrWhiteSpace(CollegeEmployeeID) ||
                string.IsNullOrWhiteSpace(CollegeName) ||
                string.IsNullOrWhiteSpace(CollegeCode) ||
                string.IsNullOrWhiteSpace(CollegeAddress) ||
                string.IsNullOrWhiteSpace(Designation) ||
                string.IsNullOrWhiteSpace(DepartmentName))
            {
                TempData["Error"] =
                    "Please fill all required fields.";

                return RedirectToAction(
                    "AddSubTPO");
            }


            // =================================================
            // PASSWORD VALIDATION
            // =================================================

            if (string.IsNullOrWhiteSpace(Password) ||
                Password != ConfirmPassword)
            {
                TempData["Error"] =
                    "Password and Confirm Password do not match.";

                return RedirectToAction(
                    "AddSubTPO");
            }


            // =================================================
            // OTP
            // =================================================

            if (string.IsNullOrWhiteSpace(OTP))
            {
                TempData["Error"] =
                    "Please enter OTP.";

                return RedirectToAction(
                    "AddSubTPO");
            }


            // =================================================
            // FILE VALIDATION
            // =================================================

            if (SupportingDocument1 == null ||
                SupportingDocument1.Length == 0 ||
                SupportingDocument2 == null ||
                SupportingDocument2.Length == 0 ||
                ProfilePhoto == null ||
                ProfilePhoto.Length == 0)
            {
                TempData["Error"] =
                    "Please upload all required documents and profile photo.";

                return RedirectToAction(
                    "AddSubTPO");
            }


            try
            {
                // =================================================
                // UPLOAD FOLDER
                // =================================================

                string uploadFolder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Uploads",
                        "TPO");


                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(
                        uploadFolder);
                }


                // =================================================
                // DOCUMENT 1
                // =================================================

                string doc1Extension =
                    Path.GetExtension(
                        SupportingDocument1.FileName);

                string doc1FileName =
                    "TPO_DOC1_" +
                    Guid.NewGuid().ToString("N") +
                    doc1Extension;

                string doc1Path =
                    Path.Combine(
                        uploadFolder,
                        doc1FileName);

                using (var stream =
                       new FileStream(
                           doc1Path,
                           FileMode.Create))
                {
                    SupportingDocument1.CopyTo(stream);
                }


                // =================================================
                // DOCUMENT 2
                // =================================================

                string doc2Extension =
                    Path.GetExtension(
                        SupportingDocument2.FileName);

                string doc2FileName =
                    "TPO_DOC2_" +
                    Guid.NewGuid().ToString("N") +
                    doc2Extension;

                string doc2Path =
                    Path.Combine(
                        uploadFolder,
                        doc2FileName);

                using (var stream =
                       new FileStream(
                           doc2Path,
                           FileMode.Create))
                {
                    SupportingDocument2.CopyTo(stream);
                }


                // =================================================
                // PROFILE PHOTO
                // =================================================

                string photoExtension =
                    Path.GetExtension(
                        ProfilePhoto.FileName);

                string photoFileName =
                    "TPO_PROFILE_" +
                    Guid.NewGuid().ToString("N") +
                    photoExtension;

                string photoPath =
                    Path.Combine(
                        uploadFolder,
                        photoFileName);

                using (var stream =
                       new FileStream(
                           photoPath,
                           FileMode.Create))
                {
                    ProfilePhoto.CopyTo(stream);
                }


                // =================================================
                // CREATE SUB TPO MODEL
                // =================================================

                TPORegistration subTPO =
                    new TPORegistration
                    {
                        FullName =
                            FullName.Trim(),

                        CollegeMailID =
                            CollegeMailID.Trim(),

                        MobileNo =
                            MobileNo.Trim(),

                        Password =
                            Password,

                        CollegeName =
                            CollegeName.Trim(),

                        CollegeCode =
                            CollegeCode.Trim(),

                        CollegeAddress =
                            CollegeAddress.Trim(),

                        OrganizationWebsiteURL =
                            OrganizationWebsiteURL?.Trim(),

                        Designation =
                            Designation.Trim(),

                        DepartmentName =
                            DepartmentName.Trim(),

                        CollegeEmployeeID =
                            CollegeEmployeeID.Trim(),

                        SupportingDocument1 =
                            doc1FileName,

                        SupportingDocument2 =
                            doc2FileName,

                        ProfilePhoto =
                            photoFileName,

                        OTP =
                            OTP.Trim(),

                        OTPVerified =
                            true,

                        Status =
                            "Pending",

                        IsApproved =
                            false,

                        IsActive =
                            false,

                        CreatedDate =
                            DateTime.Now,

                        UpdatedDate =
                            DateTime.Now
                    };


                // =================================================
                // INSERT
                // =================================================

                bool result =
                    _accountRepository.InsertSubTPO(
                        subTPO);


                // =================================================
                // INSERT FAILED
                // =================================================

                if (!result)
                {
                    TempData["Error"] =
                        "Unable to register Sub TPO.";

                    return RedirectToAction(
                        "AddSubTPO");
                }


                // =================================================
                // SUCCESS
                // =================================================

                TempData["Success"] =
                    "Sub TPO registered successfully. It is pending Super Admin approval.";

                return RedirectToAction(
                    "AddSubTPO");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error while registering Sub TPO: "
                    + ex.Message;

                return RedirectToAction(
                    "AddSubTPO");
            }
        }




        //=============================================================
        //TPO CREATE FEEDBACK - GET
        //=============================================================

        [HttpGet]
        public IActionResult CreateFeedback()
        {
            // ---------------------------------------------------------
            // CHECK TPO LOGIN
            // ---------------------------------------------------------

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // ---------------------------------------------------------
            // CREATE MODEL
            // ---------------------------------------------------------

            TPOFeedbackViewModel? model = null;


            // ---------------------------------------------------------
            // CONNECTION STRING
            // ---------------------------------------------------------

            string? connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");


            // ---------------------------------------------------------
            // GET ACTIVE FEEDBACK QUESTIONS
            // ---------------------------------------------------------

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
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
                    con.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new TPOFeedbackViewModel
                            {
                                nID =
                                    reader["nID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        reader["nID"]),

                                Que1 =
                                    reader["Que1"] == DBNull.Value
                                    ? ""
                                    : reader["Que1"].ToString(),

                                Que2 =
                                    reader["Que2"] == DBNull.Value
                                    ? ""
                                    : reader["Que2"].ToString(),

                                Que3 =
                                    reader["Que3"] == DBNull.Value
                                    ? ""
                                    : reader["Que3"].ToString(),

                                Que4 =
                                    reader["Que4"] == DBNull.Value
                                    ? ""
                                    : reader["Que4"].ToString(),

                                Que5 =
                                    reader["Que5"] == DBNull.Value
                                    ? ""
                                    : reader["Que5"].ToString(),

                                // TPO answers initially empty
                                sQue1 = "",
                                sQue2 = "",
                                sQue3 = "",
                                sQue4 = "",
                                sQue5 = "",

                                TPOID = tpoId.Value,

                                nBit =
                                    reader["nBit"] == DBNull.Value
                                    ? 1
                                    : Convert.ToInt32(
                                        reader["nBit"]),

                                nSABit =
                                    reader["nSABit"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        reader["nSABit"]),

                                nTPO = 1
                            };
                        }
                    }
                }
            }


            // ---------------------------------------------------------
            // NO QUESTION FOUND
            // ---------------------------------------------------------

            if (model == null)
            {
                return NotFound(
                    "TPO feedback questions were not found.");
            }


            // ---------------------------------------------------------
            // VIEW DATA
            // ---------------------------------------------------------

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName")
                ?? "TPO";

            ViewData["Title"] = "Create Feedback";


            return View(model);
        }

        // =============================================================
        // TPO CREATE FEEDBACK - POST
        // =============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateFeedback(
            TPOFeedbackViewModel model)
        {
            // ---------------------------------------------------------
            // CHECK TPO LOGIN
            // ---------------------------------------------------------

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // ---------------------------------------------------------
            // ALWAYS TAKE TPO ID FROM SESSION
            // DO NOT TRUST FORM VALUE
            // ---------------------------------------------------------

            model.TPOID = tpoId.Value;


            // =========================================================
            // SERVER SIDE VALIDATION
            // =========================================================

            // ---------------------------------------------------------
            // QUESTION 1
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.sQue1))
            {
                ModelState.AddModelError(
                    "sQue1",
                    "Please answer Question 1.");
            }


            // ---------------------------------------------------------
            // QUESTION 2
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.sQue2))
            {
                ModelState.AddModelError(
                    "sQue2",
                    "Please select a rating for Question 2.");
            }
            else
            {
                if (!int.TryParse(
                    model.sQue2,
                    out int rating2) ||
                    rating2 < 1 ||
                    rating2 > 5)
                {
                    ModelState.AddModelError(
                        "sQue2",
                        "Please select a valid rating.");
                }
            }


            // ---------------------------------------------------------
            // QUESTION 3
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.sQue3))
            {
                ModelState.AddModelError(
                    "sQue3",
                    "Please answer Question 3.");
            }


            // ---------------------------------------------------------
            // QUESTION 4
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.sQue4))
            {
                ModelState.AddModelError(
                    "sQue4",
                    "Please select a rating for Question 4.");
            }
            else
            {
                if (!int.TryParse(
                    model.sQue4,
                    out int rating4) ||
                    rating4 < 1 ||
                    rating4 > 5)
                {
                    ModelState.AddModelError(
                        "sQue4",
                        "Please select a valid rating.");
                }
            }


            // =========================================================
            // IF VALIDATION FAILS
            // =========================================================

            if (!ModelState.IsValid)
            {
                LoadTPOFeedbackQuestions(model);

                ViewData["Panel"] = "TPO";

                ViewData["UserName"] =
                    HttpContext.Session.GetString("TPOName")
                    ?? "TPO";

                ViewData["Title"] = "Create Feedback";

                return View(model);
            }


            // =========================================================
            // CONVERT TPO ANSWERS
            // =========================================================

            int answer1 =
                string.Equals(
                    model.sQue1,
                    "True",
                    StringComparison.OrdinalIgnoreCase)
                ? 1
                : 0;


            int answer2 =
                Convert.ToInt32(model.sQue2);


            int answer3 =
                string.Equals(
                    model.sQue3,
                    "Yes",
                    StringComparison.OrdinalIgnoreCase)
                ? 1
                : 0;


            int answer4 =
                Convert.ToInt32(model.sQue4);


            string answer5 =
                model.sQue5?.Trim() ?? "";


            // =========================================================
            // INSERT TPO FEEDBACK
            // =========================================================

            string? connectionString =
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
                nSABit,
                nTPO
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
                0,
                1
            )";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    // -------------------------------------------------
                    // QUESTION 1
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@sQue1",
                        SqlDbType.Int).Value =
                            answer1;


                    // -------------------------------------------------
                    // QUESTION 2
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@sQue2",
                        SqlDbType.Int).Value =
                            answer2;


                    // -------------------------------------------------
                    // QUESTION 3
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@sQue3",
                        SqlDbType.Int).Value =
                            answer3;


                    // -------------------------------------------------
                    // QUESTION 4
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@sQue4",
                        SqlDbType.Int).Value =
                            answer4;


                    // -------------------------------------------------
                    // QUESTION 5
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@sQue5",
                        SqlDbType.NVarChar,
                        1000).Value =
                            string.IsNullOrWhiteSpace(answer5)
                            ? DBNull.Value
                            : answer5;


                    // -------------------------------------------------
                    // TPO ID
                    // -------------------------------------------------

                    cmd.Parameters.Add(
                        "@nAdminID",
                        SqlDbType.Int).Value =
                            tpoId.Value;


                    // -------------------------------------------------
                    // EXECUTE
                    // -------------------------------------------------

                    con.Open();

                    int rows =
                        cmd.ExecuteNonQuery();


                    if (rows <= 0)
                    {
                        TempData["Error"] =
                            "TPO feedback could not be submitted.";

                        return View(model);
                    }
                }
            }


            // =========================================================
            // SUCCESS
            // =========================================================

            TempData["Success"] =
                "Feedback submitted successfully.";


            return RedirectToAction(
                "CreateFeedback",
                "TPO");
        }

        // =============================================================
        // LOAD TPO FEEDBACK QUESTIONS
        // =============================================================

        private void LoadTPOFeedbackQuestions(
            TPOFeedbackViewModel model)
        {
            string? connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");


            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
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
                    con.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.nID =
                                reader["nID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["nID"]);


                            model.Que1 =
                                reader["Que1"] == DBNull.Value
                                ? ""
                                : reader["Que1"].ToString();


                            model.Que2 =
                                reader["Que2"] == DBNull.Value
                                ? ""
                                : reader["Que2"].ToString();


                            model.Que3 =
                                reader["Que3"] == DBNull.Value
                                ? ""
                                : reader["Que3"].ToString();


                            model.Que4 =
                                reader["Que4"] == DBNull.Value
                                ? ""
                                : reader["Que4"].ToString();


                            model.Que5 =
                                reader["Que5"] == DBNull.Value
                                ? ""
                                : reader["Que5"].ToString();


                            model.nBit =
                                reader["nBit"] == DBNull.Value
                                ? 1
                                : Convert.ToInt32(
                                    reader["nBit"]);


                            model.nSABit =
                                reader["nSABit"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["nSABit"]);
                        }
                    }
                }
            }


            model.nTPO = 1;
        }

        [HttpGet]
        [Route("TPO/EditProfile")]
        public IActionResult EditProfile()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            TPORegistration? model = _accountRepository.GetTPOById(tpoId.Value);

            if (model == null)
            {
                TempData["Error"] = "TPO profile not found.";
                return RedirectToAction("Dashboard", "TPO");
            }

            ViewData["Panel"] = "TPO";
            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName") ?? "TPO";

            ViewData["Title"] = "Edit TPO Profile";

            return View(model);
        }

        [HttpGet]
        [Route("TPO/ViewProfile")]
        public IActionResult ViewProfile()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            TPORegistration? model =
                _accountRepository.GetTPOById(tpoId.Value);

            if (model == null)
            {
                TempData["Error"] = "TPO profile not found.";
                return RedirectToAction("Dashboard", "TPO");
            }

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName") ?? "TPO";

            ViewData["Title"] = "View TPO Profile";

            return View(model);
        }

        //[HttpGet]
        //[Route("TPO/CreateNotification")]
        //public IActionResult CreateNotification()
        //{


        //    return View();
        //}

        //[HttpGet]
        //[Route("TPO/SendNotification")]
        //public IActionResult SendNotification()
        //{


        //    return View();
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TPOEditProfile(TPORegistration model)
        {
            // =====================================================
            // CHECK TPO LOGIN
            // =====================================================

            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }

            // =====================================================
            // ALWAYS TAKE TPO ID FROM SESSION
            // =====================================================

            model.TPOID = tpoId.Value;


            // =====================================================
            // SERVER SIDE VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full Name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.MobileNo))
            {
                ModelState.AddModelError(
                    "MobileNo",
                    "Mobile Number is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CollegeMailID))
            {
                ModelState.AddModelError(
                    "CollegeMailID",
                    "College Mail ID is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CollegeName))
            {
                ModelState.AddModelError(
                    "CollegeName",
                    "College Name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CollegeAddress))
            {
                ModelState.AddModelError(
                    "CollegeAddress",
                    "College Address is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Designation))
            {
                ModelState.AddModelError(
                    "Designation",
                    "Designation is required.");
            }

            if (string.IsNullOrWhiteSpace(model.DepartmentName))
            {
                ModelState.AddModelError(
                    "DepartmentName",
                    "Department Name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CollegeEmployeeID))
            {
                ModelState.AddModelError(
                    "CollegeEmployeeID",
                    "College Employee ID is required.");
            }


            // =====================================================
            // IF VALIDATION FAILS
            // =====================================================

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit TPO Profile";

                ViewData["Panel"] = "TPO";

                ViewData["UserName"] =
                    HttpContext.Session.GetString("TPOName")
                    ?? "TPO";

                return View(
                    "~/Views/TPO/EditProfile.cshtml",
                    model);
            }


            // =====================================================
            // UPLOAD FOLDER
            // =====================================================

            string uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Uploads",
                "TPO");


            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }


            try
            {
                // =================================================
                // SUPPORTING DOCUMENT 1
                // =================================================

                if (model.SupportingDocument1File != null &&
                    model.SupportingDocument1File.Length > 0)
                {
                    string extension =
                        Path.GetExtension(
                            model.SupportingDocument1File.FileName);

                    string fileName =
                        "TPO_DOC1_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(
                            uploadFolder,
                            fileName);

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.SupportingDocument1File.CopyTo(stream);
                    }

                    model.SupportingDocument1 =
                        "/Uploads/TPO/" + fileName;
                }


                // =================================================
                // SUPPORTING DOCUMENT 2
                // =================================================

                if (model.SupportingDocument2File != null &&
                    model.SupportingDocument2File.Length > 0)
                {
                    string extension =
                        Path.GetExtension(
                            model.SupportingDocument2File.FileName);

                    string fileName =
                        "TPO_DOC2_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(
                            uploadFolder,
                            fileName);

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.SupportingDocument2File.CopyTo(stream);
                    }

                    model.SupportingDocument2 =
                        "/Uploads/TPO/" + fileName;
                }


                // =================================================
                // PROFILE PHOTO
                // =================================================

                if (model.ProfilePhotoFile != null &&
                    model.ProfilePhotoFile.Length > 0)
                {
                    string extension =
                        Path.GetExtension(
                            model.ProfilePhotoFile.FileName);

                    string fileName =
                        "TPO_PROFILE_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(
                            uploadFolder,
                            fileName);

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.ProfilePhotoFile.CopyTo(stream);
                    }

                    model.ProfilePhoto =
                        "/Uploads/TPO/" + fileName;
                }


                // =================================================
                // UPDATE DATABASE
                // =================================================

                bool result =
                    _accountRepository.UpdateTPOProfile(model);


                // =================================================
                // UPDATE FAILED
                // =================================================

                if (!result)
                {
                    TempData["Error"] =
                        "TPO profile could not be updated.";

                    return RedirectToAction(
                        "EditProfile",
                        "TPO");
                }


                // =================================================
                // UPDATE SESSION NAME
                // =================================================

                HttpContext.Session.SetString(
                    "TPOName",
                    model.FullName);


                // =================================================
                // SUCCESS
                // =================================================

                TempData["Success"] =
                    "TPO profile updated successfully.";


                return RedirectToAction(
                    "EditProfile",
                    "TPO");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error while updating TPO profile: "
                    + ex.Message;

                return RedirectToAction(
                    "EditProfile",
                    "TPO");
            }
        }

        // shrirang 06/10/26
        // shrirang 07/10/26
        // =====================================================
        // TPO TRAINEE INFORMATION
        // =====================================================

        // ============================================================
        // TPO - TRAINEE INFORMATION
        // ============================================================

        [HttpGet]
        [Route("TPO/TraineesInfo")]
        public IActionResult TraineesInfo(
            string? searchText,
            int? branchId,
            int? currentYear,
            int? admissionYear,
            int? passoutYear,
            int? gender)
        {
            // ========================================================
            // CHECK TPO SESSION
            // ========================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // ========================================================
            // GET LOGGED-IN TPO DETAILS
            // ========================================================

            TPORegistration? tpo =
                _accountRepository.GetTPODetails(
                    tpoId.Value);

            if (tpo == null)
            {
                TempData["Error"] =
                    "TPO details could not be found.";

                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // ========================================================
            // COLLEGE ID
            // ========================================================

            int collegeId = 0;

            if (tpo.CollegeName != null)
            {
                collegeId =
                    Convert.ToInt32(tpo.CollegeName);
            }


            // ========================================================
            // TPO DEPARTMENT
            //
            // IMPORTANT:
            // We are NOT finding department ID from DepartmentName.
            //
            // Your TPO table already contains:
            //
            // nDepartment = 1
            //
            // Therefore directly use tpo.nDepartment.
            // ========================================================

            int? departmentId =
                tpo.nDepartment;


            // ========================================================
            // GET BRANCHES FOR TPO DEPARTMENT
            // ========================================================

            //List<BranchM> branches =
            //    new List<BranchM>();

            //if (departmentId.HasValue &&
            //    departmentId.Value > 0)
            //{
            //    branches =
            //        _accountRepository.GetBranches(
            //            departmentId.Value);
            //}
            //Sanidhya 08/10/26
            List<BranchM> branches =
                    _accountRepository.GetBranches();

            // ========================================================
            // VALIDATE SELECTED BRANCH
            //
            // A TPO should only be able to filter branches that
            // belong to his/her department.
            // ========================================================

            //if (branchId.HasValue)
            //{
            //    bool branchBelongsToDepartment =
            //        branches.Any(
            //            x => x.nID == branchId.Value);

            //    if (!branchBelongsToDepartment)
            //    {
            //        branchId = null;
            //    }
            //}


            // ========================================================
            // CREATE TRAINEE FILTER
            // ========================================================

            TPOTraineeFilterM filter =
                new TPOTraineeFilterM
                {
                    CollegeId = collegeId,

                    SearchText =
                        string.IsNullOrWhiteSpace(searchText)
                            ? null
                            : searchText.Trim(),

                    BranchId = branchId,

                    CurrentYear = currentYear,

                    AdmissionYear = admissionYear,

                    PassoutYear = passoutYear,

                    Gender = gender
                };


            // ========================================================
            // GET TRAINEES
            // ========================================================

            List<TPOTraineeInfoM> trainees =
                _accountRepository.GetTPOTraineesByCollege(
                    filter);


            // ========================================================
            // VIEWBAG VALUES
            // ========================================================

            ViewBag.CollegeId =
                collegeId;

            ViewBag.CollegeName =
                tpo.CollegeName;

            ViewBag.DepartmentId =
                departmentId;

            ViewBag.DepartmentName =
                tpo.DepartmentName;

            ViewBag.Branches =
                branches;

            ViewBag.TraineeCount =
                trainees.Count;


            // ========================================================
            // PRESERVE FILTER VALUES
            // ========================================================

            ViewBag.SearchText =
                searchText;

            ViewBag.BranchId =
                branchId;

            ViewBag.CurrentYear =
                currentYear;

            ViewBag.AdmissionYear =
                admissionYear;

            ViewBag.PassoutYear =
                passoutYear;

            ViewBag.Gender =
                gender;


            // ========================================================
            // RETURN VIEW
            // ========================================================

            return View(
                "TraineesInfo",
                trainees);
        }


        // shrirang 08/10/26
        [HttpGet]
        [Route("TPO/CreateNotification")]
        public IActionResult CreateNotification()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TPO/CreateNotification")]
        public async Task<IActionResult> CreateNotification(
    string Title,
    string Type,
    string Audience,
    DateTime? Date,
    TimeSpan? Time,
    string Content,
    bool SendEmailNotification,
    IFormFile? AttachFile1,
    IFormFile? AttachFile2)
        {
            // =========================================================
            // CHECK TPO LOGIN
            // =========================================================

            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            string tpoName =
                HttpContext.Session.GetString("TPOName") ?? "TPO";


            // =========================================================
            // SERVER-SIDE VALIDATION
            // =========================================================

            if (string.IsNullOrWhiteSpace(Title))
            {
                TempData["Error"] = "Title is required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Type))
            {
                TempData["Error"] = "Type is required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Audience))
            {
                TempData["Error"] = "Audience is required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Content))
            {
                TempData["Error"] = "Content is required.";
                return View();
            }


            // =========================================================
            // CONNECTION STRING
            // =========================================================

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                TempData["Error"] = "Database connection string is not configured.";
                return View();
            }


            // =========================================================
            // FILE VARIABLES
            // =========================================================

            string? attachment1OriginalName = null;
            string? attachment1FileName = null;
            string? attachment1Path = null;
            string? attachment1ContentType = null;
            long? attachment1Size = null;

            string? attachment2OriginalName = null;
            string? attachment2FileName = null;
            string? attachment2Path = null;
            string? attachment2ContentType = null;
            long? attachment2Size = null;


            // =========================================================
            // UPLOAD FOLDER
            // =========================================================

            string uploadFolder = Path.Combine(
                _environment.WebRootPath,
                "Uploads",
                "TPO",
                "Notifications"
            );

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }


            // =========================================================
            // ALLOWED EXTENSIONS
            // =========================================================

            string[] allowedExtensions =
            {
        ".pdf",
        ".doc",
        ".docx",
        ".jpg",
        ".jpeg",
        ".png"
    };


            // =========================================================
            // MAX FILE SIZE
            // 10 MB PER FILE
            // =========================================================

            const long maxFileSize = 10 * 1024 * 1024;


            // =========================================================
            // SAVE ATTACHMENT 1
            // =========================================================

            if (AttachFile1 != null && AttachFile1.Length > 0)
            {
                string extension =
                    Path.GetExtension(AttachFile1.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] =
                        "Attachment 1 contains an invalid file type.";

                    return View();
                }

                if (AttachFile1.Length > maxFileSize)
                {
                    TempData["Error"] =
                        "Attachment 1 size cannot exceed 10 MB.";

                    return View();
                }


                attachment1OriginalName =
                    Path.GetFileName(AttachFile1.FileName);

                attachment1FileName =
                    "TPO_NOTIFICATION_1_" +
                    Guid.NewGuid().ToString("N") +
                    extension;

                string physicalFilePath =
                    Path.Combine(
                        uploadFolder,
                        attachment1FileName
                    );


                using (FileStream stream =
                       new FileStream(
                           physicalFilePath,
                           FileMode.Create))
                {
                    await AttachFile1.CopyToAsync(stream);
                }


                attachment1Path =
                    "/Uploads/TPO/Notifications/" +
                    attachment1FileName;

                attachment1ContentType =
                    AttachFile1.ContentType;

                attachment1Size =
                    AttachFile1.Length;
            }


            // =========================================================
            // SAVE ATTACHMENT 2
            // =========================================================

            if (AttachFile2 != null && AttachFile2.Length > 0)
            {
                string extension =
                    Path.GetExtension(AttachFile2.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] =
                        "Attachment 2 contains an invalid file type.";

                    return View();
                }

                if (AttachFile2.Length > maxFileSize)
                {
                    TempData["Error"] =
                        "Attachment 2 size cannot exceed 10 MB.";

                    return View();
                }


                attachment2OriginalName =
                    Path.GetFileName(AttachFile2.FileName);

                attachment2FileName =
                    "TPO_NOTIFICATION_2_" +
                    Guid.NewGuid().ToString("N") +
                    extension;

                string physicalFilePath =
                    Path.Combine(
                        uploadFolder,
                        attachment2FileName
                    );


                using (FileStream stream =
                       new FileStream(
                           physicalFilePath,
                           FileMode.Create))
                {
                    await AttachFile2.CopyToAsync(stream);
                }


                attachment2Path =
                    "/Uploads/TPO/Notifications/" +
                    attachment2FileName;

                attachment2ContentType =
                    AttachFile2.ContentType;

                attachment2Size =
                    AttachFile2.Length;
            }


            // =========================================================
            // INSERT INTO DATABASE
            // =========================================================

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string query = @"
                INSERT INTO tblTPONotification
                (
                    Title,
                    Type,
                    Audience,
                    NotificationDate,
                    NotificationTime,
                    Content,
                    SendEmailNotification,

                    Attachment1OriginalName,
                    Attachment1FileName,
                    Attachment1Path,
                    Attachment1ContentType,
                    Attachment1Size,

                    Attachment2OriginalName,
                    Attachment2FileName,
                    Attachment2Path,
                    Attachment2ContentType,
                    Attachment2Size,

                    CreatedBy,
                    CreatedByName,

                    IsActive,
                    IsDeleted,
                    CreatedDate
                )
                VALUES
                (
                    @Title,
                    @Type,
                    @Audience,
                    @NotificationDate,
                    @NotificationTime,
                    @Content,
                    @SendEmailNotification,

                    @Attachment1OriginalName,
                    @Attachment1FileName,
                    @Attachment1Path,
                    @Attachment1ContentType,
                    @Attachment1Size,

                    @Attachment2OriginalName,
                    @Attachment2FileName,
                    @Attachment2Path,
                    @Attachment2ContentType,
                    @Attachment2Size,

                    @CreatedBy,
                    @CreatedByName,

                    1,
                    0,
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";


                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(
                            "@Title",
                            SqlDbType.NVarChar,
                            250
                        ).Value =
                            Title.Trim();


                        command.Parameters.Add(
                            "@Type",
                            SqlDbType.NVarChar,
                            50
                        ).Value =
                            Type.Trim();


                        command.Parameters.Add(
                            "@Audience",
                            SqlDbType.NVarChar,
                            50
                        ).Value =
                            Audience.Trim();


                        command.Parameters.Add(
                            "@NotificationDate",
                            SqlDbType.Date
                        ).Value =
                            Date.HasValue
                                ? Date.Value.Date
                                : (object)DBNull.Value;


                        command.Parameters.Add(
                            "@NotificationTime",
                            SqlDbType.Time
                        ).Value =
                            Time.HasValue
                                ? Time.Value
                                : (object)DBNull.Value;


                        command.Parameters.Add(
                            "@Content",
                            SqlDbType.NVarChar
                        ).Value =
                            Content.Trim();


                        command.Parameters.Add(
                            "@SendEmailNotification",
                            SqlDbType.Bit
                        ).Value =
                            SendEmailNotification;


                        // ============================================
                        // ATTACHMENT 1
                        // ============================================

                        command.Parameters.Add(
                            "@Attachment1OriginalName",
                            SqlDbType.NVarChar,
                            255
                        ).Value =
                            (object?)attachment1OriginalName
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment1FileName",
                            SqlDbType.NVarChar,
                            255
                        ).Value =
                            (object?)attachment1FileName
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment1Path",
                            SqlDbType.NVarChar,
                            500
                        ).Value =
                            (object?)attachment1Path
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment1ContentType",
                            SqlDbType.NVarChar,
                            100
                        ).Value =
                            (object?)attachment1ContentType
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment1Size",
                            SqlDbType.BigInt
                        ).Value =
                            (object?)attachment1Size
                            ?? DBNull.Value;


                        // ============================================
                        // ATTACHMENT 2
                        // ============================================

                        command.Parameters.Add(
                            "@Attachment2OriginalName",
                            SqlDbType.NVarChar,
                            255
                        ).Value =
                            (object?)attachment2OriginalName
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment2FileName",
                            SqlDbType.NVarChar,
                            255
                        ).Value =
                            (object?)attachment2FileName
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment2Path",
                            SqlDbType.NVarChar,
                            500
                        ).Value =
                            (object?)attachment2Path
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment2ContentType",
                            SqlDbType.NVarChar,
                            100
                        ).Value =
                            (object?)attachment2ContentType
                            ?? DBNull.Value;


                        command.Parameters.Add(
                            "@Attachment2Size",
                            SqlDbType.BigInt
                        ).Value =
                            (object?)attachment2Size
                            ?? DBNull.Value;


                        // ============================================
                        // CREATED BY
                        // ============================================

                        command.Parameters.Add(
                            "@CreatedBy",
                            SqlDbType.Int
                        ).Value =
                            tpoId.Value;


                        command.Parameters.Add(
                            "@CreatedByName",
                            SqlDbType.NVarChar,
                            200
                        ).Value =
                            tpoName;


                        int notificationId =
                            Convert.ToInt32(
                                await command.ExecuteScalarAsync()
                            );
                    }
                }


                // =====================================================
                // SUCCESS
                // =====================================================

                TempData["Success"] =
                    "Notification created successfully.";

                return RedirectToAction(
                    "CreateNotification",
                    "TPO"
                );
            }
            catch (Exception ex)
            {
                // =====================================================
                // DELETE UPLOADED FILES IF DATABASE INSERT FAILS
                // =====================================================

                try
                {
                    if (!string.IsNullOrWhiteSpace(attachment1FileName))
                    {
                        string filePath =
                            Path.Combine(
                                uploadFolder,
                                attachment1FileName
                            );

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }


                    if (!string.IsNullOrWhiteSpace(attachment2FileName))
                    {
                        string filePath =
                            Path.Combine(
                                uploadFolder,
                                attachment2FileName
                            );

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }


                TempData["Error"] =
                    "Unable to create notification. " +
                    ex.Message;

                return View();
            }
        }

        // shrirang 08/10/26
        [HttpGet]
        [Route("TPO/ViewNotificationEvent")]
        public async Task<IActionResult> ViewNotificationEvent(
    string? type,
    string? audience,
    DateTime? fromDate,
    DateTime? toDate)
        {
            // ============================================================
            // CHECK TPO LOGIN
            // ============================================================

            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction("TPOLogin", "Account");
            }


            // ============================================================
            // VIEW DATA
            // ============================================================

            ViewData["Title"] = "View Notifications / Events";
            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName") ?? "TPO";


            // ============================================================
            // MODEL
            // ============================================================

            List<TPOViewNotificationM> notifications =
                new List<TPOViewNotificationM>();


            // ============================================================
            // CONNECTION
            // ============================================================

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");


            // ============================================================
            // SQL
            // ============================================================

            string sql = @"
    SELECT
        N.NotificationID,
        N.Title,
        N.Type,
        N.Audience,
        N.NotificationDate,
        N.NotificationTime,
        N.Content,
        N.SendEmailNotification,

        N.Attachment1OriginalName,
        N.Attachment1FileName,
        N.Attachment1Path,
        N.Attachment1ContentType,
        N.Attachment1Size,

        N.Attachment2OriginalName,
        N.Attachment2FileName,
        N.Attachment2Path,
        N.Attachment2ContentType,
        N.Attachment2Size,

        N.IsActive,
        N.IsDeleted,
        N.CreatedDate,
        N.CreatedBy,
        N.CreatedByName,

        -- =====================================================
        -- NOTIFICATION TRACKING
        -- =====================================================

        ISNULL(T.TotalSent, 0) AS TotalSent,

        ISNULL(T.TotalRead, 0) AS TotalRead,

        ISNULL(T.TotalUnread, 0) AS TotalUnread

    FROM tblTPONotification N

    -- =========================================================
    -- STUDENT NOTIFICATION TRACKING
    -- =========================================================

    OUTER APPLY
    (
        SELECT

            COUNT(1) AS TotalSent,

            SUM(
                CASE
                    WHEN R.IsRead = 1
                    THEN 1
                    ELSE 0
                END
            ) AS TotalRead,

            SUM(
                CASE
                    WHEN R.IsRead = 0
                    THEN 1
                    ELSE 0
                END
            ) AS TotalUnread

        FROM tblTPONotificationRecipient R

        WHERE
            R.NotificationID = N.NotificationID
            AND R.IsActive = 1

    ) T

    WHERE
        N.IsDeleted = 0
        AND N.CreatedBy = @CreatedBy
";

            // ============================================================
            // FILTER TYPE
            // ============================================================

            if (!string.IsNullOrWhiteSpace(type) &&
                !type.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND Type = @Type ";
            }


            // ============================================================
            // FILTER AUDIENCE
            // ============================================================

            if (!string.IsNullOrWhiteSpace(audience) &&
                !audience.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND Audience = @Audience ";
            }


            // ============================================================
            // FILTER FROM DATE
            // ============================================================

            if (fromDate.HasValue)
            {
                sql += " AND NotificationDate >= @FromDate ";
            }


            // ============================================================
            // FILTER TO DATE
            // ============================================================

            if (toDate.HasValue)
            {
                sql += " AND NotificationDate <= @ToDate ";
            }


            // ============================================================
            // ORDER
            // ============================================================

            sql += @"
        ORDER BY
            CASE
                WHEN NotificationDate IS NULL THEN 1
                ELSE 0
            END,
            NotificationDate DESC,
            NotificationTime DESC,
            NotificationID DESC
    ";


            // ============================================================
            // EXECUTE
            // ============================================================

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@CreatedBy",
                        SqlDbType.Int).Value = tpoId.Value;


                    // Type
                    if (!string.IsNullOrWhiteSpace(type) &&
                        !type.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        command.Parameters.Add(
                            "@Type",
                            SqlDbType.NVarChar, 50).Value = type;
                    }


                    // Audience
                    if (!string.IsNullOrWhiteSpace(audience) &&
                        !audience.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        command.Parameters.Add(
                            "@Audience",
                            SqlDbType.NVarChar, 50).Value = audience;
                    }


                    // From Date
                    if (fromDate.HasValue)
                    {
                        command.Parameters.Add(
                            "@FromDate",
                            SqlDbType.Date).Value = fromDate.Value.Date;
                    }


                    // To Date
                    if (toDate.HasValue)
                    {
                        command.Parameters.Add(
                            "@ToDate",
                            SqlDbType.Date).Value = toDate.Value.Date;
                    }


                    using (SqlDataReader reader =
                           await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            TPOViewNotificationM item =
                                new TPOViewNotificationM();

                            item.NotificationID =
                                Convert.ToInt32(
                                    reader["NotificationID"]);

                            item.Title =
                                reader["Title"]?.ToString() ?? "";

                            item.Type =
                                reader["Type"]?.ToString() ?? "";

                            item.Audience =
                                reader["Audience"]?.ToString() ?? "";

                            if (reader["NotificationDate"] != DBNull.Value)
                            {
                                item.NotificationDate =
                                    Convert.ToDateTime(
                                        reader["NotificationDate"]);
                            }

                            if (reader["NotificationTime"] != DBNull.Value)
                            {
                                item.NotificationTime =
                                    (TimeSpan)reader["NotificationTime"];
                            }

                            item.Content =
                                reader["Content"]?.ToString() ?? "";

                            item.SendEmailNotification =
                                reader["SendEmailNotification"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["SendEmailNotification"]);

                            item.IsActive =
                                reader["IsActive"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["IsActive"]);

                            item.IsDeleted =
                                reader["IsDeleted"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["IsDeleted"]);

                            item.CreatedDate =
                                reader["CreatedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["CreatedDate"])
                                    : DateTime.MinValue;

                            if (reader["CreatedBy"] != DBNull.Value)
                            {
                                item.CreatedBy =
                                    Convert.ToInt32(reader["CreatedBy"]);
                            }

                            item.CreatedByName =
                                reader["CreatedByName"]?.ToString() ?? "";

                            // ====================================================
                            // NOTIFICATION TRACKING
                            // ====================================================

                            item.TotalSent =
                                reader["TotalSent"] != DBNull.Value
                                    ? Convert.ToInt32(reader["TotalSent"])
                                    : 0;

                            item.TotalRead =
                                reader["TotalRead"] != DBNull.Value
                                    ? Convert.ToInt32(reader["TotalRead"])
                                    : 0;

                            item.TotalUnread =
                                reader["TotalUnread"] != DBNull.Value
                                    ? Convert.ToInt32(reader["TotalUnread"])
                                    : 0;


                            // ====================================================
                            // CALCULATE STATUS
                            // ====================================================

                            DateTime? notificationDateTime = null;

                            if (item.NotificationDate.HasValue)
                            {
                                DateTime date = item.NotificationDate.Value.Date;

                                if (item.NotificationTime.HasValue)
                                {
                                    notificationDateTime =
                                        date.Add(item.NotificationTime.Value);
                                }
                                else
                                {
                                    // Date-only notification remains active
                                    // for the complete day.
                                    notificationDateTime =
                                        date.AddDays(1).AddTicks(-1);
                                }
                            }

                            if (!item.IsActive)
                            {
                                item.Status = "Inactive";
                            }
                            else if (notificationDateTime.HasValue &&
                                     notificationDateTime.Value < DateTime.Now)
                            {
                                item.Status = "Completed";
                            }
                            else
                            {
                                item.Status = "Active";
                            }

                            // ====================================================
                            // ATTACHMENT 1
                            // ====================================================

                            item.Attachment1OriginalName =
                                reader["Attachment1OriginalName"] != DBNull.Value
                                    ? reader["Attachment1OriginalName"].ToString()
                                    : null;

                            item.Attachment1FileName =
                                reader["Attachment1FileName"] != DBNull.Value
                                    ? reader["Attachment1FileName"].ToString()
                                    : null;

                            item.Attachment1Path =
                                reader["Attachment1Path"] != DBNull.Value
                                    ? reader["Attachment1Path"].ToString()
                                    : null;

                            item.Attachment1ContentType =
                                reader["Attachment1ContentType"] != DBNull.Value
                                    ? reader["Attachment1ContentType"].ToString()
                                    : null;

                            if (reader["Attachment1Size"] != DBNull.Value)
                            {
                                item.Attachment1Size =
                                    Convert.ToInt64(reader["Attachment1Size"]);
                            }


                            // ====================================================
                            // ATTACHMENT 2
                            // ====================================================

                            item.Attachment2OriginalName =
                                reader["Attachment2OriginalName"] != DBNull.Value
                                    ? reader["Attachment2OriginalName"].ToString()
                                    : null;

                            item.Attachment2FileName =
                                reader["Attachment2FileName"] != DBNull.Value
                                    ? reader["Attachment2FileName"].ToString()
                                    : null;

                            item.Attachment2Path =
                                reader["Attachment2Path"] != DBNull.Value
                                    ? reader["Attachment2Path"].ToString()
                                    : null;

                            item.Attachment2ContentType =
                                reader["Attachment2ContentType"] != DBNull.Value
                                    ? reader["Attachment2ContentType"].ToString()
                                    : null;

                            if (reader["Attachment2Size"] != DBNull.Value)
                            {
                                item.Attachment2Size =
                                    Convert.ToInt64(reader["Attachment2Size"]);
                            }

                            notifications.Add(item);
                        }
                    }
                }
            }


            // ============================================================
            // FILTER VALUES FOR VIEW
            // ============================================================

            ViewBag.SelectedType = type ?? "All";
            ViewBag.SelectedAudience = audience ?? "All";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            ViewBag.TotalNotifications = notifications.Count;


            return View(notifications);
        }


        // shrirang 08/10/26
        [HttpGet]
        [Route("TPO/ViewNotificationDetails/{id:int}")]
        public async Task<IActionResult> ViewNotificationDetails(int id)
        {
            // ============================================================
            // CHECK TPO LOGIN
            // ============================================================

            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return Unauthorized();
            }

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            TPOViewNotificationM? item = null;

            string sql = @"
        SELECT
            NotificationID,
            Title,
            Type,
            Audience,
            NotificationDate,
            NotificationTime,
            Content,
            SendEmailNotification,

            Attachment1OriginalName,
            Attachment1FileName,
            Attachment1Path,
            Attachment1ContentType,
            Attachment1Size,

            Attachment2OriginalName,
            Attachment2FileName,
            Attachment2Path,
            Attachment2ContentType,
            Attachment2Size,

            CreatedBy,
            CreatedByName,
            IsActive,
            IsDeleted,
            CreatedDate,
            UpdatedDate

        FROM tblTPONotification

        WHERE NotificationID = @NotificationID
          AND CreatedBy = @CreatedBy
          AND IsDeleted = 0
    ";

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@NotificationID",
                        SqlDbType.Int).Value = id;

                    command.Parameters.Add(
                        "@CreatedBy",
                        SqlDbType.Int).Value = tpoId.Value;

                    using (SqlDataReader reader =
                           await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new TPOViewNotificationM();

                            item.NotificationID =
                                Convert.ToInt32(
                                    reader["NotificationID"]);

                            item.Title =
                                reader["Title"]?.ToString() ?? "";

                            item.Type =
                                reader["Type"]?.ToString() ?? "";

                            item.Audience =
                                reader["Audience"]?.ToString() ?? "";

                            if (reader["NotificationDate"] != DBNull.Value)
                            {
                                item.NotificationDate =
                                    Convert.ToDateTime(
                                        reader["NotificationDate"]);
                            }

                            if (reader["NotificationTime"] != DBNull.Value)
                            {
                                item.NotificationTime =
                                    (TimeSpan)reader["NotificationTime"];
                            }

                            item.Content =
                                reader["Content"]?.ToString() ?? "";

                            item.SendEmailNotification =
                                reader["SendEmailNotification"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["SendEmailNotification"]);

                            // ====================================================
                            // ATTACHMENT 1
                            // ====================================================

                            item.Attachment1OriginalName =
                                reader["Attachment1OriginalName"]?.ToString();

                            item.Attachment1FileName =
                                reader["Attachment1FileName"]?.ToString();

                            item.Attachment1Path =
                                reader["Attachment1Path"]?.ToString();

                            // ====================================================
                            // ATTACHMENT 2
                            // ====================================================

                            item.Attachment2OriginalName =
                                reader["Attachment2OriginalName"]?.ToString();

                            item.Attachment2FileName =
                                reader["Attachment2FileName"]?.ToString();

                            item.Attachment2Path =
                                reader["Attachment2Path"]?.ToString();

                            // ====================================================
                            // CREATED BY
                            // ====================================================

                            if (reader["CreatedBy"] != DBNull.Value)
                            {
                                item.CreatedBy =
                                    Convert.ToInt32(
                                        reader["CreatedBy"]);
                            }

                            item.CreatedByName =
                                reader["CreatedByName"]?.ToString() ?? "";

                            item.IsActive =
                                reader["IsActive"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["IsActive"]);

                            item.IsDeleted =
                                reader["IsDeleted"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    reader["IsDeleted"]);

                            if (reader["CreatedDate"] != DBNull.Value)
                            {
                                item.CreatedDate =
                                    Convert.ToDateTime(
                                        reader["CreatedDate"]);
                            }

                           
                            // ====================================================
                            // STATUS
                            // ====================================================

                            DateTime? notificationDateTime = null;

                            if (item.NotificationDate.HasValue)
                            {
                                DateTime date =
                                    item.NotificationDate.Value.Date;

                                if (item.NotificationTime.HasValue)
                                {
                                    notificationDateTime =
                                        date.Add(
                                            item.NotificationTime.Value);
                                }
                                else
                                {
                                    notificationDateTime =
                                        date.AddDays(1).AddTicks(-1);
                                }
                            }

                            if (!item.IsActive)
                            {
                                item.Status = "Inactive";
                            }
                            else if (notificationDateTime.HasValue &&
                                     notificationDateTime.Value < DateTime.Now)
                            {
                                item.Status = "Completed";
                            }
                            else
                            {
                                item.Status = "Active";
                            }
                        }
                    }
                }
            }

            if (item == null)
            {
                return NotFound();
            }

            return PartialView(
                "_ViewNotificationDetails",
                item);
        }


        // shrirang 08/10/26
        // tpo send notification
        // =====================================================
        // SEND NOTIFICATION
        // =====================================================

        [HttpGet]
        [Route("TPO/SendNotification")]
        public IActionResult SendNotification(
            int? notificationId,
            string? searchText,
            int? departmentId,
            int? branchId)
        {
            // =====================================================
            // CHECK TPO LOGIN
            // =====================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =====================================================
            // GET TPO DETAILS
            // =====================================================

            TPORegistration? tpo =
                _accountRepository.GetTPODetails(
                    tpoId.Value);

            if (tpo == null)
            {
                TempData["Error"] =
                    "TPO details could not be found.";

                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =====================================================
            // GET TPO COLLEGE
            // =====================================================

            int collegeId = 0;

            if (tpo.CollegeName != null)
            {
                collegeId =
                    Convert.ToInt32(
                        tpo.CollegeName);
            }


            if (collegeId <= 0)
            {
                TempData["Error"] =
                    "College information is not available.";

                return RedirectToAction(
                    "TPODashboard");
            }


            // =====================================================
            // GET DEPARTMENTS
            // =====================================================

            List<DepartmentM> departments =
                _accountRepository.GetDepartments();


            // =====================================================
            // VALIDATE DEPARTMENT
            // =====================================================

            if (departmentId.HasValue)
            {
                bool departmentExists =
                    departments.Any(
                        x => x.nID == departmentId.Value);

                if (!departmentExists)
                {
                    departmentId = null;
                }
            }


            // =====================================================
            // GET BRANCHES
            // =====================================================

            List<BranchM> branches =
                new List<BranchM>();

            if (departmentId.HasValue &&
                departmentId.Value > 0)
            {
                branches =
                    _accountRepository.GetBranches(
                        departmentId.Value);
            }


            // =====================================================
            // VALIDATE BRANCH
            // =====================================================

            if (branchId.HasValue)
            {
                bool branchExists =
                    branches.Any(
                        x => x.nID == branchId.Value);

                if (!branchExists)
                {
                    branchId = null;
                }
            }


            // =====================================================
            // GET NOTIFICATIONS
            // =====================================================

            List<TPONotificationListM> notifications =
                _accountRepository.GetTPONotificationsForSend(
                    tpoId.Value);


            // =====================================================
            // GET STUDENTS
            // =====================================================

            TPOTraineeFilterM filter =
                new TPOTraineeFilterM
                {
                    CollegeId = collegeId,

                    SearchText =
                        string.IsNullOrWhiteSpace(searchText)
                        ? null
                        : searchText.Trim(),

                    DepartmentId =
                        departmentId,

                    BranchId =
                        branchId
                };


            List<TPOTraineeInfoM> students =
                _accountRepository.GetTPOTraineesByCollege(
                    filter);


            // =====================================================
            // CREATE VIEW MODEL
            // =====================================================

            TPOSendNotificationM model =
                new TPOSendNotificationM
                {
                    TPOID = tpoId.Value,

                    CollegeCode =
                        tpo.CollegeCode,

                    Students =
                        students,

                    SelectedCandidateIDs =
                        new List<int>()
                };


            // =====================================================
            // SELECT NOTIFICATION
            // =====================================================

            if (notificationId.HasValue)
            {
                TPONotificationListM? selected =
                    notifications.FirstOrDefault(
                        x =>
                            x.NotificationID ==
                            notificationId.Value);

                if (selected != null)
                {
                    model.NotificationID =
                        selected.NotificationID;

                    model.Title =
                        selected.Title;

                    model.Type =
                        selected.Type;

                    model.Audience =
                        selected.Audience;

                    model.NotificationDate =
                        selected.NotificationDate;

                    model.NotificationTime =
                        selected.NotificationTime;

                    model.Content =
                        selected.Content;

                    model.Attachment1OriginalName =
                        selected.Attachment1OriginalName;

                    model.Attachment1Path =
                        selected.Attachment1Path;

                    model.Attachment2OriginalName =
                        selected.Attachment2OriginalName;

                    model.Attachment2Path =
                        selected.Attachment2Path;
                }
            }


            // =====================================================
            // VIEWBAG
            // =====================================================

            ViewBag.Notifications =
                notifications;

            ViewBag.Departments =
                departments;

            ViewBag.Branches =
                branches;

            ViewBag.CollegeId =
                collegeId;

            ViewBag.CollegeCode =
                tpo.CollegeCode;

            ViewBag.CollegeName =
                tpo.CollegeName;

            ViewBag.DepartmentId =
                departmentId;

            ViewBag.BranchId =
                branchId;

            ViewBag.SearchText =
                searchText;

            ViewBag.TraineeCount =
                students.Count;


            // =====================================================
            // RETURN VIEW
            // =====================================================

            return View(
                "SendNotification",
                model);
        }



        // =====================================================
        // SEND NOTIFICATION - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TPO/SendNotification")]
        public IActionResult SendNotification(
            TPOSendNotificationM model)
        {
            // =====================================================
            // CHECK TPO LOGIN
            // =====================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =====================================================
            // GET TPO DETAILS
            // =====================================================

            TPORegistration? tpo =
                _accountRepository.GetTPODetails(
                    tpoId.Value);

            if (tpo == null)
            {
                TempData["Error"] =
                    "TPO details could not be found.";

                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =====================================================
            // GET COLLEGE ID
            // =====================================================

            int collegeId = 0;

            if (!string.IsNullOrWhiteSpace(
                tpo.CollegeName))
            {
                if (!int.TryParse(
                    tpo.CollegeName,
                    out collegeId))
                {
                    collegeId = 0;
                }
            }


            // =====================================================
            // VALIDATE COLLEGE
            // =====================================================

            if (collegeId <= 0)
            {
                TempData["Error"] =
                    "College information is not available.";

                return RedirectToAction(
                    "SendNotification");
            }


            // =====================================================
            // VALIDATE NOTIFICATION
            // =====================================================

            if (model.NotificationID <= 0)
            {
                TempData["Error"] =
                    "Please select a notification.";

                return RedirectToAction(
                    "SendNotification");
            }


            // =====================================================
            // VALIDATE STUDENTS
            // =====================================================

            if (model.SelectedCandidateIDs == null ||
                model.SelectedCandidateIDs.Count == 0)
            {
                TempData["Error"] =
                    "Please select at least one student.";

                return RedirectToAction(
                    "SendNotification",
                    new
                    {
                        notificationId =
                            model.NotificationID
                    });
            }


            // =====================================================
            // REMOVE DUPLICATE IDS
            // =====================================================

            model.SelectedCandidateIDs =
                model.SelectedCandidateIDs
                .Where(x => x > 0)
                .Distinct()
                .ToList();


            if (model.SelectedCandidateIDs.Count == 0)
            {
                TempData["Error"] =
                    "No valid students were selected.";

                return RedirectToAction(
                    "SendNotification",
                    new
                    {
                        notificationId =
                            model.NotificationID
                    });
            }


            // =====================================================
            // SEND NOTIFICATION
            // =====================================================

            try
            {
                int insertedCount =
                    _accountRepository.SendTPONotificationToCandidates(
                        tpoId.Value,
                        model.NotificationID,
                        collegeId,
                        model.SelectedCandidateIDs);


                // =================================================
                // SUCCESS
                // =================================================

                if (insertedCount > 0)
                {
                    TempData["Success"] =
                        "Notification sent successfully to "
                        + insertedCount
                        + " student(s).";
                }
                else
                {
                    TempData["Info"] =
                        "No new recipients were added. "
                        + "The selected student(s) may have already received this notification.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] =
                    ex.Message;
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to send notification. Please try again.";
            }


            // =====================================================
            // REDIRECT BACK
            // =====================================================

            return RedirectToAction(
                "SendNotification",
                new
                {
                    notificationId =
                        model.NotificationID
                });
        }

        // =====================================================
        // GET BRANCHES BY DEPARTMENT
        // =====================================================

        [HttpGet]
        [Route("TPO/GetBranchesByDepartment")]
        public IActionResult GetBranchesByDepartment(int departmentId)
        {
            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
            {
                return Unauthorized();
            }

            if (departmentId <= 0)
            {
                return Json(new List<object>());
            }

            List<BranchM> branches =
                _accountRepository.GetBranches(departmentId);

            var result =
                branches.Select(x => new
                {
                    id = x.nID,
                    name = x.sBranch
                }).ToList();

            return Json(result);
        }


        //shrirang 09/10/26

        [HttpGet]
        [Route("TPO/EditNotification/{id:int}")]
        public IActionResult EditNotification(int id)
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
                return RedirectToAction("TPOLogin", "Account");

            TPOViewNotificationM? model = null;

            using (SqlConnection cn = new SqlConnection(
    _configuration.GetConnectionString("DefaultConnection")))
            {
                cn.Open();

                string query = @"
            SELECT *
            FROM tblTPONotification
            WHERE NotificationID = @NotificationID
              AND CreatedBy = @TPOID
              AND IsDeleted = 0";

                using SqlCommand cmd = new SqlCommand(query, cn);

                cmd.Parameters.Add("@NotificationID", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId.Value;

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    model = new TPOViewNotificationM
                    {
                        NotificationID = Convert.ToInt32(reader["NotificationID"]),
                        Title = reader["Title"]?.ToString() ?? "",
                        Type = reader["Type"]?.ToString() ?? "",
                        Audience = reader["Audience"]?.ToString() ?? "",
                        NotificationDate = reader["NotificationDate"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(reader["NotificationDate"]),
                        NotificationTime = reader["NotificationTime"] == DBNull.Value
                            ? null
                            : (TimeSpan?)reader["NotificationTime"],
                        Content = WebUtility.HtmlDecode(
    Regex.Replace(
        reader["Content"]?.ToString() ?? "",
        "<[^>]+>",
        " "))
    .Trim(),
                        SendEmailNotification =
                            reader["SendEmailNotification"] != DBNull.Value &&
                            Convert.ToBoolean(reader["SendEmailNotification"]),

                        Attachment1OriginalName =
                            reader["Attachment1OriginalName"]?.ToString(),
                        Attachment1FileName =
                            reader["Attachment1FileName"]?.ToString(),
                        Attachment1Path =
                            reader["Attachment1Path"]?.ToString(),
                        Attachment1ContentType =
                            reader["Attachment1ContentType"]?.ToString(),
                        Attachment1Size =
                            reader["Attachment1Size"] == DBNull.Value
                                ? null
                                : Convert.ToInt64(reader["Attachment1Size"]),

                        Attachment2OriginalName =
                            reader["Attachment2OriginalName"]?.ToString(),
                        Attachment2FileName =
                            reader["Attachment2FileName"]?.ToString(),
                        Attachment2Path =
                            reader["Attachment2Path"]?.ToString(),
                        Attachment2ContentType =
                            reader["Attachment2ContentType"]?.ToString(),
                        Attachment2Size =
                            reader["Attachment2Size"] == DBNull.Value
                                ? null
                                : Convert.ToInt64(reader["Attachment2Size"]),

                        IsActive = reader["IsActive"] != DBNull.Value &&
                                   Convert.ToBoolean(reader["IsActive"]),

                        IsDeleted = reader["IsDeleted"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["IsDeleted"]),

                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),

                        CreatedBy = reader["CreatedBy"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["CreatedBy"]),

                        CreatedByName = reader["CreatedByName"]?.ToString() ?? ""
                    };
                }
            }

            if (model == null)
                return NotFound("Notification not found or you are not authorized to edit it.");

            return View("EditNotification", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TPO/EditNotification/{id:int}")]
        public async Task<IActionResult> EditNotification(
    int id,
    TPOViewNotificationM model)
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null)
                return RedirectToAction("TPOLogin", "Account");

            if (id != model.NotificationID)
                return BadRequest("Invalid notification ID.");

            // Basic server-side validation
            if (string.IsNullOrWhiteSpace(model.Title) ||
                string.IsNullOrWhiteSpace(model.Type) ||
                string.IsNullOrWhiteSpace(model.Audience) ||
                string.IsNullOrWhiteSpace(model.Content))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return await LoadEditNotificationView(id, tpoId.Value, model);
            }

            // Keep these values aligned with the options in your Create form.
            if (model.Type != "Notification" && model.Type != "Event")
            {
                ModelState.AddModelError(nameof(model.Type), "Invalid notification type.");
                return await LoadEditNotificationView(id, tpoId.Value, model);
            }

            if (model.Audience != "AllStudents")
            {
                ModelState.AddModelError(nameof(model.Audience), "Invalid audience.");
                return await LoadEditNotificationView(id, tpoId.Value, model);
            }

            const long maxFileSize = 5 * 1024 * 1024; // 5 MB per attachment

            string[] allowedExtensions =
            {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx",
        ".jpg", ".jpeg", ".png"
    };

            foreach (var file in new[] { model.AttachFile1, model.AttachFile2 })
            {
                if (file == null || file.Length == 0)
                    continue;

                string extension = Path.GetExtension(file.FileName);

                if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError("", "Each attachment must be 5 MB or smaller.");
                }

                if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Unsupported attachment type.");
                }
            }

            if (!ModelState.IsValid)
                return await LoadEditNotificationView(id, tpoId.Value, model);

            // Load the existing record first and verify ownership.
            TPOViewNotificationM? existing = null;

            using (SqlConnection cn = new SqlConnection(
    _configuration.GetConnectionString("DefaultConnection")))
            {
                await cn.OpenAsync();

                const string selectSql = @"
            SELECT *
            FROM tblTPONotification
            WHERE NotificationID = @NotificationID
              AND CreatedBy = @TPOID
              AND IsDeleted = 0;";

                using SqlCommand cmd = new SqlCommand(selectSql, cn);

                cmd.Parameters.Add("@NotificationID", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId.Value;

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    existing = new TPOViewNotificationM
                    {
                        NotificationID = id,

                        Attachment1OriginalName = reader["Attachment1OriginalName"] as string,
                        Attachment1FileName = reader["Attachment1FileName"] as string,
                        Attachment1Path = reader["Attachment1Path"] as string,
                        Attachment1ContentType = reader["Attachment1ContentType"] as string,
                        Attachment1Size = reader["Attachment1Size"] == DBNull.Value
                            ? null : Convert.ToInt64(reader["Attachment1Size"]),

                        Attachment2OriginalName = reader["Attachment2OriginalName"] as string,
                        Attachment2FileName = reader["Attachment2FileName"] as string,
                        Attachment2Path = reader["Attachment2Path"] as string,
                        Attachment2ContentType = reader["Attachment2ContentType"] as string,
                        Attachment2Size = reader["Attachment2Size"] == DBNull.Value
                            ? null : Convert.ToInt64(reader["Attachment2Size"])
                    };
                }
            }

            if (existing == null)
                return NotFound("Notification not found or you are not authorized to edit it.");

            // Save replacement files using unique server-generated names.
            string uploadFolder = Path.Combine(
                _environment.WebRootPath,
                "Uploads",
                "TPO",
                "Notifications");

            Directory.CreateDirectory(uploadFolder);

            async Task<(string OriginalName, string FileName, string WebPath,
                string ContentType, long Size)> SaveAttachment(IFormFile file)
            {
                string originalName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(originalName).ToLowerInvariant();
                string savedFileName = $"{Guid.NewGuid():N}{extension}";
                string physicalPath = Path.Combine(uploadFolder, savedFileName);

                await using (var stream = new FileStream(
                    physicalPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                string contentType = Path.GetExtension(originalName).ToLowerInvariant() switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return (
                    originalName,
                    savedFileName,
                    "/Uploads/TPO/Notifications/" + savedFileName,
                    contentType,
                    file.Length
                );
            }

            try
            {
                if (model.AttachFile1?.Length > 0)
                {
                    var file = await SaveAttachment(model.AttachFile1);

                    existing.Attachment1OriginalName = file.OriginalName;
                    existing.Attachment1FileName = file.FileName;
                    existing.Attachment1Path = file.WebPath;
                    existing.Attachment1ContentType = file.ContentType;
                    existing.Attachment1Size = file.Size;
                }

                if (model.AttachFile2?.Length > 0)
                {
                    var file = await SaveAttachment(model.AttachFile2);

                    existing.Attachment2OriginalName = file.OriginalName;
                    existing.Attachment2FileName = file.FileName;
                    existing.Attachment2Path = file.WebPath;
                    existing.Attachment2ContentType = file.ContentType;
                    existing.Attachment2Size = file.Size;
                }

                using SqlConnection cn = new SqlConnection(
           _configuration.GetConnectionString("DefaultConnection"));

                await cn.OpenAsync();

                const string updateSql = @"
            UPDATE tblTPONotification
            SET
                Title = @Title,
                Type = @Type,
                Audience = @Audience,
                NotificationDate = @NotificationDate,
                NotificationTime = @NotificationTime,
                Content = @Content,
                SendEmailNotification = @SendEmailNotification,

                Attachment1OriginalName = @Attachment1OriginalName,
                Attachment1FileName = @Attachment1FileName,
                Attachment1Path = @Attachment1Path,
                Attachment1ContentType = @Attachment1ContentType,
                Attachment1Size = @Attachment1Size,

                Attachment2OriginalName = @Attachment2OriginalName,
                Attachment2FileName = @Attachment2FileName,
                Attachment2Path = @Attachment2Path,
                Attachment2ContentType = @Attachment2ContentType,
                Attachment2Size = @Attachment2Size

            WHERE NotificationID = @NotificationID
              AND CreatedBy = @TPOID
              AND IsDeleted = 0;";

                using SqlCommand cmd = new SqlCommand(updateSql, cn);

                cmd.Parameters.Add("@NotificationID", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId.Value;

                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = model.Title.Trim();
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = model.Type;
                cmd.Parameters.Add("@Audience", SqlDbType.NVarChar, 100).Value = model.Audience;

                cmd.Parameters.Add("@NotificationDate", SqlDbType.DateTime).Value =
                    model.NotificationDate.HasValue
                        ? model.NotificationDate.Value
                        : DBNull.Value;

                cmd.Parameters.Add("@NotificationTime", SqlDbType.Time).Value =
                    model.NotificationTime.HasValue
                        ? model.NotificationTime.Value
                        : DBNull.Value;

                cmd.Parameters.Add("@Content", SqlDbType.NVarChar, -1).Value = model.Content.Trim();

                cmd.Parameters.Add("@SendEmailNotification", SqlDbType.Bit).Value =
                    model.SendEmailNotification;

                AddNullableParameter(cmd, "@Attachment1OriginalName", SqlDbType.NVarChar, 255, existing.Attachment1OriginalName);
                AddNullableParameter(cmd, "@Attachment1FileName", SqlDbType.NVarChar, 255, existing.Attachment1FileName);
                AddNullableParameter(cmd, "@Attachment1Path", SqlDbType.NVarChar, 500, existing.Attachment1Path);
                AddNullableParameter(cmd, "@Attachment1ContentType", SqlDbType.NVarChar, 150, existing.Attachment1ContentType);
                AddNullableParameter(cmd, "@Attachment1Size", SqlDbType.BigInt, 0, existing.Attachment1Size);

                AddNullableParameter(cmd, "@Attachment2OriginalName", SqlDbType.NVarChar, 255, existing.Attachment2OriginalName);
                AddNullableParameter(cmd, "@Attachment2FileName", SqlDbType.NVarChar, 255, existing.Attachment2FileName);
                AddNullableParameter(cmd, "@Attachment2Path", SqlDbType.NVarChar, 500, existing.Attachment2Path);
                AddNullableParameter(cmd, "@Attachment2ContentType", SqlDbType.NVarChar, 150, existing.Attachment2ContentType);
                AddNullableParameter(cmd, "@Attachment2Size", SqlDbType.BigInt, 0, existing.Attachment2Size);

                int rowsUpdated = await cmd.ExecuteNonQueryAsync();

                if (rowsUpdated == 0)
                    return NotFound("Notification could not be updated.");

                TempData["SuccessMessage"] = "Notification updated successfully.";

                return RedirectToAction(nameof(ViewNotificationEvent));
            }
            catch (Exception ex)
            {
                // Log the exception in your application's logger in production.
                ModelState.AddModelError("", "Unable to update the notification. Please try again.");
                return await LoadEditNotificationView(id, tpoId.Value, model);
            }
        }

        private async Task<IActionResult> LoadEditNotificationView(
    int id,
    int tpoId,
    TPOViewNotificationM model)
        {
            // Re-fetch the existing attachment links so they remain visible
            // if the user submits the form with a validation error.
            using SqlConnection cn = new SqlConnection(
    _configuration.GetConnectionString("DefaultConnection"));

            await cn.OpenAsync();

            const string sql = @"
        SELECT Attachment1OriginalName, Attachment1Path,
               Attachment2OriginalName, Attachment2Path
        FROM tblTPONotification
        WHERE NotificationID = @ID
          AND CreatedBy = @TPOID
          AND IsDeleted = 0;";

            using SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return NotFound();

            model.Attachment1OriginalName =
                reader["Attachment1OriginalName"] as string;
            model.Attachment1Path =
                reader["Attachment1Path"] as string;

            model.Attachment2OriginalName =
                reader["Attachment2OriginalName"] as string;
            model.Attachment2Path =
                reader["Attachment2Path"] as string;

            return View("EditNotification", model);
        }

        private static void AddNullableParameter(
            SqlCommand cmd,
            string name,
            SqlDbType type,
            int size,
            object? value)
        {
            SqlParameter parameter = size > 0
                ? cmd.Parameters.Add(name, type, size)
                : cmd.Parameters.Add(name, type);

            parameter.Value = value ?? DBNull.Value;
        }

        // shriang 09/10/26
        // tpo schedular
        // =====================================================
        // TPO SCHEDULER
        // =====================================================

        [HttpGet]
        public IActionResult Scheduler()
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");


if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            ViewData["Panel"] = "TPO";
            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName") ?? "TPO";

            ViewData["Title"] = "TPO Scheduler";

            // Load saved schedules for the logged-in TPO.
            ViewBag.GeneralActivities =
                _schedulerRepository.GetGeneralActivities(tpoId.Value);

            ViewBag.PlacementDrives =
                _schedulerRepository.GetPlacementDrives(tpoId.Value);

            return View();


}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Scheduler(TPOSchedulerPageM model)
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            // Validate only the form the user selected.
            if (model.ActivityType == "General")
            {
                foreach (var key in ModelState.Keys
                             .Where(k => k.StartsWith(
                                 "Placement.",
                                 StringComparison.OrdinalIgnoreCase))
                             .ToList())
                {
                    ModelState.Remove(key);
                }
            }
            else if (model.ActivityType == "PlacementDrive")
            {
                foreach (var key in ModelState.Keys
                             .Where(k => k.StartsWith(
                                 "General.",
                                 StringComparison.OrdinalIgnoreCase))
                             .ToList())
                {
                    ModelState.Remove(key);
                }
            }
            else
            {
                ModelState.AddModelError(
                    "ActivityType", "Please select a valid activity type.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ActivityType == "General")
                    {
                        var activity = model.General;

                        if (activity.EndDate.Date < activity.StartDate.Date)
                        {
                            ModelState.AddModelError(
                                "General.EndDate",
                                "End date cannot be earlier than start date.");
                        }
                        else if (activity.EndDate.Date == activity.StartDate.Date
                                 && activity.EndTime <= activity.StartTime)
                        {
                            ModelState.AddModelError(
                                "General.EndTime",
                                "End time must be later than start time.");
                        }
                        else
                        {
                            activity.TPOID = tpoId.Value;

                            _schedulerRepository.SaveGeneralActivity(
                                activity, tpoId.Value);

                            TempData["SchedulerSuccess"] =
                                "General activity saved successfully.";

                            return RedirectToAction(nameof(Scheduler));
                        }
                    }
                    else if (model.ActivityType == "PlacementDrive")
                    {
                        var drive = model.Placement;

                        if (drive.EndDate.Date < drive.StartDate.Date)
                        {
                            ModelState.AddModelError(
                                "Placement.EndDate",
                                "End date cannot be earlier than start date.");
                        }
                        else if (drive.StartDate.Date == drive.EndDate.Date
                                 && drive.StartTime.HasValue
                                 && drive.EndTime.HasValue
                                 && drive.EndTime.Value <= drive.StartTime.Value)
                        {
                            ModelState.AddModelError(
                                "Placement.EndTime",
                                "End time must be later than start time.");
                        }
                        else
                        {
                            drive.TPOID = tpoId.Value;

                            if (drive.PlacementDriveID > 0)
                            {
                                bool updated = _schedulerRepository.UpdatePlacementDrive(
                                    drive, tpoId.Value);

                                if (!updated)
                                {
                                    ModelState.AddModelError(
                                        string.Empty,
                                        "Placement drive not found or you are not authorized to edit it.");
                                }
                                else
                                {
                                    TempData["SchedulerSuccess"] =
                                        "Placement drive updated successfully.";

                                    return RedirectToAction(nameof(Scheduler));
                                }
                            }
                            else
                            {
                                _schedulerRepository.SavePlacementDrive(
                                    drive, tpoId.Value);

                                TempData["SchedulerSuccess"] =
                                    "Placement drive saved successfully.";

                                return RedirectToAction(nameof(Scheduler));
                            }

                            return RedirectToAction(nameof(Scheduler));
                        }
                    }
                }
                catch (Exception)
                {
                    // Log the exception using your application's logger.
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to save the schedule. Please check the database configuration and try again.");
                }
            }

            ViewData["Panel"] = "TPO";
            ViewData["UserName"] =
                HttpContext.Session.GetString("TPOName") ?? "TPO";
            ViewData["Title"] = "TPO Scheduler";

            ViewBag.GeneralActivities =
                _schedulerRepository.GetGeneralActivities(tpoId.Value);

            ViewBag.PlacementDrives =
                _schedulerRepository.GetPlacementDrives(tpoId.Value);

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePlacementDrive(int id)
        {
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            bool deleted = _schedulerRepository.DeletePlacementDrive(
                id, tpoId.Value);

            TempData[deleted ? "SchedulerSuccess" : "SchedulerError"] =
                deleted
                    ? "Placement drive deleted successfully."
                    : "Placement drive not found or already deleted.";

            return RedirectToAction(nameof(Scheduler));
        }

    }
}