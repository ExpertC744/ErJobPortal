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
    }
}