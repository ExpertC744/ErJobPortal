using ErJobPortal.Controllers;
using ErJobPortal.Data;
using ErJobPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Data.SqlClient;
using System.Data;



namespace ErJobPortal.Repositories
{
    public class AccountRepository
    {
        private readonly DbConnection _db; public AccountRepository(DbConnection db) { _db = db; }
        // shrirang 06/10/26
        // Candidate Register 
        #region "Candidate Register"


        public int Register(CandidateRegister model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
      INSERT INTO tblCandidateRegister
      (
          sPrefix,
          sFName,
          sLName,
          sMobile,
          sAlternateMobile,
          sEmail,
          DOB,
          nGender,
          sProfileImage,
          nCollegeCode,
          sCollegeName,
          sPassword,
          RegDate,
          ModDate,
          nBit,
          nSABit,
          sOTP,
          nBranch,
          nPassoutYear,
          nDepartment,
          nCurrentYear,
          nAdmissionYear
      )
      VALUES
      (
          @sPrefix,
          @sFName,
          @sLName,
          @sMobile,
          @sAlternateMobile,
          @sEmail,
          @DOB,
          @nGender,
          @sProfileImage,
          @nCollegeCode,
          @sCollegeName,
          @sPassword,
          GETDATE(),
          GETDATE(),
          1,
          1,
          @sOTP,
          @nBranch,
          @nPassoutYear,
          @nDepartment,
          @nCurrentYear,
          @nAdmissionYear
      )";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;


                    // =====================================================
                    // PREFIX
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sPrefix",
                        model.sPrefix ?? ""
                    );


                    // =====================================================
                    // FIRST NAME
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sFName",
                        model.sFName ?? ""
                    );


                    // =====================================================
                    // LAST NAME
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sLName",
                        model.sLName ?? ""
                    );


                    // =====================================================
                    // MOBILE
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sMobile",
                        model.sMobile ?? ""
                    );


                    // =====================================================
                    // ALTERNATE MOBILE
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sAlternateMobile",
                        model.sAlternateMobile ?? ""
                    );


                    // =====================================================
                    // EMAIL
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        model.sEmail ?? ""
                    );


                    // =====================================================
                    // DATE OF BIRTH
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@DOB",
                        model.DOB.HasValue
                            ? model.DOB.Value
                            : DBNull.Value
                    );


                    // =====================================================
                    // GENDER
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nGender",
                        model.nGender
                    );


                    // =====================================================
                    // PROFILE IMAGE
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sProfileImage",
                        model.sProfileImage ?? ""
                    );


                    // =====================================================
                    // COLLEGE CODE
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nCollegeCode",
                        model.nCollegeCode
                    );


                    // =====================================================
                    // COLLEGE NAME
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                          "@sCollegeName",
                          model.sCollegeName
                    );


                    // =====================================================
                    // PASSWORD
                    // =====================================================

                    //cmd.Parameters.AddWithValue(
                    //    "@sPassword",
                    //    model.sPassword ?? ""
                    //);


                    var passwordHasher = new PasswordHasher<CandidateUser>();

                    string hashedPassword = passwordHasher.HashPassword(
                        new CandidateUser(),
                        model.sPassword ?? ""
                    );

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        hashedPassword
                    );


                    // =====================================================
                    // OTP
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@sOTP",
                        model.sOTP ?? ""
                    );


                    // =====================================================
                    // BRANCH
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nBranch",
                        model.nBranch
                    );


                    // =====================================================
                    // PASSOUT YEAR
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nPassoutYear",
                        model.nPassoutYear.HasValue
                            ? model.nPassoutYear.Value
                            : DBNull.Value
                    );


                    // =====================================================
                    // DEPARTMENT
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nDepartment",
                        model.nDepartment
                    );


                    // =====================================================
                    // CURRENT YEAR
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nCurrentYear",
                        model.nCurrentYear.HasValue
                            ? model.nCurrentYear.Value
                            : DBNull.Value
                    );


                    // =====================================================
                    // ADMISSION YEAR
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@nAdmissionYear",
                        model.nAdmissionYear.HasValue
                            ? model.nAdmissionYear.Value
                            : DBNull.Value
                    );


                    // =====================================================
                    // EXECUTE
                    // =====================================================

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        public (int CandidateID, DateTime? DOB, DateTime? RegDate)? GetCandidateRegistrationDetails(int candidateId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
        SELECT
            nID,
            DOB,
            RegDate
        FROM tblCandidateRegister
        WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", candidateId);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int id = Convert.ToInt32(dr["nID"]);

                            DateTime? dob =
                                dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]);

                            DateTime? regDate =
                                dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]);

                            return (id, dob, regDate);
                        }
                    }
                }
            }

            return null;
        }

        // Org Register
        #region Organization Register

        public int RegisterOrganization(OrganizationRegister model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd =
                    new SqlCommand("SP_RegisterOrganization", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sOrgName",
                        (object?)model.sOrgName ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sOrgUrl",
                        (object?)model.sOrgUrl ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sName",
                        (object?)model.sName ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sDesignation",
                        (object?)model.sDesignation ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@nDepartment", model.nDepartment);
                    cmd.Parameters.AddWithValue("@nBranch", model.nBranch);

                    cmd.Parameters.AddWithValue(
                        "@sMobile",
                        (object?)model.sMobile ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        (object?)model.sEmail ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@nCollegeCode", model.nCollegeCode);

                    cmd.Parameters.AddWithValue(
                        "@nCollegeName",
                        (object?)model.nCollegeName ?? DBNull.Value);

                    // STEP 1: Hash the original password
                    var passwordHasher = new PasswordHasher<OrganizationUser>();

                    string hashedPassword = passwordHasher.HashPassword(
                        new OrganizationUser(),
                        model.sPassword ?? ""
                    );

                    // STEP 2: Send ONLY the hashed password to SQL Server
                    cmd.Parameters.Add(
                        "@sPassword", SqlDbType.NVarChar, 500
                    ).Value = hashedPassword;

                    cmd.Parameters.AddWithValue(
                        "@sOTP",
                        (object?)model.sOTP ?? DBNull.Value);

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["OrganizationID"]);
                        }
                    }

                    return 0;
                }
            }
        }

        #endregion

        public (int OrganizationID, DateTime? RegDate)?
        GetOrganizationRegistrationDetails(int orgId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
                   SELECT
                       nID,
                       RegDate
                   FROM tblOrgRegistration
                   WHERE nID = @nID";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@nID",
                        orgId);

                    cn.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int organizationId =
                                Convert.ToInt32(
                                    dr["nID"]);

                            DateTime? regDate =
                                dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(
                                        dr["RegDate"]);

                            return (
                                organizationId,
                                regDate
                            );
                        }
                    }
                }
            }

            return null;
        }

        public (string CandidateName, string Email)? GetCandidateContactDetails(
    int candidateId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT sPrefix, sFName, sLName, sEmail
            FROM tblCandidateRegister
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", candidateId);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string prefix = dr["sPrefix"] == DBNull.Value
                                ? ""
                                : dr["sPrefix"].ToString() ?? "";

                            string firstName = dr["sFName"] == DBNull.Value
                                ? ""
                                : dr["sFName"].ToString() ?? "";

                            string lastName = dr["sLName"] == DBNull.Value
                                ? ""
                                : dr["sLName"].ToString() ?? "";

                            string email = dr["sEmail"] == DBNull.Value
                                ? ""
                                : dr["sEmail"].ToString() ?? "";

                            string fullName = string.Join(" ",
                                new[] { prefix, firstName, lastName }
                                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                            return (fullName, email);
                        }
                    }
                }
            }

            return null;
        }

        public (string OrganizationName, string ContactName, string Email)?
    GetOrganizationContactDetails(int orgId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT sOrgName, sName, sEmail
            FROM tblOrgRegistration
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", orgId);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string organizationName =
                                dr["sOrgName"] == DBNull.Value
                                    ? ""
                                    : dr["sOrgName"].ToString() ?? "";

                            string contactName =
                                dr["sName"] == DBNull.Value
                                    ? ""
                                    : dr["sName"].ToString() ?? "";

                            string email =
                                dr["sEmail"] == DBNull.Value
                                    ? ""
                                    : dr["sEmail"].ToString() ?? "";

                            return (organizationName, contactName, email);
                        }
                    }
                }
            }

            return null;
        }

        public List<DepartmentM> GetDepartments()
        {
            List<DepartmentM> departments = new List<DepartmentM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SP_GetDepartment", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            departments.Add(new DepartmentM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                sDepartment =
                                    dr["sDepartment"] != DBNull.Value
                                        ? dr["sDepartment"].ToString()
                                        : ""
                            });
                        }
                    }
                }
            }

            return departments;
        }

        //sanidhya 08/10/26
        public List<BranchM> GetBranches()
        {
            List<BranchM> branches = new List<BranchM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
     SELECT
         nID,
         nDepartmentID,
         sBranch
     FROM tblBranch
     ORDER BY sBranch ASC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            branches.Add(new BranchM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                nDepartmentID =
                                    dr["nDepartmentID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nDepartmentID"])
                                        : 0,

                                sBranch =
                                    dr["sBranch"] != DBNull.Value
                                        ? dr["sBranch"].ToString()
                                        : ""
                            });
                        }
                    }
                }
            }

            return branches;
        }
        //end

        
