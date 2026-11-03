using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ErJobPortal.Services;

namespace ErJobPortal.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendSuperAdminLoginDetailsAsync(
    string email,
    string password)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port =
                Convert.ToInt32(
                    _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"]
                ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";

            using MailMessage message =
                new MailMessage();

            message.From =
                new MailAddress(
                    fromEmail,
                    fromName);

            message.To.Add(email);

            message.Subject =
                "Your Er Job Portal Super Admin Login Details";

            message.Body = $@"
Dear Super Admin,

You requested your login details for your Er Job Portal Super Admin account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to the Super Admin panel.

Regards,

Er Job Portal Team
";

            message.IsBodyHtml = false;

            using SmtpClient smtp =
                new SmtpClient(
                    smtpServer,
                    port);

            smtp.EnableSsl = true;

            smtp.Credentials =
                new NetworkCredential(
                    username,
                    smtpPassword);

            await smtp.SendMailAsync(message);
        }

        public async Task SendOrganizationLoginDetailsAsync(
    string email,
    string password)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port =
                Convert.ToInt32(
                    _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"]
                ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";

            using MailMessage message =
                new MailMessage();

            message.From =
                new MailAddress(
                    fromEmail,
                    fromName);

            message.To.Add(email);

            message.Subject =
                "Your Er Job Portal Organization Login Details";

            message.Body = $@"
Dear Organization,

You requested your login details for your Er Job Portal organization account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to your organization account.

Regards,

Er Job Portal Team
";

            message.IsBodyHtml = false;

            using SmtpClient smtp =
                new SmtpClient(
                    smtpServer,
                    port);

            smtp.EnableSsl = true;

            smtp.Credentials =
                new NetworkCredential(
                    username,
                    smtpPassword);

            await smtp.SendMailAsync(message);
        }

        public async Task SendCandidateLoginDetailsAsync(
            string email,
            string password)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port =
                Convert.ToInt32(
                    _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"]
                ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";

            using MailMessage message =
                new MailMessage();

            message.From =
                new MailAddress(
                    fromEmail,
                    fromName);

            message.To.Add(email);

            message.Subject =
                "Your Er Job Portal Login Details";

            message.Body = $@"
Dear Candidate,

You requested your login details for your Er Job Portal account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to your candidate account.

Regards,

Er Job Portal Team
";

            message.IsBodyHtml = false;

            using SmtpClient smtp =
                new SmtpClient(
                    smtpServer,
                    port);

            smtp.EnableSsl = true;

            smtp.Credentials =
                new NetworkCredential(
                    username,
                    smtpPassword);

            await smtp.SendMailAsync(message);
        }

        // =========================================================
        // CONTACT US - THANK YOU EMAIL
        // =========================================================

        public async Task SendContactThankYouEmailAsync(
            string name,
            string email,
            string subject,
            string description)
        {
            try
            {
                // ---------------------------------------------
                // Validate recipient
                // ---------------------------------------------

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new Exception(
                        "Contact email address is empty.");
                }

                email = email.Trim();

                name = name?.Trim() ?? "";
                subject = subject?.Trim() ?? "";
                description = description?.Trim() ?? "";

                // ---------------------------------------------
                // Validate email format
                // ---------------------------------------------

                MailAddress recipient;

                try
                {
                    recipient = new MailAddress(email);
                }
                catch (FormatException)
                {
                    throw new Exception(
                        "Invalid contact email address: " + email);
                }

                // ---------------------------------------------
                // SMTP SETTINGS
                // ---------------------------------------------

                string smtpServer =
                    _configuration["EmailSettings:SmtpServer"] ?? "";

                int port =
                    Convert.ToInt32(
                        _configuration["EmailSettings:Port"] ?? "587");

                string username =
                    _configuration["EmailSettings:Username"] ?? "";

                string smtpPassword =
                    _configuration["EmailSettings:Password"] ?? "";

                string fromEmail =
                    _configuration["EmailSettings:FromEmail"]
                    ?? username;

                string fromName =
                    _configuration["EmailSettings:FromName"]
                    ?? "Er Job Portal";

                // ---------------------------------------------
                // CREATE EMAIL
                // ---------------------------------------------

                using MailMessage message =
                    new MailMessage();

                // Sender = your configured Er Job Portal email
                message.From =
                    new MailAddress(
                        fromEmail,
                        fromName);

                // IMPORTANT:
                // Recipient = email entered by user in Contact Us
                message.To.Add(recipient);

                message.Subject =
                    "Thank You for Contacting Us";

                message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>

<body style='margin:0;
             padding:0;
             background:#f5f7f7;
             font-family:Arial,Helvetica,sans-serif;'>

    <div style='max-width:650px;
                margin:30px auto;
                background:#ffffff;
                border:1px solid #e1e8e7;
                border-radius:10px;
                overflow:hidden;'>

        <!-- HEADER -->

        <div style='background:#0d9488;
                    padding:25px;
                    text-align:center;'>

            <h1 style='margin:0;
                       color:#ffffff;
                       font-size:24px;'>
                Thank You for Contacting Us
            </h1>

        </div>

        <!-- CONTENT -->

        <div style='padding:30px;'>

            <p style='font-size:16px;
                      color:#26333c;'>
                Dear
                <strong>
                    {WebUtility.HtmlEncode(name)}
                </strong>,
            </p>

            <p style='font-size:14px;
                      color:#52616b;
                      line-height:1.7;'>

                Thank you for contacting us.

                We have successfully received your message.
                Our team will review your enquiry and get back
                to you as soon as possible.

            </p>

            <!-- ENQUIRY DETAILS -->

            <div style='margin-top:25px;
                        padding:18px;
                        background:#f3faf9;
                        border:1px solid #e0efed;
                        border-radius:8px;'>

                <h3 style='margin-top:0;
                           color:#0d9488;
                           font-size:17px;'>
                    Your Enquiry Details
                </h3>

                <p style='font-size:14px;
                          color:#26333c;'>

                    <strong>Subject:</strong>
                    {WebUtility.HtmlEncode(subject)}

                </p>

                <p style='font-size:14px;
                          color:#26333c;
                          line-height:1.6;'>

                    <strong>Message:</strong><br />

                    {WebUtility.HtmlEncode(description)}

                </p>

            </div>

            <p style='font-size:14px;
                      color:#52616b;
                      line-height:1.7;
                      margin-top:25px;'>

                We appreciate your interest in our services.

                If you have any additional information to share,
                please feel free to contact us.

            </p>

            <p style='font-size:14px;
                      color:#26333c;
                      margin-top:25px;'>

                Regards,<br />

                <strong>Er Job Portal Team</strong>

            </p>

        </div>

        <!-- FOOTER -->

        <div style='background:#f6faf9;
                    padding:15px;
                    text-align:center;
                    border-top:1px solid #e5eeee;'>

            <p style='margin:0;
                      font-size:12px;
                      color:#7a858d;'>

                This is an automated acknowledgement email.

            </p>

        </div>

    </div>

</body>
</html>
";

                message.IsBodyHtml = true;

                // ---------------------------------------------
                // SEND EMAIL
                // ---------------------------------------------

                using SmtpClient smtp =
                    new SmtpClient(
                        smtpServer,
                        port);

                smtp.EnableSsl = true;

                smtp.Credentials =
                    new NetworkCredential(
                        username,
                        smtpPassword);

                await smtp.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new Exception(
                    "SMTP email sending failed. " +
                    "Recipient: " + email +
                    ". SMTP Error: " + ex.Message,
                    ex);
            }
            catch (FormatException ex)
            {
                throw new Exception(
                    "Invalid contact email address: " +
                    email +
                    ". Error: " + ex.Message,
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Contact thank-you email sending failed. " +
                    "Recipient: " + email +
                    ". Error: " + ex.Message,
                    ex);
            }
        }


        //TPO FORGOT PASSWORD

        public async Task SendTPOLoginDetailsAsync(
    string email,
    string password)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port =
                Convert.ToInt32(
                    _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"]
                ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";


            using MailMessage message =
                new MailMessage();


            message.From =
                new MailAddress(
                    fromEmail,
                    fromName);


            message.To.Add(email);


            message.Subject =
                "Your Er Job Portal TPO Login Credentials";


            message.Body = $@"
Dear TPO,

You requested your login credentials for your Er Job Portal TPO account.

Your login details are:

College Email ID: {email}
Temporary Password: {password}

You can use these credentials to login to your TPO account.

For security reasons, please change your password after logging in.

Regards,

Er Job Portal Team
";


            message.IsBodyHtml = false;


            using SmtpClient smtp =
                new SmtpClient(
                    smtpServer,
                    port);


            smtp.EnableSsl = true;


            smtp.Credentials =
                new NetworkCredential(
                    username,
                    smtpPassword);


            await smtp.SendMailAsync(message);
        }

        // shrirang 10/10/26

        public async Task SendSchedulerReminderEmailAsync(
            string email,
            string tpoName,
            string activityTitle,
            DateTime startDate,
            TimeSpan startTime,
            string? location)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("TPO email is required.", nameof(email));

            string smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "";
            int port = Convert.ToInt32(_configuration["EmailSettings:Port"] ?? "587");
            string username = _configuration["EmailSettings:Username"] ?? "";
            string smtpPassword = _configuration["EmailSettings:Password"] ?? "";
            string fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;
            string fromName = _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(email.Trim());
            message.Subject = "Upcoming TPO Scheduler Activity Reminder";

            message.Body =
                $"Dear {tpoName},\r\n\r\n" +
                "This is a reminder for your upcoming scheduled activity.\r\n\r\n" +
                $"Activity: {activityTitle}\r\n" +
                $"Date: {startDate:dd MMM yyyy}\r\n" +
                $"Time: {startTime:hh\\:mm}\r\n" +
                $"Location: {location ?? "Not specified"}\r\n\r\n" +
                "Please check your TPO Scheduler for details.\r\n\r\n" +
                "Regards,\r\nEr Job Portal Team";

            message.IsBodyHtml = false;

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, smtpPassword)
            };

            await smtp.SendMailAsync(message);
        }


        // shrirang 15/10/26
        // send email to candidate and org for applied post 

        // =========================================================
        // INTERNSHIP APPLICATION EMAILS
        // =========================================================

        public async Task SendCandidateApplicationConfirmationAsync(
            string candidateEmail,
            string candidateName,
            string organizationName,
            string positionName)
        {
            string body = $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <h2>Application Submitted Successfully</h2>
    <p>Dear {WebUtility.HtmlEncode(candidateName)},</p>
    <p>You have successfully applied for this internship.</p>
    <p><b>Organization:</b>
       {WebUtility.HtmlEncode(organizationName)}</p>
    <p><b>Position:</b>
       {WebUtility.HtmlEncode(positionName)}</p>
    <p>Your application has been recorded successfully.</p>
    <p>Regards,<br/>Er Job Portal Team</p>
</body>
</html>";

            await SendApplicationEmailAsync(
                candidateEmail,
                "Internship Application Submitted Successfully",
                body);
        }


        public async Task SendOrganizationApplicationNotificationAsync(
            string organizationEmail,
            string organizationName,
            string candidateName,
            string candidateEmail,
            string positionName)
        {
            string body = $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <h2>New Internship Application Received</h2>
    <p>Dear {WebUtility.HtmlEncode(organizationName)},</p>
    <p>A candidate has applied for your internship.</p>
    <p><b>Candidate Name:</b>
       {WebUtility.HtmlEncode(candidateName)}</p>
    <p><b>Candidate Email:</b>
       {WebUtility.HtmlEncode(candidateEmail)}</p>
    <p><b>Internship Position:</b>
       {WebUtility.HtmlEncode(positionName)}</p>
    <p>Please log in to ErJobPortal to review the application.</p>
    <p>Regards,<br/>Er Job Portal Team</p>
</body>
</html>";

            await SendApplicationEmailAsync(
                organizationEmail,
                "New Candidate Applied for Your Internship",
                body);
        }


        // =========================================================
        // COMMON EMAIL SENDER — USES YOUR EXISTING CONFIGURATION
        // =========================================================


        private async Task SendApplicationEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                throw new ArgumentException(
                    "Recipient email is required.",
                    nameof(recipientEmail));
            }

            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port = Convert.ToInt32(
                _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"] ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(smtpPassword) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "Email SMTP settings are missing or incomplete.");
            }

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(recipientEmail.Trim()));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, smtpPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            await smtp.SendMailAsync(message);

            _logger.LogInformation(
                "Application email sent to {Email} with subject {Subject}.",
                recipientEmail,
                subject);
        }

        public async Task SendCandidateStatusEmailAsync(string email, string candidateName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Candidate email is required.", nameof(email));

            string smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "";
            int port = Convert.ToInt32(_configuration["EmailSettings:Port"] ?? "587");
            string username = _configuration["EmailSettings:Username"] ?? "";
            string smtpPassword = _configuration["EmailSettings:Password"] ?? "";
            string fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;
            string fromName = _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            string subject = isEnabled
                ? "Your Er Job Portal Registration Has Been Enabled"
                : "Your Er Job Portal Registration Has Been Disabled";

            string safeName = WebUtility.HtmlEncode(candidateName ?? "Candidate");

            string statusMessage = isEnabled
                ? "Your candidate registration has been enabled by the Super Admin. You can now access your Er Job Portal account."
                : "Your candidate registration has been disabled by the Super Admin. Access to your candidate account may be restricted.";

            string safeMessage = WebUtility.HtmlEncode(statusMessage);

            string additionalMessage = isEnabled
                ? "Thank you for being part of Er Job Portal."
                : "If you believe this action was taken by mistake, please contact the Er Job Portal administration team.";

            string safeAdditionalMessage = WebUtility.HtmlEncode(additionalMessage);

            using var message = new MailMessage();

            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(email.Trim());
            message.Subject = subject;
            message.IsBodyHtml = true;

            message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:20px;background:#f5f7fa;
             font-family:Arial,Helvetica,sans-serif;'>
    <div style='max-width:600px;margin:auto;background:#fff;
                border:1px solid #e2e8f0;border-radius:10px;overflow:hidden;'>
        <div style='background:#0d9488;padding:22px;text-align:center;'>
            <h2 style='color:#fff;margin:0;'>Er Job Portal</h2>
        </div>

        <div style='padding:28px;'>
            <p>Dear <strong>{safeName}</strong>,</p>

            <h3 style='color:{(isEnabled ? "#198754" : "#dc3545")};'>
                Registration {(isEnabled ? "Enabled" : "Disabled")}
            </h3>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                {safeMessage}
            </p>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                {safeAdditionalMessage}
            </p>

            <p>Regards,<br><strong>Er Job Portal Team</strong></p>
        </div>

        <div style='background:#f8fafc;padding:14px;text-align:center;'>
            <p style='margin:0;color:#888;font-size:12px;'>
                This is an automated email. Please do not reply directly.
            </p>
        </div>
    </div>
</body>
</html>";


            try
            {
                using var smtp = new SmtpClient(smtpServer, port)
                {
                    EnableSsl = bool.TryParse(
                        _configuration["EmailSettings:EnableSsl"],
                        out bool enableSsl) && enableSsl,

                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, smtpPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                await smtp.SendMailAsync(message);

                _logger.LogInformation(
                    "Status email accepted by SMTP for recipient {Email}.",
                    email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP error sending candidate status email to {Email}. " +
                    "StatusCode: {StatusCode}; Details: {Details}",
                    email,
                    ex.StatusCode,
                    ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error sending candidate status email to {Email}.",
                    email);

                throw;
            }


            //await smtp.SendMailAsync(message);
        }


        public async Task SendOrganizationProfileStatusEmailAsync(
string email,
string organizationName,
bool isEnabled)
        {
            string smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "";
            int port = Convert.ToInt32(_configuration["EmailSettings:Port"] ?? "587");
            string username = _configuration["EmailSettings:Username"] ?? "";
            string password = _configuration["EmailSettings:Password"] ?? "";
            string fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;
            string fromName = _configuration["EmailSettings:FromName"] ?? "ErJobPortal";

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Organization email is required.", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(fromEmail) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Email SMTP settings are missing in appsettings.json.");
            }

            string safeName = WebUtility.HtmlEncode(
                string.IsNullOrWhiteSpace(organizationName)
                    ? "Organization"
                    : organizationName);

            string statusText = isEnabled ? "Enabled" : "Disabled";

            using var message = new MailMessage();

            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(email.Trim());

            message.Subject = isEnabled
                ? "ErJobPortal - Organization Profile Enabled"
                : "ErJobPortal - Organization Profile Disabled";

            message.IsBodyHtml = true;

            message.Body = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Arial,sans-serif;color:#333;'>
    <div style='max-width:600px;margin:auto;padding:20px;border:1px solid #ddd;border-radius:8px;'>
        <h2 style='color:#2457a7;'>ErJobPortal</h2>

        <p>Hello {safeName},</p>

        <p>Your organization profile status has been updated.</p>

        <p>Current status: <strong>{statusText}</strong></p>

        {(isEnabled
                    ? "<p>Your organization profile is now enabled.</p>"
                    : "<p>Your organization profile is now disabled. Please contact the administrator if you believe this is an error.</p>")}

        <p>Regards,<br/>ErJobPortal Team</p>
    </div>
