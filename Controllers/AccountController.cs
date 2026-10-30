using ErJobPortal.Models;
using ErJobPortal.Repositories;
using ErJobPortal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;


namespace ErJobPortal.Controllers
{
    public class AccountController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly AccountRepository _accountRepository;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly VisitorTrackingService _visitorTrackingService;
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;

        public AccountController(AccountRepository accountRepository, EmailService emailService, IConfiguration configuration, ILogger<AccountController> logger, VisitorTrackingService visitorTrackingService, IWebHostEnvironment environment, IMemoryCache memoryCache)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _visitorTrackingService = visitorTrackingService;
            _environment = environment;
            _memoryCache = memoryCache;
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
            var departments = _accountRepository.GetDepartments();
            return Json(departments);
        }

        //sanidhya 08/10/26

        // ==========================================
        // GET BRANCHES BY DEPARTMENT
        // ==========================================

        public IActionResult GetCandidateBranches()
        {
            var branches = _accountRepository.GetBranches();
            return Json(branches);
        }

        // ==========================================
        // COLLEGE
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateColleges()
        {
            var colleges = _accountRepository.GetColleges();
            return Json(colleges);
        }


        // ==========================================
        // COLLEGE CODE BY COLLEGE
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateCollegeCodes(int collegeId)
        {
            var codes = _accountRepository.GetCollegeCodes(collegeId);
            return Json(codes);
        }

        // ==============================
        // CANDIDATE REGISTER - POST
        // ==============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CandidateRegister(CandidateRegister model)
        {
            // ==========================================
            // 1. VERIFY EMAIL OTP
            // ==========================================

            string? verified =
                HttpContext.Session.GetString("CandidateOTPVerified");

            string? verifiedEmail =
                HttpContext.Session.GetString("CandidateOTPVerifiedEmail");

            string? otpEmail =
                HttpContext.Session.GetString("CandidateOTPEmail");

            string candidateEmail = (model.sEmail ?? "").Trim();

            if (verified != "true" ||
                string.IsNullOrWhiteSpace(verifiedEmail) ||
                string.IsNullOrWhiteSpace(otpEmail) ||
                !string.Equals(
                    candidateEmail, verifiedEmail,
                    StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(
                    candidateEmail, otpEmail,
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Please verify your email address using OTP before registering.";

                return View(model);
            }

            try
            {
                // ==========================================
                // 2. PROFILE IMAGE UPLOAD
                // ==========================================

                if (model.ProfileImageFile != null &&
                    model.ProfileImageFile.Length > 0)
                {
                    string[] allowedExtensions =
                    {
                ".png", ".jpg", ".jpeg"
            };

                    string extension = Path.GetExtension(
                        model.ProfileImageFile.FileName
                    ).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Only PNG, JPG and JPEG images are allowed."
                        );

                        return View(model);
                    }

                    if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Image size must not exceed 5 MB."
                        );

                        return View(model);
                    }

                    string uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "candidates"
                    );

                    Directory.CreateDirectory(uploadFolder);

                    int nextNumber = 1;

                    while (Directory.GetFiles(
                        uploadFolder,
                        nextNumber + ".*"
                    ).Length > 0)
                    {
                        nextNumber++;
                    }

                    string imageName = nextNumber + extension;

                    string filePath = Path.Combine(
                        uploadFolder,
                        imageName
                    );

                    using (var stream = new FileStream(
                        filePath,
                        FileMode.CreateNew))
                    {
                        await model.ProfileImageFile.CopyToAsync(stream);
                    }

                    model.sProfileImage = imageName;
                }
                else
                {
                    model.sProfileImage = "";
                }

                // ==========================================
                // 3. REGISTER CANDIDATE
                // ==========================================

                int result = _accountRepository.Register(model);

                if (result <= 0)
                {
                    ModelState.AddModelError(
                        "",
                        "Registration failed. Please try again."
                    );

                    return View(model);
                }

                // ==========================================
                // 4. CLEAR OTP SESSION AFTER REGISTRATION
                // ==========================================

                HttpContext.Session.Remove("CandidateOTP");
                HttpContext.Session.Remove("CandidateOTPEmail");
                HttpContext.Session.Remove("CandidateOTPExpiry");
                HttpContext.Session.Remove("CandidateOTPVerified");
                HttpContext.Session.Remove("CandidateOTPVerifiedEmail");

                // ==========================================
                // 5. SEND REGISTRATION SUCCESS EMAIL
                // ==========================================

                try
                {
                    await SendTRRegistrationEmail(
                        candidateEmail,
                        model.sFName ?? "Candidate"
                    );
                }
                catch (Exception emailEx)
                {
                    // Registration has succeeded.
                    // Email failure must not undo the registration.
                    Console.Error.WriteLine(
                        "Candidate registration email failed: " +
                        emailEx
                    );
                }

                TempData["Success"] =
                    "Registration successful. Please login.";

                return RedirectToAction("CandidateLogin");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Candidate registration failed: " + ex
                );

                ModelState.AddModelError(
                    "",
                    "Registration failed. Please try again."
                );

                return View(model);
            }
        }

        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //   public IActionResult CandidateRegister(CandidateRegister model)
        //   {
        //       // ==========================================
        //       // 1. VERIFY EMAIL OTP FIRST
        //       // ==========================================

        //       string? verified = HttpContext.Session.GetString("CandidateOTPVerified");

        //       string? verifiedEmail = HttpContext.Session.GetString("CandidateOTPVerifiedEmail");

        //       string? otpEmail = HttpContext.Session.GetString("CandidateOTPEmail");

        //       string candidateEmail = (model.sEmail ?? string.Empty).Trim();

        //       if (verified != "true" || string.IsNullOrWhiteSpace(verifiedEmail) ||
        //           string.IsNullOrWhiteSpace(otpEmail) || !string.Equals(candidateEmail, verifiedEmail, StringComparison.OrdinalIgnoreCase) ||
        //           !string.Equals(candidateEmail, otpEmail, StringComparison.OrdinalIgnoreCase))
        //       {
        //           TempData["Error"] = "Please verify your email address using OTP before registering.";
        //           return View(model);
        //       }

        //       try
        //       {
        //           // ==========================================
        //           // PROFILE IMAGE UPLOAD
        //           // ==========================================

        //           if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
        //           {
        //               string[] allowedExtensions =
        //               {
        //    ".png", ".jpg", ".jpeg"
        //};

        //               string extension = Path.GetExtension(model.ProfileImageFile.FileName).ToLowerInvariant();

        //               if (!allowedExtensions.Contains(extension))
        //               {
        //                   ModelState.AddModelError(
        //                       "ProfileImageFile",
        //                       "Only PNG, JPG and JPEG images are allowed."
        //                   );

        //                   return View(model);
        //               }

        //               if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
        //               {
        //                   ModelState.AddModelError(
        //                       "ProfileImageFile",
        //                       "Image size must be less than 5 MB."
        //                   );

        //                   return View(model);
        //               }

        //               string uploadFolder = Path.Combine(
        //                   Directory.GetCurrentDirectory(),
        //                   "wwwroot",
        //                   "uploads",
        //                   "candidates"
        //               );

        //               if (!Directory.Exists(uploadFolder))
        //               {
        //                   Directory.CreateDirectory(uploadFolder);
        //               }

        //               int nextNumber = 1;

        //               while (Directory.GetFiles(
        //                   uploadFolder,
        //                   nextNumber + ".*"
        //               ).Length > 0)
        //               {
        //                   nextNumber++;
        //               }

        //               string imageName = nextNumber + extension;

        //               string filePath = Path.Combine(
        //                   uploadFolder,
        //                   imageName
        //               );

        //               using (FileStream stream = new FileStream(
        //                   filePath,
        //                   FileMode.Create))
        //               {
        //                   model.ProfileImageFile.CopyTo(stream);
        //               }

        //               model.sProfileImage = imageName;
        //           }
        //           else
        //           {
        //               model.sProfileImage = "";
        //           }

        //           // ==========================================
        //           // REGISTER CANDIDATE
        //           // ==========================================

        //           int result = _accountRepository.Register(model);

        //           if (result > 0)
        //           {
        //               // OTP must not be reusable.
        //               HttpContext.Session.Remove("CandidateOTP");
        //               HttpContext.Session.Remove("CandidateOTPEmail");
        //               HttpContext.Session.Remove("CandidateOTPExpiry");

        //               if (result > 0)
        //               {
        //                   // Send registration success email
        //                   try
        //                   {
        //                       SendTRRegistrationEmail(model.sEmail, model.sFName);
        //                   }
        //                   catch (Exception emailEx)
        //                   {
        //                       // Registration succeeded, even if email failed
        //                       Console.WriteLine("Organization registration email error: "
        //                           + emailEx
        //                       );
        //                   }

        //                   TempData["Success"] =
        //                   "Registration successful. Please login.";

        //               return RedirectToAction("CandidateLogin");
        //           }

        //           ModelState.AddModelError(
        //               "",
        //               "Registration failed."
        //           );

        //           return View(model);
        //       }
        //       catch (Exception ex)
        //       {
        //           ModelState.AddModelError(
        //               "",
        //               "Registration failed. Please try again."
        //           );

        //           // Log ex using your application's logger if available.
        //           return View(model);
        //       }
        //   }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public IActionResult CandidateRegister(CandidateRegister model)
        //        {
        //            // STATIC OTP FOR NOW
        //            //if (model.sOTP != "123456")
        //            //{
        //            //    ModelState.AddModelError(
        //            //        "sOTP",
        //            //        "Invalid OTP. Please enter 123456."
        //            //    );
        //            //}

        //            if (!ModelState.IsValid)
        //            {
        //                return View(model);
        //            }

        //            try
        //            {

        //                // ==========================================
        //                // PROFILE IMAGE UPLOAD
        //                // ==========================================

        //                if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
        //                {
        //                    // ==========================================
        //                    // ALLOWED IMAGE TYPES
        //                    // ==========================================

        //                    string[] allowedExtensions =
        //                    {
        //    ".png",
        //    ".jpg",
        //    ".jpeg"
        //};

        //                    string extension = Path.GetExtension(
        //                            model.ProfileImageFile.FileName
        //                        ).ToLowerInvariant();

        //                    // ==========================================
        //                    // CHECK IMAGE EXTENSION
        //                    // ==========================================

        //                    if (!allowedExtensions.Contains(extension))
        //                    {
        //                        ModelState.AddModelError(
        //                            "ProfileImageFile",
        //                            "Only PNG, JPG and JPEG images are allowed."
        //                        );

        //                        return View(model);
        //                    }

        //                    // ==========================================
        //                    // MAXIMUM 5 MB
        //                    // ==========================================

        //                    if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
        //                    {
        //                        ModelState.AddModelError(
        //                            "ProfileImageFile",
        //                            "Image size must be less than 5 MB."
        //                        );

        //                        return View(model);
        //                    }

        //                    // ==========================================
        //                    // CREATE UPLOAD FOLDER
        //                    // ==========================================

        //                    string uploadFolder = Path.Combine(
        //                        Directory.GetCurrentDirectory(),
        //                        "wwwroot",
        //                        "uploads",
        //                        "candidates"
        //                    );

        //                    if (!Directory.Exists(uploadFolder))
        //                    {
        //                        Directory.CreateDirectory(uploadFolder);
        //                    }

        //                    // ==========================================
        //                    // GENERATE IMAGE NAME: 1, 2, 3, 4...
        //                    // ==========================================

        //                    int nextNumber = 1;

        //                    while (Directory.GetFiles(
        //                        uploadFolder,
        //                        nextNumber + ".*"
        //                    ).Length > 0)
        //                    {
        //                        nextNumber++;
        //                    }

        //                    // Example: 1.jpg, 2.png, 3.jpeg
        //                    string imageName =
        //                        nextNumber + extension;

        //                    // ==========================================
        //                    // SAVE IMAGE
        //                    // ==========================================

        //                    string filePath = Path.Combine(
        //                        uploadFolder,
        //                        imageName
        //                    );

        //                    using (FileStream stream =
        //                           new FileStream(
        //                               filePath,
        //                               FileMode.Create))
        //                    {
        //                        model.ProfileImageFile.CopyTo(stream);
        //                    }

        //                    // ==========================================
        //                    // ONLY IMAGE NAME GOES TO DATABASE
        //                    // ==========================================

        //                    model.sProfileImage = imageName;
        //                }
        //                else
        //                {
        //                    model.sProfileImage = "";
        //                }

        //                int result = _accountRepository.Register(model);

        //                if (result > 0)
        //                {
        //                    TempData["Success"] =
        //                        "Registration successful. Please login.";

        //                    return RedirectToAction("CandidateLogin");
        //                }

        //                ModelState.AddModelError(
        //                    "",
        //                    "Registration failed."
        //                );

        //                return View(model);
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError(
        //                    "",
        //                    ex.Message
        //                );

        //                return View(model);
        //            }
        //        }



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
        //sanidhya 08/10/26
        [HttpGet]
        public IActionResult GetBranches()
        {
            var branches =
                _accountRepository.GetBranches();

            return Json(branches);
        }
        //public IActionResult GetBranches(int departmentId)
        //{
        //    var branches =
        //        _accountRepository.GetBranches(departmentId);

        //    return Json(branches);
        //}

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
        // khushi 16-10-26
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationRegister(OrganizationRegister model)
        {
            try
            {
                // Static OTP validation
                if (model.sOTP != "123456")
                {
                    ModelState.AddModelError(
                        "sOTP",
                        "Invalid OTP. Please enter 123456."
                    );
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Repository hashes the password before saving
                int result = _accountRepository.RegisterOrganization(model);

                if (result > 0)
                {
                    TempData["Success"] =
                        "Organization Registration Successfully.";

                    return RedirectToAction("OrganizationRegister");
                }

                TempData["Error"] = "Organization Registration Failed.";
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception on the server
                Console.WriteLine(ex.ToString());

                ModelState.AddModelError(
                    "",
                    "Registration failed. Please try again."
                );

                return View(model);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult OrganizationRegister(OrganizationRegister model)
        //{
        //    try
        //    {
        //        // STATIC OTP FOR NOW
        //        if (model.sOTP != "123456")
        //        {
        //            ModelState.AddModelError(
        //                "sOTP",
        //                "Invalid OTP. Please enter 123456."
        //            );
        //        }

        //        // Check validation errors
        //        if (!ModelState.IsValid)
        //        {
        //            foreach (var item in ModelState)
        //            {
        //                foreach (var error in item.Value.Errors)
        //                {
        //                    Console.WriteLine(
        //                        $"Field: {item.Key}, Error: {error.ErrorMessage}"
        //                    );
        //                }
        //            }

        //            return View(model);
        //        }

        //        int result = _accountRepository.RegisterOrganization(model);

        //        if (result > 0)
        //        {
        //            TempData["Success"] =
        //                "Organization Registration Successfully.";

        //            return RedirectToAction("OrganizationRegister");
        //        }

        //        TempData["Error"] =
        //            "Organization Registration Failed.";

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Organization Register Error:");
        //        Console.WriteLine(ex.ToString());

        //        ModelState.AddModelError(
        //            "",
        //            ex.Message
        //        );

        //        return View(model);
        //    }
        //}

        [HttpGet]
        public IActionResult OrganizationLogin()
        {
            return View();
        }


        //khushi 27-10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationLogin(OrganizationLogin model)
        {
            // Validate email and password fields
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get organization by email
            OrganizationUser? user =
                _accountRepository.LoginOrganization(model);

            // Email not registered
            if (user == null)
            {
                ModelState.AddModelError(
                    nameof(model.sEmail),
                    "Invalid email address."
                );

                return View(model);
            }

            // Verify entered password against stored password hash
            var passwordHasher = new PasswordHasher<OrganizationUser>();

            PasswordVerificationResult passwordResult;

            try
            {
                passwordResult = passwordHasher.VerifyHashedPassword(
                    user,
                    user.sPassword ?? "",
                    model.sPassword ?? ""
                );
            }
            catch (FormatException)
            {
                TempData["Error"] =
                    "Stored password format is invalid. Please reset your password.";

                return View(model);
            }

            // Incorrect password
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    nameof(model.sPassword),
                    "Incorrect password. Please try again."
                );

                return View(model);
            }

            // Block organizations disabled by Super Admin
            if (!user.nSABit)
            {
                TempData["Error"] =
                    "Your organization account has been disabled by the Super Admin.";

                return View(model);
            }

            // Create organization sessions
            HttpContext.Session.SetInt32("OrgID", user.nID);

            HttpContext.Session.SetString(
                "OrgName",
                user.sOrgName ?? ""
            );

            HttpContext.Session.SetString(
                "OrgEmail",
                user.sEmail ?? ""
            );

            // Store organization name for the shared header
            HttpContext.Session.SetString(
                "sOrgName",
                user.sOrgName ?? ""
            );

            // Get organization logo from database
            string? companyLogo = null;

            string logoQuery = @"
   SELECT TOP 1 sCompanyLogo
   FROM tblOrgProfile
   WHERE nOrgID = @nOrgID
     AND nBit = 1
   ORDER BY nID DESC;";

            using (SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand(logoQuery, con))
            {
                cmd.Parameters.Add("@nOrgID", SqlDbType.Int).Value = user.nID;

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    companyLogo = result.ToString();
                }
            }

            // Store organization logo filename in Session
            HttpContext.Session.SetString(
                "OrganizationLogo",
                companyLogo ?? ""
            );

            // Redirect after successful login
            return RedirectToAction("Dashboard", "Organization");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult OrganizationLogin(OrganizationLogin model)
        //{
        //    // Validate email and password fields
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Get organization by email
        //    OrganizationUser? user =
        //        _accountRepository.LoginOrganization(model);

        //    // Email not registered
        //    if (user == null)
        //    {
        //        ModelState.AddModelError(
        //            nameof(model.sEmail),
        //            "Invalid email address."
        //        );

        //        return View(model);
        //    }

        //    // Verify entered password against stored password hash
        //    var passwordHasher = new PasswordHasher<OrganizationUser>();

        //    PasswordVerificationResult passwordResult;

        //    try
        //    {
        //        passwordResult = passwordHasher.VerifyHashedPassword(
        //            user,
        //            user.sPassword ?? "",
        //            model.sPassword ?? ""
        //        );
        //    }
        //    catch (FormatException)
        //    {
        //        TempData["Error"] =
        //            "Stored password format is invalid. Please reset your password.";

        //        return View(model);
        //    }

        //    // Incorrect password
        //    if (passwordResult == PasswordVerificationResult.Failed)
        //    {
        //        ModelState.AddModelError(
        //            nameof(model.sPassword),
        //            "Incorrect password. Please try again."
        //        );

        //        return View(model);
        //    }

        //    // Block organizations disabled by Super Admin
        //    if (!user.nSABit)
        //    {
        //        TempData["Error"] =
        //            "Your organization account has been disabled by the Super Admin.";

        //        return View(model);
        //    }

        //    // Create organization sessions
        //    HttpContext.Session.SetInt32("OrgID", user.nID);

        //    HttpContext.Session.SetString(
        //        "OrgName",
        //        user.sOrgName ?? ""
        //    );

        //    HttpContext.Session.SetString(
        //        "OrgEmail",
        //        user.sEmail ?? ""
        //    );

        //    // Redirect after successful login
        //    return RedirectToAction("Dashboard", "Organization");
        //}


        // candidate login

        [HttpGet]
        public IActionResult CandidateLogin()
        {
            return View();
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateLogin(CandidateLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CandidateUser? user = _accountRepository.LoginCandidate(model);

            // Email not registered
            if (user == null)
            {
                ModelState.AddModelError(
                    nameof(model.sEmail),
                    "Invalid email address."
                );

                return View(model);
            }

            // Verify password
            var passwordHasher = new PasswordHasher<CandidateUser>();

            var passwordResult = passwordHasher.VerifyHashedPassword(
                user,
                user.sPassword ?? "",
                model.sPassword ?? ""
            );

            // Incorrect password
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    nameof(model.sPassword),
                    "Incorrect password. Please try again."
                );

                return View(model);
            }

            // Candidate disabled by Super Admin
            if (!user.nSABit)
            {
                TempData["Error"] =
                    "Your account has been disabled by the Super Admin. Please contact the administrator.";

                return View(model);
            }

            // Set session
            HttpContext.Session.SetInt32("CandidateID", user.nID);

            HttpContext.Session.SetString(
                "CandidateName",
                (user.sFName + " " + user.sLName).Trim()
            );

            HttpContext.Session.SetString(
                "CandidateEmail",
                user.sEmail ?? ""
            );

            return RedirectToAction("Dashboard", "Candidate");
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

        // shrirang 15/09/26

        // ==========================================================
        // Candidate Forgot Password
        // ==========================================================

        [HttpGet]
        public IActionResult CandidateForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CandidateForgotPassword(
            CandidateForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail.Trim();

                CandidateLogin? candidate =
                    _accountRepository.GetCandidateLoginDetails(email);

                if (candidate == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No candidate account was found with this email address."
                    );

                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(candidate.sPassword))
                {
                    ModelState.AddModelError(
                        "",
                        "Password information is not available for this account."
                    );

                    return View(model);
                }

                await _emailService.SendCandidateLoginDetailsAsync(
                    candidate.sEmail,
                    candidate.sPassword
                );

                TempData["Success"] =
                    "Your login details have been sent to your registered email address.";

                return RedirectToAction("CandidateLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("Candidate Forgot Password Error");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("========================================");

                ModelState.AddModelError(
                    "",
                    "Email Error: " + ex.Message
                );

                return View(model);
            }

           
        }

        
        // ==========================================================
        // Candidate Reset Password
        // ==========================================================

        [HttpGet]
        public IActionResult CandidateResetPassword()
                {
                    return View();
                }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCandidateResetOTP(
          [FromForm] string sEmail)
        {
            string email = sEmail?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(email) ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid registered email address."
                });
            }

            try
            {
                CandidateLogin? candidate =
                    _accountRepository.GetCandidateLoginDetails(email);

                if (candidate == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No candidate account was found with this email address."
                    });
                }

                string otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // Send the email before storing the OTP.
                await _emailService.SendOtpEmailAsync(
                     email,
                     otp,
                     "password-reset");

                HttpContext.Session.SetString("CandidateResetOtp", otp);
                HttpContext.Session.SetString("CandidateResetOtpEmail", email);

                HttpContext.Session.SetString(
                    "CandidateResetOtpExpiry",
                    DateTimeOffset.UtcNow.AddMinutes(10)
                        .ToUnixTimeSeconds().ToString());

                // Clear any earlier verification.
                HttpContext.Session.Remove("CandidateResetVerifiedEmail");

                return Json(new
                {
                    success = true,
                    message = "OTP sent successfully. Please check your registered email."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Candidate Reset OTP Error: " + ex);

                return Json(new
                {
                    success = false,
                    message = "Unable to send OTP right now. Please try again later."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCandidateResetOTP(
    [FromForm] string sEmail,
    [FromForm] string otp)
        {
            string email = sEmail?.Trim() ?? "";
            string enteredOtp = otp?.Trim() ?? "";

            string? savedOtp =
                HttpContext.Session.GetString("CandidateResetOtp");

            string? savedEmail =
                HttpContext.Session.GetString("CandidateResetOtpEmail");

            string? expiryText =
                HttpContext.Session.GetString("CandidateResetOtpExpiry");

            if (string.IsNullOrWhiteSpace(savedOtp) ||
                string.IsNullOrWhiteSpace(savedEmail) ||
                string.IsNullOrWhiteSpace(expiryText) ||
                !long.TryParse(expiryText, out long expiry))
            {
                return Json(new
                {
                    success = false,
                    message = "OTP is missing or expired. Please request a new OTP."
                });
            }

            if (!string.Equals(
                email, savedEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = false,
                    message = "The email does not match the email used to request the OTP."
                });
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiry)
            {
                ClearCandidateResetOtpSession();

                return Json(new
                {
                    success = false,
                    message = "OTP has expired. Please request a new OTP."
                });
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                enteredOtp, @"^\d{6}$") ||
                !string.Equals(enteredOtp, savedOtp, StringComparison.Ordinal))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid OTP. Please enter the correct 6-digit OTP."
                });
            }

            // Store verified email server-side.
            HttpContext.Session.SetString(
                "CandidateResetVerifiedEmail", savedEmail);

            // Make the OTP one-time use.
            HttpContext.Session.Remove("CandidateResetOtp");
            HttpContext.Session.Remove("CandidateResetOtpExpiry");

            return Json(new
            {
                success = true,
                message = "Email verified successfully. You can now reset your password."
            });
        }

        private void ClearCandidateResetOtpSession()
        {
            HttpContext.Session.Remove("CandidateResetOtp");
            HttpContext.Session.Remove("CandidateResetOtpEmail");
            HttpContext.Session.Remove("CandidateResetOtpExpiry");
            HttpContext.Session.Remove("CandidateResetVerifiedEmail");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateResetPassword(
            CandidateResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail?.Trim() ?? "";

                string? verifiedEmail =
                    HttpContext.Session.GetString("CandidateResetVerifiedEmail");

                if (string.IsNullOrWhiteSpace(verifiedEmail) ||
                    !string.Equals(
                        email, verifiedEmail, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        "",
                        "Please verify the OTP for your registered email before resetting your password.");

                    return View(model);
                }

                CandidateLogin? candidate =
                    _accountRepository.GetCandidateLoginDetails(email);

                if (candidate == null)
                {
                    ClearCandidateResetOtpSession();

                    ModelState.AddModelError(
                        nameof(model.sEmail),
                        "No candidate account was found with this email address.");

                            return View(model);
                        }

                bool updated = _accountRepository.ResetCandidatePassword(
                    email,
                    model.NewPassword);

                if (!updated)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to reset password. Please try again.");

                            return View(model);
                        }

                ClearCandidateResetOtpSession();

                TempData["Success"] =
                    "Your password has been reset successfully. Please login with your new password.";

                return RedirectToAction("CandidateLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Candidate Reset Password Error: " + ex);

                ModelState.AddModelError(
                    "",
                    "Unable to reset password. Please try again later.");

                        return View(model);
                    }
                }


        // ==========================================================
        // ORGANIZATION RESET PASSWORD
        // ==========================================================

        [HttpGet]
        public IActionResult OrganizationResetPassword()
        {
            ClearOrganizationResetOtpSession();

            return View(new OrganizationResetPassword());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOrganizationResetOTP(
            [FromForm] string sEmail)
        {
            string email = sEmail?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(email) ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid registered email address."
                });
            }

            try
            {
                OrganizationLogin? organization =
                    _accountRepository.GetOrganizationLoginDetails(email);

                if (organization == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No organization account was found with this email address."
                    });
                }

                string otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // Send email first. Store OTP only after sending succeeds.
                await _emailService.SendOrganizationOtpEmailAsync(
                    email,
                    otp,
                    "password-reset");

                // Store OTP and expiry on the server.
                HttpContext.Session.SetString("OrgResetOtp", otp);
                HttpContext.Session.SetString("OrgResetOtpEmail", email);

                HttpContext.Session.SetString(
                    "OrgResetOtpExpiry",
                    DateTimeOffset.UtcNow.AddMinutes(10)
                        .ToUnixTimeSeconds().ToString());

                // A newly sent OTP invalidates any earlier verification.
                HttpContext.Session.Remove("OrgResetVerifiedEmail");

                return Json(new
                {
                    success = true,
                    message = "OTP sent successfully. Please check your registered email."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Organization Reset OTP Error: " + ex);

                return Json(new
                {
                    success = false,
                    message = "Unable to send OTP right now. Please try again later."
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOrganizationResetOTP(
                [FromForm] string sEmail,
                [FromForm] string otp)
        {
            string email = sEmail?.Trim() ?? "";
            string enteredOtp = otp?.Trim() ?? "";

            string? savedOtp =
                HttpContext.Session.GetString("OrgResetOtp");

            string? savedEmail =
                HttpContext.Session.GetString("OrgResetOtpEmail");

            string? expiryText =
                HttpContext.Session.GetString("OrgResetOtpExpiry");

            if (string.IsNullOrWhiteSpace(savedOtp) ||
                string.IsNullOrWhiteSpace(savedEmail) ||
                string.IsNullOrWhiteSpace(expiryText) ||
                !long.TryParse(expiryText, out long expiry))
            {
                return Json(new
                {
                    success = false,
                    message = "OTP is missing or expired. Please request a new OTP."
                });
            }

            if (!string.Equals(
                email, savedEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = false,
                    message = "The email does not match the email used to request the OTP."
                });
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiry)
            {
                ClearOrganizationResetOtpSession();

                return Json(new
                {
                    success = false,
                    message = "OTP has expired. Please request a new OTP."
                });
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    enteredOtp, @"^\d{6}$") ||
                !string.Equals(
                    enteredOtp, savedOtp, StringComparison.Ordinal))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid OTP. Please enter the correct 6-digit OTP."
                });
            }

            // Keep verified email on the server.
            HttpContext.Session.SetString(
                "OrgResetVerifiedEmail", savedEmail);

            // Make the OTP one-time use.
            HttpContext.Session.Remove("OrgResetOtp");
            HttpContext.Session.Remove("OrgResetOtpExpiry");

            return Json(new
            {
                success = true,
                message = "Email verified successfully. You can now reset your password."
            });
        }


        private void ClearOrganizationResetOtpSession()
        {
            HttpContext.Session.Remove("OrgResetOtp");
            HttpContext.Session.Remove("OrgResetOtpEmail");
            HttpContext.Session.Remove("OrgResetOtpExpiry");
            HttpContext.Session.Remove("OrgResetVerifiedEmail");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationResetPassword(
                            OrganizationResetPassword model)
        {
            string email = model.sEmail?.Trim() ?? "";

            string? verifiedEmail =
                HttpContext.Session.GetString("OrgResetVerifiedEmail");

            // Verify OTP authorization on the server.
            if (string.IsNullOrWhiteSpace(verifiedEmail) ||
                !string.Equals(
                    email,
                    verifiedEmail,
                    StringComparison.OrdinalIgnoreCase))
            {
                ClearOrganizationResetOtpSession();

                ModelState.AddModelError(
                    "",
                    "Please verify your organization email using OTP before resetting your password.");

                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                OrganizationLogin? organization =
                    _accountRepository.GetOrganizationLoginDetails(email);

                if (organization == null)
                {
                    ClearOrganizationResetOtpSession();

                    ModelState.AddModelError(
                        "sEmail",
                        "No organization account was found with this email address.");

                    return View(model);
                }

                bool updated = _accountRepository.ResetOrganizationPassword(
                    email,
                    model.NewPassword);

                if (!updated)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to reset password. Please try again.");

                    return View(model);
                }

                // Clear verification after a successful password reset.
                ClearOrganizationResetOtpSession();

                TempData["Success"] =
                    "Your password has been reset successfully. Please login with your new password.";

                return RedirectToAction("OrganizationLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Organization Reset Password Error: " + ex);

                ModelState.AddModelError(
                    "",
                    "Unable to reset password. Please try again later.");

                return View(model);
            }
        }


        // ==========================================================
        // Organization Forgot Password
        // ==========================================================

        [HttpGet]
        public IActionResult OrganizationForgotPassword()
        {
            return View(new OrganizationForgotPassword());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationForgotPassword(
            OrganizationForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail.Trim();

                // Check whether organization exists
                OrganizationLogin? organization =
                    _accountRepository.GetOrganizationLoginDetails(email);

                if (organization == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No organization account was found with this email address."
                    );

                    return View(model);
                }

                // Pass email to the reset page
                TempData["ResetEmail"] = organization.sEmail ?? email;

                TempData["Success"] =
                    "Organization found. Please reset your password.";

                return RedirectToAction("OrganizationResetPassword");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Organization Forgot Password Error: " + ex
                );

                ModelState.AddModelError(
                    "",
                    "Unable to process your request. Please try again later."
                );

                return View(model);
            }
        }





        //================================//
        //TPORegistration//
        //================================//
        // shrirang 23/09/26
        // ==========================================================
        // TPO REGISTRATION - GET
        // ==========================================================

        [HttpGet]
        public IActionResult TPORegister()
        {
            return View();
        }


        // ==========================================================
        // TPO REGISTRATION - POST
        // ==========================================================
        // shrirang 22/10/26


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TPORegister(TPORegistration model)
        {
            string uploadDirectory = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "tpo");

            var savedFiles = new List<string>();

            string email = model.CollegeMailID?.Trim() ?? "";

            string verifiedKey =
                "TPO_EMAIL_VERIFIED_" + email.ToLowerInvariant();

            try
            {
                model.CollegeMailID = email;
                model.MobileNo = model.MobileNo?.Trim() ?? "";

                // ============================================
                // 1. DUPLICATE EMAIL CHECK
                // ============================================

                if (!string.IsNullOrWhiteSpace(model.CollegeMailID) &&
                    _accountRepository.TPOEmailExists(model.CollegeMailID))
                {
                    ModelState.AddModelError(
                        nameof(model.CollegeMailID),
                        "This college email is already registered.");
                }

                // ============================================
                // 2. DUPLICATE MOBILE CHECK
                // ============================================

                if (!string.IsNullOrWhiteSpace(model.MobileNo) &&
                    _accountRepository.TPOMobileExists(model.MobileNo))
                {
                    ModelState.AddModelError(
                        nameof(model.MobileNo),
                        "This mobile number is already registered.");
                }

                // ============================================
                // 3. REQUIRED FILES
                // ============================================

                if (model.SupportingDocument1File == null ||
                    model.SupportingDocument1File.Length == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.SupportingDocument1File),
                        "Supporting Document 1 is required.");
                }

                if (model.SupportingDocument2File == null ||
                    model.SupportingDocument2File.Length == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.SupportingDocument2File),
                        "Supporting Document 2 is required.");
                }

                if (model.ProfilePhotoFile == null ||
                    model.ProfilePhotoFile.Length == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.ProfilePhotoFile),
                        "Profile photo is required.");
                }

                // ============================================
                // 4. FILE TYPE AND SIZE VALIDATION
                // ============================================

                string[] documentExtensions =
                {
                    ".pdf", ".jpg", ".jpeg", ".png"
                };

                string[] photoExtensions =
                {
                    ".jpg", ".jpeg", ".png"
                };

                bool IsAllowedFile(
                    IFormFile? file,
                    string[] allowedExtensions,
                    long maxSizeBytes)
                {
                    if (file == null || file.Length == 0)
                        return false;

                    string extension =
                        Path.GetExtension(file.FileName).ToLowerInvariant();

                    return allowedExtensions.Contains(extension)
                           && file.Length <= maxSizeBytes;
                }

                if (model.SupportingDocument1File != null &&
                    model.SupportingDocument1File.Length > 0 &&
                    !IsAllowedFile(
                        model.SupportingDocument1File,
                        documentExtensions,
                        10 * 1024 * 1024))
                {
                    ModelState.AddModelError(
                        nameof(model.SupportingDocument1File),
                        "Document 1 must be PDF, JPG, JPEG or PNG and not exceed 10 MB.");
                }

                if (model.SupportingDocument2File != null &&
                    model.SupportingDocument2File.Length > 0 &&
                    !IsAllowedFile(
                        model.SupportingDocument2File,
                        documentExtensions,
                        10 * 1024 * 1024))
                {
                    ModelState.AddModelError(
                        nameof(model.SupportingDocument2File),
                        "Document 2 must be PDF, JPG, JPEG or PNG and not exceed 10 MB.");
                }

                if (model.ProfilePhotoFile != null &&
                    model.ProfilePhotoFile.Length > 0 &&
                    !IsAllowedFile(
                        model.ProfilePhotoFile,
                        photoExtensions,
                        5 * 1024 * 1024))
                {
                    ModelState.AddModelError(
                        nameof(model.ProfilePhotoFile),
                        "Profile photo must be JPG, JPEG or PNG and not exceed 5 MB.");
                }

                // ============================================
                // 5. CHECK SERVER-SIDE EMAIL VERIFICATION
                // ============================================

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError(
                        nameof(model.CollegeMailID),
                        "College email is required.");

                    return View(model);
                }

                if (!_memoryCache.TryGetValue(
                        verifiedKey,
                        out bool isVerified) ||
                    !isVerified)
                {
                    ModelState.AddModelError(
                        nameof(model.OTP),
                        "Please verify your college email using OTP before registering.");

                    return View(model);
                }

                // Do not check the OTP again here.
                // VerifyTPOOTP has already validated and consumed it.
                // Keep the verification marker if other fields are invalid.

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // ============================================
                // 6. SAVE UPLOADED FILES
                // ============================================

                Directory.CreateDirectory(uploadDirectory);

                async Task<string> SaveFileAsync(IFormFile file)
                {
                    string extension =
                        Path.GetExtension(file.FileName).ToLowerInvariant();

                    string fileName =
                        Guid.NewGuid().ToString("N") + extension;

                    string fullPath =
                        Path.Combine(uploadDirectory, fileName);

                    await using (var stream = new FileStream(
                        fullPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedFiles.Add(fullPath);

                    // Save relative paths in the database.
                    return Path.Combine("uploads", "tpo", fileName)
                        .Replace("\\", "/");
                }

                model.SupportingDocument1 =
                    await SaveFileAsync(model.SupportingDocument1File!);

                model.SupportingDocument2 =
                    await SaveFileAsync(model.SupportingDocument2File!);

                model.ProfilePhoto =
                    await SaveFileAsync(model.ProfilePhotoFile!);

                // ============================================
                // 7. SET SERVER-VERIFIED REGISTRATION VALUES
                // ============================================

                model.OTPVerified = true;
                model.OTP = null;
                model.OTPType = "Email";

                model.Status = "Pending";
                model.IsApproved = false;
                model.IsActive = false;

                // ============================================
                // 8. SAVE REGISTRATION
                // ============================================

                int result = _accountRepository.RegisterTPO(model);

                if (result > 0)
                {
                    // Prevent reuse of this email verification.
                    _memoryCache.Remove(verifiedKey);

                    TempData["Success"] =
                        "TPO registration submitted successfully. Your account is pending Super Admin approval.";

                    return RedirectToAction("TPOLogin", "Account");
                }

                ModelState.AddModelError(
                    string.Empty,
                    "TPO registration failed. Please try again.");

                // Clean up files if database insertion fails.
                foreach (string path in savedFiles)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during TPO registration.");

                // Clean up uploaded files if registration throws an exception.
                foreach (string path in savedFiles)
                {
                    try
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    catch (Exception cleanupException)
                    {
                        _logger.LogWarning(
                            cleanupException,
                            "Unable to clean up a TPO registration upload.");
                    }
                }

                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred during registration. Please try again.");

                return View(model);
            }
        }
        // ==========================================================
        // HELPER: DELETE UPLOADED FILE IF REGISTRATION FAILS
        // ==========================================================

        private void DeleteUploadedFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to delete uploaded file: " + ex.Message);
            }
        }



        // ==========================================================
        // TPO LOGIN - GET
        // ==========================================================

        [HttpGet]
        public IActionResult TPOLogin()
        {
            return View();
        }


        // =========================================================
        // TPO LOGIN - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TPOLogin(TPOLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.CollegeMailID.Trim();

                // Find TPO by college email
                TPOLoginResult? tpo = _accountRepository.LoginTPO(email);

                // Email not registered
                if (tpo == null)
                {
                    ModelState.AddModelError(
                        nameof(model.CollegeMailID),
                        "Invalid email address."
                    );

                    return View(model);
                }

                // Verify password
                var passwordHasher = new PasswordHasher<string>();

                PasswordVerificationResult passwordResult;

                try
                {
                    passwordResult = passwordHasher.VerifyHashedPassword(
                        email,
                        tpo.PasswordHash ?? "",
                        model.Password
                    );
                }
                catch (FormatException ex)
                {
                    _logger.LogError(
                        ex,
                        "Invalid password hash format for TPO email {Email}",
                        email
                    );

                    TempData["Error"] =
                        "Your stored password format is invalid. Please reset your password.";

                    return View(model);
                }

                // Incorrect password
                if (passwordResult == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(
                        nameof(model.Password),
                        "Incorrect password. Please try again."
                    );

                    return View(model);
                }

                // Set TPO session after successful login
                HttpContext.Session.SetInt32("TPOID", tpo.TPOID);

                HttpContext.Session.SetString(
                    "TPOName",
                    tpo.FullName ?? ""
                );

                HttpContext.Session.SetString(
                    "TPOEmail",
                    tpo.CollegeMailID ?? ""
                );

                HttpContext.Session.SetString(
                    "TPOCollegeName",
                    tpo.CollegeName ?? ""
                );

                HttpContext.Session.SetString("TPORole", "TPO");

                // 0 = Main TPO, 1 = Sub-TPO
                HttpContext.Session.SetString(
                    "SubTpoBit",
                    tpo.SubTpoBit ? "1" : "0"
                );

                // Record successful login; tracking failure must not block login
                try
                {
                    await _visitorTrackingService.TrackVisitAsync(
                        HttpContext,
                        "TPO",
                        "TPOLogin",
                        tpo.TPOID,
                        "TPO",
                        true
                    );
                }
                catch (Exception trackingException)
                {
                    _logger.LogError(
                        trackingException,
                        "Failed to record TPO login activity for TPOID {TPOID}",
                        tpo.TPOID
                    );
                }

                return RedirectToAction("Dashboard", "TPO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TPO login failed.");

                TempData["Error"] = "Unable to login. Please try again later.";

                return View(model);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> TPOLogin(TPOLogin model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(model.CollegeMailID))
        //        {
        //            ModelState.AddModelError(
        //                "",
        //                "Please enter your College Mail ID."
        //            );

        //            return View(model);
        //        }

        //        string email = model.CollegeMailID.Trim();

        //        TPOLoginResult? tpo =
        //            _accountRepository.LoginTPO(email);

        //        // CHECK TPO EXISTS
        //        if (tpo == null)
        //        {
        //            ModelState.AddModelError(
        //                "",
        //                "Invalid College Mail ID or Password."
        //            );

        //            return View(model);
        //        }

        //        // CHECK PASSWORD
        //        // Existing comparison retained from your code.
        //        if (tpo.PasswordHash != model.Password)
        //        {
        //            ModelState.AddModelError(
        //                "",
        //                "Invalid College Mail ID or Password."
        //            );

        //            return View(model);
        //        }

        //        // SET TPO SESSION
        //        HttpContext.Session.SetInt32(
        //            "TPOID",
        //            tpo.TPOID
        //        );

        //        HttpContext.Session.SetString(
        //            "TPOName",
        //            tpo.FullName ?? ""
        //        );

        //        HttpContext.Session.SetString(
        //            "TPOEmail",
        //            tpo.CollegeMailID ?? ""
        //        );

        //        HttpContext.Session.SetString(
        //            "TPOCollegeName",
        //            tpo.CollegeName ?? ""
        //        );

        //        HttpContext.Session.SetString(
        //            "TPORole",
        //            "TPO"
        //        );

        //        // 0 = Main TPO, 1 = Sub-TPO
        //        HttpContext.Session.SetString(
        //            "SubTpoBit",
        //            tpo.SubTpoBit ? "1" : "0"
        //        );

        //        // RECORD SUCCESSFUL TPO LOGIN
        //        try
        //        {
        //            await _visitorTrackingService.TrackVisitAsync(
        //                HttpContext,
        //                "TPO",
        //                "TPOLogin",
        //                tpo.TPOID,
        //                "TPO",
        //                true
        //            );
        //        }
        //        catch (Exception trackingException)
        //        {
        //            // Tracking failure must not prevent a valid login.
        //            _logger.LogError(
        //                trackingException,
        //                "Failed to record TPO login activity for TPOID {TPOID}",
        //                tpo.TPOID
        //            );
        //        }

        //        // REDIRECT TO TPO DASHBOARD
        //        return RedirectToAction(
        //            "Dashboard",
        //            "TPO"
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "TPO login failed.");

        //        ModelState.AddModelError(
        //            "",
        //            "Unable to login. Please try again later."
        //        );

        //        return View(model);
        //    }
        //}


        // ==========================================================
        // TPO FORGOT PASSWORD - GET
        // ==========================================================

        [HttpGet]
        public IActionResult TPOForgotPassword()
        {
            return View();
        }


        // ==========================================================
        // TPO FORGOT PASSWORD - POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TPOForgotPassword(
            TPOForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.CollegeMailID.Trim();

                // =====================================================
                // GET TPO DETAILS
                // =====================================================

                TPOLoginResult? tpo =
                    _accountRepository.LoginTPO(email);

                if (tpo == null)
                {
                    ModelState.AddModelError(
                        "CollegeMailID",
                        "No TPO account was found with this college email address."
                    );

                    return View(model);
                }

                // =====================================================
                // CHECK PASSWORD
                // =====================================================

                if (string.IsNullOrWhiteSpace(tpo.PasswordHash))
                {
                    ModelState.AddModelError(
                        "",
                        "Password information is not available for this account."
                    );

                    return View(model);
                }

                // =====================================================
                // SEND ORIGINAL PASSWORD
                // PasswordHash column contains normal password
                // =====================================================

                await _emailService.SendTPOLoginDetailsAsync(
                    tpo.CollegeMailID,
                    tpo.PasswordHash
                );

                // =====================================================
                // SUCCESS
                // =====================================================

                TempData["Success"] =
                    "Your login credentials have been sent to your registered college email address.";

                return RedirectToAction("TPOLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("TPO Forgot Password Error");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("========================================");

                ModelState.AddModelError(
                    "",
                    "Unable to send login credentials. Please try again later."
                );

                return View(model);
            }
        }

        // ==========================================================
        // TPO RESET PASSWORD - GET
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTPOResetOTP(string collegeMailID)
        {
            collegeMailID = collegeMailID?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(collegeMailID) ||
                !new EmailAddressAttribute().IsValid(collegeMailID))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid college email."
                });
            }

            // Confirm that the TPO account exists.
            if (!_accountRepository.TPOEmailExists(collegeMailID))
            {
                return Json(new
                {
                    success = false,
                    message = "No TPO account was found for this email."
                });
            }

            string normalizedEmail = collegeMailID.ToLowerInvariant();

            string otpKey = "TPO_RESET_EMAIL_OTP_" + normalizedEmail;
            string verifiedKey = "TPO_RESET_EMAIL_VERIFIED_" + normalizedEmail;

            string otp = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            try
            {
                await _emailService.SendTPOOtpEmailAsync(collegeMailID, otp);

                _memoryCache.Set(
                    otpKey,
                    otp,
                    TimeSpan.FromMinutes(10));

                // Sending a new OTP invalidates any previous verification.
                _memoryCache.Remove(verifiedKey);

                return Json(new
                {
                    success = true,
                    message = "Password reset OTP sent to your college email."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send TPO password reset OTP.");

                _memoryCache.Remove(otpKey);

                return Json(new
                {
                    success = false,
                    message = "Unable to send OTP. Please try again."
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyTPOResetOTP(
            string collegeMailID,
            string otp)
        {
            collegeMailID = collegeMailID?.Trim() ?? "";
            otp = otp?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(collegeMailID) ||
                !new EmailAddressAttribute().IsValid(collegeMailID))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid college email."
                });
            }

            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                return Json(new
                {
                    success = false,
                    message = "Enter the six-digit OTP."
                });
            }

            string normalizedEmail = collegeMailID.ToLowerInvariant();

            string otpKey = "TPO_RESET_EMAIL_OTP_" + normalizedEmail;
            string verifiedKey = "TPO_RESET_EMAIL_VERIFIED_" + normalizedEmail;

            if (!_memoryCache.TryGetValue(otpKey, out string? storedOtp) ||
                string.IsNullOrWhiteSpace(storedOtp))
            {
                return Json(new
                {
                    success = false,
                    message = "OTP expired. Please request a new OTP."
                });
            }

            if (!string.Equals(storedOtp, otp, StringComparison.Ordinal))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid OTP."
                });
            }

            // Consume OTP after successful verification.
            _memoryCache.Remove(otpKey);

            _memoryCache.Set(
                verifiedKey,
                true,
                TimeSpan.FromMinutes(10));

            return Json(new
            {
                success = true,
                message = "OTP verified successfully. You can reset your password."
            });
        }



        [HttpGet]
        public IActionResult TPORestPassword()
        {
            return View();
        }

        // ==========================================================
        // TPO RESET PASSWORD - POST
        // ==========================================================


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TPORestPassword(
            TPORestPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.CollegeMailID?.Trim() ?? "";

                string verifiedKey =
                    "TPO_RESET_EMAIL_VERIFIED_" + email.ToLowerInvariant();

                if (!_memoryCache.TryGetValue(
                        verifiedKey,
                        out bool isVerified) ||
                    !isVerified)
                {
                    ModelState.AddModelError(
                        "",
                        "Please verify the OTP sent to your college email first.");

                    return View(model);
                }

                // Keep your existing ModelState and password-confirmation checks
                // before this point.

                bool updated = _accountRepository.ResetTPOPassword(
                    email,
                    model.NewPassword);

                if (!updated)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to reset password. Please try again.");

                    return View(model);
                }

                // OTP verification cannot be reused after a successful reset.
                _memoryCache.Remove(verifiedKey);

                TempData["Success"] =
                    "Password reset successfully. Please log in with your new password.";

                return RedirectToAction("TPOLogin", "Account");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "========================================"
                );

                Console.WriteLine(
                    "TPO Reset Password Error"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                Console.WriteLine(
                    "========================================"
                );

                ModelState.AddModelError(
                    "",
                    "Unable to reset password. Please try again later."
                );

                return View(model);
            }
        }

        // shrirang 05/10/26
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TPOEditProfile(TPORegistration model)
        {
            // Get logged-in TPO from session
            int? tpoId = HttpContext.Session.GetInt32("TPOID");

            if (tpoId == null || tpoId <= 0)
            {
                return RedirectToAction("TPOLogin", "Account");
            }

            // IMPORTANT:
            // Never take TPOID from the form.
            // Use the logged-in user's session ID.
            model.TPOID = tpoId.Value;

            // Password is not being edited from Edit Profile
            ModelState.Remove(nameof(TPORegistration.Password));
            ModelState.Remove(nameof(TPORegistration.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                ViewData["Panel"] = "TPO";
                ViewData["UserName"] =
                    HttpContext.Session.GetString("TPOName") ?? "TPO";

                ViewData["Title"] = "Edit TPO Profile";

                return View("~/Views/TPO/EditProfile.cshtml", model);
            }

            try
            {
                // ==========================================
                // UPLOAD FOLDER
                // ==========================================

                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "Uploads",
                    "TPO");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // ==========================================
                // SUPPORTING DOCUMENT 1
                // ==========================================

                if (model.SupportingDocument1File != null &&
                    model.SupportingDocument1File.Length > 0)
                {
                    string extension =
                        Path.GetExtension(model.SupportingDocument1File.FileName);

                    string fileName =
                        "TPO_DOC1_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(uploadFolder, fileName);

                    using (FileStream stream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        model.SupportingDocument1File.CopyTo(stream);
                    }

                    model.SupportingDocument1 = fileName;
                }

                // ==========================================
                // SUPPORTING DOCUMENT 2
                // ==========================================

                if (model.SupportingDocument2File != null &&
                    model.SupportingDocument2File.Length > 0)
                {
                    string extension =
                        Path.GetExtension(model.SupportingDocument2File.FileName);

                    string fileName =
                        "TPO_DOC2_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(uploadFolder, fileName);

                    using (FileStream stream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        model.SupportingDocument2File.CopyTo(stream);
                    }

                    model.SupportingDocument2 = fileName;
                }

                // ==========================================
                // PROFILE PHOTO
                // ==========================================

                if (model.ProfilePhotoFile != null &&
                    model.ProfilePhotoFile.Length > 0)
                {
                    string extension =
                        Path.GetExtension(model.ProfilePhotoFile.FileName);

                    string fileName =
                        "TPO_PROFILE_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string filePath =
                        Path.Combine(uploadFolder, fileName);

                    using (FileStream stream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfilePhotoFile.CopyTo(stream);
                    }

                    model.ProfilePhoto = fileName;
                }

                // ==========================================
                // UPDATE DATABASE
                // ==========================================

                bool updated =
                    _accountRepository.UpdateTPOProfile(model);

                if (!updated)
                {
                    TempData["Error"] =
                        "TPO profile could not be updated.";

                    return RedirectToAction("EditProfile", "TPO");
                }

                // Update session name
                HttpContext.Session.SetString(
                    "TPOName",
                    model.FullName ?? "TPO");

                TempData["Success"] =
                    "TPO profile updated successfully.";

                return RedirectToAction("ViewProfile", "TPO");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error while updating TPO profile: " + ex.Message;

                return RedirectToAction("EditProfile", "TPO");
            }
        }




        #region "View Post Master"

        [HttpGet]
        [Route("Account/ViewOrgPost/{id}")]
        public IActionResult ViewOrgPost(
            int id,
            int nOrgID,
            string? matchingPostIds)
        {
            int orgID = nOrgID;

            OrgPostM? model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection") ?? "";

            // =====================================================
            // GET REQUESTED ORGANIZATION POST
            // =====================================================

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SP_GetOrganizationPostList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@nOrgID", SqlDbType.Int).Value = orgID;

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int postID = Convert.ToInt32(dr["nID"]);

                        if (postID != id)
                        {
                            continue;
                        }

                        model = new OrgPostM();

                        // BASIC
                        model.nID = Convert.ToInt32(dr["nID"]);

                        // POSITION
                        model.nPositionID =
                            Convert.ToInt32(dr["nPositionID"]);

                        model.sPositionName =
                            dr["sPositionName"]?.ToString() ?? "";

                        // REQUIRED TRAINEES
                        model.nRequiredTrainees =
                            Convert.ToInt32(dr["nRequiredTrainees"]);

                        // GENDER
                        model.nGenderID =
                            Convert.ToInt32(dr["nGenderID"]);

                        model.sGenderName =
                            dr["sGenderName"]?.ToString() ?? "";

                        // MINIMUM QUALIFICATION
                        model.nMinimumQualificationID =
                            Convert.ToInt32(dr["nMinimumQualificationID"]);

                        model.sMinimumQualificationName =
                            dr["sMinimumQualificationName"]?.ToString() ?? "";

                        // LOCATION
                        model.sCountryName =
                            dr["sCountryName"]?.ToString() ?? "";

                        model.sStateName =
                            dr["sStateName"]?.ToString() ?? "";

                        model.nCityName =
                            dr["nCityName"]?.ToString() ?? "";

                        // WORKING HOURS
                        model.sWorkingHours =
                            dr["sWorkingHours"]?.ToString() ?? "";

                        // INTERNSHIP TYPE
                        model.nInternshipTypeID =
                            Convert.ToInt32(dr["nInternshipTypeID"]);

                        model.sInternshipTypeName =
                            dr["sInternshipTypeName"]?.ToString() ?? "";

                        // WORKING SHIFT
                        model.sWorkingShift =
                            dr["sWorkingShift"]?.ToString() ?? "";

                        // FELLOWSHIP TYPE
                        model.nInternshipFellowshipTypeID =
                            Convert.ToInt32(
                                dr["nInternshipFellowshipTypeID"]);

                        model.sInternshipFellowshipTypeName =
                            dr["sInternshipFellowshipTypeName"]?.ToString() ?? "";

                        // TOTAL CHARGES
                        model.sTotalCharges =
                            dr["sTotalCharges"] == DBNull.Value
                                ? null
                                : Convert.ToDecimal(dr["sTotalCharges"]);

                        model.sCurrency =
                            dr["sCurrency"]?.ToString() ?? "";

                        // TRAINING
                        model.nTrainingInvolvedID =
                            Convert.ToInt32(dr["nTrainingInvolvedID"]);

                        model.sTrainingInvolvedName =
                            dr["sTrainingInvolvedName"]?.ToString() ?? "";

                        // DURATION
                        model.nInternshipDurationID =
                            Convert.ToInt32(dr["nInternshipDurationID"]);

                        model.sInternshipDurationName =
                            dr["sInternshipDurationName"]?.ToString() ?? "";

                        // DATES
                        model.dStartDate =
                            Convert.ToDateTime(dr["dStartDate"]);

                        model.dCompletionDate =
                            Convert.ToDateTime(dr["dCompletionDate"]);

                        // INTERNSHIP MODE
                        model.nInternshipModeID =
                            Convert.ToInt32(dr["nInternshipModeID"]);

                        model.sInternshipModeName =
                            dr["sInternshipModeName"]?.ToString() ?? "";

                        // DIVYANG
                        model.sDivyang =
                            dr["sDivyang"]?.ToString() ?? "";

                        // LANGUAGE
                        model.sLanguageKnown =
                            dr["sLanguageKnown"]?.ToString() ?? "";

                        // WORKING DAYS
                        model.sWorkingDays =
                            dr["sWorkingDays"] == DBNull.Value
                                ? ""
                                : dr["sWorkingDays"]?.ToString() ?? "";

                        // FACILITIES
                        model.sFacilities =
                            dr["sFacilities"] == DBNull.Value
                                ? ""
                                : dr["sFacilities"]?.ToString() ?? "";

                        // MEDICAL SKILLS
                        model.sMedicalSkills =
                            dr["sMedicalSkills"] == DBNull.Value
                                ? null
                                : dr["sMedicalSkills"]?.ToString();

                        // TECHNICAL SKILLS
                        model.sTechnicalSkills =
                            dr["sTechnicalSkills"] == DBNull.Value
                                ? null
                                : dr["sTechnicalSkills"]?.ToString();

                        // NON-TECHNICAL SKILLS
                        model.sNonTechnicalSkills =
                            dr["sNonTechnicalSkills"] == DBNull.Value
                                ? null
                                : dr["sNonTechnicalSkills"]?.ToString();

                        // SYSTEM
                        model.dRegisterDate =
                            dr["dRegisterDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["dRegisterDate"]);

                        model.dModDate =
                            dr["dModDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["dModDate"]);

                        model.nBit =
                            dr["nBit"] != DBNull.Value &&
                            Convert.ToBoolean(dr["nBit"]);

                        model.nSABit =
                            dr["nSABit"] != DBNull.Value &&
                            Convert.ToBoolean(dr["nSABit"]);

                        model.nOrgID =
                            Convert.ToInt32(dr["nOrgID"]);

                        break;
                    }
                }
            }

            // =====================================================
            // POST NOT FOUND
            // =====================================================

            if (model == null)
            {
                return NotFound();
            }

            // =====================================================
            // CONVERT SKILL IDS TO SKILL NAMES
            // =====================================================

            model.sMedicalSkills = GetSkillNames(
                model.sMedicalSkills,
                "tblMedicalSkills",
                "nMedicalSkillID",
                "sMedicalSkillName");

            model.sTechnicalSkills = GetSkillNames(
                model.sTechnicalSkills,
                "tblTechnicalSkills",
                "nTechnicalSkillID",
                "sTechnicalSkillName");

            model.sNonTechnicalSkills = GetSkillNames(
                model.sNonTechnicalSkills,
                "tblNonTechnicalSkills",
                "nNonTechnicalSkillID",
                "sNonTechnicalSkillName");

            // =====================================================
            // GET ORGANIZATION LOGO
            // =====================================================

            string? companyLogo = null;

            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SP_GetOrganizationProfile", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@nOrgID", SqlDbType.Int).Value = orgID;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        companyLogo =
                            dr["sCompanyLogo"] != DBNull.Value
                                ? dr["sCompanyLogo"].ToString()
                                : null;
                    }
                }
            }

            // =====================================================
            // PREVIOUS / NEXT NAVIGATION
            // Use matching post IDs passed from InternshipDetails.
            // Preserve their original table order.
            // =====================================================

            List<int> postIds = (matchingPostIds ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out int postId) ? postId : 0)
                .Where(postId => postId > 0)
                .Distinct()
                .ToList();

            int currentIndex = postIds.IndexOf(id);

            ViewBag.PreviousPostId =
                currentIndex > 0
                    ? postIds[currentIndex - 1]
                    : (int?)null;

            ViewBag.NextPostId =
                currentIndex >= 0 && currentIndex < postIds.Count - 1
                    ? postIds[currentIndex + 1]
                    : (int?)null;

            ViewBag.MatchingPostIds = string.Join(",", postIds);

            // =====================================================
            // VIEWBAG VALUES
            // =====================================================

            ViewBag.CompanyLogo = companyLogo;
            ViewBag.OrgID = orgID;
            ViewBag.OrgName = HttpContext.Session.GetString("OrgName");
            ViewBag.OrgEmail = HttpContext.Session.GetString("OrgEmail");

            return View(model);
        }

        private string GetSkillNames(
            string? skillIds,
            string tableName,
            string idColumn,
            string nameColumn)
        {
            if (string.IsNullOrWhiteSpace(skillIds))
                return "";

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection") ?? "";

            if (string.IsNullOrWhiteSpace(connectionString))
                return "";

            List<string> skillNames = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = $@"
            SELECT {nameColumn}
            FROM {tableName}
            WHERE {idColumn} IN (
                SELECT TRY_CAST(value AS INT)
                FROM STRING_SPLIT(@SkillIds, ',')
            )
            ORDER BY {idColumn}";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@SkillIds", SqlDbType.VarChar)
                        .Value = skillIds;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr[nameColumn] != DBNull.Value)
                            {
                                skillNames.Add(dr[nameColumn].ToString()!);
                            }
                        }
                    }
                }
            }

            return string.Join(", ", skillNames);
        }

        #endregion


        #region "View Profile"



        #endregion



        private async Task SendOrganizationRegistrationEmail(string email, string organizationName)
        {
            string mailUser = "certwala.iipl@gmail.com";
            string mailPass = "yfwc saci hntt gbee";

            using var message = new MailMessage();

            message.From = new MailAddress(mailUser, "1Trainee");
            message.To.Add(email);
            message.Subject = "Organization Registration Successful";
            message.IsBodyHtml = true;

            message.Body =
                $"<p>Dear <strong>{WebUtility.HtmlEncode(organizationName)}</strong>,</p>" +
                "<p>Your organization has been registered successfully on 1Trainee.</p>" +
                "<p>You can now access the platform using your registered account.</p>" +
                "<p>Regards,<br/>Team 1Trainee</p>";

            using var smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mailUser, mailPass);

            await smtp.SendMailAsync(message);
        }


        private async Task SendTRRegistrationEmail(string email, string candidateName)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Candidate email is required.",
                    nameof(email)
                );
            }

            string mailUser = _configuration["RegistrationEmailSettings:Username"] ?? "";

            string mailPass = _configuration["RegistrationEmailSettings:Password"] ?? "";

            string fromName =
                _configuration["RegistrationEmailSettings:FromName"]
                ?? "1Trainee";

            string smtpServer =
                _configuration["RegistrationEmailSettings:SmtpServer"]
                ?? "smtp.gmail.com";

            int port = int.TryParse(
                _configuration["RegistrationEmailSettings:Port"],
                out int configuredPort)
                    ? configuredPort
                    : 587;

            if (string.IsNullOrWhiteSpace(mailUser) || string.IsNullOrWhiteSpace(mailPass))
            {
                throw new InvalidOperationException("Registration email SMTP settings are missing.");
            }

            string safeName = WebUtility.HtmlEncode(
                string.IsNullOrWhiteSpace(candidateName)
                    ? "Candidate"
                    : candidateName
            );

            using var message = new MailMessage();

            message.From = new MailAddress(mailUser, fromName);
            message.To.Add(email.Trim());

            message.Subject =
                "Candidate Registration Successful - 1Trainee";

            message.IsBodyHtml = true;

            message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:20px;background:#f5f7fa;
             font-family:Arial,Helvetica,sans-serif;'>

    <div style='max-width:600px;margin:auto;background:#ffffff;
                border:1px solid #e2e8f0;border-radius:10px;
                overflow:hidden;'>

        <div style='background:#0d9488;padding:22px;text-align:center;'>
            <h2 style='color:#ffffff;margin:0;'>1Trainee</h2>
        </div>

        <div style='padding:28px;'>
            <p>Dear <strong>{safeName}</strong>,</p>

            <h3 style='color:#198754;'>
                Registration Successful!
            </h3>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                Your candidate registration has been completed
                successfully on 1Trainee.
            </p>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                You can now log in using your registered account.
            </p>

            <p>
                Regards,<br>
                <strong>1Trainee Team</strong>
            </p>
        </div>

        <div style='background:#f8fafc;padding:14px;text-align:center;'>
            <p style='margin:0;color:#888;font-size:12px;'>
                This is an automated email. Please do not reply directly.
            </p>
        </div>

    </div>
