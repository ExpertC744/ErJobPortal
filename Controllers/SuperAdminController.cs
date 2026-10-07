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
        [Route("SuperAdmin/Dashboard/{id:int}")]
        public IActionResult Dashboard(int id)
        {
            if (id != 1)
            {
                return NotFound();
            }

            int traineeRegistrationCount = 0;
            int organizationRegistrationCount = 0;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
 SELECT
     (SELECT COUNT(nID)
      FROM tblCandidateRegister) AS CandidateCount,

     (SELECT COUNT(nID)
      FROM tblOrgRegistration) AS OrganizationCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        traineeRegistrationCount =
                            Convert.ToInt32(dr["CandidateCount"]);

                        organizationRegistrationCount =
                            Convert.ToInt32(dr["OrganizationCount"]);
                    }
                }
            }



            List<SATraineeListM> trainees =
                _repository.GetAllTrainees();

            List<OrganizationUser> organizations =
                _repository.GetAllOrganizationList();

            ViewBag.TraineeRegistrationCount =
                traineeRegistrationCount;

            ViewBag.OrganizationRegistrationCount =
                organizationRegistrationCount;

            ViewBag.Trainees = trainees;

            ViewBag.Organizations = organizations;

            ViewBag.SuperAdminID = id;


            SAChartsViewModel saCharts = new SAChartsViewModel();

            using (SqlConnection chartCon = new SqlConnection(connectionString))
            {
                chartCon.Open();

                using (SqlCommand chartCmd =
                       new SqlCommand("SP_GetSACharts", chartCon))
                {
                    chartCmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader chartReader =
                           chartCmd.ExecuteReader())
                    {
                        // =====================================================
                        // RESULT SET 1 - CANDIDATE REGISTRATION
                        // =====================================================

                        while (chartReader.Read())
                        {
                            saCharts.CandidateData.Add(new SAChartData
                            {
                                Year = Convert.ToInt32(chartReader["Year"]),
                                Month = Convert.ToInt32(chartReader["Month"]),
                                TotalCount = Convert.ToInt32(chartReader["TotalCount"])
                            });
                        }


                        // =====================================================
                        // RESULT SET 2 - INTERNSHIP POSTS
                        // =====================================================

                        if (chartReader.NextResult())
                        {
                            while (chartReader.Read())
                            {
                                saCharts.InternshipPostData.Add(new SAChartData
                                {
                                    Year = Convert.ToInt32(chartReader["Year"]),
                                    Month = Convert.ToInt32(chartReader["Month"]),
                                    TotalCount = Convert.ToInt32(chartReader["TotalCount"])
                                });
                            }
                        }
                    }
                }
            }



            return View(saCharts);
        }

        // ==========================================
        // CANDIDATE LIST
        // ==========================================
        [HttpGet]
        [Route("SuperAdmin/SATrainees/{id:int}")]
        public IActionResult SATrainees()
        {
            List<SATraineeListM> trainees = _repository.GetSATraineeList();
            return View(trainees);
        }



        [HttpPost]
        public IActionResult ToggleCandidateStatus(int id)
        {
            try
            {
                bool result = _repository.ToggleCandidateStatus(id);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        status = _repository.GetCandidateStatus(id)
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Candidate status could not be updated."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }






        [HttpGet]
        [Route("SuperAdmin/SATraineeProfileList/{id:int}")]
        public IActionResult SATraineeProfileList()
        {
            var trainees = new List<SATraineeProfileListM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection is not configured."
                );

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                CP.nID,
                CP.CandidateID,

                -- Candidate Name
                CR.sFName,
                CR.sLName,

                -- Profile Details
                CP.RegDate,
                CP.Pincode,

                -- SSC
                CP.SSC_YEAR,
                CP.SSC_DIVISION,

                -- HSC / DIPLOMA
                CP.HSC_DIPLOMA_YEAR,
                CP.HSC_DIPLOMA_DIVISION,

                -- GRADUATION
                CP.Graduation_Year,
                CP.Graduation_Division,
                CP.Graduation_Stream,

                -- PG
                CP.PG_Year,
                CP.PG_Division,
                CP.PG_Stream,

                -- PHD
                CP.PhD_Year,
                CP.PhD_Status,
                CP.PhD_Topic,

                -- PREFERENCE
                CP.Preferred_Country,
                CP.Preferred_State,
                CP.Preferred_City,

                CP.Internship_FellowshipType,

                -- FILES
                CP.sPhoto,
                CP.sResume

            FROM tblCandidateProfile CP

            INNER JOIN tblCandidateRegister CR
                ON CP.CandidateID = CR.nID

            WHERE CP.nBit = 1
              AND CP.nSABit = 1

            ORDER BY CP.nID DESC;
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int srNo = 1;

                        while (reader.Read())
                        {
                            var trainee = new SATraineeProfileListM();

                            // =========================================
                            // SERIAL NUMBER
                            // =========================================

                            trainee.SrNo = srNo++;


                            // =========================================
                            // CANDIDATE ID
                            // =========================================

                            trainee.CandidateID =
                                reader["CandidateID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        reader["CandidateID"]
                                    );


                            // =========================================
                            // FIRST NAME
                            // =========================================

                            string firstName =
                                reader["sFName"] == DBNull.Value
                                    ? ""
                                    : reader["sFName"].ToString();


                            // =========================================
                            // LAST NAME
                            // =========================================

                            string lastName =
                                reader["sLName"] == DBNull.Value
                                    ? ""
                                    : reader["sLName"].ToString();


                            // =========================================
                            // FULL NAME
                            // =========================================

                            trainee.TraineeName =
                                $"{firstName} {lastName}".Trim();


                            // =========================================
                            // PROFILE CREATED DATE
                            // =========================================

                            trainee.ProfileCreatedDate =
                                reader["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(
                                        reader["RegDate"]
                                    );


                            // =========================================
                            // PHOTO
                            // =========================================

                            trainee.Photo =
                                reader["sPhoto"] == DBNull.Value
                                    ? ""
                                    : reader["sPhoto"].ToString();


                            // =========================================
                            // PINCODE
                            // =========================================

                            trainee.Pincode =
                                reader["Pincode"] == DBNull.Value
                                    ? ""
                                    : reader["Pincode"].ToString();


                            // =========================================
                            // SSC
                            // =========================================

                            trainee.SSC_YEAR =
                                reader["SSC_YEAR"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["SSC_YEAR"]
                                    );

                            trainee.SSC_DIVISION =
                                reader["SSC_DIVISION"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["SSC_DIVISION"]
                                    );


                            // =========================================
                            // HSC / DIPLOMA
                            // =========================================

                            trainee.HSC_DIPLOMA_YEAR =
                                reader["HSC_DIPLOMA_YEAR"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["HSC_DIPLOMA_YEAR"]
                                    );

                            trainee.HSC_DIPLOMA_DIVISION =
                                reader["HSC_DIPLOMA_DIVISION"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["HSC_DIPLOMA_DIVISION"]
                                    );


                            // =========================================
                            // GRADUATION
                            // =========================================

                            trainee.Graduation_Year =
                                reader["Graduation_Year"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["Graduation_Year"]
                                    );

                            trainee.Graduation_Division =
                                reader["Graduation_Division"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["Graduation_Division"]
                                    );

                            trainee.Graduation_Stream =
                                reader["Graduation_Stream"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["Graduation_Stream"]
                                    );


                            // =========================================
                            // PG
                            // =========================================

                            trainee.PG_Year =
                                reader["PG_Year"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["PG_Year"]
                                    );

                            trainee.PG_Division =
                                reader["PG_Division"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["PG_Division"]
                                    );

                            trainee.PG_Stream =
                                reader["PG_Stream"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["PG_Stream"]
                                    );


                            // =========================================
                            // PHD
                            // =========================================

                            trainee.PhD_Year =
                                reader["PhD_Year"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["PhD_Year"]
                                    );

                            trainee.PhD_Status =
                                reader["PhD_Status"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader["PhD_Status"]
                                    );

                            trainee.PhD_Topic =
                                reader["PhD_Topic"] == DBNull.Value
                                    ? ""
                                    : reader["PhD_Topic"].ToString();


                            // =========================================
                            // PREFERRED LOCATION
                            // =========================================

                            string country =
                                reader["Preferred_Country"] == DBNull.Value
                                    ? ""
                                    : reader["Preferred_Country"].ToString();

                            string state =
                                reader["Preferred_State"] == DBNull.Value
                                    ? ""
                                    : reader["Preferred_State"].ToString();

                            string city =
                                reader["Preferred_City"] == DBNull.Value
                                    ? ""
                                    : reader["Preferred_City"].ToString();

                            trainee.PreferredLocation =
                                string.Join(
                                    ", ",
                                    new[]
                                    {
                                city,
                                state,
                                country
                                    }
                                    .Where(x =>
                                        !string.IsNullOrWhiteSpace(x))
                                );


                            // =========================================
                            // INTERNSHIP / FELLOWSHIP
                            // =========================================

                            trainee.Internship_FellowshipType =
                                reader["Internship_FellowshipType"]
                                    == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        reader[
                                            "Internship_FellowshipType"
                                        ]
                                    );


                            // =========================================
                            // RESUME
                            // =========================================

                            trainee.Resume =
                                reader["sResume"] == DBNull.Value
                                    ? ""
                                    : reader["sResume"].ToString();


                            // =========================================
                            // RESUME UPLOADED
                            // =========================================

                            trainee.ResumeUploaded =
                                !string.IsNullOrWhiteSpace(
                                    trainee.Resume
                                );


                            // =========================================
                            // ADD TO LIST
                            // =========================================

                            trainees.Add(trainee);
                        }
                    }
                }
            }

            return View(trainees);
        }


        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        [HttpGet]
        [Route("SuperAdmin/SAOrganizations/{id:int}")]
        public IActionResult SAOrganizations()
        {
            List<OrganizationUser> organization = _repository.GetAllOrganizationList();
            return View(organization);
        }


        [HttpPost]
        [Route("SuperAdmin/ToggleOrganizationSABit")]
        public IActionResult ToggleOrganizationSABit(int id, bool status)
        {
            try
            {
                bool result = _repository.ToggleOrganizationSABit(id, status);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        status = status
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Organization status could not be updated."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public IActionResult SAOrgProfileList(int? id, bool? status)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            try
            {


                // =====================================================
                // GET ORGANIZATION LIST
                // =====================================================

                var orgList = new List<OrgProfileListM>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT
                    OP.nID,
                    OP.nOrgID,

                    ORG.sOrgName,
                    ORG.sEmail,

                    OP.sCompanyLogo,
                    OP.sCompanyAddress,
                    OP.sGSTNo,
                    OP.sCINNo,
                    OP.nEmployeeStrength,
                    OP.dDateOfBirth,
                    OP.nEstablishmentYear,
                    OP.dCreatedDate,
                    OP.nSABit

                FROM tblOrgProfile OP

                INNER JOIN tblOrgRegistration ORG
                    ON OP.nOrgID = ORG.nID

                WHERE ORG.nSABit = 1

                ORDER BY OP.nID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int srNo = 1;

                            while (reader.Read())
                            {
                                orgList.Add(new OrgProfileListM
                                {
                                    SrNo = srNo++,

                                    nID = reader["nID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["nID"])
                                        : 0,

                                    nOrgID = reader["nOrgID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["nOrgID"])
                                        : 0,

                                    ProfileCreationDate =
                                        reader["dCreatedDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["dCreatedDate"])
                                        : null,

                                    OrganizationName =
                                        reader["sOrgName"] != DBNull.Value
                                        ? reader["sOrgName"].ToString()
                                        : "",

                                    OrganizationLogo =
                                        reader["sCompanyLogo"] != DBNull.Value
                                        ? reader["sCompanyLogo"].ToString()
                                        : "",

                                    OrganizationAddress =
                                        reader["sCompanyAddress"] != DBNull.Value
                                        ? reader["sCompanyAddress"].ToString()
                                        : "",

                                    VerificationEmail =
                                        reader["sEmail"] != DBNull.Value
                                        ? reader["sEmail"].ToString()
                                        : "",

                                    GSTNo =
                                        reader["sGSTNo"] != DBNull.Value
                                        ? reader["sGSTNo"].ToString()
                                        : "",

                                    CINNo =
                                        reader["sCINNo"] != DBNull.Value
                                        ? reader["sCINNo"].ToString()
                                        : "",

                                    CurrentEmployeeStrength =
                                        reader["nEmployeeStrength"] != DBNull.Value
                                        ? reader["nEmployeeStrength"].ToString()
                                        : "",

                                    DOB =
                                        reader["dDateOfBirth"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["dDateOfBirth"])
                                        : null,

                                    EstablishmentYear =
                                        reader["nEstablishmentYear"] != DBNull.Value
                                        ? Convert.ToInt32(reader["nEstablishmentYear"])
                                        : null,

                                    nSABit =
                                        reader["nSABit"] != DBNull.Value &&
                                        Convert.ToBoolean(reader["nSABit"])
                                });
                            }
                        }
                    }
                }

                return View(orgList);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public IActionResult OrganizationProfileDetails(int id)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            try
            {
                var organizations = new List<OrgProfileListM>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT
                    OP.nID,
                    OP.nOrgID,

                    ORG.sOrgName,
                    ORG.sEmail,

                    OP.sCompanyLogo,
                    OP.sCompanyAddress,
                    OP.sGSTNo,
                    OP.sCINNo,
                    OP.nEmployeeStrength,
                    OP.dDateOfBirth,
                    OP.nEstablishmentYear,
                    OP.dCreatedDate,
                    OP.nSABit

                FROM tblOrgProfile OP

                INNER JOIN tblOrgRegistration ORG
                    ON OP.nOrgID = ORG.nID

                WHERE ORG.nSABit = 1

                ORDER BY OP.nID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int srNo = 1;

                            while (reader.Read())
                            {
                                organizations.Add(new OrgProfileListM
                                {
                                    SrNo = srNo++,

                                    nID = reader["nID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["nID"])
                                        : 0,

                                    nOrgID = reader["nOrgID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["nOrgID"])
                                        : 0,

                                    ProfileCreationDate =
                                        reader["dCreatedDate"] != DBNull.Value
                                            ? Convert.ToDateTime(reader["dCreatedDate"])
                                            : null,

                                    OrganizationName =
                                        reader["sOrgName"] != DBNull.Value
                                            ? reader["sOrgName"].ToString()
                                            : "",

                                    OrganizationLogo =
                                        reader["sCompanyLogo"] != DBNull.Value
                                            ? reader["sCompanyLogo"].ToString()
                                            : "",

                                    OrganizationAddress =
                                        reader["sCompanyAddress"] != DBNull.Value
                                            ? reader["sCompanyAddress"].ToString()
                                            : "",

                                    VerificationEmail =
                                        reader["sEmail"] != DBNull.Value
                                            ? reader["sEmail"].ToString()
                                            : "",

                                    GSTNo =
                                        reader["sGSTNo"] != DBNull.Value
                                            ? reader["sGSTNo"].ToString()
                                            : "",

                                    CINNo =
                                        reader["sCINNo"] != DBNull.Value
                                            ? reader["sCINNo"].ToString()
                                            : "",

                                    CurrentEmployeeStrength =
                                        reader["nEmployeeStrength"] != DBNull.Value
                                            ? reader["nEmployeeStrength"].ToString()
                                            : "",

                                    DOB =
                                        reader["dDateOfBirth"] != DBNull.Value
                                            ? Convert.ToDateTime(reader["dDateOfBirth"])
                                            : null,

                                    EstablishmentYear =
                                        reader["nEstablishmentYear"] != DBNull.Value
                                            ? Convert.ToInt32(reader["nEstablishmentYear"])
                                            : null,

                                    nSABit =
                                        reader["nSABit"] != DBNull.Value &&
                                        Convert.ToBoolean(reader["nSABit"])
                                });
                            }
                        }
                    }
                }

                // No organizations
                if (organizations.Count == 0)
                {
                    return NotFound();
                }

                // Find currently selected organization
                int currentIndex = organizations.FindIndex(x => x.nID == id);

                if (currentIndex == -1)
                {
                    return NotFound();
                }

                // Previous organization
                int? previousId = null;

                if (currentIndex < organizations.Count - 1)
                {
                    previousId = organizations[currentIndex + 1].nID;
                }

                // Next organization
                int? nextId = null;

                if (currentIndex > 0)
                {
                    nextId = organizations[currentIndex - 1].nID;
                }

                var model = new OrganizationProfileDetailsVM
                {
                    Organization = organizations[currentIndex],

                    CurrentIndex = currentIndex + 1,

                    TotalRecords = organizations.Count,

                    PreviousId = previousId,

                    NextId = nextId
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("SuperAdmin/AddObjective/{id:int}")]
        public IActionResult AddObjective()
        {
            return View();
        }

        [HttpGet]
        [Route("SuperAdmin/ViewObjective/{id:int}")]
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

        // sanidhya 02/10/26
        // =========================================================
        // EDIT OBJECTIVE - GET
        // URL:
        // /SuperAdmin/EditObjective/1004
        // =========================================================
        // sanidhya 02/10/26
        // =========================================================
        // EDIT OBJECTIVE - GET
        // URL:
        // /SuperAdmin/EditObjective/1004
        // =========================================================
        [HttpGet]
        [Route("SuperAdmin/EditObjective/{id:int}")]
        public IActionResult EditObjective(int id)
        {
            SAObjectiveM model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                string query = @"
    SELECT
        nID,
        nGenderID,
        sObjective,
        dCreatedDate,
        nBit,
        nSABit
    FROM tblObjective
    WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new SAObjectiveM
                            {
                                nID = reader["nID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["nID"])
                                    : 0,

                                nGenderID = reader["nGenderID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["nGenderID"])
                                    : 0,

                                sObjective = reader["sObjective"] != DBNull.Value
                                    ? reader["sObjective"].ToString()
                                    : "",

                                dCreatedDate = reader["dCreatedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["dCreatedDate"])
                                    : DateTime.Now,

                                nBit = reader["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(reader["nBit"]),

                                nSABit = reader["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(reader["nSABit"])
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

        // =========================================================
        // EDIT OBJECTIVE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditObjective(SAObjectiveM model)
        {
            if (model.nGenderID <= 0)
            {
                ModelState.AddModelError("nGenderID", "Please select gender.");
            }

            if (string.IsNullOrWhiteSpace(model.sObjective))
            {
                ModelState.AddModelError("sObjective", "Please enter objective.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();

                    string query = @"
        UPDATE tblObjective
        SET
            nGenderID = @nGenderID,
            sObjective = @sObjective
        WHERE nID = @nID";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.Add("@nID", SqlDbType.Int)
                            .Value = model.nID;

                        cmd.Parameters.Add("@nGenderID", SqlDbType.Int)
                            .Value = model.nGenderID;

                        cmd.Parameters.Add("@sObjective", SqlDbType.NVarChar, -1)
                            .Value = model.sObjective.Trim();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound();
                        }
                    }
                }

                TempData["SuccessMessage"] =
                    "Objective updated successfully.";

                return RedirectToAction("ViewObjective", new { id = 1 });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update objective: " + ex.Message);

                return View(model);
            }
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
        [Route("SuperAdmin/SAOrgPostList/{id:int}")]
        public IActionResult SAOrgPostList()
        {
            List<OrgPostM> posts = new List<OrgPostM>();

            string query = @"
        SELECT
            P.nID,

            P.nPositionID,
            POS.sName AS sPositionName,

            P.nRequiredTrainees,

            P.nGenderID,
            G.sName AS sGenderName,

            P.nMinimumQualificationID,
            Q.sName AS sMinimumQualificationName,

            P.sCountryName,
            P.sStateName,
            P.nCityName,

            P.sWorkingHours,

            P.nInternshipTypeID,
            IT.sName AS sInternshipTypeName,

            P.sWorkingShift,

            P.nInternshipFellowshipTypeID,
            IFT.sName AS sInternshipFellowshipTypeName,

            P.sTotalCharges,
            P.sCurrency,

            P.nTrainingInvolvedID,
            TI.sName AS sTrainingInvolvedName,

            P.nInternshipDurationID,
            D.sName AS sInternshipDurationName,

            P.dStartDate,
            P.dCompletionDate,

            P.nInternshipModeID,
            IM.sName AS sInternshipModeName,

            P.sDivyang,
            P.sLanguageKnown,

            P.sWorkingDays,
            P.sFacilities,

            P.sTechnicalSkills,
            P.sMedicalSkills,
            P.sNonTechnicalSkills,

            P.dRegisterDate,
            P.dModDate,

            P.nBit,
            P.nSABit,
            P.nOrgID

        FROM tblPost P

        LEFT JOIN tblPositions POS
            ON P.nPositionID = POS.nID

        LEFT JOIN tblGender G
            ON P.nGenderID = G.nID

        LEFT JOIN tblMinimumQualification Q
            ON P.nMinimumQualificationID = Q.nID

        LEFT JOIN tblInternshipType IT
            ON P.nInternshipTypeID = IT.nID

        LEFT JOIN tblInternshipFellowshipType IFT
            ON P.nInternshipFellowshipTypeID = IFT.nID

        LEFT JOIN tblTrainingInvolved TI
            ON P.nTrainingInvolvedID = TI.nID

        LEFT JOIN tblInternshipDuration D
            ON P.nInternshipDurationID = D.nID

        LEFT JOIN tblInternshipMode IM
            ON P.nInternshipModeID = IM.nID

        ORDER BY P.nID DESC;
    ";

            using (SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM item = new OrgPostM();

                            item.nID = dr["nID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nID"])
                                : 0;

                            item.nPositionID = dr["nPositionID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nPositionID"])
                                : 0;

                            item.sPositionName =
                                dr["sPositionName"]?.ToString() ?? "";

                            item.nRequiredTrainees =
                                dr["nRequiredTrainees"] != DBNull.Value
                                ? Convert.ToInt32(dr["nRequiredTrainees"])
                                : 0;

                            item.nGenderID =
                                dr["nGenderID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nGenderID"])
                                : 0;

                            item.sGenderName =
                                dr["sGenderName"]?.ToString() ?? "";

                            item.nMinimumQualificationID =
                                dr["nMinimumQualificationID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nMinimumQualificationID"])
                                : 0;

                            item.sMinimumQualificationName =
                                dr["sMinimumQualificationName"]?.ToString() ?? "";

                            item.sCountryName =
                                dr["sCountryName"]?.ToString() ?? "";

                            item.sStateName =
                                dr["sStateName"]?.ToString() ?? "";

                            item.nCityName =
                                dr["nCityName"]?.ToString() ?? "";

                            item.sWorkingHours =
                                dr["sWorkingHours"]?.ToString() ?? "";

                            item.nInternshipTypeID =
                                dr["nInternshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipTypeID"])
                                : 0;

                            item.sInternshipTypeName =
                                dr["sInternshipTypeName"]?.ToString() ?? "";

                            item.sWorkingShift =
                                dr["sWorkingShift"]?.ToString() ?? "";

                            item.nInternshipFellowshipTypeID =
                                dr["nInternshipFellowshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipFellowshipTypeID"])
                                : 0;

                            item.sInternshipFellowshipTypeName =
                                dr["sInternshipFellowshipTypeName"]?.ToString() ?? "";

                            item.sTotalCharges =
                                dr["sTotalCharges"] != DBNull.Value
                                ? Convert.ToDecimal(dr["sTotalCharges"])
                                : null;

                            item.sCurrency =
                                dr["sCurrency"]?.ToString() ?? "";

                            item.nTrainingInvolvedID =
                                dr["nTrainingInvolvedID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nTrainingInvolvedID"])
                                : 0;

                            item.sTrainingInvolvedName =
                                dr["sTrainingInvolvedName"]?.ToString() ?? "";

                            item.nInternshipDurationID =
                                dr["nInternshipDurationID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipDurationID"])
                                : 0;

                            item.sInternshipDurationName =
                                dr["sInternshipDurationName"]?.ToString() ?? "";

                            item.dStartDate =
                                dr["dStartDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dStartDate"])
                                : DateTime.MinValue;

                            item.dCompletionDate =
                                dr["dCompletionDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dCompletionDate"])
                                : DateTime.MinValue;

                            item.nInternshipModeID =
                                dr["nInternshipModeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipModeID"])
                                : 0;

                            item.sInternshipModeName =
                                dr["sInternshipModeName"]?.ToString() ?? "";

                            item.sDivyang =
                                dr["sDivyang"]?.ToString() ?? "";

                            item.sLanguageKnown =
                                dr["sLanguageKnown"]?.ToString() ?? "";

                            item.sWorkingDays =
                                dr["sWorkingDays"]?.ToString() ?? "";

                            item.sFacilities =
                                dr["sFacilities"]?.ToString() ?? "";

                            item.sTechnicalSkills =
                                dr["sTechnicalSkills"]?.ToString() ?? "";

                            item.sMedicalSkills =
                                dr["sMedicalSkills"]?.ToString() ?? "";

                            item.sNonTechnicalSkills =
                                dr["sNonTechnicalSkills"]?.ToString() ?? "";

                            item.dRegisterDate =
                                dr["dRegisterDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dRegisterDate"])
                                : null;

                            item.dModDate =
                                dr["dModDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dModDate"])
                                : null;

                            item.nBit =
                                dr["nBit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nBit"]);

                            item.nSABit =
                                dr["nSABit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nSABit"]);

                            item.nOrgID =
                                dr["nOrgID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nOrgID"])
                                : 0;

                            posts.Add(item);
                        }
                    }
                }
            }

            return View(posts);
        }




        [HttpPost]
        public IActionResult ToggleOrganizationStatus(int id, bool status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = @"
                UPDATE tblOrgProfile
                SET nSABit = @nSABit
                WHERE nID = @nID
            ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@nSABit", SqlDbType.Bit).Value = status;
                        cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                        con.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Organization profile not found."
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    status = status
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //[HttpGet]
        //[Route("SuperAdmin/AddObjective/{id:int}")]
        //public IActionResult AddObjective()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[Route("SuperAdmin/ViewObjective/{id:int}")]
        //public IActionResult ViewObjective()
        //{
        //    List<SAObjectiveM> objectives = new List<SAObjectiveM>();

        //    string connectionString =
        //        _configuration.GetConnectionString("DefaultConnection");

        //    // Get Objectives
        //    using (SqlConnection cn = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SP_GetObjective", cn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cn.Open();

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    objectives.Add(new SAObjectiveM
        //                    {
        //                        nID = Convert.ToInt32(reader["nID"]),
        //                        nGenderID = Convert.ToInt32(reader["nGenderID"]),
        //                        sObjective = reader["sObjective"]?.ToString() ?? "",
        //                        dCreatedDate = Convert.ToDateTime(reader["dCreatedDate"]),
        //                        nBit = Convert.ToBoolean(reader["nBit"]),
        //                        nSABit = Convert.ToBoolean(reader["nSABit"])
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    // Get Gender Names
        //    Dictionary<int, string> genderNames =
        //        new Dictionary<int, string>();

        //    using (SqlConnection cn = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SP_GetGender", cn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cn.Open();

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int id = Convert.ToInt32(reader["nID"]);
        //                    string name = reader["sName"]?.ToString() ?? "";

        //                    genderNames[id] = name;
        //                }
        //            }
        //        }
        //    }

        //    ViewBag.GenderNames = genderNames;

        //    return View(objectives);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddObjective(SAObjectiveM model)
        //{
        //    if (model.nGenderID <= 0)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Please select gender."
        //        });
        //    }

        //    if (string.IsNullOrWhiteSpace(model.sObjective))
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Please enter objective."
        //        });
        //    }

        //    try
        //    {
        //        string connectionString =
        //            _configuration.GetConnectionString("DefaultConnection");

        //        using (SqlConnection cn = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand cmd =
        //                   new SqlCommand("SP_AddObjective", cn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                cmd.Parameters.Add("@nGenderID", SqlDbType.Int)
        //                    .Value = model.nGenderID;

        //                cmd.Parameters.Add("@sObjective", SqlDbType.NVarChar)
        //                    .Value = model.sObjective.Trim();

        //                await cn.OpenAsync();

        //                await cmd.ExecuteNonQueryAsync();
        //            }
        //        }

        //        return Json(new
        //        {
        //            success = true,
        //            message = "Objective added successfully."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Error: " + ex.Message
        //        });
        //    }
        //}


        //// ==========================================
        //// CHANGE OBJECTIVE STATUS
        //// ==========================================
        //[HttpPost]
        //public async Task<IActionResult> ChangeStatus(int id, bool status)
        //{
        //    try
        //    {
        //        string connectionString =
        //            _configuration.GetConnectionString("DefaultConnection");

        //        using (SqlConnection cn = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand cmd =
        //                   new SqlCommand("SP_ChangeObjectiveStatus", cn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                cmd.Parameters.Add("@nID", SqlDbType.Int)
        //                    .Value = id;

        //                cmd.Parameters.Add("@nBit", SqlDbType.Bit)
        //                    .Value = status;

        //                await cn.OpenAsync();

        //                await cmd.ExecuteNonQueryAsync();
        //            }
        //        }

        //        return Json(new
        //        {
        //            success = true,
        //            message = "Status changed successfully."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Error: " + ex.Message
        //        });
        //    }
        //}


        [HttpGet]
        [Route("SuperAdmin/SAFeedback/{id:int}")]
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
        [Route("SuperAdmin/EditSAFeedback/{id:int}")]
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
        [Route("SuperAdmin/DetailsSAFeedback/{id:int}")]
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
        [Route("SuperAdmin/SAOrgFeedback/{id:int}")]
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
        [Route("SuperAdmin/EditSAOrgFeedback/{id:int}")]
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
        [Route("SuperAdmin/DetailsSAOrgFeedback/{id:int}")]
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

        [HttpGet]
        [Route("SuperAdmin/CandidateFeedback/{id:int}")]
        //        public IActionResult CandidateFeedback()
        //        {
        //            List<SACandidateFeedbackM> feedbackList = new List<SACandidateFeedbackM>();

        //            string connectionString =
        //                _configuration.GetConnectionString("DefaultConnection");

        //            using (SqlConnection con = new SqlConnection(connectionString))
        //            {
        //                con.Open();

        //                string query = @"
        //WITH LatestFeedback AS
        //(
        //    SELECT
        //        CF.nID AS FeedbackID,
        //        CF.nAdminID,
        //        CF.nSABit,
        //        ROW_NUMBER() OVER
        //        (
        //            PARTITION BY CF.nAdminID
        //            ORDER BY CF.nID DESC
        //        ) AS RowNum
        //    FROM tblCandidateFeedback CF
        //)

        //SELECT
        //    CR.nID,
        //    CR.sFName,
        //    CR.sLName,
        //    LF.FeedbackID,
        //    LF.nAdminID,
        //    ISNULL(LF.nSABit, 0) AS nSABit

        //FROM tblCandidateRegister CR

        //INNER JOIN LatestFeedback LF
        //    ON CR.nID = LF.nAdminID

        //WHERE LF.RowNum = 1

        //ORDER BY LF.FeedbackID DESC";

        //                using (SqlCommand cmd = new SqlCommand(query, con))
        //                {
        //                    using (SqlDataReader dr = cmd.ExecuteReader())
        //                    {
        //                        while (dr.Read())
        //                        {
        //                            SACandidateFeedbackM model = new SACandidateFeedbackM();

        //                            model.nID = Convert.ToInt32(dr["nID"]);
        //                            model.sFName = dr["sFName"]?.ToString();
        //                            model.sLName = dr["sLName"]?.ToString();

        //                            if (dr["nAdminID"] != DBNull.Value)
        //                            {
        //                                model.nAdminID = Convert.ToInt32(dr["nAdminID"]);
        //                            }

        //                            model.nSABit = Convert.ToInt32(dr["nSABit"]);

        //                            model.Status =
        //                                model.nSABit == 1 ? "Enabled" : "Disabled";

        //                            feedbackList.Add(model);
        //                        }
        //                    }
        //                }
        //            }

        //            return View(feedbackList);
        //        }


        [HttpGet]
        [Route("SuperAdmin/CandidateFeedback")]
        public IActionResult CandidateFeedback()
        {
            List<SACandidateFeedbackM> feedbackList =
                new List<SACandidateFeedbackM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
WITH LatestFeedback AS
(
    SELECT
        CF.nID AS FeedbackID,
        CF.nAdminID,
        CF.nSABit,

        ROW_NUMBER() OVER
        (
            PARTITION BY CF.nAdminID
            ORDER BY CF.nID DESC
        ) AS RowNum

    FROM tblCandidateFeedback CF
)

SELECT
    CR.nID,
    CR.sFName,
    CR.sLName,

    LF.FeedbackID,
    LF.nAdminID,

    ISNULL(LF.nSABit, 0) AS nSABit

FROM tblCandidateRegister CR

INNER JOIN LatestFeedback LF
    ON CR.nID = LF.nAdminID

WHERE LF.RowNum = 1

ORDER BY LF.FeedbackID DESC";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SACandidateFeedbackM model =
                                new SACandidateFeedbackM();


                            // Candidate ID
                            model.nID =
                                Convert.ToInt32(
                                    dr["nID"]);


                            // Candidate Name
                            model.sFName =
                                dr["sFName"]?.ToString();

                            model.sLName =
                                dr["sLName"]?.ToString();


                            // Actual Feedback ID
                            model.FeedbackID =
                                Convert.ToInt32(
                                    dr["FeedbackID"]);


                            // Candidate ID stored in feedback
                            if (dr["nAdminID"] != DBNull.Value)
                            {
                                model.nAdminID =
                                    Convert.ToInt32(
                                        dr["nAdminID"]);
                            }


                            // Super Admin status
                            model.nSABit =
                                Convert.ToInt32(
                                    dr["nSABit"]);


                            model.Status =
                                model.nSABit == 1
                                ? "Enabled"
                                : "Disabled";


                            feedbackList.Add(model);
                        }
                    }
                }
            }

            return View(feedbackList);
        }
        //Enable / Disable Method
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public IActionResult ToggleCandidateFeedback(int id)
        {
            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            UPDATE tblCandidateFeedback
            SET
                nSABit =
                    CASE
                        WHEN ISNULL(nSABit, 0) = 1
                            THEN 0
                        ELSE 1
                    END,

                ModDate = GETDATE()

            WHERE nID = @FeedbackID";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@FeedbackID",
                        SqlDbType.Int).Value = id;


                    int result =
                        cmd.ExecuteNonQuery();


                    if (result == 0)
                    {
                        return BadRequest();
                    }
                }
            }

            return Ok();
        }


        [HttpGet]
        [Route("SuperAdmin/DetailsCandidateFeedback/{id:int}")]
        public IActionResult DetailsCandidateFeedback(int id)
        {
            SACandidateFeedbackDetailsM model =
                new SACandidateFeedbackDetailsM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT
                CR.nID,
                CR.sFName,
                CR.sLName,
                CR.sProfileImage,

                CF.nAdminID,
                CF.sQue1,
                CF.sQue2,
                CF.sQue3,
                CF.sQue4,
                CF.sQue5,
                CF.nSABit,
                CF.RegDate,
                CF.ModDate

            FROM tblCandidateRegister CR

            INNER JOIN tblCandidateFeedback CF
                ON CR.nID = CF.nAdminID

            WHERE CR.nID = @nID
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Candidate details
                            model.nID =
                                Convert.ToInt32(dr["nID"]);

                            model.sFName =
                                dr["sFName"] == DBNull.Value
                                ? ""
                                : dr["sFName"].ToString();

                            model.sLName =
                                dr["sLName"] == DBNull.Value
                                ? ""
                                : dr["sLName"].ToString();

                            model.sProfileImage =
                                dr["sProfileImage"] == DBNull.Value
                                ? ""
                                : dr["sProfileImage"].ToString();


                            // Feedback details
                            model.nAdminID =
                                Convert.ToInt32(dr["nAdminID"]);

                            model.sQue1 =
                                Convert.ToInt32(dr["sQue1"]);

                            model.sQue2 =
                                Convert.ToInt32(dr["sQue2"]);

                            model.sQue3 =
                                Convert.ToInt32(dr["sQue3"]);

                            model.sQue4 =
                                Convert.ToInt32(dr["sQue4"]);

                            model.sQue5 =
                                dr["sQue5"] == DBNull.Value
                                ? ""
                                : dr["sQue5"].ToString();


                            // Super Admin status
                            model.nSABit =
                                dr["nSABit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nSABit"]);


                            // Dates
                            if (dr["RegDate"] != DBNull.Value)
                            {
                                model.RegDate =
                                    Convert.ToDateTime(dr["RegDate"]);
                            }

                            if (dr["ModDate"] != DBNull.Value)
                            {
                                model.ModDate =
                                    Convert.ToDateTime(dr["ModDate"]);
                            }
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


        [HttpGet]
        [Route("SuperAdmin/OrganizationFeedback/{id:int}")]
        public IActionResult OrganizationFeedback()
        {
            List<SAOrganizationFeedbackM> feedbackList =
                new List<SAOrganizationFeedbackM>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
WITH LatestFeedback AS
(
    SELECT
        F.nID AS FeedbackID,
        F.nSAID,
        F.nSABit,

        ROW_NUMBER() OVER
        (
            PARTITION BY F.nSAID
            ORDER BY F.nID DESC
        ) AS RowNum

    FROM tblSAOrgFeedback F
)

SELECT
    O.nID,
    O.sOrgName,
    LF.FeedbackID,
    ISNULL(LF.nSABit, 0) AS nSABit

FROM tblOrgRegistration O

INNER JOIN LatestFeedback LF
    ON O.nID = LF.nSAID

WHERE LF.RowNum = 1

ORDER BY LF.FeedbackID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SAOrganizationFeedbackM model =
                                new SAOrganizationFeedbackM();

                            model.nID =
                                Convert.ToInt32(dr["nID"]);

                            model.sOrgName =
                                dr["sOrgName"] == DBNull.Value
                                    ? ""
                                    : dr["sOrgName"].ToString();

                            model.nSABit =
                                Convert.ToInt32(dr["nSABit"]);

                            model.Status =
                                model.nSABit == 1
                                    ? "Enabled"
                                    : "Disabled";

                            feedbackList.Add(model);
                        }
                    }
                }
            }

            return View(feedbackList);
        }


        // Enable / Disable Organization Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleOrganizationFeedback(int id)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
       UPDATE tblSAOrgFeedback
       SET 
           nSABit = CASE
                       WHEN ISNULL(nSABit, 0) = 1 THEN 0
                       ELSE 1
                    END,
           dModDate = GETDATE()
       WHERE nID = @id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int result = cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        return BadRequest(
                            "Organization feedback record not found."
                        );
                    }
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("SuperAdmin/DetailsOrgFeedback/{id:int}")]
        public IActionResult DetailsOrgFeedback(int id)
        {
            SAOrgFeedbackDetailsM model = new SAOrgFeedbackDetailsM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
SELECT TOP 1

    /* ================= ORGANIZATION ================= */
    ORG.nID,
    ORG.sOrgName,

    /* ================= ORGANIZATION PROFILE ================= */
    OP.sCompanyLogo,

    /* ================= SA FEEDBACK QUESTIONS ================= */
    SAF.nID AS FeedbackID,
    SAF.nSAID,

    SAF.Que1,
    SAF.Que2,
    SAF.Que3,
    SAF.Que4,
    SAF.Que5,

    /* ================= ORGANIZATION ANSWERS ================= */
    OFB.sQue1 AS Ans1,
    OFB.sQue2 AS Ans2,
    OFB.sQue3 AS Ans3,
    OFB.sQue4 AS Ans4,
    OFB.sQue5 AS Ans5,

    /* ================= STATUS ================= */
    OFB.nSABit,

    /* ================= DATES ================= */
    OFB.RegDate,
    OFB.ModDate

FROM tblOrgRegistration ORG

/* Organization Profile */
LEFT JOIN tblOrgProfile OP
    ON OP.nOrgID = ORG.nID

/* SA defined feedback questions */
INNER JOIN tblSAOrgFeedback SAF
    ON SAF.nSAID = ORG.nID

/* Organization submitted answers */
INNER JOIN tblOrgFeedback OFB
    ON OFB.nAdminID = ORG.nID

WHERE ORG.nID = @OrgID

ORDER BY SAF.nID DESC, OFB.nID DESC;
";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@OrgID", SqlDbType.Int).Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            return NotFound();
                        }

                        // =========================================
                        // ORGANIZATION
                        // =========================================

                        model.nID = dr["nID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["nID"]);

                        model.sOrgName = dr["sOrgName"] == DBNull.Value
                            ? ""
                            : dr["sOrgName"].ToString();

                        // =========================================
                        // ORGANIZATION LOGO
                        // =========================================

                        model.sOrgLogo = dr["sCompanyLogo"] == DBNull.Value
                            ? ""
                            : dr["sCompanyLogo"].ToString();

                        // =========================================
                        // FEEDBACK ID
                        // =========================================

                        model.FeedbackID = dr["FeedbackID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["FeedbackID"]);

                        model.nSAID = dr["nSAID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["nSAID"]);

                        // =========================================
                        // QUESTION 1
                        // Question comes from tblSAOrgFeedback
                        // Answer comes from tblOrgFeedback
                        // =========================================

                        model.sQue1 = dr["Que1"] == DBNull.Value
                            ? ""
                            : dr["Que1"].ToString();

                        model.sAns1 = dr["Ans1"] == DBNull.Value
                            ? ""
                            : dr["Ans1"].ToString();

                        // =========================================
                        // QUESTION 2
                        // =========================================

                        model.sQue2 = dr["Que2"] == DBNull.Value
                            ? ""
                            : dr["Que2"].ToString();

                        model.sAns2 = dr["Ans2"] == DBNull.Value
                            ? ""
                            : dr["Ans2"].ToString();

                        // =========================================
                        // QUESTION 3
                        // =========================================

                        model.sQue3 = dr["Que3"] == DBNull.Value
                            ? ""
                            : dr["Que3"].ToString();

                        model.sAns3 = dr["Ans3"] == DBNull.Value
                            ? ""
                            : dr["Ans3"].ToString();

                        // =========================================
                        // QUESTION 4
                        // =========================================

                        model.sQue4 = dr["Que4"] == DBNull.Value
                            ? ""
                            : dr["Que4"].ToString();

                        model.sAns4 = dr["Ans4"] == DBNull.Value
                            ? ""
                            : dr["Ans4"].ToString();

                        // =========================================
                        // QUESTION 5
                        // =========================================

                        model.sQue5 = dr["Que5"] == DBNull.Value
                            ? ""
                            : dr["Que5"].ToString();

                        model.sAns5 = dr["Ans5"] == DBNull.Value
                            ? ""
                            : dr["Ans5"].ToString();

                        // =========================================
                        // STATUS
                        // =========================================

                        model.nSABit =
                            dr["nSABit"] != DBNull.Value &&
                            Convert.ToBoolean(dr["nSABit"]);

                        // =========================================
                        // REG DATE
                        // =========================================

                        if (dr["RegDate"] != DBNull.Value)
                        {
                            model.RegDate =
                                Convert.ToDateTime(dr["RegDate"]);
                        }

                        // =========================================
                        // MOD DATE
                        // =========================================

                        if (dr["ModDate"] != DBNull.Value)
                        {
                            model.ModDate =
                                Convert.ToDateTime(dr["ModDate"]);
                        }
                    }
                }
            }

            return View(model);
        }


        [HttpGet]
        [Route("SuperAdmin/ContactList/{id:int}")]
        public IActionResult ContactList()
        {
            List<ViewModelTrainee> contactList = new List<ViewModelTrainee>();

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
         SELECT
             FullName,
             Email,
             MobileNo,
             Subject,
             Description
         FROM tblContactUs
         ORDER BY nID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ViewModelTrainee model = new ViewModelTrainee
                                {
                                    FullName = reader["FullName"] == DBNull.Value
                                        ? ""
                                        : reader["FullName"].ToString(),

                                    Email = reader["Email"] == DBNull.Value
                                        ? ""
                                        : reader["Email"].ToString(),

                                    MobileNo = reader["MobileNo"] == DBNull.Value
                                        ? ""
                                        : reader["MobileNo"].ToString(),

                                    Subject = reader["Subject"] == DBNull.Value
                                        ? ""
                                        : reader["Subject"].ToString(),

                                    Description = reader["Description"] == DBNull.Value
                                        ? ""
                                        : reader["Description"].ToString()
                                };

                                contactList.Add(model);
                            }
                        }
                    }
                }

                return View(contactList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load contact list: " + ex.Message;

                return View(contactList);
            }
        }


        //Radhika26-09

        [HttpGet]
        public IActionResult TraineeReg()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SATraineeSelection()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SADetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAOrganizationList()
        {
            return View();
        }
        [HttpGet]
        [Route("SuperAdmin/SAAllOrganizationPost")]
        public IActionResult SAAllOrganizationPost()
        {
            List<OrgPostM> posts = new List<OrgPostM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SP_GetSAOrgPostsWithMatchingTraineeCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM item = new OrgPostM();

                            item.nID = dr["nID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(dr["nID"]);

                            item.nPositionID = dr["nPositionID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(dr["nPositionID"]);

                            item.sPositionName =
                                dr["sPositionName"]?.ToString() ?? "";

                            item.nRequiredTrainees =
                                dr["nRequiredTrainees"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nRequiredTrainees"]);

                            item.MatchingTraineeCount =
                                dr["MatchingTraineeCount"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["MatchingTraineeCount"]);

                            item.nGenderID =
                                dr["nGenderID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nGenderID"]);

                            item.sGenderName =
                                dr["sGenderName"]?.ToString() ?? "";

                            item.nMinimumQualificationID =
                                dr["nMinimumQualificationID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nMinimumQualificationID"]);

                            item.sMinimumQualificationName =
                                dr["sMinimumQualificationName"]?.ToString() ?? "";

                            item.sCountryName =
                                dr["sCountryName"]?.ToString() ?? "";

                            item.sStateName =
                                dr["sStateName"]?.ToString() ?? "";

                            item.nCityName =
                                dr["nCityName"]?.ToString() ?? "";

                            item.sWorkingHours =
                                dr["sWorkingHours"]?.ToString() ?? "";

                            item.nInternshipTypeID =
                                dr["nInternshipTypeID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nInternshipTypeID"]);

                            item.sInternshipTypeName =
                                dr["sInternshipTypeName"]?.ToString() ?? "";

                            item.sWorkingShift =
                                dr["sWorkingShift"]?.ToString() ?? "";

                            item.nInternshipFellowshipTypeID =
                                dr["nInternshipFellowshipTypeID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nInternshipFellowshipTypeID"]);

                            item.sInternshipFellowshipTypeName =
                                dr["sInternshipFellowshipTypeName"]?.ToString() ?? "";

                            item.nTrainingInvolvedID =
                                dr["nTrainingInvolvedID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nTrainingInvolvedID"]);

                            item.sTrainingInvolvedName =
                                dr["sTrainingInvolvedName"]?.ToString() ?? "";

                            item.nInternshipDurationID =
                                dr["nInternshipDurationID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nInternshipDurationID"]);

                            item.sInternshipDurationName =
                                dr["sInternshipDurationName"]?.ToString() ?? "";

                            item.nInternshipModeID =
                                dr["nInternshipModeID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nInternshipModeID"]);

                            item.sInternshipModeName =
                                dr["sInternshipModeName"]?.ToString() ?? "";

                            item.sDivyang =
                                dr["sDivyang"]?.ToString() ?? "";

                            item.sWorkingDays =
                                dr["sWorkingDays"]?.ToString() ?? "";

                            item.sFacilities =
                                dr["sFacilities"]?.ToString() ?? "";

                            item.dRegisterDate =
                                dr["dRegisterDate"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(dr["dRegisterDate"]);

                                                        item.dModDate =
                                dr["dModDate"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(dr["dModDate"]);

                            item.nBit =
                                dr["nBit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nBit"]);

                            item.nSABit =
                                dr["nSABit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nSABit"]);

                            item.nOrgID =
                                dr["nOrgID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nOrgID"]);

                            posts.Add(item);
                        }
                    }
                }
            }

            return View(posts);
        }

        // khushi 03-10-26
        [HttpPost]
        [Route("SuperAdmin/ToggleSAAllOrganizationPostStatus")]
        public void ToggleSAAllOrganizationPostStatus(int id)
        {
            using (SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(@"
      UPDATE tblPost
      SET nBit = CASE
                  WHEN ISNULL(nBit, 0) = 1 THEN 0
                  ELSE 1
                 END
      WHERE nID = @nID", con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public IActionResult ViewOrgPost(int id)
        {
            List<OrgPostM> posts = new List<OrgPostM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            string query = @"
        SELECT
            P.nID,

            P.nPositionID,
            POS.sName AS sPositionName,

            P.nRequiredTrainees,

            P.nGenderID,
            G.sName AS sGenderName,

            P.nMinimumQualificationID,
            Q.sName AS sMinimumQualificationName,

            P.sCountryName,
            P.sStateName,
            P.nCityName,

            P.sWorkingHours,

            P.nInternshipTypeID,
            IT.sName AS sInternshipTypeName,

            P.sWorkingShift,

            P.nInternshipFellowshipTypeID,
            IFT.sName AS sInternshipFellowshipTypeName,

            P.sTotalCharges,
            P.sCurrency,

            P.nTrainingInvolvedID,
            TI.sName AS sTrainingInvolvedName,

            P.nInternshipDurationID,
            D.sName AS sInternshipDurationName,

            P.dStartDate,
            P.dCompletionDate,

            P.nInternshipModeID,
            IM.sName AS sInternshipModeName,

            P.sDivyang,
            P.sLanguageKnown,

            P.sWorkingDays,
            P.sFacilities,

            P.sTechnicalSkills,
            P.sMedicalSkills,
            P.sNonTechnicalSkills,

            P.dRegisterDate,
            P.dModDate,

            P.nBit,
            P.nSABit,
            P.nOrgID

        FROM tblPost P

        LEFT JOIN tblPositions POS
            ON P.nPositionID = POS.nID

        LEFT JOIN tblGender G
            ON P.nGenderID = G.nID

        LEFT JOIN tblMinimumQualification Q
            ON P.nMinimumQualificationID = Q.nID

        LEFT JOIN tblInternshipType IT
            ON P.nInternshipTypeID = IT.nID

        LEFT JOIN tblInternshipFellowshipType IFT
            ON P.nInternshipFellowshipTypeID = IFT.nID

        LEFT JOIN tblTrainingInvolved TI
            ON P.nTrainingInvolvedID = TI.nID

        LEFT JOIN tblInternshipDuration D
            ON P.nInternshipDurationID = D.nID

        LEFT JOIN tblInternshipMode IM
            ON P.nInternshipModeID = IM.nID

        ORDER BY P.nID DESC;
    ";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrgPostM item = new OrgPostM();

                            item.nID = dr["nID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nID"])
                                : 0;

                            item.nPositionID = dr["nPositionID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nPositionID"])
                                : 0;

                            item.sPositionName =
                                dr["sPositionName"]?.ToString() ?? "";

                            item.nRequiredTrainees =
                                dr["nRequiredTrainees"] != DBNull.Value
                                ? Convert.ToInt32(dr["nRequiredTrainees"])
                                : 0;

                            item.nGenderID =
                                dr["nGenderID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nGenderID"])
                                : 0;

                            item.sGenderName =
                                dr["sGenderName"]?.ToString() ?? "";

                            item.nMinimumQualificationID =
                                dr["nMinimumQualificationID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nMinimumQualificationID"])
                                : 0;

                            item.sMinimumQualificationName =
                                dr["sMinimumQualificationName"]?.ToString() ?? "";

                            item.sCountryName =
                                dr["sCountryName"]?.ToString() ?? "";

                            item.sStateName =
                                dr["sStateName"]?.ToString() ?? "";

                            item.nCityName =
                                dr["nCityName"]?.ToString() ?? "";

                            item.sWorkingHours =
                                dr["sWorkingHours"]?.ToString() ?? "";

                            item.nInternshipTypeID =
                                dr["nInternshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipTypeID"])
                                : 0;

                            item.sInternshipTypeName =
                                dr["sInternshipTypeName"]?.ToString() ?? "";

                            item.sWorkingShift =
                                dr["sWorkingShift"]?.ToString() ?? "";

                            item.nInternshipFellowshipTypeID =
                                dr["nInternshipFellowshipTypeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipFellowshipTypeID"])
                                : 0;

                            item.sInternshipFellowshipTypeName =
                                dr["sInternshipFellowshipTypeName"]?.ToString() ?? "";

                            item.sTotalCharges =
                                dr["sTotalCharges"] != DBNull.Value
                                ? Convert.ToDecimal(dr["sTotalCharges"])
                                : null;

                            item.sCurrency =
                                dr["sCurrency"]?.ToString() ?? "";

                            item.nTrainingInvolvedID =
                                dr["nTrainingInvolvedID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nTrainingInvolvedID"])
                                : 0;

                            item.sTrainingInvolvedName =
                                dr["sTrainingInvolvedName"]?.ToString() ?? "";

                            item.nInternshipDurationID =
                                dr["nInternshipDurationID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipDurationID"])
                                : 0;

                            item.sInternshipDurationName =
                                dr["sInternshipDurationName"]?.ToString() ?? "";

                            item.dStartDate =
                                dr["dStartDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dStartDate"])
                                : DateTime.MinValue;

                            item.dCompletionDate =
                                dr["dCompletionDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dCompletionDate"])
                                : DateTime.MinValue;

                            item.nInternshipModeID =
                                dr["nInternshipModeID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nInternshipModeID"])
                                : 0;

                            item.sInternshipModeName =
                                dr["sInternshipModeName"]?.ToString() ?? "";

                            item.sDivyang =
                                dr["sDivyang"]?.ToString() ?? "";

                            item.sLanguageKnown =
                                dr["sLanguageKnown"]?.ToString() ?? "";

                            item.sWorkingDays =
                                dr["sWorkingDays"]?.ToString() ?? "";

                            item.sFacilities =
                                dr["sFacilities"]?.ToString() ?? "";

                            item.sTechnicalSkills =
                                dr["sTechnicalSkills"]?.ToString() ?? "";

                            item.sMedicalSkills =
                                dr["sMedicalSkills"]?.ToString() ?? "";

                            item.sNonTechnicalSkills =
                                dr["sNonTechnicalSkills"]?.ToString() ?? "";

                            item.dRegisterDate =
                                dr["dRegisterDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dRegisterDate"])
                                : null;

                            item.dModDate =
                                dr["dModDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dModDate"])
                                : null;

                            item.nBit =
                                dr["nBit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nBit"]);

                            item.nSABit =
                                dr["nSABit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nSABit"]);

                            item.nOrgID =
                                dr["nOrgID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nOrgID"])
                                : 0;

                            posts.Add(item);
                        }
                    }
                }
            }

            // No posts found
            if (posts.Count == 0)
            {
                return NotFound();
            }

            // Find selected post
            int currentIndex = posts.FindIndex(x => x.nID == id);

            if (currentIndex == -1)
            {
                return NotFound();
            }

            // -----------------------------------------
            // Previous
            // -----------------------------------------

            int? previousId = null;

            if (currentIndex < posts.Count - 1)
            {
                previousId = posts[currentIndex + 1].nID;
            }

            // -----------------------------------------
            // Next
            // -----------------------------------------

            int? nextId = null;

            if (currentIndex > 0)
            {
                nextId = posts[currentIndex - 1].nID;
            }

            // -----------------------------------------
            // ViewModel
            // -----------------------------------------

            OrgPostDetailsVM model = new OrgPostDetailsVM
            {
                Post = posts[currentIndex],

                CurrentIndex = currentIndex + 1,

                TotalRecords = posts.Count,

                PreviousId = previousId,

                NextId = nextId
            };

            return View(model);

        }

        [HttpGet]
        public IActionResult SAFinalSelection()
        {
            return View();
        }

        //khushi 01-10-26
        [HttpGet]
        public IActionResult SACharts()
        {
            SAChartsViewModel model = new SAChartsViewModel();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand("SP_GetSACharts", con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        /* =========================================
                           1. CANDIDATE DATA
                           tblcandidateRegister
                        ========================================= */

                        while (reader.Read())
                        {
                            model.CandidateData.Add(
                                new SAChartData
                                {
                                    Year =
                                        Convert.ToInt32(
                                            reader["Year"]
                                        ),

                                    Month =
                                        Convert.ToInt32(
                                            reader["Month"]
                                        ),

                                    TotalCount =
                                        Convert.ToInt32(
                                            reader["TotalCount"]
                                        )
                                }
                            );
                        }


                        /* =========================================
                           2. TBLPOST DATA
                           tblPost
                        ========================================= */

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.InternshipPostData.Add(
                                    new SAChartData
                                    {
                                        Year =
                                            Convert.ToInt32(
                                                reader["Year"]
                                            ),

                                        Month =
                                            Convert.ToInt32(
                                                reader["Month"]
                                            ),

                                        TotalCount =
                                            Convert.ToInt32(
                                                reader["TotalCount"]
                                            )
                                    }
                                );
                            }
                        }
                    }
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult SAAddAdvertisement()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAAdvertisementList()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SABillingDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAUpdatedImage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAReceiptDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAViewAdvertisement()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAAddInstallment()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAInvoiceDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAClientPaymentDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAViewInvoice()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SABillingControl()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SAAddBankDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAViewBankDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAEditBankDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAViewTaxesDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAViewPlan()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAEditTaxesDetails()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SAEditPlan()
        {
            return View();
        }

        #region "Elegible Trainee"
        [HttpGet]
        [Route("SuperAdmin/EligibleTrainees/{id:int}")]
        public IActionResult EligibleTrainees(int id, int orgID)
        {
            List<EligibleTraineeM> trainees = new List<EligibleTraineeM>();

            using (SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SP_GetCandidatesByPost", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Post ID
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                    // Organization ID
                    cmd.Parameters.Add("@OrganizationID", SqlDbType.Int).Value = orgID;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            EligibleTraineeM item = new EligibleTraineeM();

                            item.CandidateID =
                                Convert.ToInt32(dr["CandidateID"]);

                            item.Name =
                                dr["Name"] == DBNull.Value
                                    ? ""
                                    : dr["Name"].ToString();

                            item.EmailID =
                                dr["EmailID"] == DBNull.Value
                                    ? ""
                                    : dr["EmailID"].ToString();

                            item.Gender =
                                dr["Gender"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["Gender"]);

                            item.DateOfBirth =
                                dr["DateOfBirth"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DateOfBirth"]);

                            item.PostID =
                                Convert.ToInt32(dr["PostID"]);

                            item.GenderRequired =
                                dr["GenderRequired"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["GenderRequired"]);

                            item.InternshipType =
                                dr["InternshipType"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["InternshipType"]);

                            item.MinQualification =
                                dr["MinQualification"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["MinQualification"]);

                            item.MaxQualification =
                                dr["MaxQualification"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["MaxQualification"]);

                            item.MedicalSkill =
                                dr["MedicalSkill"] == DBNull.Value
                                    ? ""
                                    : dr["MedicalSkill"].ToString();

                            item.TechnicalSkill =
                                dr["TechnicalSkill"] == DBNull.Value
                                    ? ""
                                    : dr["TechnicalSkill"].ToString();

                            item.NonTechnicalSkill =
                                dr["NonTechnicalSkill"] == DBNull.Value
                                    ? ""
                                    : dr["NonTechnicalSkill"].ToString();

                            item.MedicalSkillNames =
                                dr["MedicalSkillNames"] == DBNull.Value
                                    ? ""
                                    : dr["MedicalSkillNames"].ToString();

                            item.TechnicalSkillNames =
                                dr["TechnicalSkillNames"] == DBNull.Value
                                    ? ""
                                    : dr["TechnicalSkillNames"].ToString();

                            item.NonTechnicalSkillNames =
                                dr["NonTechnicalSkillNames"] == DBNull.Value
                                    ? ""
                                    : dr["NonTechnicalSkillNames"].ToString();

                            item.MatchedSkillType =
                                dr["MatchedSkillType"] == DBNull.Value
                                    ? ""
                                    : dr["MatchedSkillType"].ToString();

                            item.CApply =
                                dr["CApply"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["CApply"]);

                            item.Status =
                                dr["Status"] == DBNull.Value
                                    ? ""
                                    : dr["Status"].ToString();

                            item.FinalStatus =
                                dr["FinalStatus"] == DBNull.Value
                                    ? ""
                                    : dr["FinalStatus"].ToString();

                            item.Comment =
                                dr["Comment"] == DBNull.Value
                                    ? ""
                                    : dr["Comment"].ToString();

                            trainees.Add(item);
                        }
                    }
                }
            }

            ViewBag.PostID = id;
            ViewBag.OrgID = orgID;

            return View(trainees);
        }
        #endregion

        // sanidhya 06/10/26
        // =========================================================
        // SUPER ADMIN - VIEW TRAINEE SELECTED RESUME
        // =========================================================

        // ============================================================
        // VIEW TRAINEE RESUME
        // Opens the SAME resume template selected by the candidate
        // ============================================================

        // ============================================================
        // VIEW TRAINEE RESUME
        // Opens EXACTLY the same resume selected by the candidate
        // ============================================================

        [HttpGet]
        [Route("SuperAdmin/ViewTraineeResume/{id:int}")]
        public IActionResult ViewTraineeResume(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            // IMPORTANT:
            // Use the same Resume action already used by the
            // Candidate section.
            //
            // Resume(int id) internally:
            // 1. Gets candidate profile
            // 2. Gets Resume_Profile
            // 3. Finds selected template
            // 4. Opens that exact ViewProfileXX page

            return RedirectToAction(
                "Resume",
                "Resume",
                new { id = id }
            );
        }
    }
}