</body>
</html>";

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            await smtp.SendMailAsync(message);
        }



        public async Task SendOtpEmailAsync(
                string email,
                string otp,
                string purpose = "registration")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Recipient email is required.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(otp))
            {
                throw new ArgumentException(
                    "OTP is required.",
                    nameof(otp));
            }

            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            string portValue =
                _configuration["EmailSettings:Port"] ?? "587";

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"] ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(smtpPassword) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings are missing or incomplete in configuration.");
            }

            if (!int.TryParse(portValue, out int smtpPort))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port must be a valid number.");
            }

            using var message = new MailMessage();

            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(email.Trim()));
           message.Subject = "Your Er Job Portal OTP";

            bool isPasswordReset = string.Equals(
            purpose,
            "password-reset",
            StringComparison.OrdinalIgnoreCase);

            message.Subject = isPasswordReset
                ? "Your Er Job Portal Password Reset OTP"
                : "Your Er Job Portal OTP";

            // Use \r\n for line breaks inside a regular C# string.
            message.Body =
                 "Dear Candidate,\r\n\r\n" +
                 $"Your OTP is: {otp}\r\n\r\n" +
                 "This OTP is valid for 10 minutes. Please do not share it with anyone.\r\n\r\n" +
                 (isPasswordReset
                     ? "Enter this OTP on the password reset page to verify your email and reset your password.\r\n\r\n"
                     : "Enter this OTP on the registration page to complete your email verification.\r\n\r\n") +
                 "If you did not request this OTP, please ignore this email.\r\n\r\n" +
                 "Regards,\r\n" +
                 "Er Job Portal Team";

            message.IsBodyHtml = false;

            using var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, smtpPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            try
            {
                _logger.LogInformation(
                    "Sending candidate OTP email to {Email}.",
                    email);

                await smtp.SendMailAsync(message);

                _logger.LogInformation(
                    "Candidate OTP email sent successfully to {Email}.",
                    email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP error while sending candidate OTP to {Email}.",
                    email);


                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while sending candidate OTP to {Email}.",
                    email);

                throw;
            }
        }



        // khushi 16-10-26
        public async Task SendOrganizationOtpEmailAsync(
       string email,
       string otp,
       string purpose = "registration")
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            string portValue =
                _configuration["EmailSettings:Port"] ?? "587";

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string password =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"] ?? "";

            string fromName =
                _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings are missing or incomplete in configuration.");
            }

            if (!int.TryParse(portValue, out int port))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port must be a valid number.");
            }

            bool isPasswordReset =
                string.Equals(
                    purpose,
                    "password-reset",
                    StringComparison.OrdinalIgnoreCase);

            using var message = new MailMessage();

            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(email));

            message.Subject = isPasswordReset
                ? "Er Job Portal - Organization Password Reset OTP"
                : "Er Job Portal - Organization Email Verification";

            message.Body = isPasswordReset
                ? "Dear Organization,\n\n" +
                  $"Your OTP to reset your organization account password is: {otp}\n\n" +
                  "This OTP is valid for 10 minutes. Please do not share it with anyone.\n\n" +
                  "If you did not request a password reset, you can ignore this email.\n\n" +
                  "Regards,\nEr Job Portal Team"
                : "Dear Organization,\n\n" +
                  $"Your OTP for verifying your organization email address is: {otp}\n\n" +
                  "This OTP is valid for 10 minutes. Please do not share it with anyone.\n\n" +
                  "Enter this OTP on the organization registration page to complete your email verification.\n\n" +
                  "Regards,\nEr Job Portal Team";

            using var smtp = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            try
            {
                _logger.LogInformation(
                    "Sending organization OTP email to {Email}", email);

                await smtp.SendMailAsync(message);

                _logger.LogInformation(
                    "Organization OTP email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to send organization OTP email to {Email}", email);

                throw;
            }
        }





        public async Task SendSuperAdminResetOtpEmailAsync(
                string email,
                string otp)
        {
            var smtpServer =
                _configuration["EmailSettings:SmtpServer"];

            var port =
                int.TryParse(
                    _configuration["EmailSettings:Port"],
                    out int parsedPort)
                    ? parsedPort
                    : 587;

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            var fromEmail =
                _configuration["EmailSettings:FromEmail"];

            var fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "SMTP email settings are incomplete.");
            }

            using var message = new System.Net.Mail.MailMessage();

            message.From = new System.Net.Mail.MailAddress(
                fromEmail, fromName);

            message.To.Add(email);
            message.Subject = "Super Admin Password Reset OTP";
            message.IsBodyHtml = true;

            message.Body = $@"
        <div style='font-family:Arial,sans-serif;
                    max-width:560px;margin:auto;padding:24px;
                    border:1px solid #e5e7eb;border-radius:12px;'>
            <h2 style='color:#1e7b67;'>
                Super Admin Password Reset
            </h2>

            <p>Use the OTP below to verify your email address.</p>

            <div style='font-size:30px;font-weight:bold;
                        letter-spacing:8px;color:#1e7b67;
                        padding:16px 0;'>
                {System.Net.WebUtility.HtmlEncode(otp)}
            </div>

            <p>This OTP expires in <b>10 minutes</b> and can
               be used only once.</p>

            <p>If you did not request a password reset,
               you can ignore this email.</p>

            <hr />
            <small>Er Job Portal — Authorized Super Admin Access</small>
        </div>";

            using var client = new System.Net.Mail.SmtpClient(
                smtpServer, port);

            client.EnableSsl = true;
            client.Credentials =
                new System.Net.NetworkCredential(username, password);

            await client.SendMailAsync(message);
        }



        public async Task SendTPOOtpEmailAsync(string email, string otp)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            string portValue =
                _configuration["EmailSettings:Port"] ?? "587";

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string password =
                _configuration["EmailSettings:Password"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"] ?? "";

            string fromName =
                _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings are missing or incomplete in configuration.");
            }

            if (!int.TryParse(portValue, out int port))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port must be a valid number.");
            }

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(otp))
            {
                throw new ArgumentException(
                    "Email address and OTP are required.");
            }

            using var message = new MailMessage();

            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(email.Trim()));

            message.Subject = "Er Job Portal - TPO Email Verification";

            message.Body =
                "Dear TPO,\n\n" +
                $"Your OTP for verifying your college email address is: {otp}\n\n" +
                "This OTP is valid for 10 minutes. Please do not share it with anyone.\n\n" +
                "Enter this OTP on the TPO registration page to complete your email verification.\n\n" +
                "Regards,\n" +
                "Er Job Portal Team";

            using var smtp = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            try
            {
                _logger.LogInformation(
                    "Sending TPO OTP email to {Email}", email);

                await smtp.SendMailAsync(message);

                _logger.LogInformation(
                    "TPO OTP email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send TPO OTP email to {Email}", email);

                throw;
            }
        }









        // =========================================================
        // TPO / SUB-TPO APPROVAL EMAIL
        // =========================================================
        public async Task SendTPOApprovalEmailAsync(
            string email,
            string fullName,
            string tpoType)
        {
            ValidateTPOEmailInputs(email, tpoType);

            string safeName =
                WebUtility.HtmlEncode(
                    string.IsNullOrWhiteSpace(fullName)
                        ? "TPO"
                        : fullName.Trim());

            string safeType =
                WebUtility.HtmlEncode(tpoType);

            using var message = new MailMessage();

            message.From = GetConfiguredFromAddress();
            message.To.Add(new MailAddress(email.Trim()));
            message.Subject =
                $"Your Er Job Portal {tpoType} Registration Is Approved";
            message.IsBodyHtml = true;

            message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:20px;background:#f5f7fa;
             font-family:Arial,Helvetica,sans-serif;'>
    <div style='max-width:600px;margin:auto;background:#fff;
                border:1px solid #e2e8f0;border-radius:10px;overflow:hidden;'>

        <div style='background:#0d9488;padding:22px;text-align:center;'>
            <h2 style='color:#fff;margin:0;'>Er Job Portal</h2>
        </div>

        <div style='padding:28px;'>
            <p>Dear <strong>{safeName}</strong>,</p>

            <h3 style='color:#198754;'>
                {safeType} Registration Approved!
            </h3>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                Congratulations! Your {safeType} registration
                has been approved by the Super Admin.
                You can now log in to Er Job Portal using
                your registered credentials.
            </p>

            <p>Regards,<br>
                <strong>Er Job Portal Team</strong>
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

            await SendTPOEmailMessageAsync(message, email);
        }


        // =========================================================
        // TPO / SUB-TPO REJECTION EMAIL
        // =========================================================
        public async Task SendTPORejectionEmailAsync(
            string email,
            string fullName,
            string tpoType,
            string rejectionReason)
        {
            ValidateTPOEmailInputs(email, tpoType);

            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                throw new ArgumentException(
                    "Rejection reason is required.",
                    nameof(rejectionReason));
            }

            string safeName =
                WebUtility.HtmlEncode(
                    string.IsNullOrWhiteSpace(fullName)
                        ? "TPO"
                        : fullName.Trim());

            string safeType =
                WebUtility.HtmlEncode(tpoType);

            string safeReason =
                WebUtility.HtmlEncode(rejectionReason.Trim());

            using var message = new MailMessage();

            message.From = GetConfiguredFromAddress();
            message.To.Add(new MailAddress(email.Trim()));
            message.Subject =
                $"Your Er Job Portal {tpoType} Registration Update";
            message.IsBodyHtml = true;

            message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:20px;background:#f5f7fa;
             font-family:Arial,Helvetica,sans-serif;'>
    <div style='max-width:600px;margin:auto;background:#fff;
                border:1px solid #e2e8f0;border-radius:10px;overflow:hidden;'>

        <div style='background:#0d9488;padding:22px;text-align:center;'>
            <h2 style='color:#fff;margin:0;'>Er Job Portal</h2>
        </div>

        <div style='padding:28px;'>
            <p>Dear <strong>{safeName}</strong>,</p>

            <h3 style='color:#dc3545;'>
                {safeType} Registration Not Approved
            </h3>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                We regret to inform you that your {safeType}
                registration has been rejected by the Super Admin.
            </p>

            <div style='background:#fff5f5;border-left:4px solid #dc3545;
                        padding:15px;margin:20px 0;'>
                <strong>Reason for rejection:</strong>
                <p style='color:#555;font-size:14px;line-height:1.8;'>
                    {safeReason}
                </p>
            </div>

            <p style='color:#555;font-size:14px;line-height:1.8;'>
                If you have questions about this decision, please
                contact the Er Job Portal administration team.
            </p>

            <p>Regards,<br>
                <strong>Er Job Portal Team</strong>
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

            await SendTPOEmailMessageAsync(message, email);
        }


        // =========================================================
        // VALIDATE TPO EMAIL INPUTS
        // =========================================================
        private static void ValidateTPOEmailInputs(
            string email,
            string tpoType)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "TPO email address is required.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(tpoType))
            {
                throw new ArgumentException(
                    "TPO type is required.",
                    nameof(tpoType));
            }

            // Only these two types are expected from the controller.
            if (!string.Equals(tpoType, "TPO",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(tpoType, "Sub-TPO",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "TPO type must be TPO or Sub-TPO.",
                    nameof(tpoType));
            }
        }


        // =========================================================
        // CONFIGURED SENDER ADDRESS
        // =========================================================
        private MailAddress GetConfiguredFromAddress()
        {
            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"] ?? username;

            string fromName =
                _configuration["EmailSettings:FromName"] ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "EmailSettings:FromEmail is missing.");
            }

            return new MailAddress(fromEmail, fromName);
        }


        // =========================================================
        // COMMON SMTP SENDER FOR TPO EMAILS
        // =========================================================
        private async Task SendTPOEmailMessageAsync(
            MailMessage message,
            string recipientEmail)
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            string portValue =
                _configuration["EmailSettings:Port"] ?? "587";

            bool enableSsl = bool.TryParse(
                _configuration["EmailSettings:EnableSsl"],
                out bool configuredSsl)
                ? configuredSsl
                : true;

            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException(
                    "SMTP settings are missing. Check EmailSettings in configuration.");
            }

            if (!int.TryParse(portValue, out int port) ||
                port < 1 || port > 65535)
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port must be a valid port number.");
            }

            using var smtpClient = new SmtpClient(smtpServer, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    username,
                    smtpPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            try
            {
                await smtpClient.SendMailAsync(message);

                _logger.LogInformation(
                    "TPO notification email sent to {RecipientEmail}.",
                    recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send TPO notification email to {RecipientEmail}.",
                    recipientEmail);

                throw;
            }
        }



        //private async Task SendTPOEmailMessageAsync(
        //    MailMessage message,
        //    string recipientEmail)
        //{
        //    string smtpServer =
        //        _configuration["EmailSettings:SmtpServer"] ?? "";

        //    int port = int.TryParse(
        //        _configuration["EmailSettings:Port"],
        //        out int configuredPort)
        //        ? configuredPort
        //        : 587;

        //    string username =
        //        _configuration["EmailSettings:Username"] ?? "";

        //    string smtpPassword =
        //        _configuration["EmailSettings:Password"] ?? "";

        //    bool enableSsl = bool.TryParse(
        //        _configuration["EmailSettings:EnableSsl"],
        //        out bool configuredSsl)
        //        ? configuredSsl
        //        : true;

        //    string fromEmail =
        //        _configuration["EmailSettings:FromEmail"] ?? username;

        //    if (string.IsNullOrWhiteSpace(smtpServer))
        //        throw new InvalidOperationException(
        //            "EmailSettings:SmtpServer is missing.");

        //    if (string.IsNullOrWhiteSpace(username))
        //        throw new InvalidOperationException(
        //            "EmailSettings:Username is missing.");

        //    if (string.IsNullOrWhiteSpace(smtpPassword))
        //        throw new InvalidOperationException(
        //            "EmailSettings:Password is missing.");

        //    if (string.IsNullOrWhiteSpace(fromEmail))
        //        throw new InvalidOperationException(
        //            "EmailSettings:FromEmail is missing.");

        //    using var smtpClient = new SmtpClient(smtpServer, port)
        //    {
        //        EnableSsl = enableSsl,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(
        //            username,
        //            smtpPassword),
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        Timeout = 30000
        //    };

        //    try
        //    {
        //        await smtpClient.SendMailAsync(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(
        //            ex,
        //            "Failed to send TPO email to {RecipientEmail}. SMTP server: {SmtpServer}, port: {Port}",
        //            recipientEmail,
        //            smtpServer,
        //            port);

        //        throw;
        //    }
        //}




    }
}