using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ErJobPortal.Controllers
{
    public class TPOController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public TPOController(
            AccountRepository accountRepository,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
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

        [HttpGet]
        [Route("TPO/CreateNotification")]
        public IActionResult CreateNotification()
        {


            return View();
        }

        [HttpGet]
        [Route("TPO/SendNotification")]
        public IActionResult SendNotification()
        {


            return View();
        }


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

        // =====================================================
        // TPO TRAINEE INFORMATION
        // =====================================================

        [HttpGet]
        public IActionResult TraineesInfo()
        {
            // =================================================
            // CHECK TPO LOGIN
            // =================================================

            int? tpoId =
                HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction(
                    "TPOLogin",
                    "Account");
            }


            // =================================================
            // GET LOGGED-IN TPO COLLEGE
            // =================================================

            string collegeName =
                HttpContext.Session.GetString(
                    "TPOCollegeName")
                ?? "";


            // =================================================
            // COLLEGE NOT FOUND
            // =================================================

            if (string.IsNullOrWhiteSpace(collegeName))
            {
                TempData["Error"] =
                    "College information is not available for this TPO.";

                return RedirectToAction(
                    "Dashboard",
                    "TPO");
            }


            // =================================================
            // GET ONLY THIS COLLEGE'S TRAINEES
            // =================================================

            List<TPOTraineeInfoM> trainees =
                _accountRepository.GetTPOTraineesByCollege(
                    collegeName);


            // =================================================
            // PAGE INFORMATION
            // =================================================

            ViewData["Panel"] = "TPO";

            ViewData["UserName"] =
                HttpContext.Session.GetString(
                    "TPOName")
                ?? "TPO";

            ViewData["Title"] =
                "Trainee Information";

            ViewBag.CollegeName =
                collegeName;

            ViewBag.TraineeCount =
                trainees.Count;


            // =================================================
            // SEND DATA TO VIEW
            // =================================================

            return View(trainees);
        }
    }
}