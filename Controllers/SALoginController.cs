using ErJobPortal.Models;
using ErJobPortal.Repositories;
using ErJobPortal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ErJobPortal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;
        private readonly EmailService _emailService;

        public SALoginController(AccountRepository repository, EmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        // ==========================================================
        // SUPER ADMIN LOGIN - GET
        // ==========================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // ==========================================================
        // SUPER ADMIN LOGIN - POST
        // ==========================================================

        [HttpPost]
        public IActionResult Index(SALoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Retrieve Super Admin by email
            var user = _repository.Login(
                model.sEmail ?? "",
                ""
            );

            if (user == null)
            {
                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }

            var passwordHasher = new PasswordHasher<SALoginModel>();

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
                ViewBag.Error =
                    "Stored password is not in a valid hash format. Please reset or migrate the password.";

                return View(model);
            }

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }

            // Store Super Admin details in session
            HttpContext.Session.SetString(
                "SAID",
                user.nID.ToString()
            );

            HttpContext.Session.SetString(
                "SAName",
                user.sFName ?? ""
            );

            HttpContext.Session.SetString(
                "SARole",
                user.sRole ?? ""
            );

            // Redirect to Super Admin Dashboard
            return RedirectToAction(
                "Dashboard",
                "SuperAdmin",
                new { id = user.nID }
            );
        }


        // ==========================================================
        // SUPER ADMIN RESET PASSWORD - GET
        // ==========================================================

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }


        // ==========================================================
        // SUPER ADMIN RESET PASSWORD - POST
        // ==========================================================

        private void ClearSuperAdminResetOtpSession()
        {
            HttpContext.Session.Remove("SAResetOtp");
            HttpContext.Session.Remove("SAResetOtpEmail");
            HttpContext.Session.Remove("SAResetOtpExpiry");
            HttpContext.Session.Remove("SAResetVerifiedEmail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendSuperAdminResetOTP(
            [FromForm] string sEmail)
        {
            string email = (sEmail ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid email address."
                });
            }

            try
            {
                SALoginModel? admin =
                    _repository.GetSuperAdminLoginDetails(email);

                if (admin == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No active Super Admin account was found with this email."
                    });
                }

                // Generate a secure 6-digit OTP.
                string otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // Send the OTP before storing it in the session.
                await _emailService.SendSuperAdminResetOtpEmailAsync(
                    email, otp);

                ClearSuperAdminResetOtpSession();

                HttpContext.Session.SetString("SAResetOtp", otp);
                HttpContext.Session.SetString("SAResetOtpEmail", email);
                HttpContext.Session.SetString(
                    "SAResetOtpExpiry",
                    DateTimeOffset.UtcNow.AddMinutes(10)
                        .ToUnixTimeSeconds().ToString());

                return Json(new
                {
                    success = true,
                    message = "OTP sent to your registered email. It expires in 10 minutes."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Super Admin Reset OTP Error: " + ex);

                return Json(new
                {
                    success = false,
                    message = "Unable to send OTP right now. Please try again later."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifySuperAdminResetOTP(
            [FromForm] string sEmail,
            [FromForm] string otp)
        {
            string email = (sEmail ?? "").Trim();
            string enteredOtp = (otp ?? "").Trim();

            string? savedOtp =
                HttpContext.Session.GetString("SAResetOtp");

            string? savedEmail =
                HttpContext.Session.GetString("SAResetOtpEmail");

            string? expiryText =
                HttpContext.Session.GetString("SAResetOtpExpiry");

            if (string.IsNullOrWhiteSpace(savedOtp) ||
                string.IsNullOrWhiteSpace(savedEmail) ||
                string.IsNullOrWhiteSpace(expiryText))
            {
                return Json(new
                {
                    success = false,
                    message = "Please request a new OTP."
                });
            }

            if (!long.TryParse(expiryText, out long expiry))
            {
                ClearSuperAdminResetOtpSession();

                return Json(new
                {
                    success = false,
                    message = "Invalid OTP session. Please request a new OTP."
                });
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
            {
                ClearSuperAdminResetOtpSession();

                return Json(new
                {
                    success = false,
                    message = "OTP has expired. Please request a new one."
                });
            }

            if (!string.Equals(savedEmail, email,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = false,
                    message = "Email address does not match the requested OTP."
                });
            }

            if (enteredOtp.Length != 6 ||
                !enteredOtp.All(char.IsDigit) ||
                !string.Equals(savedOtp, enteredOtp,
                    StringComparison.Ordinal))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid OTP. Please try again."
                });
            }

            // Mark this email as verified on the server.
            HttpContext.Session.SetString(
                "SAResetVerifiedEmail", savedEmail);

            // Make the OTP one-time use.
            HttpContext.Session.Remove("SAResetOtp");
            HttpContext.Session.Remove("SAResetOtpExpiry");

            return Json(new
            {
                success = true,
                message = "Email verified successfully. You can now reset your password."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(
                SuperAdminResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = (model.sEmail ?? "").Trim();

            // OTP verification is mandatory.
            string? verifiedEmail =
                HttpContext.Session.GetString("SAResetVerifiedEmail");

            if (string.IsNullOrWhiteSpace(verifiedEmail) ||
                !string.Equals(
                    verifiedEmail,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    "",
                    "Please verify your email using OTP before resetting your password.");

                return View(model);
            }

            try
            {
                SALoginModel? admin =
                    _repository.GetSuperAdminLoginDetails(email);

                if (admin == null)
                {
                    ClearSuperAdminResetOtpSession();

                    ModelState.AddModelError(
                        "sEmail",
                        "No Super Admin account was found with this email address.");

                    return View(model);
                }

                bool updated =
                    _repository.ResetSuperAdminPassword(
                        email, model.NewPassword);

                if (!updated)
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to reset password. Please try again.");

                    return View(model);
                }

                ClearSuperAdminResetOtpSession();

                TempData["Success"] =
                    "Your password has been reset successfully. Please login with your new password.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Super Admin Reset Password Error: " + ex);

                ModelState.AddModelError(
                    "",
                    "Unable to reset password. Please try again later.");

                return View(model);
            }
        }


        // ==========================================================
        // SUPER ADMIN FORGOT PASSWORD - GET
        // ==========================================================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        // ==========================================================
        // SUPER ADMIN FORGOT PASSWORD - POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            SAForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email =
                    model.sEmail.Trim();


                // ==================================================
                // GET SUPER ADMIN LOGIN DETAILS
                // ==================================================

                SALoginModel? admin =
                    _repository.GetSuperAdminLoginDetails(
                        email);


                if (admin == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No Super Admin account was found with this email address."
                    );

                    return View(model);
                }


                // ==================================================
                // CHECK PASSWORD
                // ==================================================

                if (string.IsNullOrWhiteSpace(
                    admin.sPassword))
                {
                    ModelState.AddModelError(
                        "",
                        "Password information is not available for this account."
                    );

                    return View(model);
                }


                // ==================================================
                // SEND LOGIN DETAILS
                // ==================================================

                await _emailService.SendSuperAdminLoginDetailsAsync(
                    admin.sEmail,
                    admin.sPassword
                );


                // ==================================================
                // SUCCESS
                // ==================================================

                TempData["Success"] =
                    "Your login details have been sent to your registered email address.";


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "========================================");

                Console.WriteLine(
                    "Super Admin Forgot Password Error");

                Console.WriteLine(
                    ex.ToString());

                Console.WriteLine(
                    "========================================");


                ModelState.AddModelError(
                    "",
                    "Unable to send login details. Please try again later."
                );

                return View(model);
            }
        }
    }
}