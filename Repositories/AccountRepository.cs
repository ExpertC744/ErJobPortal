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
        private readonly DbConnection _db;

        public AccountRepository(DbConnection db)
        {
            _db = db;
        }

        // Candidate Register 
        #region "Candidate Register"
        public int Register(CandidateRegister model) 
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_RegisterCandidate", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@sFName", model.sFName);
                    cmd.Parameters.AddWithValue("@sLName", model.sLName);
                    cmd.Parameters.AddWithValue("@sMobile", model.sMobile);
                    cmd.Parameters.AddWithValue("@sEmail", model.sEmail);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                    cmd.Parameters.AddWithValue("@nGender", model.nGender);
                    cmd.Parameters.AddWithValue("@sProfileImage", model.sProfileImage ?? "");
                    cmd.Parameters.AddWithValue("@nCollegeCode", model.nCollegeCode);
                    cmd.Parameters.AddWithValue("@sCollegeName", model.sCollegeName);
                    cmd.Parameters.AddWithValue("@sPassword", model.sPassword);
                    cmd.Parameters.AddWithValue("@sOTP", model.sOTP ?? "");
                    cmd.Parameters.AddWithValue("@nBranch", model.nBranch);
                    cmd.Parameters.AddWithValue("@nPassoutYear", model.nPassoutYear);
                    cn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        // Org Register
        #region "Organization Register"
        public int RegisterOrganization(OrganizationRegister model)
        {
            using (SqlConnection cn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_RegisterOrganization", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@sOrgName", model.sOrgName ?? "");
                    cmd.Parameters.AddWithValue("@sOrgUrl", model.sOrgUrl ?? "");
                    cmd.Parameters.AddWithValue("@sName", model.sName ?? "");
                    cmd.Parameters.AddWithValue("@sDesignation", model.sDesignation ?? "");
                    cmd.Parameters.AddWithValue("@sMobile", model.sMobile ?? "");
                    cmd.Parameters.AddWithValue("@sEmail", model.sEmail ?? "");
                    cmd.Parameters.AddWithValue("@nCollegeCode", model.nCollegeCode);
                    cmd.Parameters.AddWithValue("@nCollegeName", model.nCollegeName);
                    cmd.Parameters.AddWithValue("@sPassword", model.sPassword ?? "");
                    cmd.Parameters.AddWithValue("@sOTP", model.sOTP ?? "");
                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

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
                            return new OrganizationUser
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                sOrgName = dr["sOrgName"].ToString(),
                                sOrgUrl = dr["sOrgUrl"].ToString(),
                                sName = dr["sName"].ToString(),
                                sDesignation = dr["sDesignation"].ToString(),
                                sMobile = dr["sMobile"].ToString(),
                                sEmail = dr["sEmail"].ToString(),
                                nCollegeCode = Convert.ToInt32(dr["nCollegeCode"]),
                                nCollegeName = Convert.ToInt32(dr["nCollegeName"])
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
                using (SqlCommand cmd = new SqlCommand("SP_CandidateLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@sEmail", model.sEmail ?? "");
                    cmd.Parameters.AddWithValue("@sPassword", model.sPassword ?? "");
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new CandidateUser
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                sFName = dr["sFName"].ToString(),
                                sLName = dr["sLName"].ToString(),
                                sMobile = dr["sMobile"].ToString(),
                                sEmail = dr["sEmail"].ToString(),
                                DOB = dr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["DOB"]),
                                nGender = Convert.ToInt32(dr["nGender"]),
                                sProfileImage = dr["sProfileImage"].ToString(),
                                nCollegeCode = Convert.ToInt32(dr["nCollegeCode"]),
                                sCollegeName = Convert.ToInt32(dr["sCollegeName"])
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
                RegDate,
                ModDate,
                nBit,
                nSABit
            FROM tblCandidateRegister
            WHERE nBit = 1
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
                                    : dr["sFName"].ToString()!,

                                sLName = dr["sLName"] == DBNull.Value
                                    ? ""
                                    : dr["sLName"].ToString()!,

                                sMobile = dr["sMobile"] == DBNull.Value
                                    ? ""
                                    : dr["sMobile"].ToString()!,

                                sEmail = dr["sEmail"] == DBNull.Value
                                    ? ""
                                    : dr["sEmail"].ToString()!,

                                DOB = dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]),

                                nGender = dr["nGender"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nGender"]),

                                sProfileImage = dr["sProfileImage"] == DBNull.Value
                                    ? ""
                                    : dr["sProfileImage"].ToString()!,

                                nCollegeCode = dr["nCollegeCode"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nCollegeCode"]),

                                sCollegeName = dr["sCollegeName"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["sCollegeName"]),

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
            WHERE nBit = 1
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

    }
}
