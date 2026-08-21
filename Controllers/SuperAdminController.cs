using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ErJobPortal.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountRepository _repository;

        public SuperAdminController(IConfiguration configuration, AccountRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            int traineeRegistrationCount = 0;
            int organizationRegistrationCount = 0;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT (SELECT COUNT(nID) FROM tblCandidateRegister) AS CandidateCount, (SELECT COUNT(nID)  FROM tblOrgRegistration) AS OrganizationCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            traineeRegistrationCount = Convert.ToInt32(dr["CandidateCount"]);
                            organizationRegistrationCount = Convert.ToInt32(dr["OrganizationCount"]);
                        }
                    }
                }
            }

            // Get complete trainee list
            List<SATraineeListM> trainees = _repository.GetAllTrainees();
            List<OrganizationUser> organizations = _repository.GetAllOrganizationList();
            ViewBag.TraineeRegistrationCount = traineeRegistrationCount;
            ViewBag.OrganizationRegistrationCount = organizationRegistrationCount;
            ViewBag.Trainees = trainees;
            ViewBag.Organizations = organizations;
            return View();
        }

        // ==========================================
        // CANDIDATE LIST
        // ==========================================
        [HttpGet]
        public IActionResult SATrainees()
        {
            List<SATraineeListM> trainees = _repository.GetSATraineeList();
            return View(trainees);
        }

        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        [HttpGet]
        public IActionResult SAOrganizations()
        {
            List<OrganizationUser> organization = _repository.GetAllOrganizationList();
            return View(organization);
        }

        [HttpGet]
        public IActionResult AddObjective()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewObjective()
        {
            List<SAObjectiveM> objectives = new List<SAObjectiveM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            // Get Objectives
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetObjective", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            objectives.Add(new SAObjectiveM
                            {
                                nID = Convert.ToInt32(reader["nID"]),
                                nGenderID = Convert.ToInt32(reader["nGenderID"]),
                                sObjective = reader["sObjective"]?.ToString() ?? "",
                                dCreatedDate = Convert.ToDateTime(reader["dCreatedDate"]),
                                nBit = Convert.ToBoolean(reader["nBit"]),
                                nSABit = Convert.ToBoolean(reader["nSABit"])
                            });
                        }
                    }
                }
            }

            // Get Gender Names
            Dictionary<int, string> genderNames =
                new Dictionary<int, string>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetGender", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["nID"]);
                            string name = reader["sName"]?.ToString() ?? "";

                            genderNames[id] = name;
                        }
                    }
                }
            }

            ViewBag.GenderNames = genderNames;

            return View(objectives);
        }

        [HttpPost]
        public async Task<IActionResult> AddObjective(SAObjectiveM model)
        {
            if (model.nGenderID <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Please select gender."
                });
            }

            if (string.IsNullOrWhiteSpace(model.sObjective))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter objective."
                });
            }

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("SP_AddObjective", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@nGenderID", SqlDbType.Int)
                            .Value = model.nGenderID;

                        cmd.Parameters.Add("@sObjective", SqlDbType.NVarChar)
                            .Value = model.sObjective.Trim();

                        await cn.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Objective added successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }


        // ==========================================
        // CHANGE OBJECTIVE STATUS
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, bool status)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("SP_ChangeObjectiveStatus", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@nID", SqlDbType.Int)
                            .Value = id;

                        cmd.Parameters.Add("@nBit", SqlDbType.Bit)
                            .Value = status;

                        await cn.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Status changed successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }


        [HttpGet]
        public IActionResult SAFeedback()
        {
            List<SAFeedbackM> feedbackList = new List<SAFeedbackM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSATRFeedback
      WHERE ISNULL(nBit, 1) = 1
      ORDER BY nID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            feedbackList.Add(new SAFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] != DBNull.Value
                                    ? dr["Que1"].ToString()
                                    : "",

                                Ans1 = dr["Ans1"] != DBNull.Value
                                    ? dr["Ans1"].ToString()
                                    : "",

                                Que2 = dr["Que2"] != DBNull.Value
                                    ? dr["Que2"].ToString()
                                    : "",

                                Ans2 = dr["Ans2"] != DBNull.Value
                                    ? dr["Ans2"].ToString()
                                    : "",

                                Que3 = dr["Que3"] != DBNull.Value
                                    ? dr["Que3"].ToString()
                                    : "",

                                Ans3 = dr["Ans3"] != DBNull.Value
                                    ? dr["Ans3"].ToString()
                                    : "",

                                Que4 = dr["Que4"] != DBNull.Value
                                    ? dr["Que4"].ToString()
                                    : "",

                                Ans4 = dr["Ans4"] != DBNull.Value
                                    ? dr["Ans4"].ToString()
                                    : "",

                                Que5 = dr["Que5"] != DBNull.Value
                                    ? dr["Que5"].ToString()
                                    : "",

                                Ans5 = dr["Ans5"] != DBNull.Value
                                    ? dr["Ans5"].ToString()
                                    : "",

                                nBit = dr["nBit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nBit"])
                                    : true,

                                nSABit = dr["nSABit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nSABit"])
                                    : true,

                                nSAID = dr["nSAID"] != DBNull.Value
                                    ? Convert.ToInt32(dr["nSAID"])
                                    : 0,

                                dRegDate = dr["dRegDate"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["dRegDate"])
                                    : null,

                                dModDate = dr["dModDate"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["dModDate"])
                                    : null
                            });
                        }
                    }
                }
            }

            return View(feedbackList);
        }


        // =========================================================
        // EDIT FEEDBACK - GET
        // URL:
        // /SuperAdmin/EditSAFeedback/1
        // =========================================================

        [HttpGet]
        public IActionResult EditSAFeedback(int id)
        {
            SAFeedbackM model = new SAFeedbackM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          nSAID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5
      FROM tblSATRFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model.nID = Convert.ToInt32(dr["nID"]);

                            model.nSAID =
                                dr["nSAID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(dr["nSAID"]);

                            model.Que1 =
                                dr["Que1"] == DBNull.Value
                                ? null
                                : dr["Que1"].ToString();

                            model.Ans1 =
                                dr["Ans1"] == DBNull.Value
                                ? null
                                : dr["Ans1"].ToString();

                            model.Que2 =
                                dr["Que2"] == DBNull.Value
                                ? null
                                : dr["Que2"].ToString();

                            model.Ans2 =
                                dr["Ans2"] == DBNull.Value
                                ? null
                                : dr["Ans2"].ToString();

                            model.Que3 =
                                dr["Que3"] == DBNull.Value
                                ? null
                                : dr["Que3"].ToString();

                            model.Ans3 =
                                dr["Ans3"] == DBNull.Value
                                ? null
                                : dr["Ans3"].ToString();

                            model.Que4 =
                                dr["Que4"] == DBNull.Value
                                ? null
                                : dr["Que4"].ToString();

                            model.Ans4 =
                                dr["Ans4"] == DBNull.Value
                                ? null
                                : dr["Ans4"].ToString();

                            model.Que5 =
                                dr["Que5"] == DBNull.Value
                                ? null
                                : dr["Que5"].ToString();

                            model.Ans5 =
                                dr["Ans5"] == DBNull.Value
                                ? null
                                : dr["Ans5"].ToString();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSAFeedback(SAFeedbackM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      UPDATE tblSATRFeedback
      SET
          Que1 = @Que1,
          Ans1 = @Ans1,
          Que2 = @Que2,
          Ans2 = @Ans2,
          Que3 = @Que3,
          Ans3 = @Ans3,
          Que4 = @Que4,
          Ans4 = @Ans4,
          Que5 = @Que5,
          Ans5 = @Ans5
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int)
                        .Value = model.nID;

                    cmd.Parameters.Add("@Que1", SqlDbType.NVarChar)
                        .Value = (object?)model.Que1 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans1", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans1 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que2", SqlDbType.NVarChar)
                        .Value = (object?)model.Que2 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans2", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans2 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que3", SqlDbType.NVarChar)
                        .Value = (object?)model.Que3 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans3", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans3 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que4", SqlDbType.NVarChar)
                        .Value = (object?)model.Que4 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans4", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans4 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que5", SqlDbType.NVarChar)
                        .Value = (object?)model.Que5 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans5", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans5 ?? DBNull.Value;


                    int rowsAffected = cmd.ExecuteNonQuery();


                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            TempData["SuccessMessage"] = "Feedback updated successfully.";

            return RedirectToAction("SAFeedback");
        }


        // =========================================================
        // EDIT FEEDBACK - POST
        // =========================================================
        [HttpGet]
        public IActionResult DetailsSAFeedback(int id)
        {
            SAFeedbackM model = new SAFeedbackM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSATRFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model.nID = Convert.ToInt32(dr["nID"]);

                            model.Que1 = dr["Que1"]?.ToString();
                            model.Ans1 = dr["Ans1"]?.ToString();

                            model.Que2 = dr["Que2"]?.ToString();
                            model.Ans2 = dr["Ans2"]?.ToString();

                            model.Que3 = dr["Que3"]?.ToString();
                            model.Ans3 = dr["Ans3"]?.ToString();

                            model.Que4 = dr["Que4"]?.ToString();
                            model.Ans4 = dr["Ans4"]?.ToString();

                            model.Que5 = dr["Que5"]?.ToString();
                            model.Ans5 = dr["Ans5"]?.ToString();

                            model.nBit = dr["nBit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nBit"]);

                            model.nSABit = dr["nSABit"] != DBNull.Value &&
                                           Convert.ToBoolean(dr["nSABit"]);

                            model.nSAID = dr["nSAID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nSAID"])
                                : 0;

                            model.dRegDate = dr["dRegDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dRegDate"])
                                : DateTime.MinValue;

                            model.dModDate = dr["dModDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dModDate"])
                                : DateTime.MinValue;
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return View(model);
        }



        // =========================================================
        // SA ORGANIZATION FEEDBACK - GET
        // =========================================================
        [HttpGet]
        public IActionResult SAOrgFeedback()
        {
            List<SAOrgFeedbackM> feedbackList =
                new List<SAOrgFeedbackM>();

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
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE ISNULL(nBit, 1) = 1
      ORDER BY nID DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            feedbackList.Add(new SAOrgFeedbackM
                            {
                                nID =
                                    dr["nID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nID"])
                                        : 0,

                                Que1 =
                                    dr["Que1"] != DBNull.Value
                                        ? dr["Que1"].ToString()
                                        : "",

                                Ans1 =
                                    dr["Ans1"] != DBNull.Value
                                        ? dr["Ans1"].ToString()
                                        : "",

                                Que2 =
                                    dr["Que2"] != DBNull.Value
                                        ? dr["Que2"].ToString()
                                        : "",

                                Ans2 =
                                    dr["Ans2"] != DBNull.Value
                                        ? dr["Ans2"].ToString()
                                        : "",

                                Que3 =
                                    dr["Que3"] != DBNull.Value
                                        ? dr["Que3"].ToString()
                                        : "",

                                Ans3 =
                                    dr["Ans3"] != DBNull.Value
                                        ? dr["Ans3"].ToString()
                                        : "",

                                Que4 =
                                    dr["Que4"] != DBNull.Value
                                        ? dr["Que4"].ToString()
                                        : "",

                                Ans4 =
                                    dr["Ans4"] != DBNull.Value
                                        ? dr["Ans4"].ToString()
                                        : "",

                                Que5 =
                                    dr["Que5"] != DBNull.Value
                                        ? dr["Que5"].ToString()
                                        : "",

                                Ans5 =
                                    dr["Ans5"] != DBNull.Value
                                        ? dr["Ans5"].ToString()
                                        : "",

                                nBit =
                                    dr["nBit"] != DBNull.Value
                                        ? Convert.ToBoolean(dr["nBit"])
                                        : true,

                                nSABit =
                                    dr["nSABit"] != DBNull.Value
                                        ? Convert.ToBoolean(dr["nSABit"])
                                        : true,

                                nSAID =
                                    dr["nSAID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nSAID"])
                                        : 0,

                                dRegDate =
                                    dr["dRegDate"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["dRegDate"])
                                        : null,

                                dModDate =
                                    dr["dModDate"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["dModDate"])
                                        : null
                            });
                        }
                    }
                }
            }

            return View(feedbackList);
        }

        [HttpGet]
        public IActionResult EditSAOrgFeedback(int id)
        {
            SAOrgFeedbackM model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1, Ans1,
          Que2, Ans2,
          Que3, Ans3,
          Que4, Ans4,
          Que5, Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SAOrgFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] == DBNull.Value ? "" : dr["Que1"].ToString(),
                                Ans1 = dr["Ans1"] == DBNull.Value ? "" : dr["Ans1"].ToString(),

                                Que2 = dr["Que2"] == DBNull.Value ? "" : dr["Que2"].ToString(),
                                Ans2 = dr["Ans2"] == DBNull.Value ? "" : dr["Ans2"].ToString(),

                                Que3 = dr["Que3"] == DBNull.Value ? "" : dr["Que3"].ToString(),
                                Ans3 = dr["Ans3"] == DBNull.Value ? "" : dr["Ans3"].ToString(),

                                Que4 = dr["Que4"] == DBNull.Value ? "" : dr["Que4"].ToString(),
                                Ans4 = dr["Ans4"] == DBNull.Value ? "" : dr["Ans4"].ToString(),

                                Que5 = dr["Que5"] == DBNull.Value ? "" : dr["Que5"].ToString(),
                                Ans5 = dr["Ans5"] == DBNull.Value ? "" : dr["Ans5"].ToString(),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"]),

                                nSAID = dr["nSAID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nSAID"]),

                                dRegDate = dr["dRegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dRegDate"]),

                                dModDate = dr["dModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dModDate"])
                            };
                        }
                    }
                }
            }

            if (model == null)
                return NotFound();

            return View(model);
        }

        // =========================================================
        // SA ORGANIZATION FEEDBACK - UPDATE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSAOrgFeedback(SAOrgFeedbackM model)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
          UPDATE tblSAOrgFeedback
          SET
              Que1 = @Que1,
              Ans1 = @Ans1,

              Que2 = @Que2,
              Ans2 = @Ans2,

              Que3 = @Que3,
              Ans3 = @Ans3,

              Que4 = @Que4,
              Ans4 = @Ans4,

              Que5 = @Que5,
              Ans5 = @Ans5,

              dModDate = GETDATE()

          WHERE nID = @nID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@nID", SqlDbType.Int)
                            .Value = model.nID;

                        cmd.Parameters.Add("@Que1", SqlDbType.NVarChar)
                            .Value = (object?)model.Que1 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans1", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans1 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que2", SqlDbType.NVarChar)
                            .Value = (object?)model.Que2 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans2", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans2 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que3", SqlDbType.NVarChar)
                            .Value = (object?)model.Que3 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans3", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans3 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que4", SqlDbType.NVarChar)
                            .Value = (object?)model.Que4 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans4", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans4 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que5", SqlDbType.NVarChar)
                            .Value = (object?)model.Que5 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans5", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans5 ?? DBNull.Value;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            TempData["ErrorMessage"] =
                                "Feedback record not found.";

                            return View(model);
                        }
                    }
                }

                TempData["SuccessMessage"] =
                    "Feedback updated successfully.";

                return RedirectToAction("SAOrgFeedback");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " + ex.Message;

                return View(model);
            }
        }


        [HttpGet]
        public IActionResult DetailsSAOrgFeedback(int id)
        {
            SAOrgFeedbackM model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1, Ans1,
          Que2, Ans2,
          Que3, Ans3,
          Que4, Ans4,
          Que5, Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int)
                        .Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SAOrgFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] == DBNull.Value
                                    ? ""
                                    : dr["Que1"].ToString(),

                                Ans1 = dr["Ans1"] == DBNull.Value
                                    ? ""
                                    : dr["Ans1"].ToString(),

                                Que2 = dr["Que2"] == DBNull.Value
                                    ? ""
                                    : dr["Que2"].ToString(),

                                Ans2 = dr["Ans2"] == DBNull.Value
                                    ? ""
                                    : dr["Ans2"].ToString(),

                                Que3 = dr["Que3"] == DBNull.Value
                                    ? ""
                                    : dr["Que3"].ToString(),

                                Ans3 = dr["Ans3"] == DBNull.Value
                                    ? ""
                                    : dr["Ans3"].ToString(),

                                Que4 = dr["Que4"] == DBNull.Value
                                    ? ""
                                    : dr["Que4"].ToString(),

                                Ans4 = dr["Ans4"] == DBNull.Value
                                    ? ""
                                    : dr["Ans4"].ToString(),

                                Que5 = dr["Que5"] == DBNull.Value
                                    ? ""
                                    : dr["Que5"].ToString(),

                                Ans5 = dr["Ans5"] == DBNull.Value
                                    ? ""
                                    : dr["Ans5"].ToString(),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"]),

                                nSAID = dr["nSAID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nSAID"]),

                                dRegDate = dr["dRegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dRegDate"]),

                                dModDate = dr["dModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dModDate"])
                            };
                        }
                    }
                }
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


    }
}