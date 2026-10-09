using ErJobPortal.Controllers;
using ErJobPortal.Data;
using ErJobPortal.Models;
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

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        model.sPassword ?? ""
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
                        (object?)model.sOrgName ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sOrgUrl",
                        (object?)model.sOrgUrl ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sName",
                        (object?)model.sName ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sDesignation",
                        (object?)model.sDesignation ?? DBNull.Value
                    );

                    // Department
                    cmd.Parameters.AddWithValue(
                        "@nDepartment",
                        model.nDepartment
                    );

                    // Branch
                    cmd.Parameters.AddWithValue(
                        "@nBranch",
                        model.nBranch
                    );

                    cmd.Parameters.AddWithValue(
                        "@sMobile",
                        (object?)model.sMobile ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        (object?)model.sEmail ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@nCollegeCode",
                        model.nCollegeCode
                    );

                    cmd.Parameters.AddWithValue(
                        "@nCollegeName",
                        (object?)model.nCollegeName ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        (object?)model.sPassword ?? DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@sOTP",
                        (object?)model.sOTP ?? DBNull.Value
                    );

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
                using (SqlCommand cmd =
                       new SqlCommand("SP_OrganizationLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        model.sEmail ?? ""
                    );

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        model.sPassword ?? ""
                    );

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new OrganizationUser
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

                                // IMPORTANT
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

      
        #region "Candidate Login"

        public CandidateUser? LoginCandidate(CandidateLogin model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd =
                       new SqlCommand("SP_CandidateLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        model.sEmail ?? ""
                    );

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        model.sPassword ?? ""
                    );

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

                                sProfileImage = dr["sProfileImage"] == DBNull.Value
                                    ? ""
                                    : dr["sProfileImage"].ToString(),

                                nCollegeCode = dr["nCollegeCode"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nCollegeCode"]),

                                sCollegeName = dr["sCollegeName"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["sCollegeName"]),

                                // Super Admin Enable / Disable status
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

        public SALoginModel Login(string email, string password)
        {
            SALoginModel model = null;

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();

                string query = @"SELECT *
                  FROM tblSuperAdmin
                  WHERE nBit = 1
                  AND sEmail = @Email
                  AND sPassword = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SALoginModel
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                SAID = Convert.ToInt32(dr["SAID"]),
                                sFName = dr["sFName"].ToString(),
                                sLName = dr["sLName"].ToString(),
                                sEmail = dr["sEmail"].ToString(),
                                sMobile = dr["sMobile"].ToString(),
                                sRole = dr["sRole"].ToString()
                            };
                        }
                    }
                }
            }

            return model;
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
            List<OrganizationUser> list =
                new List<OrganizationUser>();

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


        public bool ResetCandidatePassword(string email, string newPassword) { using (SqlConnection cn = _db.GetConnection()) { string query = @" UPDATE tblCandidateRegister SET sPassword = @NewPassword, ModDate = GETDATE() WHERE sEmail = @Email AND nBit = 1"; using (SqlCommand cmd = new SqlCommand(query, cn)) { cmd.Parameters.Add("@NewPassword", SqlDbType.NVarChar, 300).Value = newPassword; cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim(); cn.Open(); int rowsAffected = cmd.ExecuteNonQuery(); return rowsAffected > 0; } } }


       
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
              AND nBit = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@NewPassword",
                        SqlDbType.NVarChar,
                        300
                    ).Value = newPassword;

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
        public bool ResetSuperAdminPassword(string email, string newPassword) { using (SqlConnection cn = _db.GetConnection()) { string query = @" UPDATE tblSuperAdmin SET sPassword = @NewPassword, ModDate = GETDATE() WHERE sEmail = @Email AND nBit = 1"; using (SqlCommand cmd = new SqlCommand(query, cn)) { cmd.Parameters.Add("@NewPassword", SqlDbType.NVarChar, 300).Value = newPassword; cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email.Trim(); cn.Open(); int rowsAffected = cmd.ExecuteNonQuery(); return rowsAffected > 0; } } }



        // TPO REGISTRATION//

        // shrirang 23/09/26

        // ==========================================================
        // TPO REGISTRATION
        // ==========================================================

        public int RegisterTPO(TPORegistration model)
        {
            // ==========================================================
            // HASH PASSWORD
            // ==========================================================

            var passwordHasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<string>();

            string passwordHash =
                passwordHasher.HashPassword(
                    model.CollegeMailID,
                    model.Password
                );


            // ==========================================================
            // DATABASE CONNECTION
            // ==========================================================

            using (SqlConnection cn = _db.GetConnection())
            {
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
                CreatedDate
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
                GETDATE()
            )";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    // ==================================================
                    // BASIC DETAILS
                    // ==================================================

                    cmd.Parameters.Add(
                        "@FullName",
                        SqlDbType.NVarChar,
                        150
                    ).Value =
                        model.FullName ?? "";

                    cmd.Parameters.Add(
                        "@CollegeMailID",
                        SqlDbType.NVarChar,
                        200
                    ).Value =
                        model.CollegeMailID ?? "";

                    cmd.Parameters.Add(
                        "@MobileNo",
                        SqlDbType.VarChar,
                        15
                    ).Value =
                        model.MobileNo ?? "";


                    // ==================================================
                    // PASSWORD HASH
                    // ==================================================

                    cmd.Parameters.Add(
                        "@PasswordHash",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        passwordHash;


                    // ==================================================
                    // COLLEGE DETAILS
                    // ==================================================

                    cmd.Parameters.Add(
                        "@CollegeName",
                        SqlDbType.NVarChar,
                        250
                    ).Value =
                        model.CollegeName ?? "";

                    cmd.Parameters.Add(
                        "@CollegeCode",
                        SqlDbType.NVarChar,
                        100
                    ).Value =
                        string.IsNullOrWhiteSpace(model.CollegeCode)
                            ? (object)DBNull.Value
                            : model.CollegeCode;

                    cmd.Parameters.Add(
                        "@CollegeAddress",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        model.CollegeAddress ?? "";

                    cmd.Parameters.Add(
                        "@OrganizationWebsiteURL",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            model.OrganizationWebsiteURL)
                            ? (object)DBNull.Value
                            : model.OrganizationWebsiteURL;


                    // ==================================================
                    // EMPLOYEE DETAILS
                    // ==================================================

                    cmd.Parameters.Add(
                        "@Designation",
                        SqlDbType.NVarChar,
                        150
                    ).Value =
                        model.Designation ?? "";

                    cmd.Parameters.Add(
                        "@DepartmentName",
                        SqlDbType.NVarChar,
                        150
                    ).Value =
                        model.DepartmentName ?? "";

                    cmd.Parameters.Add(
                        "@CollegeEmployeeID",
                        SqlDbType.NVarChar,
                        100
                    ).Value =
                        model.CollegeEmployeeID ?? "";


                    // ==================================================
                    // DOCUMENTS
                    // ==================================================

                    cmd.Parameters.Add(
                        "@SupportingDocument1",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            model.SupportingDocument1)
                            ? (object)DBNull.Value
                            : model.SupportingDocument1;

                    cmd.Parameters.Add(
                        "@SupportingDocument2",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            model.SupportingDocument2)
                            ? (object)DBNull.Value
                            : model.SupportingDocument2;

                    cmd.Parameters.Add(
                        "@ProfilePhoto",
                        SqlDbType.NVarChar,
                        500
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            model.ProfilePhoto)
                            ? (object)DBNull.Value
                            : model.ProfilePhoto;


                    // ==================================================
                    // OTP
                    // ==================================================

                    cmd.Parameters.Add(
                        "@OTP",
                        SqlDbType.VarChar,
                        10
                    ).Value =
                        string.IsNullOrWhiteSpace(model.OTP)
                            ? (object)DBNull.Value
                            : model.OTP;

                    cmd.Parameters.Add(
                        "@OTPVerified",
                        SqlDbType.Bit
                    ).Value =
                        model.OTPVerified;


                    // ==================================================
                    // APPROVAL STATUS
                    // ==================================================

                    cmd.Parameters.Add(
                        "@Status",
                        SqlDbType.NVarChar,
                        50
                    ).Value =
                        string.IsNullOrWhiteSpace(model.Status)
                            ? "Pending"
                            : model.Status;

                    cmd.Parameters.Add(
                        "@IsApproved",
                        SqlDbType.Bit
                    ).Value =
                        model.IsApproved;

                    cmd.Parameters.Add(
                        "@IsActive",
                        SqlDbType.Bit
                    ).Value =
                        model.IsActive;


                    // ==================================================
                    // EXECUTE
                    // ==================================================

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }



        // =========================================================
        // TPO LOGIN
        // =========================================================

        public TPOLoginResult LoginTPO(string collegeMailID)
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
                IsActive
            FROM tblTPORegistration
            WHERE CollegeMailID = @CollegeMailID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CollegeMailID", SqlDbType.NVarChar, 200)
                                  .Value = collegeMailID.Trim();

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TPOLoginResult
                            {
                                TPOID = Convert.ToInt32(reader["TPOID"]),

                                FullName = reader["FullName"].ToString(),

                                CollegeMailID =
                                    reader["CollegeMailID"].ToString(),

                                MobileNo =
                                    reader["MobileNo"].ToString(),

                                PasswordHash =
                                    reader["PasswordHash"].ToString(),

                                CollegeName =
                                    reader["CollegeName"].ToString(),

                                CollegeCode =
                                    reader["CollegeCode"] == DBNull.Value
                                        ? null
                                        : reader["CollegeCode"].ToString(),

                                Designation =
                                    reader["Designation"].ToString(),

                                DepartmentName =
                                    reader["DepartmentName"].ToString(),

                                OTPVerified =
                                    Convert.ToBoolean(reader["OTPVerified"]),

                                Status =
                                    reader["Status"].ToString(),

                                IsApproved =
                                    Convert.ToBoolean(reader["IsApproved"]),

                                IsActive =
                                    Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }

            return null;
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
            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
            UPDATE tblTPORegistration
            SET
                PasswordHash = @Password,
                UpdatedDate = GETDATE()
            WHERE CollegeMailID = @CollegeMailID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@Password",
                        SqlDbType.NVarChar,
                        500
                    ).Value = newPassword;

                    cmd.Parameters.Add(
                        "@CollegeMailID",
                        SqlDbType.NVarChar,
                        200
                    ).Value = collegeMailID.Trim();

                    cn.Open();

                    int result = cmd.ExecuteNonQuery();

                    return result > 0;
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
            using (SqlConnection cn = _db.GetConnection())
            {
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
                subtopbit
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
                1
            )";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@FullName",
                        model.FullName ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CollegeMailID",
                        model.CollegeMailID ?? "");

                    cmd.Parameters.AddWithValue(
                        "@MobileNo",
                        model.MobileNo ?? "");

                    // =====================================================
                    // PASSWORD
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@PasswordHash",
                        model.Password ?? "");

                    // =====================================================
                    // COLLEGE
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@CollegeName",
                        model.CollegeName ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CollegeCode",
                        model.CollegeCode ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CollegeAddress",
                        model.CollegeAddress ?? "");

                    cmd.Parameters.AddWithValue(
                        "@OrganizationWebsiteURL",
                        string.IsNullOrWhiteSpace(
                            model.OrganizationWebsiteURL)
                            ? (object)DBNull.Value
                            : model.OrganizationWebsiteURL);

                    // =====================================================
                    // PROFESSIONAL
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@Designation",
                        model.Designation ?? "");

                    cmd.Parameters.AddWithValue(
                        "@DepartmentName",
                        model.DepartmentName ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CollegeEmployeeID",
                        model.CollegeEmployeeID ?? "");

                    // =====================================================
                    // DOCUMENTS
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@SupportingDocument1",
                        string.IsNullOrWhiteSpace(
                            model.SupportingDocument1)
                            ? (object)DBNull.Value
                            : model.SupportingDocument1);

                    cmd.Parameters.AddWithValue(
                        "@SupportingDocument2",
                        string.IsNullOrWhiteSpace(
                            model.SupportingDocument2)
                            ? (object)DBNull.Value
                            : model.SupportingDocument2);

                    cmd.Parameters.AddWithValue(
                        "@ProfilePhoto",
                        string.IsNullOrWhiteSpace(
                            model.ProfilePhoto)
                            ? (object)DBNull.Value
                            : model.ProfilePhoto);

                    // =====================================================
                    // OTP
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@OTP",
                        string.IsNullOrWhiteSpace(model.OTP)
                            ? (object)DBNull.Value
                            : model.OTP);

                    cmd.Parameters.AddWithValue(
                        "@OTPVerified",
                        model.OTPVerified);

                    // =====================================================
                    // APPROVAL
                    // =====================================================

                    cmd.Parameters.AddWithValue(
                        "@Status",
                        string.IsNullOrWhiteSpace(model.Status)
                            ? "Pending"
                            : model.Status);

                    cmd.Parameters.AddWithValue(
                        "@IsApproved",
                        model.IsApproved);

                    cmd.Parameters.AddWithValue(
                        "@IsActive",
                        model.IsActive);

                    // =====================================================
                    // EXECUTE
                    // =====================================================

                    cn.Open();

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

        // shrirang 06/10/26
        // shrirang 07/10/26
        // =========================================================
        // TPO TRAINEE INFO
        // GET TRAINEES BY COLLEGE
        // =========================================================

        // =====================================================
        // shrirang 07/10/26
        // GET TPO TRAINEES BY COLLEGE
        // =====================================================
        // =====================================================
        // shrirang 07/10/26
        // GET TPO TRAINEES BY COLLEGE
        // =====================================================
        // =====================================================
        // shrirang 07/10/26
        // GET TPO TRAINEES BY COLLEGE
        // =====================================================
        // =====================================================
        // shrirang 08/10/26
        // GET TPO TRAINEES BY COLLEGE
        // USED FOR TPO TRAINEES + SEND NOTIFICATION
        // =====================================================
        #region "Get TPO Trainees By College"

        public List<TPOTraineeInfoM> GetTPOTraineesByCollege(
            TPOTraineeFilterM filter)
        {
            List<TPOTraineeInfoM> trainees =
                new List<TPOTraineeInfoM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
SELECT
    C.nID,
    C.sPrefix,
    C.sFName,
    C.sLName,
    C.sMobile,
    C.sEmail,
    C.DOB,
    C.nGender,
    C.sProfileImage,
    C.nCollegeCode,
    C.sCollegeName,
    C.nDepartment,
    C.nBranch,
    C.nCurrentYear,
    C.nAdmissionYear,
    C.nPassoutYear,
    C.RegDate,
    C.nSABit,
    CP.sResume AS SelectedResume

FROM tblCandidateRegister C

LEFT JOIN tblCandidateProfile CP
    ON CP.CandidateID = C.nID

WHERE
    TRY_CONVERT(INT, C.sCollegeName) = @CollegeID

    AND ISNULL(C.nBit, 0) = 1

    AND
    (
        @SearchText = ''
        OR ISNULL(C.sFName, '') LIKE '%' + @SearchText + '%'
        OR ISNULL(C.sLName, '') LIKE '%' + @SearchText + '%'
        OR ISNULL(C.sMobile, '') LIKE '%' + @SearchText + '%'
        OR ISNULL(C.sEmail, '') LIKE '%' + @SearchText + '%'
        OR CONVERT(VARCHAR(20), C.DOB, 105)
           LIKE '%' + @SearchText + '%'
    )

    AND
    (
        @DepartmentId IS NULL
        OR C.nDepartment = @DepartmentId
    )

    AND
    (
        @BranchId IS NULL
        OR C.nBranch = @BranchId
    )

    AND
    (
        @CurrentYear IS NULL
        OR C.nCurrentYear = @CurrentYear
    )

    AND
    (
        @AdmissionYear IS NULL
        OR C.nAdmissionYear = @AdmissionYear
    )

    AND
    (
        @PassoutYear IS NULL
        OR C.nPassoutYear = @PassoutYear
    )

    AND
    (
        @Gender IS NULL
        OR C.nGender = @Gender
    )

ORDER BY C.nID DESC;
";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    // =================================================
                    // COLLEGE
                    // =================================================

                    cmd.Parameters.Add(
                        "@CollegeID",
                        SqlDbType.Int).Value =
                            filter.CollegeId;


                    // =================================================
                    // SEARCH
                    // =================================================

                    cmd.Parameters.Add(
                        "@SearchText",
                        SqlDbType.NVarChar,
                        200).Value =
                            string.IsNullOrWhiteSpace(
                                filter.SearchText)
                            ? ""
                            : filter.SearchText.Trim();


                    // =================================================
                    // DEPARTMENT
                    // =================================================

                    cmd.Parameters.Add(
                        "@DepartmentId",
                        SqlDbType.Int).Value =
                            filter.DepartmentId.HasValue
                            ? filter.DepartmentId.Value
                            : DBNull.Value;


                    // =================================================
                    // BRANCH
                    // =================================================

                    cmd.Parameters.Add(
                        "@BranchId",
                        SqlDbType.Int).Value =
                            filter.BranchId.HasValue
                            ? filter.BranchId.Value
                            : DBNull.Value;


                    // =================================================
                    // CURRENT YEAR
                    // =================================================

                    cmd.Parameters.Add(
                        "@CurrentYear",
                        SqlDbType.Int).Value =
                            filter.CurrentYear.HasValue
                            ? filter.CurrentYear.Value
                            : DBNull.Value;


                    // =================================================
                    // ADMISSION YEAR
                    // =================================================

                    cmd.Parameters.Add(
                        "@AdmissionYear",
                        SqlDbType.Int).Value =
                            filter.AdmissionYear.HasValue
                            ? filter.AdmissionYear.Value
                            : DBNull.Value;


                    // =================================================
                    // PASSOUT YEAR
                    // =================================================

                    cmd.Parameters.Add(
                        "@PassoutYear",
                        SqlDbType.Int).Value =
                            filter.PassoutYear.HasValue
                            ? filter.PassoutYear.Value
                            : DBNull.Value;


                    // =================================================
                    // GENDER
                    // =================================================

                    cmd.Parameters.Add(
                        "@Gender",
                        SqlDbType.Int).Value =
                            filter.Gender.HasValue
                            ? filter.Gender.Value
                            : DBNull.Value;


                    cn.Open();


                    // =================================================
                    // READ STUDENTS
                    // =================================================

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TPOTraineeInfoM trainee =
                                new TPOTraineeInfoM
                                {
                                    nID =
                                        reader["nID"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            reader["nID"]),

                                    sPrefix =
                                        reader["sPrefix"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            reader["sPrefix"]),

                                    sFName =
                                        reader["sFName"] == DBNull.Value
                                        ? null
                                        : reader["sFName"].ToString(),

                                    sLName =
                                        reader["sLName"] == DBNull.Value
                                        ? null
                                        : reader["sLName"].ToString(),

                                    sMobile =
                                        reader["sMobile"] == DBNull.Value
                                        ? null
                                        : reader["sMobile"].ToString(),

                                    sEmail =
                                        reader["sEmail"] == DBNull.Value
                                        ? null
                                        : reader["sEmail"].ToString(),

                                    DOB =
                                        reader["DOB"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(
                                            reader["DOB"]),

                                    nGender =
                                        reader["nGender"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            reader["nGender"]),

                                    sProfileImage =
                                        reader["sProfileImage"] == DBNull.Value
                                        ? null
                                        : reader["sProfileImage"].ToString(),

                                    nCollegeCode =
                                        reader["nCollegeCode"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            reader["nCollegeCode"]),

                                    nCollegeName =
                                        reader["sCollegeName"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            reader["sCollegeName"]),

                                    nDepartment =
                                        reader["nDepartment"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            reader["nDepartment"]),

                                    nBranch =
                                        reader["nBranch"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            reader["nBranch"]),

                                    nCurrentYear =
                                        reader["nCurrentYear"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            reader["nCurrentYear"]),

                                    nAdmissionYear =
                                        reader["nAdmissionYear"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            reader["nAdmissionYear"]),

                                    nPassoutYear =
                                        reader["nPassoutYear"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            reader["nPassoutYear"]),

                                    RegDate =
                                        reader["RegDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(
                                            reader["RegDate"]),

                                    nSABit =
                                        reader["nSABit"] != DBNull.Value &&
                                        Convert.ToBoolean(
                                            reader["nSABit"]),

                                    SelectedResume =
                                        reader["SelectedResume"] == DBNull.Value
                                        ? null
                                        : reader["SelectedResume"].ToString()
                                };


                            // =================================================
                            // GENDER NAME
                            // =================================================

                            trainee.GenderName =
                                trainee.nGender switch
                                {
                                    1 => "Male",
                                    2 => "Female",
                                    3 => "Other",
                                    _ => "-"
                                };


                            // =================================================
                            // BRANCH NAME
                            // =================================================

                            trainee.BranchName = "-";


                            if (trainee.nBranch.HasValue)
                            {
                                List<BranchM> branchList = GetBranches();

                                BranchM? branch =
                                    branchList.FirstOrDefault(
                                        x => x.nID == trainee.nBranch.Value);

                                if (branch != null)
                                {
                                    trainee.BranchName = branch.sBranch;
                                }
                            }


                            trainees.Add(trainee);
                        }
                    }
                }
            }

            return trainees;
        }

        #endregion

        public int? GetDepartmentIdByName(string? departmentName)
        {
            // =====================================================
            // VALIDATE DEPARTMENT NAME
            // =====================================================

            if (string.IsNullOrWhiteSpace(departmentName))
            {
                return null;
            }


            // =====================================================
            // GET ALL DEPARTMENTS
            // =====================================================

            List<DepartmentM> departments =
                GetDepartments();


            if (departments == null ||
                departments.Count == 0)
            {
                return null;
            }


            // =====================================================
            // FIND DEPARTMENT
            // =====================================================

            DepartmentM? department =
                departments.FirstOrDefault(
                    x =>
                        !string.IsNullOrWhiteSpace(x.sDepartment)
                        &&
                        x.sDepartment.Trim().Equals(
                            departmentName.Trim(),
                            StringComparison.OrdinalIgnoreCase));


            // =====================================================
            // RETURN DEPARTMENT ID
            // =====================================================

            if (department == null)
            {
                return null;
            }


            return department.nID;
        }

        // shrirang 08/10/26
        // notification tpo
        public List<TPONotificationListM> GetTPONotificationList(
    int tpoId,
    TPONotificationFilterM filter)
        {
            List<TPONotificationListM> list =
                new List<TPONotificationListM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
SELECT
    NotificationID,
    Title,
    Type,
    Audience,
    NotificationDate,
    NotificationTime,
    Content,
    SendEmailNotification,

    Attachment1OriginalName,
    Attachment1Path,

    Attachment2OriginalName,
    Attachment2Path,

    CreatedBy,
    CreatedByName,

    IsActive,
    IsDeleted,
    CreatedDate

FROM tblTPONotification

WHERE CreatedBy = @CreatedBy
AND IsDeleted = 0
";

                if (!string.IsNullOrWhiteSpace(filter.Type))
                {
                    query += @"
AND Type = @Type
";
                }

                if (!string.IsNullOrWhiteSpace(filter.Audience))
                {
                    query += @"
AND Audience = @Audience
";
                }

                if (filter.FromDate.HasValue)
                {
                    query += @"
AND NotificationDate >= @FromDate
";
                }

                if (filter.ToDate.HasValue)
                {
                    query += @"
AND NotificationDate <= @ToDate
";
                }

                query += @"
ORDER BY CreatedDate DESC,
         NotificationID DESC";


                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@CreatedBy",
                        tpoId);


                    if (!string.IsNullOrWhiteSpace(filter.Type))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Type",
                            filter.Type);
                    }


                    if (!string.IsNullOrWhiteSpace(filter.Audience))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Audience",
                            filter.Audience);
                    }


                    if (filter.FromDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue(
                            "@FromDate",
                            filter.FromDate.Value.Date);
                    }


                    if (filter.ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue(
                            "@ToDate",
                            filter.ToDate.Value.Date);
                    }


                    cn.Open();


                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TPONotificationListM item =
                                new TPONotificationListM();


                            // ============================
                            // Basic Information
                            // ============================

                            item.NotificationID =
                                Convert.ToInt32(
                                    reader["NotificationID"]);


                            item.Title =
                                reader["Title"]?.ToString()
                                ?? string.Empty;


                            item.Type =
                                reader["Type"]?.ToString()
                                ?? string.Empty;


                            item.Audience =
                                reader["Audience"]?.ToString()
                                ?? string.Empty;


                            // ============================
                            // Date
                            // ============================

                            if (reader["NotificationDate"]
                                != DBNull.Value)
                            {
                                item.NotificationDate =
                                    Convert.ToDateTime(
                                        reader["NotificationDate"]);
                            }


                            // ============================
                            // Time
                            // ============================

                            if (reader["NotificationTime"]
                                != DBNull.Value)
                            {
                                item.NotificationTime =
                                    (TimeSpan)
                                    reader["NotificationTime"];
                            }


                            // ============================
                            // Content
                            // ============================

                            item.Content =
                                reader["Content"]?.ToString()
                                ?? string.Empty;


                            // ============================
                            // Email
                            // ============================

                            item.SendEmailNotification =
                                reader["SendEmailNotification"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader[
                                        "SendEmailNotification"]);


                            // ============================
                            // Attachment 1
                            // ============================

                            item.Attachment1OriginalName =
                                reader[
                                    "Attachment1OriginalName"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment1OriginalName"]
                                    .ToString()
                                :
                                null;


                            item.Attachment1Path =
                                reader[
                                    "Attachment1Path"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment1Path"]
                                    .ToString()
                                :
                                null;


                            // ============================
                            // Attachment 2
                            // ============================

                            item.Attachment2OriginalName =
                                reader[
                                    "Attachment2OriginalName"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment2OriginalName"]
                                    .ToString()
                                :
                                null;


                            item.Attachment2Path =
                                reader[
                                    "Attachment2Path"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment2Path"]
                                    .ToString()
                                :
                                null;


                            // ============================
                            // Created By
                            // ============================

                            if (reader["CreatedBy"]
                                != DBNull.Value)
                            {
                                item.CreatedBy =
                                    Convert.ToInt32(
                                        reader["CreatedBy"]);
                            }


                            item.CreatedByName =
                                reader["CreatedByName"]
                                != DBNull.Value
                                ?
                                reader["CreatedByName"]
                                    .ToString()
                                :
                                null;


                            // ============================
                            // Status
                            // ============================

                            item.IsActive =
                                reader["IsActive"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader["IsActive"]);


                            item.IsDeleted =
                                reader["IsDeleted"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader["IsDeleted"]);


                            // ============================
                            // Created Date
                            // ============================

                            item.CreatedDate =
                                Convert.ToDateTime(
                                    reader["CreatedDate"]);


                            // ============================
                            // Display Date / Time
                            // ============================

                            DateTime? notificationDateTime =
                                null;


                            if (item.NotificationDate.HasValue)
                            {
                                notificationDateTime =
                                    item.NotificationDate.Value.Date;


                                if (item.NotificationTime.HasValue)
                                {
                                    notificationDateTime =
                                        notificationDateTime.Value
                                        .Add(
                                            item.NotificationTime
                                            .Value);
                                }
                            }


                            if (notificationDateTime.HasValue)
                            {
                                item.DisplayDateTime =
                                    notificationDateTime.Value
                                    .ToString(
                                        "dd/MM/yyyy hh:mm tt");


                                if (notificationDateTime.Value
                                    < DateTime.Now)
                                {
                                    item.Status =
                                        "Completed";
                                }
                                else
                                {
                                    item.Status =
                                        "Active";
                                }
                            }
                            else
                            {
                                item.Status =
                                    item.IsActive
                                    ?
                                    "Active"
                                    :
                                    "Inactive";
                            }


                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

        // =====================================================
        // shrirang 08/10/26
        // TPO SEND NOTIFICATION
        // GET TPO NOTIFICATIONS
        // =====================================================
        #region "Get TPO Notifications For Send"

        public List<TPONotificationListM> GetTPONotificationsForSend(
            int tpoId)
        {
            List<TPONotificationListM> list =
                new List<TPONotificationListM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                string query = @"
SELECT
    N.NotificationID,
    N.Title,
    N.Type,
    N.Audience,
    N.NotificationDate,
    N.NotificationTime,
    N.Content,
    N.SendEmailNotification,

    N.Attachment1OriginalName,
    N.Attachment1Path,

    N.Attachment2OriginalName,
    N.Attachment2Path,

    N.CreatedBy,
    N.CreatedByName,

    N.IsActive,
    N.IsDeleted,
    N.CreatedDate,

    -- =====================================================
    -- NOTIFICATION TRACKING
    -- =====================================================

    ISNULL(T.TotalSent, 0) AS TotalSent,

    ISNULL(T.TotalRead, 0) AS TotalRead,

    ISNULL(T.TotalUnread, 0) AS TotalUnread

FROM tblTPONotification N

-- =========================================================
-- CALCULATE STUDENT NOTIFICATION TRACKING
-- =========================================================

OUTER APPLY
(
    SELECT
        COUNT(1) AS TotalSent,

        SUM(
    CASE
        WHEN ISNULL(R.IsRead, 0) = 1
        THEN 1
        ELSE 0
    END
) AS TotalRead,

SUM(
    CASE
        WHEN ISNULL(R.IsRead, 0) = 0
        THEN 1
        ELSE 0
    END
) AS TotalUnread

    FROM tblTPONotificationRecipient R

    WHERE
        R.NotificationID = N.NotificationID
        AND R.IsActive = 1

) T

WHERE
    N.CreatedBy = @CreatedBy

    AND N.IsDeleted = 0

ORDER BY
    N.CreatedDate DESC,
    N.NotificationID DESC;
";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add(
                        "@CreatedBy",
                        SqlDbType.Int).Value = tpoId;

                    cn.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TPONotificationListM item =
                                new TPONotificationListM();


                            // =================================================
                            // BASIC INFORMATION
                            // =================================================

                            item.NotificationID =
                                Convert.ToInt32(
                                    reader["NotificationID"]);

                            item.Title =
                                reader["Title"]?.ToString()
                                ?? string.Empty;

                            item.Type =
                                reader["Type"]?.ToString()
                                ?? string.Empty;

                            item.Audience =
                                reader["Audience"]?.ToString()
                                ?? string.Empty;


                            // =================================================
                            // DATE
                            // =================================================

                            if (reader["NotificationDate"]
                                != DBNull.Value)
                            {
                                item.NotificationDate =
                                    Convert.ToDateTime(
                                        reader["NotificationDate"]);
                            }


                            // =================================================
                            // TIME
                            // =================================================

                            if (reader["NotificationTime"]
                                != DBNull.Value)
                            {
                                item.NotificationTime =
                                    (TimeSpan)
                                    reader["NotificationTime"];
                            }


                            // =================================================
                            // CONTENT
                            // =================================================

                            item.Content =
                                reader["Content"]?.ToString()
                                ?? string.Empty;


                            // =================================================
                            // EMAIL
                            // =================================================

                            item.SendEmailNotification =
                                reader["SendEmailNotification"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader[
                                        "SendEmailNotification"]);


                            // =================================================
                            // ATTACHMENT 1
                            // =================================================

                            item.Attachment1OriginalName =
                                reader[
                                    "Attachment1OriginalName"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment1OriginalName"]
                                    .ToString()
                                :
                                null;

                            item.Attachment1Path =
                                reader[
                                    "Attachment1Path"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment1Path"]
                                    .ToString()
                                :
                                null;


                            // =================================================
                            // ATTACHMENT 2
                            // =================================================

                            item.Attachment2OriginalName =
                                reader[
                                    "Attachment2OriginalName"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment2OriginalName"]
                                    .ToString()
                                :
                                null;

                            item.Attachment2Path =
                                reader[
                                    "Attachment2Path"]
                                != DBNull.Value
                                ?
                                reader[
                                    "Attachment2Path"]
                                    .ToString()
                                :
                                null;


                            // =================================================
                            // CREATED BY
                            // =================================================

                            if (reader["CreatedBy"]
                                != DBNull.Value)
                            {
                                item.CreatedBy =
                                    Convert.ToInt32(
                                        reader["CreatedBy"]);
                            }

                            item.CreatedByName =
                                reader["CreatedByName"]
                                != DBNull.Value
                                ?
                                reader["CreatedByName"]
                                    .ToString()
                                :
                                null;


                            // =================================================
                            // ACTIVE / DELETED
                            // =================================================

                            item.IsActive =
                                reader["IsActive"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader["IsActive"]);

                            item.IsDeleted =
                                reader["IsDeleted"]
                                != DBNull.Value
                                &&
                                Convert.ToBoolean(
                                    reader["IsDeleted"]);


                            // =================================================
                            // CREATED DATE
                            // =================================================

                            item.CreatedDate =
                                Convert.ToDateTime(
                                    reader["CreatedDate"]);

                            // =================================================
                            // NOTIFICATION TRACKING
                            // =================================================

                            item.TotalSent =
                                reader["TotalSent"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalSent"])
                                : 0;

                            item.TotalRead =
                                reader["TotalRead"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalRead"])
                                : 0;

                            item.TotalUnread =
                                reader["TotalUnread"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalUnread"])
                                : 0;


                            // =================================================
                            // DISPLAY DATE / TIME
                            // =================================================

                            DateTime? notificationDateTime =
                                null;

                            if (item.NotificationDate.HasValue)
                            {
                                notificationDateTime =
                                    item.NotificationDate.Value.Date;

                                if (item.NotificationTime.HasValue)
                                {
                                    notificationDateTime =
                                        notificationDateTime.Value.Add(
                                            item.NotificationTime.Value);
                                }
                            }


                            if (notificationDateTime.HasValue)
                            {
                                item.DisplayDateTime =
                                    notificationDateTime.Value.ToString(
                                        "dd/MM/yyyy hh:mm tt");


                                if (notificationDateTime.Value
                                    < DateTime.Now)
                                {
                                    item.Status =
                                        "Completed";
                                }
                                else
                                {
                                    item.Status =
                                        "Active";
                                }
                            }
                            else
                            {
                                item.Status =
                                    item.IsActive
                                    ? "Active"
                                    : "Inactive";
                            }


                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

        #endregion


        #region "Send TPO Notification To Selected Candidates"

        public int SendTPONotificationToCandidates(
            int tpoId,
            int notificationId,
            int collegeId,
            List<int> candidateIds)
        {
            if (candidateIds == null || candidateIds.Count == 0)
            {
                return 0;
            }

            // Remove duplicate Candidate IDs
            candidateIds =
                candidateIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (candidateIds.Count == 0)
            {
                return 0;
            }

            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                using (SqlTransaction transaction =
                       cn.BeginTransaction())
                {
                    try
                    {
                        // =====================================================
                        // STEP 1
                        // VERIFY NOTIFICATION BELONGS TO THIS TPO
                        // =====================================================

                        string notificationQuery = @"
SELECT COUNT(1)
FROM tblTPONotification
WHERE
    NotificationID = @NotificationID
    AND CreatedBy = @TPOID
    AND ISNULL(IsDeleted, 0) = 0;
";

                        using (SqlCommand notificationCmd =
                               new SqlCommand(
                                   notificationQuery,
                                   cn,
                                   transaction))
                        {
                            notificationCmd.Parameters.Add(
                                "@NotificationID",
                                SqlDbType.Int).Value =
                                    notificationId;

                            notificationCmd.Parameters.Add(
                                "@TPOID",
                                SqlDbType.Int).Value =
                                    tpoId;

                            int notificationExists =
                                Convert.ToInt32(
                                    notificationCmd.ExecuteScalar());

                            if (notificationExists == 0)
                            {
                                throw new InvalidOperationException(
                                    "The selected notification does not belong to the logged-in TPO."
                                );
                            }
                        }


                        // =====================================================
                        // STEP 2
                        // CREATE PARAMETER LIST
                        // =====================================================

                        List<string> candidateParameters =
                            new List<string>();

                        for (int i = 0;
                             i < candidateIds.Count;
                             i++)
                        {
                            string parameterName =
                                "@CandidateID" + i;

                            candidateParameters.Add(
                                parameterName);
                        }


                        // =====================================================
                        // STEP 3
                        // INSERT ONLY VALID CANDIDATES
                        //
                        // SECURITY:
                        // Candidate must belong to the TPO's college.
                        // =====================================================

                        string insertQuery = $@"
INSERT INTO tblTPONotificationRecipient
(
    NotificationID,
    CandidateID,
    IsRead,
    SentDate,
    IsActive
)
SELECT
    @NotificationID,
    C.nID,
    0,
    GETDATE(),
    1

FROM tblCandidateRegister C

WHERE
    C.nID IN
    (
        {string.Join(",", candidateParameters)}
    )

    AND TRY_CONVERT(INT, C.sCollegeName) = @CollegeID

    AND ISNULL(C.nBit, 0) = 1

    AND NOT EXISTS
    (
        SELECT 1
        FROM tblTPONotificationRecipient R
        WHERE
            R.NotificationID =
                @NotificationID

            AND R.CandidateID =
                C.nID
    );
";


                        using (SqlCommand insertCmd =
                               new SqlCommand(
                                   insertQuery,
                                   cn,
                                   transaction))
                        {
                            // =================================================
                            // NOTIFICATION
                            // =================================================

                            insertCmd.Parameters.Add(
                                "@NotificationID",
                                SqlDbType.Int).Value =
                                    notificationId;


                            // =================================================
                            // COLLEGE
                            // =================================================

                            insertCmd.Parameters.Add(
                                "@CollegeID",
                                SqlDbType.Int).Value =
                                    collegeId;


                            // =================================================
                            // CANDIDATES
                            // =================================================

                            for (int i = 0;
                                 i < candidateIds.Count;
                                 i++)
                            {
                                insertCmd.Parameters.Add(
                                    candidateParameters[i],
                                    SqlDbType.Int).Value =
                                        candidateIds[i];
                            }


                            // =================================================
                            // EXECUTE
                            // =================================================

                            int insertedCount =
                                insertCmd.ExecuteNonQuery();


                            // =================================================
                            // COMMIT
                            // =================================================

                            transaction.Commit();


                            return insertedCount;
                        }
                    }
                    catch
                    {
                        // =================================================
                        // ROLLBACK
                        // =================================================

                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            // Ignore rollback error
                        }

                        throw;
                    }
                }
            }
        }

        #endregion


        // shrirang 08/10/26
        // candidate notification section
        public int GetCandidateUnreadNotificationCount(int candidateId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                string query = @"
SELECT COUNT(1)
FROM tblTPONotificationRecipient R
INNER JOIN tblTPONotification N
    ON N.NotificationID = R.NotificationID
WHERE
    R.CandidateID = @CandidateID
    AND R.IsActive = 1
    AND R.IsRead = 0
    AND N.IsActive = 1
    AND N.IsDeleted = 0;
";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                                  .Value = candidateId;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public List<CandidateNotificationM> GetCandidateNotifications(
    int candidateId)
        {
            List<CandidateNotificationM> notifications =
                new List<CandidateNotificationM>();

            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                string query = @"
SELECT
    R.RecipientID,
    R.NotificationID,

    N.Title,
    N.Type,
    N.Audience,
    N.NotificationDate,
    N.NotificationTime,
    N.Content,

    R.IsRead,
    R.ReadDate,
    R.SentDate,

    N.CreatedDate,

    N.Attachment1OriginalName,
    N.Attachment1Path,

    N.Attachment2OriginalName,
    N.Attachment2Path

FROM tblTPONotificationRecipient R

INNER JOIN tblTPONotification N
    ON N.NotificationID = R.NotificationID

WHERE
    R.CandidateID = @CandidateID
    AND R.IsActive = 1
    AND N.IsActive = 1
    AND N.IsDeleted = 0

ORDER BY
    R.SentDate DESC,
    R.RecipientID DESC;
";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                                  .Value = candidateId;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            CandidateNotificationM item =
                                new CandidateNotificationM();

                            item.RecipientID =
                                Convert.ToInt32(dr["RecipientID"]);

                            item.NotificationID =
                                Convert.ToInt32(dr["NotificationID"]);

                            item.Title =
                                dr["Title"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(dr["Title"])!;

                            item.Type =
                                dr["Type"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(dr["Type"])!;

                            item.Audience =
                                dr["Audience"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(dr["Audience"])!;

                            item.NotificationDate =
                                dr["NotificationDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(
                                        dr["NotificationDate"]);

                            item.NotificationTime =
                                dr["NotificationTime"] == DBNull.Value
                                    ? null
                                    : (TimeSpan?)dr["NotificationTime"];

                            item.Content =
                                dr["Content"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(dr["Content"])!;

                            item.IsRead =
                                dr["IsRead"] != DBNull.Value &&
                                Convert.ToBoolean(dr["IsRead"]);

                            item.ReadDate =
                                dr["ReadDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["ReadDate"]);

                            item.SentDate =
                                dr["SentDate"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(dr["SentDate"]);

                            item.CreatedDate =
                                dr["CreatedDate"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(dr["CreatedDate"]);

                            item.Attachment1OriginalName =
                                dr["Attachment1OriginalName"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        dr["Attachment1OriginalName"]);

                            item.Attachment1Path =
                                dr["Attachment1Path"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        dr["Attachment1Path"]);

                            item.Attachment2OriginalName =
                                dr["Attachment2OriginalName"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        dr["Attachment2OriginalName"]);

                            item.Attachment2Path =
                                dr["Attachment2Path"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        dr["Attachment2Path"]);

                            notifications.Add(item);
                        }
                    }
                }
            }

            return notifications;
        }

        public CandidateNotificationM? GetCandidateNotification(
    int candidateId,
    int notificationId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                string query = @"
SELECT
    R.RecipientID,
    R.NotificationID,

    N.Title,
    N.Type,
    N.Audience,
    N.NotificationDate,
    N.NotificationTime,
    N.Content,

    R.IsRead,
    R.ReadDate,
    R.SentDate,

    N.CreatedDate,

    N.Attachment1OriginalName,
    N.Attachment1Path,

    N.Attachment2OriginalName,
    N.Attachment2Path

FROM tblTPONotificationRecipient R

INNER JOIN tblTPONotification N
    ON N.NotificationID = R.NotificationID

WHERE
    R.CandidateID = @CandidateID
    AND R.NotificationID = @NotificationID
    AND R.IsActive = 1
    AND N.IsActive = 1
    AND N.IsDeleted = 0;
";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                                  .Value = candidateId;

                    cmd.Parameters.Add("@NotificationID", SqlDbType.Int)
                                  .Value = notificationId;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            return null;
                        }

                        CandidateNotificationM item =
                            new CandidateNotificationM();

                        item.RecipientID =
                            Convert.ToInt32(dr["RecipientID"]);

                        item.NotificationID =
                            Convert.ToInt32(dr["NotificationID"]);

                        item.Title =
                            dr["Title"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(dr["Title"])!;

                        item.Type =
                            dr["Type"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(dr["Type"])!;

                        item.Audience =
                            dr["Audience"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(dr["Audience"])!;

                        item.NotificationDate =
                            dr["NotificationDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    dr["NotificationDate"]);

                        item.NotificationTime =
                            dr["NotificationTime"] == DBNull.Value
                                ? null
                                : (TimeSpan?)dr["NotificationTime"];

                        item.Content =
                            dr["Content"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(dr["Content"])!;

                        item.IsRead =
                            dr["IsRead"] != DBNull.Value &&
                            Convert.ToBoolean(dr["IsRead"]);

                        item.ReadDate =
                            dr["ReadDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(dr["ReadDate"]);

                        item.SentDate =
                            dr["SentDate"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(dr["SentDate"]);

                        item.CreatedDate =
                            dr["CreatedDate"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(dr["CreatedDate"]);

                        item.Attachment1OriginalName =
                            dr["Attachment1OriginalName"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    dr["Attachment1OriginalName"]);

                        item.Attachment1Path =
                            dr["Attachment1Path"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    dr["Attachment1Path"]);

                        item.Attachment2OriginalName =
                            dr["Attachment2OriginalName"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    dr["Attachment2OriginalName"]);

                        item.Attachment2Path =
                            dr["Attachment2Path"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    dr["Attachment2Path"]);

                        return item;
                    }
                }
            }
        }

        public bool MarkCandidateNotificationAsRead(
    int candidateId,
    int notificationId)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                cn.Open();

                string query = @"
UPDATE R
SET
    R.IsRead = 1,
    R.ReadDate = GETDATE()
FROM tblTPONotificationRecipient R
INNER JOIN tblTPONotification N
    ON N.NotificationID = R.NotificationID
WHERE
    R.CandidateID = @CandidateID
    AND R.NotificationID = @NotificationID
    AND R.IsActive = 1
    AND N.IsActive = 1
    AND N.IsDeleted = 0
    AND R.IsRead = 0;
";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                                  .Value = candidateId;

                    cmd.Parameters.Add("@NotificationID", SqlDbType.Int)
                                  .Value = notificationId;

                    int rows =
                        cmd.ExecuteNonQuery();

                    return rows > 0;
                }
            }
        }
    }
}
