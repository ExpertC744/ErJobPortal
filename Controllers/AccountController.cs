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


        // ==========================================
        // GET BRANCHES BY DEPARTMENT
        // ==========================================

        [HttpGet]
        public IActionResult GetCandidateBranches(int departmentId)
        {
            var branches = _accountRepository.GetBranches(departmentId);
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

                // ==========================================
                // PROFILE IMAGE UPLOAD
                // ==========================================

                if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
                {
                    // ==========================================
                    // ALLOWED IMAGE TYPES
                    // ==========================================

                    string[] allowedExtensions =
                    {
    ".png",
    ".jpg",
    ".jpeg"
};

                    string extension = Path.GetExtension(
                            model.ProfileImageFile.FileName
                        ).ToLowerInvariant();

                    // ==========================================
                    // CHECK IMAGE EXTENSION
                    // ==========================================

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Only PNG, JPG and JPEG images are allowed."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // MAXIMUM 5 MB
                    // ==========================================

                    if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Image size must be less than 5 MB."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // CREATE UPLOAD FOLDER
                    // ==========================================

                    string uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "candidates"
                    );

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // ==========================================
                    // GENERATE IMAGE NAME: 1, 2, 3, 4...
                    // ==========================================

                    int nextNumber = 1;

                    while (Directory.GetFiles(
                        uploadFolder,
                        nextNumber + ".*"
                    ).Length > 0)
                    {
                        nextNumber++;
                    }

                    // Example: 1.jpg, 2.png, 3.jpeg
                    string imageName =
                        nextNumber + extension;

                    // ==========================================
                    // SAVE IMAGE
                    // ==========================================

                    string filePath = Path.Combine(
                        uploadFolder,
                        imageName
                    );

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.ProfileImageFile.CopyTo(stream);
                    }

                    // ==========================================
                    // ONLY IMAGE NAME GOES TO DATABASE
                    // ==========================================

                    model.sProfileImage = imageName;
                }
                else
                {
                    model.sProfileImage = "";
                }

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
        [HttpPost]
        public IActionResult OrganizationLogin(OrganizationLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OrganizationUser? user =
                _accountRepository.LoginOrganization(model);

            if (user != null)
            {
                // Super Admin disabled organization
                if (!user.nSABit)
                {
                    TempData["Error"] =
                        "Your organization has been disabled by the Super Admin. Please contact the administrator.";

                    return View(model);
                }

                // Normal login
                HttpContext.Session.SetInt32("OrgID", user.nID);

                HttpContext.Session.SetString(
                    "OrgName",
                    user.sOrgName ?? ""
                );

                HttpContext.Session.SetString(
                    "OrgEmail",
                    user.sEmail ?? ""
                );

                HttpContext.Session.SetString(
                    "OrgCode",
                    user.orgCode ?? ""
                );

                return RedirectToAction(
                    "Dashboard",
                    "Organization"
                );
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
                // =====================================================
                // SUPER ADMIN DISABLED CANDIDATE
                // =====================================================

                if (!user.nSABit)
                {
                    TempData["Error"] =
                        "Your account has been disabled by the Super Admin. Please contact the administrator.";

                    return View(model);
                }

                // =====================================================
                // CANDIDATE ENABLED
                // =====================================================

                HttpContext.Session.SetInt32(
                    "CandidateID",
                    user.nID
                );

                HttpContext.Session.SetString(
                    "CandidateName",
                    (user.sFName + " " + user.sLName).Trim()
                );

                HttpContext.Session.SetString(
                    "CandidateEmail",
                    user.sEmail ?? ""
                );

                return RedirectToAction(
                    "Dashboard",
                    "Candidate"
                );
            }

            // =====================================================
            // INVALID LOGIN
            // =====================================================

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrganizationForgotPassword(
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

                if (string.IsNullOrWhiteSpace(organization.sPassword))
                {
                    ModelState.AddModelError(
                        "",
                        "Password information is not available for this account."
                    );

                    return View(model);
                }

                // Send login details to registered email
                await _emailService.SendOrganizationLoginDetailsAsync(
                    organization.sEmail,
                    organization.sPassword
                );

                TempData["Success"] =
                    "Your login details have been sent to your registered email address.";

                return RedirectToAction("OrganizationLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("Organization Forgot Password Error");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("========================================");

                ModelState.AddModelError(
                    "",
                    "Email Error: " + ex.Message
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
    }
}