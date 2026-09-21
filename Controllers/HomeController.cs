using ErJobPortal.Data;
using ErJobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ErJobPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbConnection _dbConnection;

        public HomeController(ILogger<HomeController> logger, DbConnection dbConnection)
        {
            _logger = logger;
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int traineeCount = 0;
            int organizationCount = 0;
            int postCount = 0;
            int stateCount = 0;
            int countryCount = 0;

            using (SqlConnection con = _dbConnection.GetConnection())
            {
                con.Open();

                // =====================================================
                // TRAINEE, ORGANIZATION AND POST COUNT
                // =====================================================

                string query = @"
      SELECT
          (SELECT COUNT(nID)
           FROM tblCandidateRegister) AS CandidateCount,

          (SELECT COUNT(nID)
           FROM tblOrgRegistration) AS OrganizationCount,

          (SELECT COUNT(nID)
           FROM tblPost) AS PostCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
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

                // =====================================================
                // STATE COUNT
                // =====================================================

                using (SqlCommand cmdState =
                       new SqlCommand("SP_CountState", con))
                {
                    cmdState.CommandType = CommandType.StoredProcedure;

                    object? result = cmdState.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        stateCount = Convert.ToInt32(result);
                    }
                }

                // =====================================================
                // COUNTRY COUNT
                // =====================================================

                using (SqlCommand cmdCountry =
                       new SqlCommand("SP_CountCountry", con))
                {
                    cmdCountry.CommandType = CommandType.StoredProcedure;

                    object? result = cmdCountry.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        countryCount = Convert.ToInt32(result);
                    }
                }
            }

            // =====================================================
            // SEND COUNTS TO VIEW
            // =====================================================

            ViewBag.TraineeCount = traineeCount;
            ViewBag.OrganizationCount = organizationCount;
            ViewBag.PostCount = postCount;
            ViewBag.StateCount = stateCount;
            ViewBag.CountryCount = countryCount;

            return View("Index");
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
            return View();
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
