using ErJobPortal.Data;
using ErJobPortal.Models;
using ErJobPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;

namespace ErJobPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;
        private readonly DbConnection _dbConnection;
        private readonly IConfiguration _configuration;

        public HomeController(
            ILogger<HomeController> logger,
            DbConnection dbConnection,
            EmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _dbConnection = dbConnection;
            _emailService = emailService;
            _configuration = configuration;
        }



        //public IActionResult Index()
        //{
        //    List<HomeTraineeCardM> trainees = new List<HomeTraineeCardM>();
        //    List<HomeFeedbackCardM> feedbacks = new List<HomeFeedbackCardM>();

        //    string connectionString =
        //        _configuration.GetConnectionString("DefaultConnection");

        //    using (SqlConnection cn = new SqlConnection(connectionString))
        //    {
        //        cn.Open();

        //        // ============================================================
        //        // TRAINEE LIST
        //        // ============================================================

        //        string traineeQuery = @"
        //    SELECT 
        //        CR.nID AS CandidateID,

        //        LTRIM(RTRIM(
        //            ISNULL(CR.sFName, '') + ' ' +
        //            ISNULL(CR.sLName, '')
        //        )) AS TraineeName,

        //        CP.sPhoto AS ProfileImage,

        //        '' AS Designation,

        //        LTRIM(RTRIM(
        //            ISNULL(CP.Preferred_City, '') +
        //            CASE 
        //                WHEN CP.Preferred_City IS NOT NULL
        //                     AND CP.Preferred_City <> ''
        //                     AND CP.Preferred_State IS NOT NULL
        //                     AND CP.Preferred_State <> ''
        //                THEN ', '
        //                ELSE ''
        //            END +
        //            ISNULL(CP.Preferred_State, '')
        //        )) AS Location

        //    FROM tblCandidateRegister CR

        //    INNER JOIN tblCandidateProfile CP
        //        ON CP.CandidateID = CR.nID

        //    WHERE ISNULL(CR.nBit, 1) = 1
        //      AND ISNULL(CP.nBit, 1) = 1

        //    ORDER BY CR.nID DESC";


        //        using (SqlCommand cmd = new SqlCommand(traineeQuery, cn))
        //        {
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    trainees.Add(new HomeTraineeCardM
        //                    {
        //                        CandidateID =
        //                            Convert.ToInt32(dr["CandidateID"]),

        //                        TraineeName =
        //                            dr["TraineeName"]?.ToString(),

        //                        ProfileImage =
        //                            dr["ProfileImage"]?.ToString(),

        //                        Designation =
        //                            dr["Designation"]?.ToString(),

        //                        Location =
        //                            dr["Location"]?.ToString()
        //                    });
        //                }
        //            }
        //        }


        //        // ============================================================
        //        // CANDIDATE FEEDBACK
        //        // ============================================================

        //        string feedbackQuery = @"
        //    SELECT
        //        CF.nAdminID AS CandidateID,

        //        LTRIM(RTRIM(
        //            ISNULL(CR.sFName, '') + ' ' +
        //            ISNULL(CR.sLName, '')
        //        )) AS TraineeName,

        //        CP.sPhoto AS ProfileImage,

        //        '' AS Designation,

        //        LTRIM(RTRIM(
        //            ISNULL(CP.Preferred_City, '') +
        //            CASE
        //                WHEN CP.Preferred_City IS NOT NULL
        //                     AND CP.Preferred_City <> ''
        //                     AND CP.Preferred_State IS NOT NULL
        //                     AND CP.Preferred_State <> ''
        //                THEN ', '
        //                ELSE ''
        //            END +
        //            ISNULL(CP.Preferred_State, '')
        //        )) AS Location,

        //        ISNULL(CF.sQue5, '') AS Feedback,

        //        ISNULL(
        //            TRY_CONVERT(INT, CF.sQue2),
        //            0
        //        ) AS Rating

        //    FROM tblCandidateFeedback CF

        //    INNER JOIN tblCandidateRegister CR
        //        ON CR.nID = CF.nAdminID

        //    INNER JOIN tblCandidateProfile CP
        //        ON CP.CandidateID = CR.nID

        //    WHERE ISNULL(CF.nSABit, 0) = 1

        //      AND ISNULL(CR.nBit, 1) = 1

        //      AND ISNULL(CP.nBit, 1) = 1

        //      AND ISNULL(LTRIM(RTRIM(CF.sQue5)), '') <> ''

        //    ORDER BY CF.nID DESC";


        //        using (SqlCommand cmd = new SqlCommand(feedbackQuery, cn))
        //        {
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    feedbacks.Add(new HomeFeedbackCardM
        //                    {
        //                        CandidateID =
        //                            Convert.ToInt32(dr["CandidateID"]),

        //                        TraineeName =
        //                            dr["TraineeName"]?.ToString(),

        //                        ProfileImage =
        //                            dr["ProfileImage"]?.ToString(),

        //                        Designation =
        //                            dr["Designation"]?.ToString(),

        //                        Location =
        //                            dr["Location"]?.ToString(),

        //                        Feedback =
        //                            dr["Feedback"]?.ToString(),

        //                        Rating =
        //                            dr["Rating"] == DBNull.Value
        //                                ? 0
        //                                : Convert.ToInt32(dr["Rating"])
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    // Send feedback to Home/Index.cshtml
        //    ViewBag.Feedbacks = feedbacks;

        //    return View(trainees);
        //}



        public IActionResult Index()
        {
            // ============================================================
            // CODE 1 : TRAINEE + FEEDBACK LIST
            // ============================================================

            List<HomeTraineeCardM> trainees = new List<HomeTraineeCardM>();
            List<HomeFeedbackCardM> feedbacks = new List<HomeFeedbackCardM>();


            // ============================================================
            // CODE 2 : DASHBOARD COUNTS
            // ============================================================

            int traineeCount = 0;
            int organizationCount = 0;
            int postCount = 0;
            int stateCount = 0;
            int countryCount = 0;


            // ============================================================
            // DATABASE CONNECTION
            // ============================================================

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");


            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();


                // ========================================================
                // CODE 1
                // TRAINEE LIST
                // ========================================================

                string traineeQuery = @"
       SELECT  
           CR.nID AS CandidateID,

           LTRIM(RTRIM(
               ISNULL(CR.sFName, '') + ' ' +
               ISNULL(CR.sLName, '')
           )) AS TraineeName,

           CP.sPhoto AS ProfileImage,

           '' AS Designation,

           LTRIM(RTRIM(
               ISNULL(CP.Preferred_City, '') +
               CASE
                   WHEN CP.Preferred_City IS NOT NULL
                        AND CP.Preferred_City <> ''
                        AND CP.Preferred_State IS NOT NULL
                        AND CP.Preferred_State <> ''
                   THEN ', '
                   ELSE ''
               END +
               ISNULL(CP.Preferred_State, '')
           )) AS Location

       FROM tblCandidateRegister CR

       INNER JOIN tblCandidateProfile CP
           ON CP.CandidateID = CR.nID

       WHERE ISNULL(CR.nBit, 1) = 1
         AND ISNULL(CP.nBit, 1) = 1

       ORDER BY CR.nID DESC";


                using (SqlCommand cmd = new SqlCommand(traineeQuery, cn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            trainees.Add(new HomeTraineeCardM
                            {
                                CandidateID =
                                    Convert.ToInt32(dr["CandidateID"]),

                                TraineeName =
                                    dr["TraineeName"]?.ToString(),

                                ProfileImage =
                                    dr["ProfileImage"]?.ToString(),

                                Designation =
                                    dr["Designation"]?.ToString(),

                                Location =
                                    dr["Location"]?.ToString()
                            });
                        }
                    }
                }


                // ========================================================
                // CODE 1
                // CANDIDATE FEEDBACK
                // ========================================================

                string feedbackQuery = @"
       SELECT
           CF.nAdminID AS CandidateID,

           LTRIM(RTRIM(
               ISNULL(CR.sFName, '') + ' ' +
               ISNULL(CR.sLName, '')
           )) AS TraineeName,

           CP.sPhoto AS ProfileImage,

           '' AS Designation,

           LTRIM(RTRIM(
               ISNULL(CP.Preferred_City, '') +
               CASE
                   WHEN CP.Preferred_City IS NOT NULL
                        AND CP.Preferred_City <> ''
                        AND CP.Preferred_State IS NOT NULL
                        AND CP.Preferred_State <> ''
                   THEN ', '
                   ELSE ''
               END +
               ISNULL(CP.Preferred_State, '')
           )) AS Location,

           ISNULL(CF.sQue5, '') AS Feedback,

           ISNULL(
               TRY_CONVERT(INT, CF.sQue2),
               0
           ) AS Rating

       FROM tblCandidateFeedback CF

       INNER JOIN tblCandidateRegister CR
           ON CR.nID = CF.nAdminID

       INNER JOIN tblCandidateProfile CP
           ON CP.CandidateID = CR.nID

       WHERE ISNULL(CF.nSABit, 0) = 1

         AND ISNULL(CR.nBit, 1) = 1

         AND ISNULL(CP.nBit, 1) = 1

         AND ISNULL(LTRIM(RTRIM(CF.sQue5)), '') <> ''

       ORDER BY CF.nID DESC";


                using (SqlCommand cmd = new SqlCommand(feedbackQuery, cn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            feedbacks.Add(new HomeFeedbackCardM
                            {
                                CandidateID =
                                    Convert.ToInt32(dr["CandidateID"]),

                                TraineeName =
                                    dr["TraineeName"]?.ToString(),

                                ProfileImage =
                                    dr["ProfileImage"]?.ToString(),

                                Designation =
                                    dr["Designation"]?.ToString(),

                                Location =
                                    dr["Location"]?.ToString(),

                                Feedback =
                                    dr["Feedback"]?.ToString(),

                                Rating =
                                    dr["Rating"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(dr["Rating"])
                            });
                        }
                    }
                }


                // ========================================================
                // CODE 2
                // TRAINEE, ORGANIZATION AND POST COUNT
                // ========================================================

                string countQuery = @"
       SELECT
           (SELECT COUNT(nID)
            FROM tblCandidateRegister) AS CandidateCount,

           (SELECT COUNT(nID)
            FROM tblOrgRegistration) AS OrganizationCount,

           (SELECT COUNT(nID)
            FROM tblPost) AS PostCount;";


                using (SqlCommand cmd = new SqlCommand(countQuery, cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        traineeCount =
                            Convert.ToInt32(dr["CandidateCount"]);

                        organizationCount =
                            Convert.ToInt32(dr["OrganizationCount"]);

                        postCount =
                            Convert.ToInt32(dr["PostCount"]);
                    }
                }


                // ========================================================
                // CODE 2
                // STATE COUNT
                // ========================================================

                using (SqlCommand cmdState =
                       new SqlCommand("SP_CountState", cn))
                {
                    cmdState.CommandType =
                        CommandType.StoredProcedure;

                    object? result =
                        cmdState.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        stateCount =
                            Convert.ToInt32(result);
                    }
                }


                // ========================================================
                // CODE 2
                // COUNTRY COUNT
                // ========================================================

                using (SqlCommand cmdCountry =
                       new SqlCommand("SP_CountCountry", cn))
                {
                    cmdCountry.CommandType =
                        CommandType.StoredProcedure;

                    object? result =
                        cmdCountry.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        countryCount =
                            Convert.ToInt32(result);
                    }
                }
            }


            // ============================================================
            // SEND FEEDBACK LIST TO VIEW
            // ============================================================

            ViewBag.Feedbacks = feedbacks;


            // ============================================================
            // SEND DASHBOARD COUNTS TO VIEW
            // ============================================================

            ViewBag.TraineeCount =
                traineeCount;

            ViewBag.OrganizationCount =
                organizationCount;

            ViewBag.PostCount =
                postCount;

            ViewBag.StateCount =
                stateCount;

            ViewBag.CountryCount =
                countryCount;


            // ============================================================
            // RETURN HOME VIEW
            // ============================================================

            return View(trainees);
        }


        public IActionResult About()
        {
            return View();
        }
        public IActionResult Organization()
        {
            return View();
        }
        public IActionResult TeamInvolved()
        {
            return View();
        }
        public IActionResult Terms_Cond()
        {
            return View();
        }
        public IActionResult Org_blog()
        {
            return View();
        }
        public IActionResult Benefits()
        {
            return View();
        }
        public IActionResult Tips()
        {
            return View();
        }
        public IActionResult TopCarriers()
        {
            return View();
        }
        public IActionResult Transformation()
        {
            return View();
        }
        public IActionResult Embracing_change()
        {
            return View();
        }
        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ViewModelTrainee rg)
        {
            try
            {
                // ==========================================
                // VALIDATION
                // ==========================================

                if (rg == null)
                {
                    TempData["ContactUs"] =
                        "Please enter valid contact details.";

                    return RedirectToAction("Contact");
                }

                if (string.IsNullOrWhiteSpace(rg.FullName) ||
                    string.IsNullOrWhiteSpace(rg.Email) ||
                    string.IsNullOrWhiteSpace(rg.Subject) ||
                    string.IsNullOrWhiteSpace(rg.Description))
                {
                    TempData["ContactUs"] =
                        "Please fill all required fields.";

                    return RedirectToAction("Contact");
                }

                // ==========================================
                // TRIM VALUES
                // ==========================================

                rg.FullName = rg.FullName.Trim();
                rg.Email = rg.Email.Trim();
                rg.MobileNo = rg.MobileNo?.Trim();
                rg.Subject = rg.Subject.Trim();
                rg.Description = rg.Description.Trim();

                // ==========================================
                // SAVE CONTACT DATA
                // ==========================================

                using (SqlConnection con =
                    _dbConnection.GetConnection())
                {
                    await con.OpenAsync();

                    string query = @"
                INSERT INTO tblContactUs
                (
                    FullName,
                    Email,
                    MobileNo,
                    Subject,
                    Description
                )
                VALUES
                (
                    @FullName,
                    @Email,
                    @MobileNo,
                    @Subject,
                    @Description
                )";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@FullName",
                            (object?)rg.FullName ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Email",
                            (object?)rg.Email ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@MobileNo",
                            (object?)rg.MobileNo ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Subject",
                            (object?)rg.Subject ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Description",
                            (object?)rg.Description ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // ==========================================
                // SEND THANK-YOU EMAIL
                // TO USER-ENTERED EMAIL ADDRESS
                // ==========================================

                try
                {
                    await _emailService.SendContactThankYouEmailAsync(
                        rg.FullName,
                        rg.Email,
                        rg.Subject,
                        rg.Description);

                    _logger.LogInformation(
                        "Contact thank-you email sent successfully to {Email}",
                        rg.Email);
                }
                catch (Exception emailEx)
                {
                    // Contact data is already saved.
                    // Log email failure separately.

                    _logger.LogError(
                        emailEx,
                        "Contact saved but thank-you email failed. Recipient: {Email}",
                        rg.Email);

                    TempData["ContactUs"] =
                        "Your message was saved, but the confirmation email " +
                        "could not be sent. Please try again.";

                    return RedirectToAction("Contact");
                }

                // ==========================================
                // SUCCESS
                // ==========================================

                ModelState.Clear();

                TempData["ContactUs"] =
                    "Thank you for contacting us! " +
                    "Your message has been sent successfully.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while saving Contact Us form.");

                TempData["ContactUs"] =
                    "Something went wrong. Please try again.";

                return RedirectToAction("Contact");
            }
        }

        public IActionResult Interview_Preparation()
        {
            return View();
        }

        public IActionResult IndustryNews()
        {
            return View();
        }

        public IActionResult MarketInsight()
        {
            return View();
        }

        public IActionResult EmergingTechnologies()
        {
            return View();
        }

        public IActionResult CareerTrends()
        {
            return View();
        }

        public IActionResult ExpertOpinion()
        {
            return View();
        }

        public IActionResult SectorSpecificNews()
        {
            return View();
        }

        public ActionResult SuccessStories()
        {

            return View();
        }

        public ActionResult CoverLetter()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