</body>
</html>";

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailUser, mailPass),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            await smtp.SendMailAsync(message);
        }

        // khushi 14-10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCandidateOTP(
            string email,
            string otpType)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter your email address."
                });
            }

            if (!string.Equals(
                otpType, "Email",
                StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = false,
                    message = "Please select Email OTP verification."
                });
            }

            email = email.Trim();

            try
            {
                string otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // Send email first
                await _emailService.SendOtpEmailAsync(email, otp);

                // Save OTP only after SMTP sending completes
                HttpContext.Session.SetString("CandidateOTP", otp);
                HttpContext.Session.SetString("CandidateOTPEmail", email);

                HttpContext.Session.SetString(
                    "CandidateOTPExpiry",
                    DateTimeOffset.UtcNow
                        .AddMinutes(10)
                        .ToUnixTimeSeconds()
                        .ToString());

                HttpContext.Session.Remove("CandidateOTPVerified");
                HttpContext.Session.Remove("CandidateOTPVerifiedEmail");

                return Json(new
                {
                    success = true,
                    message = "OTP sending request completed. Check your inbox and Spam folder."
                });
            }
            catch (Exception ex)
            {
                // Log the actual SMTP error
                _logger.LogError(
                    ex,
                    "SendCandidateOTP failed for {Email}",
                    email);

                return Json(new
                {
                    success = false,
                    message = "Email sending failed. Check the application logs."
                });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCandidateOTP(string email, string otp)
        {
            email = (email ?? string.Empty).Trim();
            otp = (otp ?? string.Empty).Trim();

            string? savedOTP =
                HttpContext.Session.GetString("CandidateOTP");

            string? savedEmail =
                HttpContext.Session.GetString("CandidateOTPEmail");

            string? expiryText =
                HttpContext.Session.GetString("CandidateOTPExpiry");

            if (string.IsNullOrWhiteSpace(savedOTP) ||
                string.IsNullOrWhiteSpace(savedEmail) ||
                string.IsNullOrWhiteSpace(expiryText))
            {
                return Json(new
                {
                    success = false,
                    message = "Please request a new OTP first."
                });
            }

            if (!long.TryParse(expiryText, out long expiry))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid OTP session. Please request a new OTP."
                });
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
            {
                HttpContext.Session.Remove("CandidateOTP");
                HttpContext.Session.Remove("CandidateOTPEmail");
                HttpContext.Session.Remove("CandidateOTPExpiry");
                HttpContext.Session.Remove("CandidateOTPVerified");
                HttpContext.Session.Remove("CandidateOTPVerifiedEmail");

                return Json(new
                {
                    success = false,
                    message = "OTP has expired. Please request a new OTP."
                });
            }

            if (!string.Equals(email, savedEmail,
                StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = false,
                    message = "The email address has changed. Please request a new OTP."
                });
            }

            if (!string.Equals(otp, savedOTP, StringComparison.Ordinal))
            {
                return Json(new
                {
                    success = false,
                    message = "Incorrect OTP. Please try again."
                });
            }

            // Mark this exact email as verified.
            HttpContext.Session.SetString("CandidateOTPVerified", "true");
            HttpContext.Session.SetString("CandidateOTPVerifiedEmail", savedEmail);

            // OTP can no longer be reused.
            HttpContext.Session.Remove("CandidateOTP");

            return Json(new
            {
                success = true,
                message = "Email verified successfully!"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOrganizationOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                !System.Net.Mail.MailAddress.TryCreate(email.Trim(), out var address) ||
                !string.Equals(address.Address, email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Enter a valid email address." });
            }

            email = address.Address.Trim().ToLowerInvariant();

            string otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            try
            {
                // Send first. Store the OTP only after the email succeeds.
                await _emailService.SendOrganizationOtpEmailAsync(email, otp);

                HttpContext.Session.SetString("OrgOtp", otp);
                HttpContext.Session.SetString("OrgOtpEmail", email);
                HttpContext.Session.SetString(
                    "OrgOtpExpiry",
                    DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString());

                HttpContext.Session.Remove("OrgOtpVerified");

                return Json(new
                {
                    success = true,
                    message = "OTP sent to your email. It is valid for 10 minutes."
                });
            }
            catch (Exception ex)
            {
                // Log the exception on the server; do not expose SMTP details to the user.
                Console.WriteLine($"Organization OTP email failed: {ex}");

                HttpContext.Session.Remove("OrgOtp");
                HttpContext.Session.Remove("OrgOtpEmail");
                HttpContext.Session.Remove("OrgOtpExpiry");
                HttpContext.Session.Remove("OrgOtpVerified");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Could not send OTP. Check the email settings and try again."
                });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOrganizationOtp(string email, string otp)
        {
            string savedOtp = HttpContext.Session.GetString("OrgOtp") ?? "";
            string savedEmail = HttpContext.Session.GetString("OrgOtpEmail") ?? "";
            string expiryValue = HttpContext.Session.GetString("OrgOtpExpiry") ?? "";

            bool validExpiry =
                long.TryParse(expiryValue, out long expiry) &&
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() < expiry;

            string normalizedEmail = (email ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(savedOtp) ||
                string.IsNullOrWhiteSpace(otp) ||
                otp.Length != 6 ||
                !otp.All(char.IsDigit) ||
                !validExpiry ||
                !string.Equals(normalizedEmail, savedEmail, StringComparison.Ordinal) ||
                !string.Equals(otp, savedOtp, StringComparison.Ordinal))
            {
                HttpContext.Session.Remove("OrgOtpVerified");

                return Json(new
                {
                    success = false,
                    message = "Invalid OTP, incorrect email, or OTP expired."
                });
            }

            HttpContext.Session.SetString("OrgOtpVerified", "true");

            return Json(new
            {
                success = true,
                message = "Email verified successfully."
            });
        }


        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> CandidateActivityLog()
        {
            // Get the ID of the currently logged-in candidate.
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            var activities = new List<CandidateActivityLogViewModel>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection is missing from configuration.");

            const string query = @"
        SELECT
            VisitID,
            IPAddress,
            ISP,
            Country,
            Region,
            City,
            TimeZone,
            BrowserName,
            BrowserVersion,
            OperatingSystem,
            DeviceType,
            RequestPath,
            PageName,
            ReferrerUrl,
            VisitDateTime,
            IsLoggedIn
        FROM dbo.tblLoginPageVisits
        WHERE UserID = @CandidateID
          AND UserType = @UserType
          AND PanelType = @PanelType
        ORDER BY VisitDateTime DESC;";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = candidateId.Value;

            command.Parameters.Add("@UserType", SqlDbType.NVarChar, 30)
                .Value = "Candidate";

            command.Parameters.Add("@PanelType", SqlDbType.NVarChar, 30)
                .Value = "Candidate";

            await connection.OpenAsync();

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    activities.Add(new CandidateActivityLogViewModel
                    {
                        VisitID = Convert.ToInt64(reader["VisitID"]),
                        IPAddress = reader["IPAddress"] as string,
                        ISP = reader["ISP"] as string,
                        Country = reader["Country"] as string,
                        Region = reader["Region"] as string,
                        City = reader["City"] as string,
                        TimeZone = reader["TimeZone"] as string,
                        BrowserName = reader["BrowserName"] as string,
                        BrowserVersion = reader["BrowserVersion"] as string,
                        OperatingSystem = reader["OperatingSystem"] as string,
                        DeviceType = reader["DeviceType"] as string,
                        RequestPath = reader["RequestPath"] as string,
                        PageName = reader["PageName"] as string,
                        ReferrerUrl = reader["ReferrerUrl"] as string,
                        VisitDateTime = Convert.ToDateTime(reader["VisitDateTime"]),
                        IsLoggedIn = Convert.ToBoolean(reader["IsLoggedIn"])
                    });
                }
            }

            return View(activities);
        }


        //khushi 22-10
        // Check whether email already exists
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckCandidateEmail(string? sEmail)
        {
            if (string.IsNullOrWhiteSpace(sEmail))
                return Json(true);

            bool exists = _accountRepository
                .IsCandidateEmailExists(sEmail.Trim());

            return Json(!exists);
        }

        //khushi 22-10
        // Check whether mobile number already exists
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckCandidateMobile(string? sMobile)
        {
            if (string.IsNullOrWhiteSpace(sMobile))
                return Json(true);

            bool exists = _accountRepository
                .IsCandidateMobileExists(sMobile.Trim());

            return Json(!exists);
        }

        //khushi 22-10
        // Check organization email
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckOrganizationEmail(string? sEmail)
        {
            if (string.IsNullOrWhiteSpace(sEmail))
                return Json(true);

            bool exists = _accountRepository
                .IsOrganizationEmailExists(sEmail.Trim());

            return Json(!exists);
        }

        //khushi 22-10
        // Check organization mobile
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckOrganizationMobile(string? sMobile)
        {
            if (string.IsNullOrWhiteSpace(sMobile))
                return Json(true);

            bool exists = _accountRepository
                .IsOrganizationMobileExists(sMobile.Trim());

            return Json(!exists);
        }









        #region Edit Candidate Registration

        private const string EditCandidateView =
            "~/Views/Candidate/EditCandidateRegistration.cshtml";

        // ============================================================
        // EDIT CANDIDATE REGISTRATION - GET
        // ============================================================

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult EditCandidateRegistration()
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (!candidateId.HasValue || candidateId.Value <= 0)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            try
            {
                CandidateRegister? model =
                    _accountRepository.GetCandidateRegistrationForEdit(
                        candidateId.Value);

                if (model == null)
                {
                    return NotFound(
                        "Candidate registration details not found.");
                }

                ViewData["Title"] = "Edit Candidate Registration";
                ViewData["Panel"] = "Candidate";
                ViewData["UserName"] =
                    HttpContext.Session.GetString("CandidateName")
                    ?? "Candidate";

                return View(EditCandidateView, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading Candidate registration for ID {CandidateID}",
                    candidateId.Value);

                TempData["Error"] =
                    "Unable to load Candidate registration details.";

                return RedirectToAction("Dashboard", "Candidate");
            }
        }

        // ============================================================
        // EDIT CANDIDATE REGISTRATION - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCandidateRegistration(
            CandidateRegister model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (!candidateId.HasValue || candidateId.Value <= 0)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            ViewData["Title"] = "Edit Candidate Registration";
            ViewData["Panel"] = "Candidate";
            ViewData["UserName"] =
                HttpContext.Session.GetString("CandidateName")
                ?? "Candidate";

            CandidateRegister? existingCandidate;

            try
            {
                existingCandidate =
                    _accountRepository.GetCandidateRegistrationForEdit(
                        candidateId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to load Candidate ID {CandidateID}",
                    candidateId.Value);

                ModelState.AddModelError(
                    "",
                    "Unable to load your registration details.");

                return View(EditCandidateView, model);
            }

            if (existingCandidate == null)
            {
                return NotFound(
                    "Candidate registration details not found.");
            }

            // ============================================================
            // REMOVE REGISTRATION-ONLY VALIDATION
            // ============================================================

            ModelState.Remove(nameof(model.sPassword));
            ModelState.Remove(nameof(model.sOTP));
            ModelState.Remove(nameof(model.ProfileImageFile));

            // ============================================================
            // NORMALIZE INPUT
            // ============================================================

            model.sPrefix = model.sPrefix?.Trim();
            model.sFName = model.sFName?.Trim();
            model.sLName = model.sLName?.Trim();
            model.sEmail = model.sEmail?.Trim();
            model.sMobile = model.sMobile?.Trim();
            model.sAlternateMobile =
                model.sAlternateMobile?.Trim();

            // Preserve existing image by default
            model.sProfileImage = existingCandidate.sProfileImage;

            // ============================================================
            // NO EMAIL OR MOBILE DUPLICATE CHECKS
            // ============================================================

            // Candidates may change email and mobile without
            // duplicate-registration validation.

            // ============================================================
            // CHECK COLLEGE, CODE, DEPARTMENT AND BRANCH
            // ============================================================

            if (!_accountRepository.GetColleges()
                .Any(c => c.nID == model.sCollegeName))
            {
                ModelState.AddModelError(
                    nameof(model.sCollegeName),
                    "Please select a valid college.");
            }
            else if (!_accountRepository
                .GetCollegeCodes(model.sCollegeName)
                .Any(c => int.TryParse(c.nCode, out int code)
                          && code == model.nCollegeCode))
            {
                ModelState.AddModelError(
                    nameof(model.nCollegeCode),
                    "Please select a valid college code.");
            }

            if (!_accountRepository.GetDepartments()
                .Any(d => d.nID == model.nDepartment))
            {
                ModelState.AddModelError(
                    nameof(model.nDepartment),
                    "Please select a valid department.");
            }
            else if (!_accountRepository
                .GetBranches(model.nDepartment)
                .Any(b => b.nID == model.nBranch))
            {
                ModelState.AddModelError(
                    nameof(model.nBranch),
                    "Please select a valid branch.");
            }

            // ============================================================
            // OPTIONAL PROFILE IMAGE VALIDATION
            // ============================================================

            IFormFile? image = model.ProfileImageFile;

            if (image != null)
            {
                string extension =
                    Path.GetExtension(image.FileName).ToLowerInvariant();

                string[] allowedExtensions =
                {
            ".jpg",
            ".jpeg",
            ".png"
        };

                if (image.Length == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.ProfileImageFile),
                        "The selected image is empty.");
                }
                else if (image.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        nameof(model.ProfileImageFile),
                        "Profile image cannot exceed 5 MB.");
                }
                else if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        nameof(model.ProfileImageFile),
                        "Only JPG, JPEG and PNG images are allowed.");
                }
                else
                {
                    byte[] header = new byte[8];

                    using (Stream stream = image.OpenReadStream())
                    {
                        int read = await stream.ReadAsync(
                            header, 0, header.Length);

                        bool isJpeg =
                            read >= 3 &&
                            header[0] == 0xFF &&
                            header[1] == 0xD8 &&
                            header[2] == 0xFF;

                        bool isPng =
                            read >= 8 &&
                            header[0] == 0x89 &&
                            header[1] == 0x50 &&
                            header[2] == 0x4E &&
                            header[3] == 0x47 &&
                            header[4] == 0x0D &&
                            header[5] == 0x0A &&
                            header[6] == 0x1A &&
                            header[7] == 0x0A;

                        bool validSignature =
                            (extension == ".png" && isPng) ||
                            ((extension == ".jpg" ||
                              extension == ".jpeg") && isJpeg);

                        if (!validSignature)
                        {
                            ModelState.AddModelError(
                                nameof(model.ProfileImageFile),
                                "The selected file is not a valid image.");
                        }
                    }
                }
            }

            // ============================================================
            // VALIDATE FORM BEFORE UPLOADING FILES
            // ============================================================

            if (!ModelState.IsValid)
            {
                return View(EditCandidateView, model);
            }

            string? newlySavedImagePath = null;

            try
            {
                // ========================================================
                // SAVE NEW PROFILE IMAGE
                // ========================================================

                if (image != null)
                {
                    string extension =
                        Path.GetExtension(image.FileName).ToLowerInvariant();

                    string uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "candidates");

                    Directory.CreateDirectory(uploadFolder);

                    string fileName =
                        "CAND_" +
                        candidateId.Value + "_" +
                        Guid.NewGuid().ToString("N") +
                        extension;

                    string fullPath =
                        Path.Combine(uploadFolder, fileName);

                    newlySavedImagePath = fullPath;

                    using (FileStream stream = new FileStream(
                        fullPath,
                        FileMode.CreateNew))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Save filename only
                    model.sProfileImage = fileName;
                }

                // ========================================================
                // UPDATE DATABASE
                // ========================================================

                bool updated =
                    _accountRepository.UpdateCandidateRegistrationForEdit(
                        candidateId.Value,
                        model);

                if (!updated)
                {
                    if (!string.IsNullOrEmpty(newlySavedImagePath) &&
                        System.IO.File.Exists(newlySavedImagePath))
                    {
                        System.IO.File.Delete(newlySavedImagePath);
                    }

                    ModelState.AddModelError(
                        "",
                        "Candidate registration could not be updated.");

                    model.sProfileImage =
                        existingCandidate.sProfileImage;

                    return View(EditCandidateView, model);
                }

                // ========================================================
                // UPDATE SESSION
                // ========================================================

                string candidateName =
                    ((model.sFName ?? "") + " " +
                     (model.sLName ?? "")).Trim();

                HttpContext.Session.SetString(
                    "CandidateName",
                    candidateName);

                HttpContext.Session.SetString(
                    "CandidateEmail",
                    model.sEmail ?? "");

                // ========================================================
                // SUCCESS
                // ========================================================

                TempData["Success"] =
                    "Candidate registration updated successfully.";

                return RedirectToAction(
                    nameof(EditCandidateRegistration));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating Candidate ID {CandidateID}",
                    candidateId.Value);

                if (!string.IsNullOrEmpty(newlySavedImagePath))
                {
                    try
                    {
                        if (System.IO.File.Exists(newlySavedImagePath))
                        {
                            System.IO.File.Delete(newlySavedImagePath);
                        }
                    }
                    catch (IOException cleanupEx)
                    {
                        _logger.LogWarning(
                            cleanupEx,
                            "Unable to clean up Candidate image.");
                    }
                    catch (UnauthorizedAccessException cleanupEx)
                    {
                        _logger.LogWarning(
                            cleanupEx,
                            "Unable to clean up Candidate image.");
                    }
                }

                model.sProfileImage =
                    existingCandidate.sProfileImage;

                ModelState.AddModelError(
                    "",
                    "An error occurred while updating your profile.");

                return View(EditCandidateView, model);
            }
        }

        #endregion







    }
}