// Get branches by Department ID
public List<BranchM> GetBranches(int departmentId)
        {
            List<BranchM> branches = new List<BranchM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                nID,
                nDepartmentID,
                sBranch
            FROM tblBranch
            WHERE nDepartmentID = @DepartmentID
            ORDER BY sBranch ASC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentId;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            branches.Add(new BranchM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                nDepartmentID = dr["nDepartmentID"] != DBNull.Value
                                    ? Convert.ToInt32(dr["nDepartmentID"])
                                    : 0,

                                sBranch = dr["sBranch"] != DBNull.Value
                                    ? dr["sBranch"].ToString()
                                    : ""
                            });
                        }
                    }
                }
            }

            return branches;
        }


        public List<CollegeM> GetColleges()
        {
            List<CollegeM> colleges = new List<CollegeM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetCollege", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            colleges.Add(new CollegeM
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                sCollegeName = dr["sCollegeName"].ToString()
                            });
                        }
                    }
                }
            }

            return colleges;
        }


        public List<CollegeCodeM> GetCollegeCodes(int collegeId)
        {
            List<CollegeCodeM> codes = new List<CollegeCodeM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetCollegeCode", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nCollegeID", collegeId);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            codes.Add(new CollegeCodeM
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                nCollegeID = Convert.ToInt32(dr["nCollegeID"]),
                                nCode = dr["nCode"].ToString()
                            });
                        }
                    }
                }
            }

            return codes;
        }


        #region "Org Login"
        public OrganizationLogin? OrganizationLogin(OrganizationLogin model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_OrganizationLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sEmail", model.sEmail ?? "");
                    cmd.Parameters.AddWithValue("@sPassword", model.sPassword ?? "");
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new OrganizationLogin
                            {
                                sEmail = dr["sEmail"].ToString(),
                                sPassword = dr["sPassword"]?.ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public OrganizationUser? LoginOrganization(OrganizationLogin model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
     SELECT
         nID,
         sOrgName,
         sOrgUrl,
         sName,
         sDesignation,
         sMobile,
         sEmail,
         nCollegeCode,
         nCollegeName,
         sPassword,
         nSABit
     FROM tblOrgRegistration
     WHERE sEmail = @sEmail
       AND nBit = 1;";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(
                        "@sEmail", SqlDbType.NVarChar, 256
                    ).Value = model.sEmail ?? "";

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                            return null;

                        return new OrganizationUser
                        {
                            nID = Convert.ToInt32(dr["nID"]),

                            sOrgName = dr["sOrgName"] == DBNull.Value
                                ? "" : dr["sOrgName"].ToString(),

                            sOrgUrl = dr["sOrgUrl"] == DBNull.Value
                                ? "" : dr["sOrgUrl"].ToString(),

                            sName = dr["sName"] == DBNull.Value
                                ? "" : dr["sName"].ToString(),

                            sDesignation = dr["sDesignation"] == DBNull.Value
                                ? "" : dr["sDesignation"].ToString(),

                            sMobile = dr["sMobile"] == DBNull.Value
                                ? "" : dr["sMobile"].ToString(),

                            sEmail = dr["sEmail"] == DBNull.Value
                                ? "" : dr["sEmail"].ToString(),

                            nCollegeCode = dr["nCollegeCode"] == DBNull.Value
                                ? null : Convert.ToInt32(dr["nCollegeCode"]),

                            nCollegeName = dr["nCollegeName"] == DBNull.Value
                                ? null : Convert.ToInt32(dr["nCollegeName"]),

                            // Retrieve the stored hash
                            sPassword = dr["sPassword"] == DBNull.Value
                                ? "" : dr["sPassword"].ToString(),

                            nSABit = dr["nSABit"] != DBNull.Value
                                && Convert.ToBoolean(dr["nSABit"])
                        };
                    }
                }
            }
        }
        #endregion



        #region "Candidate Login"

        public CandidateUser? LoginCandidate(CandidateLogin model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
     SELECT
         nID,
         sFName,
         sLName,
         sMobile,
         sEmail,
         DOB,
         nGender,
         sProfileImage,
         nCollegeCode,
         sCollegeName,
         sPassword,
         nSABit
     FROM tblCandidateRegister
     WHERE sEmail = @sEmail
       AND nBit = 1;";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(
                        "@sEmail",
                        SqlDbType.NVarChar,
                        256
                    ).Value = model.sEmail ?? "";

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new CandidateUser
                            {
                                nID = dr["nID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nID"]),

                                sFName = dr["sFName"] == DBNull.Value
                                    ? ""
                                    : dr["sFName"].ToString(),

                                sLName = dr["sLName"] == DBNull.Value
                                    ? ""
                                    : dr["sLName"].ToString(),

                                sMobile = dr["sMobile"] == DBNull.Value
                                    ? ""
                                    : dr["sMobile"].ToString(),

                                sEmail = dr["sEmail"] == DBNull.Value
                                    ? ""
                                    : dr["sEmail"].ToString(),

                                DOB = dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]),

                                nGender = dr["nGender"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nGender"]),

                                sProfileImage =
                                    dr["sProfileImage"] == DBNull.Value
                                    ? ""
                                    : dr["sProfileImage"].ToString(),

                                nCollegeCode =
                                    dr["nCollegeCode"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nCollegeCode"]),

                                sCollegeName =
                                    dr["sCollegeName"] == DBNull.Value
                                    ? ""
                                    : dr["sCollegeName"].ToString(),

                                // Retrieve the stored password hash
                                sPassword = dr["sPassword"] == DBNull.Value
                                    ? ""
                                    : dr["sPassword"].ToString(),

                                // Super Admin enable/disable status
                                nSABit = dr["nSABit"] != DBNull.Value
                                    && Convert.ToBoolean(dr["nSABit"])
                            };
                        }
                    }
                }
            }

            return null;
        }


        #endregion


        #region "SA Login"

        public SALoginModel? Login(string email, string password)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
      SELECT
          nID,
          SAID,
          sFName,
          sLName,
          sEmail,
          sMobile,
          sRole,
          sPassword
      FROM tblSuperAdmin
      WHERE nBit = 1
        AND sEmail = @Email;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        256
                    ).Value = email.Trim();

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new SALoginModel
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                SAID = Convert.ToInt32(dr["SAID"]),
                                sFName = dr["sFName"]?.ToString(),
                                sLName = dr["sLName"]?.ToString(),
                                sEmail = dr["sEmail"]?.ToString(),
                                sMobile = dr["sMobile"]?.ToString(),
                                sRole = dr["sRole"]?.ToString(),

                                // Retrieve the stored password hash
                                sPassword = dr["sPassword"] == DBNull.Value
                                    ? ""
                                    : dr["sPassword"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        // Get All Candidate List
        public DataTable GetAllCandidate()
        {
            using (SqlConnection con = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tblCandidateRegister ORDER BY RegDate DESC", con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Enable / Disable Candidate
        public int UpdateCandidateStatus(int nID, bool nSABit)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"UPDATE tblCandidateRegister SET nSABit=@nSABit WHERE nID=@nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", nID);
                    cmd.Parameters.AddWithValue("@nSABit", nSABit);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Get All Organization List
        public DataTable GetAllOrganization()
        {
            using (SqlConnection con = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tblOrgRegistration ORDER BY RegDate DESC", con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Enable / Disable Organization
        public int UpdateOrganizationStatus(int nID, bool nSABit)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"UPDATE tblOrgRegistration
                  SET nSABit=@nSABit,
                      ModDate=GETDATE()
                  WHERE nID=@nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", nID);
                    cmd.Parameters.AddWithValue("@nSABit", nSABit);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        // Feedback
        #region "Feedback"

        // Insert Feedback
        public int InsertFeedback(FeedbackM model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
             INSERT INTO tblFeedback
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
                 nSABit
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
                 0
             )";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@sQue1",
                        (object?)model.sQue1 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue2",
                        (object?)model.sQue2 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue3",
                        (object?)model.sQue3 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue4",
                        (object?)model.sQue4 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue5",
                        (object?)model.sQue5 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@nAdminID",
                        (object?)model.nAdminID ?? DBNull.Value);

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }


        // Get All Feedback
        public List<FeedbackM> GetAllFeedback()
        {
            List<FeedbackM> list = new List<FeedbackM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
             SELECT
                 nID,
                 sQue1,
                 sQue2,
                 sQue3,
                 sQue4,
                 sQue5,
                 nAdminID,
                 RegDate,
                 ModDate,
                 nBit,
                 nSABit
             FROM tblFeedback
             WHERE nBit = 1
             ORDER BY RegDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            FeedbackM model = new FeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                sQue1 = dr["sQue1"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue1"]),

                                sQue2 = dr["sQue2"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue2"]),

                                sQue3 = dr["sQue3"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue3"]),

                                sQue4 = dr["sQue4"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue4"]),

                                sQue5 = dr["sQue5"] == DBNull.Value
                                    ? null
                                    : dr["sQue5"].ToString(),

                                nAdminID = dr["nAdminID"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["nAdminID"]),

                                RegDate = dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]),

                                ModDate = dr["ModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["ModDate"]),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"])
                            };

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }


        // Get Feedback By ID
        public FeedbackM? GetFeedbackById(int id)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
             SELECT
                 nID,
                 sQue1,
                 sQue2,
                 sQue3,
                 sQue4,
                 sQue5,
                 nAdminID,
                 RegDate,
                 ModDate,
                 nBit,
                 nSABit
             FROM tblFeedback
             WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new FeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                sQue1 = dr["sQue1"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue1"]),

                                sQue2 = dr["sQue2"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue2"]),

                                sQue3 = dr["sQue3"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue3"]),

                                sQue4 = dr["sQue4"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["sQue4"]),

                                sQue5 = dr["sQue5"] == DBNull.Value
                                    ? null
                                    : dr["sQue5"].ToString(),

                                nAdminID = dr["nAdminID"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["nAdminID"]),

                                RegDate = dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]),

                                ModDate = dr["ModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["ModDate"]),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"])
                            };
                        }
                    }
                }
            }

            return null;
        }


        // Update Feedback
        public int UpdateFeedback(FeedbackM model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
             UPDATE tblFeedback
             SET
                 sQue1 = @sQue1,
                 sQue2 = @sQue2,
                 sQue3 = @sQue3,
                 sQue4 = @sQue4,
                 sQue5 = @sQue5,
                 ModDate = GETDATE()
             WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@nID",
                        model.nID);

                    cmd.Parameters.AddWithValue(
                        "@sQue1",
                        (object?)model.sQue1 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue2",
                        (object?)model.sQue2 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue3",
                        (object?)model.sQue3 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue4",
                        (object?)model.sQue4 ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sQue5",
                        (object?)model.sQue5 ?? DBNull.Value);

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }


        // Delete Feedback
        public int DeleteFeedback(int id)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
             UPDATE tblFeedback
             SET
                 nBit = 0,
                 ModDate = GETDATE()
             WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion


        // Get All Trainees
        #region "Trainee"

        //public List<SATraineeListM> GetAllTrainees()
        //{
        //    List<SATraineeListM> list = new List<SATraineeListM>();

        //    using (SqlConnection cn = _db.GetConnection())
        //    {
        //        string query = @"
        //    SELECT
        //        nID,
        //        sFName,
        //        sLName,
        //        sMobile,
        //        sEmail,
        //        DOB,
        //        nGender,
        //        sProfileImage,
        //        nCollegeCode,
        //        sCollegeName,
        //        RegDate,
        //        ModDate,
        //        nBit,
        //        nSABit
        //    FROM tblCandidateRegister
        //    WHERE nBit = 1
        //    ORDER BY nID DESC";

        //        using (SqlCommand cmd = new SqlCommand(query, cn))
        //        {
        //            cn.Open();

        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    SATraineeListM model = new TraineeM
        //                    {
        //                        nID = dr["nID"] == DBNull.Value
        //                            ? 0
        //                            : Convert.ToInt32(dr["nID"]),

        //                        sFName = dr["sFName"] == DBNull.Value
        //                            ? null
        //                            : dr["sFName"].ToString(),

        //                        sLName = dr["sLName"] == DBNull.Value
        //                            ? null
        //                            : dr["sLName"].ToString(),

        //                        sMobile = dr["sMobile"] == DBNull.Value
        //                            ? null
        //                            : dr["sMobile"].ToString(),

        //                        sEmail = dr["sEmail"] == DBNull.Value
        //                            ? null
        //                            : dr["sEmail"].ToString(),

        //                        DOB = dr["DOB"] == DBNull.Value
        //                            ? (DateTime?)null
        //                            : Convert.ToDateTime(dr["DOB"]),

        //                        nGender = dr["nGender"] == DBNull.Value
        //                            ? 0
        //                            : Convert.ToInt32(dr["nGender"]),

        //                        sProfileImage = dr["sProfileImage"] == DBNull.Value
        //                            ? null
        //                            : dr["sProfileImage"].ToString(),

        //                        nCollegeCode = dr["nCollegeCode"] == DBNull.Value
        //                            ? 0
        //                            : Convert.ToInt32(dr["nCollegeCode"]),

        //                        sCollegeName = dr["sCollegeName"] == DBNull.Value
        //                            ? 0
        //                            : Convert.ToInt32(dr["sCollegeName"]),

        //                        RegDate = dr["RegDate"] == DBNull.Value
        //                            ? (DateTime?)null
        //                            : Convert.ToDateTime(dr["RegDate"]),

        //                        ModDate = dr["ModDate"] == DBNull.Value
        //                            ? (DateTime?)null
        //                            : Convert.ToDateTime(dr["ModDate"]),

        //                        nBit = dr["nBit"] != DBNull.Value &&
        //                               Convert.ToBoolean(dr["nBit"]),

        //                        nSABit = dr["nSABit"] != DBNull.Value &&
        //                                 Convert.ToBoolean(dr["nSABit"])
        //                    };

        //                    list.Add(model);
        //                }
        //            }
        //        }
        //    }

        //    return list;
        //}

        #endregion

        #region "Super Admin Trainee List"

        public List<SATraineeListM> GetAllTrainees()
        {
            return GetSATraineeList();
        }

        public List<SATraineeListM> GetSATraineeList()
        {
            List<SATraineeListM> list = new List<SATraineeListM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                nID,
                sFName,
                sLName,
                sMobile,
                sEmail,
                DOB,
                nGender,
                sProfileImage,
                nCollegeCode,
                sCollegeName,
                sPassword,
                RegDate,
                ModDate,
                nBit,
                nSABit,
                sOTP,
                nBranch,
                nPassoutYear,
                nDepartment
            FROM tblCandidateRegister
            WHERE nSABit = 1
            ORDER BY nID DESC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SATraineeListM model = new SATraineeListM
                            {
                                nID = dr["nID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nID"]),

                                sFName = dr["sFName"] == DBNull.Value
                                    ? ""
                                    : dr["sFName"].ToString(),

                                sLName = dr["sLName"] == DBNull.Value
                                    ? ""
                                    : dr["sLName"].ToString(),

                                sMobile = dr["sMobile"] == DBNull.Value
                                    ? ""
                                    : dr["sMobile"].ToString(),

                                sEmail = dr["sEmail"] == DBNull.Value
                                    ? ""
                                    : dr["sEmail"].ToString(),

                                DOB = dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]),

                                nGender = dr["nGender"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nGender"]),

                                sProfileImage = dr["sProfileImage"] == DBNull.Value
                                    ? ""
                                    : dr["sProfileImage"].ToString(),

                                nCollegeCode = dr["nCollegeCode"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nCollegeCode"]),

                                sCollegeName = dr["sCollegeName"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["sCollegeName"]),

                                sPassword = dr["sPassword"] == DBNull.Value
                                    ? ""
                                    : dr["sPassword"].ToString(),

                                RegDate = dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]),

                                ModDate = dr["ModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["ModDate"]),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"]),

                                sOTP = dr["sOTP"] == DBNull.Value
                                    ? ""
                                    : dr["sOTP"].ToString(),

                                nBranch = dr["nBranch"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["nBranch"]),

                                nPassoutYear = dr["nPassoutYear"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["nPassoutYear"]),

                                nDepartment = dr["nDepartment"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["nDepartment"])
                            };

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }



        //Super Admin Candidate List Status

        public bool ToggleCandidateStatus(int id)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            UPDATE tblCandidateRegister
            SET nSABit = CASE 
                            WHEN nSABit = 1 THEN 0
                            ELSE 1
                       END,
                ModDate = GETDATE()
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    cn.Open();

                    int rows = cmd.ExecuteNonQuery();

                    return rows > 0;
                }
            }
        }

        public bool GetCandidateStatus(int id)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT nSABit
            FROM tblCandidateRegister
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    cn.Open();

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        return false;
                    }

                    return Convert.ToBoolean(result);
                }
            }
        }
        #endregion

        #region Super Admin Organization List

        public List<OrganizationUser> GetAllOrganizationList()
        {
            List<OrganizationUser> list = new List<OrganizationUser>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                nID,
                sOrgName,
                sOrgUrl,
                sName,
                sDesignation,
                sMobile,
                sEmail,
                nCollegeCode,
                nCollegeName,
                sPassword,
                RegDate,
                ModDate,
                nBit,
                nSABit
            FROM tblOrgRegistration
            ORDER BY nID DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrganizationUser model =
                                new OrganizationUser
                                {
                                    nID = dr["nID"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(dr["nID"]),

                                    sOrgName = dr["sOrgName"] == DBNull.Value
                                        ? ""
                                        : dr["sOrgName"].ToString(),

                                    sOrgUrl = dr["sOrgUrl"] == DBNull.Value
                                        ? ""
                                        : dr["sOrgUrl"].ToString(),

                                    sName = dr["sName"] == DBNull.Value
                                        ? ""
                                        : dr["sName"].ToString(),

                                    sDesignation = dr["sDesignation"] == DBNull.Value
                                        ? ""
                                        : dr["sDesignation"].ToString(),

                                    sMobile = dr["sMobile"] == DBNull.Value
                                        ? ""
                                        : dr["sMobile"].ToString(),

                                    sEmail = dr["sEmail"] == DBNull.Value
                                        ? ""
                                        : dr["sEmail"].ToString(),

                                    nCollegeCode = dr["nCollegeCode"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(dr["nCollegeCode"]),

                                    nCollegeName = dr["nCollegeName"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(dr["nCollegeName"]),

                                    sPassword = dr["sPassword"] == DBNull.Value
                                        ? ""
                                        : dr["sPassword"].ToString(),

                                    RegDate = dr["RegDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(dr["RegDate"]),

                                    ModDate = dr["ModDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(dr["ModDate"]),

                                    nBit = dr["nBit"] != DBNull.Value &&
                                           Convert.ToBoolean(dr["nBit"]),

                                    nSABit = dr["nSABit"] != DBNull.Value &&
                                             Convert.ToBoolean(dr["nSABit"])
                                };

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        // Super Admin Organization List status
        public bool ToggleOrganizationSABit(int id, bool status)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            UPDATE tblOrgRegistration
            SET 
                nSABit = @nSABit,
                ModDate = GETDATE()
            WHERE nID = @nID";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@nID", id);
                    cmd.Parameters.AddWithValue("@nSABit", status ? 1 : 0);

                    cn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

       

        // shrirang 15/09/26

        #region "Candidate Forgotpass"

        // ==========================================================
        // GET CANDIDATE LOGIN DETAILS BY EMAIL
        // ==========================================================

        public CandidateLogin? GetCandidateLoginDetails(string email)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                sEmail,
                sPassword
            FROM tblCandidateRegister
            WHERE sEmail = @Email
              AND nBit = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        200
                    ).Value = email.Trim();

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CandidateLogin
                            {
                                sEmail = reader["sEmail"] == DBNull.Value
                                    ? ""
                                    : reader["sEmail"].ToString(),

                                sPassword = reader["sPassword"] == DBNull.Value
                                    ? ""
                                    : reader["sPassword"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        #endregion


        public bool ResetCandidatePassword(string email, string newPassword)
        {
            // Hash the new password before saving it
            var passwordHasher = new PasswordHasher<CandidateUser>();

            string hashedPassword = passwordHasher.HashPassword(
                new CandidateUser(),
                newPassword
            );

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
     UPDATE tblCandidateRegister
     SET sPassword = @NewPassword,
         ModDate = GETDATE()
     WHERE sEmail = @Email
       AND nBit = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@NewPassword",
                        SqlDbType.NVarChar,
                        500
                    ).Value = hashedPassword;

                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        200
                    ).Value = email.Trim();

                    cn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // ==========================================================
        // ORGANIZATION RESET PASSWORD
        // ==========================================================

        public OrganizationLogin? GetOrganizationLoginDetails(string email)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                sEmail,
                sPassword
            FROM tblOrgRegistration
            WHERE sEmail = @Email
              AND nBit = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        200
                    ).Value = email.Trim();

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrganizationLogin
                            {
                                sEmail = reader["sEmail"] == DBNull.Value
                                    ? ""
                                    : reader["sEmail"].ToString(),

                                sPassword = reader["sPassword"] == DBNull.Value
                                    ? ""
                                    : reader["sPassword"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }


        // ==========================================================
        // UPDATE ORGANIZATION PASSWORD
        // ==========================================================

        public bool ResetOrganizationPassword(
     string email,
     string newPassword)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
     UPDATE tblOrgRegistration
     SET
         sPassword = @NewPassword,
         ModDate = GETDATE()
     WHERE sEmail = @Email
       AND nBit = 1;";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    // Hash the new password before saving
                    var passwordHasher =
                        new PasswordHasher<OrganizationUser>();

                    string hashedPassword = passwordHasher.HashPassword(
                        new OrganizationUser(),
                        newPassword
                    );

                    cmd.Parameters.Add(
                        "@NewPassword",
                        SqlDbType.NVarChar,
                        500
                    ).Value = hashedPassword;

                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        150
                    ).Value = email.Trim();

                    cn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // sa reset pass
        public SALoginModel? GetSuperAdminLoginDetails(string email) { using (SqlConnection cn = _db.GetConnection()) { string query = @" SELECT sEmail, sPassword FROM tblSuperAdmin WHERE sEmail = @Email AND nBit = 1"; using (SqlCommand cmd = new SqlCommand(query, cn)) { cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email.Trim(); cn.Open(); using (SqlDataReader reader = cmd.ExecuteReader()) { if (reader.Read()) { return new SALoginModel { sEmail = reader["sEmail"] == DBNull.Value ? "" : reader["sEmail"].ToString(), sPassword = reader["sPassword"] == DBNull.Value ? "" : reader["sPassword"].ToString() }; } } } } return null; }



        public bool ResetSuperAdminPassword(string email, string newPassword)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                var passwordHasher = new PasswordHasher<SALoginModel>();

                string hashedPassword = passwordHasher.HashPassword(
                    new SALoginModel(),
                    newPassword
                );

                string query = @"
      UPDATE tblSuperAdmin
      SET sPassword = @Password,
          ModDate = GETDATE()
      WHERE sEmail = @Email
        AND nBit = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 500)
                        .Value = hashedPassword;

                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150)
                        .Value = email;

                    cn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // TPO REGISTRATION//

        // shrirang 23/09/26

        // ==========================================================
        // TPO REGISTRATION
        // ==========================================================
        // shrirang 22/10/26

        public int RegisterTPO(TPORegistration model)
        {
            // ==========================================================
            // 1. VALIDATE AND HASH PASSWORD
            // ==========================================================

            if (string.IsNullOrWhiteSpace(model.CollegeMailID))
            {
                throw new InvalidOperationException(
                    "College email address is required."
                );
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new InvalidOperationException(
                    "Password is required."
                );
            }

            var passwordHasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<string>();

            string collegeEmail = model.CollegeMailID.Trim();

            string passwordHash = passwordHasher.HashPassword(
                collegeEmail,
                model.Password
            );

            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                // ==========================================================
                // 2. VALIDATE SELECTED DEPARTMENT ID
                // ==========================================================

                if (model.nDepartment == null || model.nDepartment <= 0)
                {
                    throw new InvalidOperationException(
                        "Please select a valid department."
                    );
                }

                // ==========================================================
                // 3. GET DEPARTMENT NAME USING DEPARTMENT ID
                // ==========================================================

                string departmentName;

                string departmentQuery = @"
            SELECT sDepartment
            FROM dbo.tblDepartment
            WHERE nID = @DepartmentID";

                using (SqlCommand departmentCmd =
                       new SqlCommand(departmentQuery, cn))
                {
                    departmentCmd.Parameters.Add(
                        "@DepartmentID",
                        SqlDbType.Int
                    ).Value = model.nDepartment.Value;

                    object? departmentResult =
                        departmentCmd.ExecuteScalar();

                    if (departmentResult == null ||
                        departmentResult == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "The selected department was not found."
                        );
                    }

                    departmentName =
                        Convert.ToString(departmentResult) ?? "";

                    if (string.IsNullOrWhiteSpace(departmentName))
                    {
                        throw new InvalidOperationException(
                            "The selected department has no name."
                        );
                    }
                }

                // ==========================================================
                // 4. INSERT TPO REGISTRATION
                // ==========================================================

                string query = @"
            INSERT INTO dbo.tblTPORegistration
            (
                FullName,
                CollegeMailID,
                MobileNo,
                PasswordHash,
                CollegeName,
                CollegeCode,
                CollegeAddress,
                OrganizationWebsiteURL,
                Designation,
                DepartmentName,
                CollegeEmployeeID,
                SupportingDocument1,
                SupportingDocument2,
                ProfilePhoto,
                OTP,
                OTPVerified,
                Status,
                IsApproved,
                IsActive,
                CreatedDate,
                RegDate,
                ModDate,
                nBit,
                nSABit
            )
            VALUES
            (
                @FullName,
                @CollegeMailID,
                @MobileNo,
                @PasswordHash,
                @CollegeName,
                @CollegeCode,
                @CollegeAddress,
                @OrganizationWebsiteURL,
                @Designation,
                @DepartmentName,
                @CollegeEmployeeID,
                @SupportingDocument1,
                @SupportingDocument2,
                @ProfilePhoto,
                @OTP,
                @OTPVerified,
                @Status,
                @IsApproved,
                @IsActive,
                GETDATE(),
                GETDATE(),
                GETDATE(),
                1,
                1
            )";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    // Basic information
                    cmd.Parameters.Add(
                        "@FullName", SqlDbType.NVarChar, 150
                    ).Value = model.FullName ?? "";

                    cmd.Parameters.Add(
                        "@CollegeMailID", SqlDbType.NVarChar, 200
                    ).Value = collegeEmail;

                    cmd.Parameters.Add(
                        "@MobileNo", SqlDbType.VarChar, 15
                    ).Value = model.MobileNo ?? "";

                    // Store password hash, never the plain-text password
                    cmd.Parameters.Add(
                        "@PasswordHash", SqlDbType.NVarChar, 500
                    ).Value = passwordHash;

                    // College information
                    cmd.Parameters.Add(
                        "@CollegeName", SqlDbType.NVarChar, 250
                    ).Value = model.CollegeName ?? "";

                    cmd.Parameters.Add(
                        "@CollegeCode", SqlDbType.NVarChar, 100
                    ).Value = string.IsNullOrWhiteSpace(model.CollegeCode)
                        ? (object)DBNull.Value
                        : model.CollegeCode;

                    cmd.Parameters.Add(
                        "@CollegeAddress", SqlDbType.NVarChar, 500
                    ).Value = model.CollegeAddress ?? "";

                    cmd.Parameters.Add(
                        "@OrganizationWebsiteURL", SqlDbType.NVarChar, 500
                    ).Value = string.IsNullOrWhiteSpace(
                        model.OrganizationWebsiteURL)
                        ? (object)DBNull.Value
                        : model.OrganizationWebsiteURL;

                    // Designation and department
                    cmd.Parameters.Add(
                        "@Designation", SqlDbType.NVarChar, 150
                    ).Value = model.Designation ?? "";

                    // Use the department name retrieved from the database
                    cmd.Parameters.Add(
                        "@DepartmentName", SqlDbType.NVarChar, 150
                    ).Value = departmentName;

                    cmd.Parameters.Add(
                        "@CollegeEmployeeID", SqlDbType.NVarChar, 100
                    ).Value = model.CollegeEmployeeID ?? "";

                    // Supporting documents
                    cmd.Parameters.Add(
                        "@SupportingDocument1", SqlDbType.NVarChar, 500
                    ).Value = string.IsNullOrWhiteSpace(
                        model.SupportingDocument1)
                        ? (object)DBNull.Value
                        : model.SupportingDocument1;

                    cmd.Parameters.Add(
                        "@SupportingDocument2", SqlDbType.NVarChar, 500
                    ).Value = string.IsNullOrWhiteSpace(
                        model.SupportingDocument2)
                        ? (object)DBNull.Value
                        : model.SupportingDocument2;

                    cmd.Parameters.Add(
                        "@ProfilePhoto", SqlDbType.NVarChar, 500
                    ).Value = string.IsNullOrWhiteSpace(model.ProfilePhoto)
                        ? (object)DBNull.Value
                        : model.ProfilePhoto;

                    // OTP information
                    cmd.Parameters.Add(
                        "@OTP", SqlDbType.VarChar, 10
                    ).Value = string.IsNullOrWhiteSpace(model.OTP)
                        ? (object)DBNull.Value
                        : model.OTP;

                    cmd.Parameters.Add(
                        "@OTPVerified", SqlDbType.Bit
                    ).Value = model.OTPVerified;

                    // Account status
                    cmd.Parameters.Add(
                        "@Status", SqlDbType.NVarChar, 50
                    ).Value = string.IsNullOrWhiteSpace(model.Status)
                        ? "Pending"
                        : model.Status;

                    cmd.Parameters.Add(
                        "@IsApproved", SqlDbType.Bit
                    ).Value = model.IsApproved;

                    cmd.Parameters.Add(
                        "@IsActive", SqlDbType.Bit
                    ).Value = model.IsActive;

                    // ======================================================
                    // 5. EXECUTE INSERT
                    // ======================================================

                    return cmd.ExecuteNonQuery();
                }
            }
        }




        // =========================================================
        // TPO LOGIN
        // =========================================================

        public TPOLoginResult? LoginTPO(string collegeMailID)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                TPOID,
                FullName,
                CollegeMailID,
                MobileNo,
                PasswordHash,
                CollegeName,
                CollegeCode,
                Designation,
                DepartmentName,
                OTPVerified,
                Status,
                IsApproved,
                IsActive,
                subtopbit
            FROM tblTPORegistration
            WHERE CollegeMailID = @CollegeMailID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 200)
                        .Value = collegeMailID.Trim();

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new TPOLoginResult
                        {
                            TPOID = Convert.ToInt32(reader["TPOID"]),
                            FullName = reader["FullName"]?.ToString() ?? "",
                            CollegeMailID = reader["CollegeMailID"]?.ToString() ?? "",
                            MobileNo = reader["MobileNo"]?.ToString() ?? "",
                            PasswordHash = reader["PasswordHash"]?.ToString() ?? "",
                            CollegeName = reader["CollegeName"]?.ToString() ?? "",

                            CollegeCode = reader["CollegeCode"] == DBNull.Value
                                ? null
                                : reader["CollegeCode"].ToString(),

                            Designation = reader["Designation"]?.ToString() ?? "",
                            DepartmentName = reader["DepartmentName"]?.ToString() ?? "",

                            OTPVerified = reader["OTPVerified"] != DBNull.Value
                                && Convert.ToBoolean(reader["OTPVerified"]),

                            Status = reader["Status"]?.ToString() ?? "",

                            IsApproved = reader["IsApproved"] != DBNull.Value
                                && Convert.ToBoolean(reader["IsApproved"]),

                            IsActive = reader["IsActive"] != DBNull.Value
                                && Convert.ToBoolean(reader["IsActive"]),

                            SubTpoBit = reader["subtopbit"] != DBNull.Value
                                && Convert.ToBoolean(reader["subtopbit"])
                        };
                    }
                }
            }
        }



        public bool TPOEmailExists(string collegeMailID)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT COUNT(1)
            FROM tblTPORegistration
            WHERE CollegeMailID = @CollegeMailID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 200)
                        .Value = collegeMailID.Trim();

                    cn.Open();

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        // RESET PASSWORD TPO
        public bool ResetTPOPassword(
      string collegeMailID,
      string newPassword)
        {
            collegeMailID = collegeMailID.Trim();

            // Hash the new password using the same approach as registration.
            var passwordHasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<string>();

            string passwordHash = passwordHasher.HashPassword(
                collegeMailID,
                newPassword);

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            UPDATE tblTPORegistration
            SET
                PasswordHash = @PasswordHash,
                UpdatedDate = GETDATE()
            WHERE CollegeMailID = @CollegeMailID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@PasswordHash",
                        SqlDbType.NVarChar,
                        500).Value = passwordHash;

                    cmd.Parameters.Add(
                        "@CollegeMailID",
                        SqlDbType.NVarChar,
                        200).Value = collegeMailID;

                    cn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // shrirang 03/10/26
        // =========================================================
        // GET TPO DETAILS BY TPO ID
        // =========================================================

        // =========================================================
        // GET TPO DETAILS BY TPO ID
        // =========================================================

        // shrirang 03/10/26
        // =========================================================
        // GET TPO DETAILS BY TPO ID
        // =========================================================

        public TPORegistration? GetTPODetails(int tpoId)
        {
            TPORegistration? model = null;

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                FullName,
                CollegeMailID,
                MobileNo,
                CollegeName,
                CollegeCode,
                CollegeAddress,
                OrganizationWebsiteURL,
                Designation,
                DepartmentName,
                CollegeEmployeeID,
                SupportingDocument1,
                SupportingDocument2,
                ProfilePhoto,
                OTPVerified,
                Status,
                IsApproved,
                IsActive,
                ApprovedBy,
                ApprovedDate,
                RejectionReason,
                CreatedDate,
                UpdatedDate
            FROM tblTPORegistration
            WHERE TPOID = @TPOID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@TPOID",
                        SqlDbType.Int
                    ).Value = tpoId;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new TPORegistration
                            {
                                // ==============================
                                // PERSONAL DETAILS
                                // ==============================

                                FullName =
                                    reader["FullName"] == DBNull.Value
                                        ? ""
                                        : reader["FullName"].ToString(),

                                CollegeMailID =
                                    reader["CollegeMailID"] == DBNull.Value
                                        ? ""
                                        : reader["CollegeMailID"].ToString(),

                                MobileNo =
                                    reader["MobileNo"] == DBNull.Value
                                        ? ""
                                        : reader["MobileNo"].ToString(),

                                // ==============================
                                // COLLEGE DETAILS
                                // ==============================

                                CollegeName =
                                    reader["CollegeName"] == DBNull.Value
                                        ? ""
                                        : reader["CollegeName"].ToString(),

                                CollegeCode =
                                    reader["CollegeCode"] == DBNull.Value
                                        ? ""
                                        : reader["CollegeCode"].ToString(),

                                CollegeAddress =
                                    reader["CollegeAddress"] == DBNull.Value
                                        ? ""
                                        : reader["CollegeAddress"].ToString(),

                                OrganizationWebsiteURL =
                                    reader["OrganizationWebsiteURL"] == DBNull.Value
                                        ? ""
                                        : reader["OrganizationWebsiteURL"].ToString(),

                                // ==============================
                                // PROFESSIONAL DETAILS
                                // ==============================

                                Designation =
                                    reader["Designation"] == DBNull.Value
                                        ? ""
                                        : reader["Designation"].ToString(),

                                DepartmentName =
                                    reader["DepartmentName"] == DBNull.Value
                                        ? ""
                                        : reader["DepartmentName"].ToString(),

                                CollegeEmployeeID =
                                    reader["CollegeEmployeeID"] == DBNull.Value
                                        ? ""
                                        : reader["CollegeEmployeeID"].ToString(),

                                // ==============================
                                // DOCUMENTS
                                // ==============================

                                SupportingDocument1 =
                                    reader["SupportingDocument1"] == DBNull.Value
                                        ? null
                                        : reader["SupportingDocument1"].ToString(),

                                SupportingDocument2 =
                                    reader["SupportingDocument2"] == DBNull.Value
                                        ? null
                                        : reader["SupportingDocument2"].ToString(),

                                ProfilePhoto =
                                    reader["ProfilePhoto"] == DBNull.Value
                                        ? null
                                        : reader["ProfilePhoto"].ToString(),

                                // ==============================
                                // STATUS
                                // ==============================

                                OTPVerified =
                                    reader["OTPVerified"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["OTPVerified"]),

                                Status =
                                    reader["Status"] == DBNull.Value
                                        ? null
                                        : reader["Status"].ToString(),

                                IsApproved =
                                    reader["IsApproved"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["IsApproved"]),

                                IsActive =
                                    reader["IsActive"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["IsActive"]),

                                // ==============================
                                // APPROVAL DETAILS
                                // ==============================

                                ApprovedBy =
                                    reader["ApprovedBy"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(reader["ApprovedBy"]),

                                ApprovedDate =
                                    reader["ApprovedDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["ApprovedDate"]),

                                RejectionReason =
                                    reader["RejectionReason"] == DBNull.Value
                                        ? null
                                        : reader["RejectionReason"].ToString(),

                                // ==============================
                                // DATE DETAILS
                                // ==============================

                                CreatedDate =
                                    reader["CreatedDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["CreatedDate"]),

                                UpdatedDate =
                                    reader["UpdatedDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["UpdatedDate"])
                            };
                        }
                    }
                }
            }

            return model;
        }

        // shrirang 05/10/26
        // shrirang 05/10/26
        // shrirang 05/10/26
        // =========================================================
        // INSERT SUB TPO
        // =========================================================


        public bool InsertSubTPO(TPORegistration model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // CollegeName now receives the selected numeric college ID
            // from the College dropdown.
            if (!int.TryParse(model.CollegeName, out int collegeId) ||
                collegeId <= 0)
            {
                throw new InvalidOperationException(
                    "Please select a valid college.");
            }

            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                // Validate the selected college.
                // Change tblCollege if your actual college table has another name.
                using (SqlCommand checkCmd = new SqlCommand(@"
            SELECT COUNT(1)
            FROM tblCollege
            WHERE nID = @CollegeID
              AND ISNULL(nBit, 0) = 1;", cn))
                {
                    checkCmd.Parameters.Add("@CollegeID", SqlDbType.Int)
                        .Value = collegeId;

                    int collegeExists =
                        Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (collegeExists == 0)
                    {
                        throw new InvalidOperationException(
                            "The selected college does not exist or is inactive.");
                    }
                }

                // Insert Sub-TPO.
                // ParentTPOID must be populated by the controller
                // from the logged-in main TPO's session.
                string query = @"
            INSERT INTO tblTPORegistration
            (
                FullName,
                CollegeMailID,
                MobileNo,
                PasswordHash,
                CollegeName,
                CollegeCode,
                CollegeAddress,
                OrganizationWebsiteURL,
                Designation,
                DepartmentName,
                CollegeEmployeeID,
                SupportingDocument1,
                SupportingDocument2,
                ProfilePhoto,
                OTP,
                OTPVerified,
                Status,
                IsApproved,
                IsActive,
                CreatedDate,
                UpdatedDate,
                subtopbit,
                ParentTPOID
            )
            VALUES
            (
                @FullName,
                @CollegeMailID,
                @MobileNo,
                @PasswordHash,
                @CollegeName,
                @CollegeCode,
                @CollegeAddress,
                @OrganizationWebsiteURL,
                @Designation,
                @DepartmentName,
                @CollegeEmployeeID,
                @SupportingDocument1,
                @SupportingDocument2,
                @ProfilePhoto,
                @OTP,
                @OTPVerified,
                @Status,
                @IsApproved,
                @IsActive,
                GETDATE(),
                GETDATE(),
                1,
                @ParentTPOID
            );";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 150)
                        .Value = (object?)model.FullName ?? DBNull.Value;

                    cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 150)
                        .Value = (object?)model.CollegeMailID ?? DBNull.Value;

                    cmd.Parameters.Add("@MobileNo", SqlDbType.NVarChar, 20)
                        .Value = (object?)model.MobileNo ?? DBNull.Value;

                    // IMPORTANT:
                    // Store a password hash here, not the raw password.
                    // Use the same hashing mechanism as your TPO login.
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500)
                        .Value = (object?)model.Password ?? DBNull.Value;

                    // Store numeric college ID as text in the existing
                    // CollegeName column, without changing the schema.
                    cmd.Parameters.Add("@CollegeName", SqlDbType.NVarChar, 250)
                        .Value = collegeId.ToString();

                    cmd.Parameters.Add("@CollegeCode", SqlDbType.NVarChar, 50)
                        .Value = (object?)model.CollegeCode ?? DBNull.Value;

                    cmd.Parameters.Add("@CollegeAddress", SqlDbType.NVarChar, 500)
                        .Value = (object?)model.CollegeAddress ?? DBNull.Value;

                    cmd.Parameters.Add("@OrganizationWebsiteURL", SqlDbType.NVarChar, 250)
                        .Value = string.IsNullOrWhiteSpace(model.OrganizationWebsiteURL)
                            ? DBNull.Value
                            : (object)model.OrganizationWebsiteURL.Trim();

                    cmd.Parameters.Add("@Designation", SqlDbType.NVarChar, 150)
                        .Value = (object?)model.Designation ?? DBNull.Value;

                    cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 150)
                        .Value = (object?)model.DepartmentName ?? DBNull.Value;

                    cmd.Parameters.Add("@CollegeEmployeeID", SqlDbType.NVarChar, 100)
                        .Value = (object?)model.CollegeEmployeeID ?? DBNull.Value;

                    cmd.Parameters.Add("@SupportingDocument1", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.SupportingDocument1)
                            ? DBNull.Value
                            : (object)model.SupportingDocument1;

                    cmd.Parameters.Add("@SupportingDocument2", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.SupportingDocument2)
                            ? DBNull.Value
                            : (object)model.SupportingDocument2;

                    cmd.Parameters.Add("@ProfilePhoto", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.ProfilePhoto)
                            ? DBNull.Value
                            : (object)model.ProfilePhoto;

                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 10)
                        .Value = string.IsNullOrWhiteSpace(model.OTP)
                            ? DBNull.Value
                            : (object)model.OTP;

                    cmd.Parameters.Add("@OTPVerified", SqlDbType.Bit)
                        .Value = model.OTPVerified;

                    // New Sub-TPO registration awaits approval.
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50)
                        .Value = "Pending";

                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit)
                        .Value = false;

                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit)
                        .Value = false;

                    // ParentTPOID must be set by the controller.
                    if (model.ParentTPOID <= 0)
                    {
                        throw new InvalidOperationException(
                            "The parent TPO could not be identified.");
                    }

                    cmd.Parameters.Add("@ParentTPOID", SqlDbType.Int)
                        .Value = model.ParentTPOID;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }




        //public List<TPORegistration> GetAllTPODetails()
        //{
        //    List<TPORegistration> list = new List<TPORegistration>();

        //    using (SqlConnection cn = _db.GetConnection())
        //    {
        //        string query = @"
        //    SELECT
        //        TPOID,
        //        FullName,
        //        CollegeMailID,
        //        MobileNo,
        //        CollegeName,
        //        CollegeCode,
        //        CollegeAddress,
        //        OrganizationWebsiteURL,
        //        Designation,
        //        DepartmentName,
        //        CollegeEmployeeID,
        //        SupportingDocument1,
        //        SupportingDocument2,
        //        ProfilePhoto,
        //        OTP,
        //        OTPVerified,
        //        Status,
        //        IsApproved,
        //        IsActive,
        //        CreatedDate,
        //        UpdatedDate,

        //        CASE
        //            WHEN ISNULL(subtopbit, 0) = 1
        //                THEN 'Sub TPO'
        //            ELSE 'TPO'
        //        END AS TPOType

        //    FROM tblTPORegistration
        //    ORDER BY TPOID ASC";

        //        using (SqlCommand cmd = new SqlCommand(query, cn))
        //        {
        //            cn.Open();

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    TPORegistration tpo = new TPORegistration
        //                    {
        //                        TPOID =
        //                            Convert.ToInt32(reader["TPOID"]),

        //                        FullName =
        //                            reader["FullName"]?.ToString(),

        //                        CollegeMailID =
        //                            reader["CollegeMailID"]?.ToString(),

        //                        MobileNo =
        //                            reader["MobileNo"]?.ToString(),

        //                        CollegeName =
        //                            reader["CollegeName"]?.ToString(),

        //                        CollegeCode =
        //                            reader["CollegeCode"]?.ToString(),

        //                        CollegeAddress =
        //                            reader["CollegeAddress"]?.ToString(),

        //                        OrganizationWebsiteURL =
        //                            reader["OrganizationWebsiteURL"]?.ToString(),

        //                        Designation =
        //                            reader["Designation"]?.ToString(),

        //                        DepartmentName =
        //                            reader["DepartmentName"]?.ToString(),

        //                        CollegeEmployeeID =
        //                            reader["CollegeEmployeeID"]?.ToString(),

        //                        SupportingDocument1 =
        //                            reader["SupportingDocument1"]?.ToString(),

        //                        SupportingDocument2 =
        //                            reader["SupportingDocument2"]?.ToString(),

        //                        ProfilePhoto =
        //                            reader["ProfilePhoto"]?.ToString(),

        //                        OTP =
        //                            reader["OTP"]?.ToString(),

        //                        OTPVerified =
        //                            reader["OTPVerified"] != DBNull.Value &&
        //                            Convert.ToBoolean(reader["OTPVerified"]),

        //                        Status =
        //                            reader["Status"]?.ToString(),

        //                        IsApproved =
        //                            reader["IsApproved"] != DBNull.Value &&
        //                            Convert.ToBoolean(reader["IsApproved"]),

        //                        IsActive =
        //                            reader["IsActive"] != DBNull.Value &&
        //                            Convert.ToBoolean(reader["IsActive"]),

        //                        CreatedDate =
        //                            reader["CreatedDate"] == DBNull.Value
        //                                ? null
        //                                : Convert.ToDateTime(reader["CreatedDate"]),

        //                        UpdatedDate =
        //                            reader["UpdatedDate"] == DBNull.Value
        //                                ? null
        //                                : Convert.ToDateTime(reader["UpdatedDate"])
        //                    };

        //                    // Store TPO Type using ViewBag later
        //                    list.Add(tpo);
        //                }
        //            }
        //        }
        //    }

        //    return list;
        //}

        // shriang 05/10/26
        public List<TPORegistration> GetAllTPODetails()
        {
            List<TPORegistration> list = new List<TPORegistration>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                TPOID,
                FullName,
                CollegeMailID,
                MobileNo,
                CollegeName,
                CollegeCode,
                CollegeAddress,
                OrganizationWebsiteURL,
                Designation,
                DepartmentName,
                CollegeEmployeeID,
                SupportingDocument1,
                SupportingDocument2,
                ProfilePhoto,
                OTP,
                OTPVerified,
                Status,
                IsApproved,
                IsActive,
                CreatedDate,
                UpdatedDate,
                subtopbit
            FROM tblTPORegistration
            ORDER BY TPOID ASC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TPORegistration tpo = new TPORegistration
                            {
                                TPOID = Convert.ToInt32(reader["TPOID"]),

                                FullName = reader["FullName"]?.ToString(),

                                CollegeMailID =
                                    reader["CollegeMailID"]?.ToString(),

                                MobileNo =
                                    reader["MobileNo"]?.ToString(),

                                CollegeName =
                                    reader["CollegeName"]?.ToString(),

                                CollegeCode =
                                    reader["CollegeCode"]?.ToString(),

                                CollegeAddress =
                                    reader["CollegeAddress"]?.ToString(),

                                OrganizationWebsiteURL =
                                    reader["OrganizationWebsiteURL"]?.ToString(),

                                Designation =
                                    reader["Designation"]?.ToString(),

                                DepartmentName =
                                    reader["DepartmentName"]?.ToString(),

                                CollegeEmployeeID =
                                    reader["CollegeEmployeeID"]?.ToString(),

                                SupportingDocument1 =
                                    reader["SupportingDocument1"]?.ToString(),

                                SupportingDocument2 =
                                    reader["SupportingDocument2"]?.ToString(),

                                ProfilePhoto =
                                    reader["ProfilePhoto"]?.ToString(),

                                OTP =
                                    reader["OTP"]?.ToString(),

                                OTPVerified =
                                    reader["OTPVerified"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["OTPVerified"]),

                                Status =
                                    reader["Status"]?.ToString(),

                                IsApproved =
                                    reader["IsApproved"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["IsApproved"]),

                                IsActive =
                                    reader["IsActive"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["IsActive"]),

                                CreatedDate =
                                    reader["CreatedDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["CreatedDate"]),

                                SubTpoBit = reader["subtopbit"] == DBNull.Value
                                        ? null
                                        : Convert.ToBoolean(reader["subtopbit"]),

                                UpdatedDate =
                                    reader["UpdatedDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["UpdatedDate"])
                            };

                            list.Add(tpo);
                        }
                    }
                }
            }

            return list;
        }

        // shrirang 05/10/26
        public TPORegistration? GetTPOById(int tpoId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            SELECT
                TPOID,
                FullName,
                CollegeMailID,
                MobileNo,
                CollegeName,
                CollegeCode,
                CollegeAddress,
                OrganizationWebsiteURL,
                Designation,
                DepartmentName,
                CollegeEmployeeID,
                SupportingDocument1,
                SupportingDocument2,
                ProfilePhoto,
                Status,
                IsApproved,
                IsActive,
                CreatedDate,
                UpdatedDate,
                subtopbit
            FROM tblTPORegistration
            WHERE TPOID = @TPOID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new TPORegistration
                        {
                            TPOID = Convert.ToInt32(reader["TPOID"]),

                            FullName = reader["FullName"] == DBNull.Value
                                ? ""
                                : reader["FullName"].ToString(),

                            CollegeMailID = reader["CollegeMailID"] == DBNull.Value
                                ? ""
                                : reader["CollegeMailID"].ToString(),

                            MobileNo = reader["MobileNo"] == DBNull.Value
                                ? ""
                                : reader["MobileNo"].ToString(),

                            CollegeName = reader["CollegeName"] == DBNull.Value
                                ? ""
                                : reader["CollegeName"].ToString(),

                            CollegeCode = reader["CollegeCode"] == DBNull.Value
                                ? ""
                                : reader["CollegeCode"].ToString(),

                            CollegeAddress = reader["CollegeAddress"] == DBNull.Value
                                ? ""
                                : reader["CollegeAddress"].ToString(),

                            OrganizationWebsiteURL =
                                reader["OrganizationWebsiteURL"] == DBNull.Value
                                ? ""
                                : reader["OrganizationWebsiteURL"].ToString(),

                            Designation = reader["Designation"] == DBNull.Value
                                ? ""
                                : reader["Designation"].ToString(),

                            DepartmentName = reader["DepartmentName"] == DBNull.Value
                                ? ""
                                : reader["DepartmentName"].ToString(),

                            CollegeEmployeeID =
                                reader["CollegeEmployeeID"] == DBNull.Value
                                ? ""
                                : reader["CollegeEmployeeID"].ToString(),

                            SupportingDocument1 =
                                reader["SupportingDocument1"] == DBNull.Value
                                ? null
                                : reader["SupportingDocument1"].ToString(),

                            SupportingDocument2 =
                                reader["SupportingDocument2"] == DBNull.Value
                                ? null
                                : reader["SupportingDocument2"].ToString(),

                            ProfilePhoto =
                                reader["ProfilePhoto"] == DBNull.Value
                                ? null
                                : reader["ProfilePhoto"].ToString(),

                            Status =
                                reader["Status"] == DBNull.Value
                                ? null
                                : reader["Status"].ToString(),

                            IsApproved =
                                reader["IsApproved"] != DBNull.Value &&
                                Convert.ToBoolean(reader["IsApproved"]),

                            IsActive =
                                reader["IsActive"] != DBNull.Value &&
                                Convert.ToBoolean(reader["IsActive"]),

                            CreatedDate =
                                reader["CreatedDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["CreatedDate"]),

                            UpdatedDate =
                                reader["UpdatedDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["UpdatedDate"])
                        };
                    }
                }
            }
        }

        public bool UpdateTPOProfile(TPORegistration model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            UPDATE tblTPORegistration
            SET
                FullName = @FullName,
                CollegeMailID = @CollegeMailID,
                MobileNo = @MobileNo,
                CollegeName = @CollegeName,
                CollegeCode = @CollegeCode,
                CollegeAddress = @CollegeAddress,
                OrganizationWebsiteURL = @OrganizationWebsiteURL,
                Designation = @Designation,
                DepartmentName = @DepartmentName,
                CollegeEmployeeID = @CollegeEmployeeID,
                SupportingDocument1 = 
                    CASE
                        WHEN @SupportingDocument1 IS NOT NULL
                             AND @SupportingDocument1 <> ''
                        THEN @SupportingDocument1
                        ELSE SupportingDocument1
                    END,
                SupportingDocument2 =
                    CASE
                        WHEN @SupportingDocument2 IS NOT NULL
                             AND @SupportingDocument2 <> ''
                        THEN @SupportingDocument2
                        ELSE SupportingDocument2
                    END,
                ProfilePhoto =
                    CASE
                        WHEN @ProfilePhoto IS NOT NULL
                             AND @ProfilePhoto <> ''
                        THEN @ProfilePhoto
                        ELSE ProfilePhoto
                    END,
                UpdatedDate = GETDATE()
            WHERE TPOID = @TPOID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@TPOID", SqlDbType.Int)
                        .Value = model.TPOID;

                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 150)
                        .Value = model.FullName ?? "";

                    cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 200)
                        .Value = model.CollegeMailID ?? "";

                    cmd.Parameters.Add("@MobileNo", SqlDbType.NVarChar, 15)
                        .Value = model.MobileNo ?? "";

                    cmd.Parameters.Add("@CollegeName", SqlDbType.NVarChar, 250)
                        .Value = model.CollegeName ?? "";

                    cmd.Parameters.Add("@CollegeCode", SqlDbType.NVarChar, 100)
                        .Value = string.IsNullOrWhiteSpace(model.CollegeCode)
                            ? DBNull.Value
                            : model.CollegeCode;

                    cmd.Parameters.Add("@CollegeAddress", SqlDbType.NVarChar, 500)
                        .Value = model.CollegeAddress ?? "";

                    cmd.Parameters.Add("@OrganizationWebsiteURL", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.OrganizationWebsiteURL)
                            ? DBNull.Value
                            : model.OrganizationWebsiteURL;

                    cmd.Parameters.Add("@Designation", SqlDbType.NVarChar, 150)
                        .Value = model.Designation ?? "";

                    cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 150)
                        .Value = model.DepartmentName ?? "";

                    cmd.Parameters.Add("@CollegeEmployeeID", SqlDbType.NVarChar, 100)
                        .Value = model.CollegeEmployeeID ?? "";

                    cmd.Parameters.Add("@SupportingDocument1", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.SupportingDocument1)
                            ? DBNull.Value
                            : model.SupportingDocument1;

                    cmd.Parameters.Add("@SupportingDocument2", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.SupportingDocument2)
                            ? DBNull.Value
                            : model.SupportingDocument2;

                    cmd.Parameters.Add("@ProfilePhoto", SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(model.ProfilePhoto)
                            ? DBNull.Value
                            : model.ProfilePhoto;

                    cn.Open();

                    int rows = cmd.ExecuteNonQuery();

                    return rows > 0;
                }
            }
        }


        //public bool UpdateTPOProfile(TPORegistration model)
        //{
        //    using (SqlConnection cn = _db.GetConnection())
        //    {
        //        string query = @"
        //    UPDATE tblTPORegistration
        //    SET
        //        FullName = @FullName,
        //        CollegeMailID = @CollegeMailID,
        //        MobileNo = @MobileNo,
        //        CollegeName = @CollegeName,
        //        CollegeCode = @CollegeCode,
        //        CollegeAddress = @CollegeAddress,
        //        OrganizationWebsiteURL = @OrganizationWebsiteURL,
        //        Designation = @Designation,
        //        DepartmentName = @DepartmentName,
        //        CollegeEmployeeID = @CollegeEmployeeID,

        //        SupportingDocument1 =
        //            CASE
        //                WHEN @SupportingDocument1 IS NULL
        //                     OR @SupportingDocument1 = ''
        //                THEN SupportingDocument1
        //                ELSE @SupportingDocument1
        //            END,

        //        SupportingDocument2 =
        //            CASE
        //                WHEN @SupportingDocument2 IS NULL
        //                     OR @SupportingDocument2 = ''
        //                THEN SupportingDocument2
        //                ELSE @SupportingDocument2
        //            END,

        //        ProfilePhoto =
        //            CASE
        //                WHEN @ProfilePhoto IS NULL
        //                     OR @ProfilePhoto = ''
        //                THEN ProfilePhoto
        //                ELSE @ProfilePhoto
        //            END,

        //        UpdatedDate = GETDATE()

        //    WHERE TPOID = @TPOID";

        //        using (SqlCommand cmd = new SqlCommand(query, cn))
        //        {
        //            cmd.Parameters.Add("@TPOID", SqlDbType.Int)
        //                .Value = model.TPOID;

        //            cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 150)
        //                .Value = model.FullName?.Trim() ?? "";

        //            cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 200)
        //                .Value = model.CollegeMailID?.Trim() ?? "";

        //            cmd.Parameters.Add("@MobileNo", SqlDbType.NVarChar, 20)
        //                .Value = model.MobileNo?.Trim() ?? "";

        //            cmd.Parameters.Add("@CollegeName", SqlDbType.NVarChar, 250)
        //                .Value = model.CollegeName?.Trim() ?? "";

        //            cmd.Parameters.Add("@CollegeCode", SqlDbType.NVarChar, 100)
        //                .Value = string.IsNullOrWhiteSpace(model.CollegeCode)
        //                    ? DBNull.Value
        //                    : model.CollegeCode.Trim();

        //            cmd.Parameters.Add("@CollegeAddress", SqlDbType.NVarChar, 500)
        //                .Value = model.CollegeAddress?.Trim() ?? "";

        //            cmd.Parameters.Add("@OrganizationWebsiteURL", SqlDbType.NVarChar, 500)
        //                .Value = string.IsNullOrWhiteSpace(model.OrganizationWebsiteURL)
        //                    ? DBNull.Value
        //                    : model.OrganizationWebsiteURL.Trim();

        //            cmd.Parameters.Add("@Designation", SqlDbType.NVarChar, 150)
        //                .Value = model.Designation?.Trim() ?? "";

        //            cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 150)
        //                .Value = model.DepartmentName?.Trim() ?? "";

        //            cmd.Parameters.Add("@CollegeEmployeeID", SqlDbType.NVarChar, 100)
        //                .Value = model.CollegeEmployeeID?.Trim() ?? "";

        //            cmd.Parameters.Add("@SupportingDocument1", SqlDbType.NVarChar, 500)
        //                .Value = string.IsNullOrWhiteSpace(model.SupportingDocument1)
        //                    ? DBNull.Value
        //                    : model.SupportingDocument1;

        //            cmd.Parameters.Add("@SupportingDocument2", SqlDbType.NVarChar, 500)
        //                .Value = string.IsNullOrWhiteSpace(model.SupportingDocument2)
        //                    ? DBNull.Value
        //                    : model.SupportingDocument2;

        //            cmd.Parameters.Add("@ProfilePhoto", SqlDbType.NVarChar, 500)
        //                .Value = string.IsNullOrWhiteSpace(model.ProfilePhoto)
        //                    ? DBNull.Value
        //                    : model.ProfilePhoto;

        //            cn.Open();

        //            return cmd.ExecuteNonQuery() > 0;
        //        }
        //    }
        //}


        public bool TPOMobileExists(string mobileNo)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
                return false;

            using (Microsoft.Data.SqlClient.SqlConnection cn =
                   _db.GetConnection())
            {
                const string query = @"
     SELECT CASE WHEN EXISTS
     (
         SELECT 1
         FROM tblTPORegistration
         WHERE LTRIM(RTRIM(MobileNo)) = LTRIM(RTRIM(@MobileNo))
     )
     THEN 1 ELSE 0 END;";

                using (Microsoft.Data.SqlClient.SqlCommand cmd =
                       new Microsoft.Data.SqlClient.SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@MobileNo",
                        System.Data.SqlDbType.VarChar,
                        15
                    ).Value = mobileNo.Trim();

                    cn.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
        }
    }
}
