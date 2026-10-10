using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ErJobPortal.Data;
using ErJobPortal.Models;

namespace ErJobPortal.Repositories
{
    public class TPOSchedulerRepository
    {
        private readonly DbConnection _db;


    public TPOSchedulerRepository(DbConnection db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // =========================================================
        // GENERAL ACTIVITY - INSERT
        // =========================================================
        public int SaveGeneralActivity(
            TPOSchedulerActivityM model,
            int tpoId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            INSERT INTO dbo.tblTPOSchedulerActivity
            (
                TPOID, Title, Category, Description,
                StartDate, EndDate, StartTime, EndTime,
                Location, Participants, Reminder,
                ReminderChannel, Status, CreatedDate, IsActive
            )
            OUTPUT INSERTED.ActivityID
            VALUES
            (
                @TPOID, @Title, @Category, @Description,
                @StartDate, @EndDate, @StartTime, @EndTime,
                @Location, @Participants, @Reminder,
                @ReminderChannel, @Status, SYSDATETIME(), 1
            );";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                AddActivityParameters(cmd, model, tpoId, includeId: false);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // =========================================================
        // GENERAL ACTIVITY - GET ALL FOR THE LOGGED-IN TPO
        // =========================================================
        public List<TPOSchedulerActivityM> GetGeneralActivities(int tpoId)
        {
            var activities = new List<TPOSchedulerActivityM>();

            const string query = @"
            SELECT
                ActivityID, TPOID, Title, Category, Description,
                StartDate, EndDate, StartTime, EndTime,
                Location, Participants, Reminder,
                ReminderChannel, Status, CreatedDate, ModifiedDate
            FROM dbo.tblTPOSchedulerActivity
            WHERE TPOID = @TPOID
              AND IsActive = 1
            ORDER BY StartDate, StartTime, ActivityID;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activities.Add(new TPOSchedulerActivityM
                        {
                            ActivityID = GetInt(reader, "ActivityID"),
                            TPOID = GetInt(reader, "TPOID"),
                            Title = GetString(reader, "Title"),
                            Category = GetString(reader, "Category"),
                            Description = GetString(reader, "Description"),
                            StartDate = GetDateTime(reader, "StartDate"),
                            EndDate = GetDateTime(reader, "EndDate"),
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"],
                            Location = GetNullableString(reader, "Location"),
                            Participants = GetString(reader, "Participants"),
                            Reminder = GetNullableString(reader, "Reminder"),
                            ReminderChannel = GetNullableString(reader, "ReminderChannel"),
                            Status = GetString(reader, "Status")
                        });
                    }
                }
            }

            return activities;
        }

        // =========================================================
        // GENERAL ACTIVITY - UPDATE
        // =========================================================
        public bool UpdateGeneralActivity(
            TPOSchedulerActivityM model,
            int tpoId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            UPDATE dbo.tblTPOSchedulerActivity
            SET
                Title = @Title,
                Category = @Category,
                Description = @Description,
                StartDate = @StartDate,
                EndDate = @EndDate,
                StartTime = @StartTime,
                EndTime = @EndTime,
                Location = @Location,
                Participants = @Participants,
                Reminder = @Reminder,
                ReminderChannel = @ReminderChannel,
                Status = @Status,
                ModifiedDate = SYSDATETIME()
            WHERE ActivityID = @ActivityID
              AND TPOID = @TPOID
              AND IsActive = 1;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                AddActivityParameters(cmd, model, tpoId, includeId: true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =========================================================
        // GENERAL ACTIVITY - SOFT DELETE
        // =========================================================
        public bool DeleteGeneralActivity(int activityId, int tpoId)
        {
            const string query = @"
            UPDATE dbo.tblTPOSchedulerActivity
            SET IsActive = 0,
                ModifiedDate = SYSDATETIME()
            WHERE ActivityID = @ActivityID
              AND TPOID = @TPOID
              AND IsActive = 1;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.Add("@ActivityID", SqlDbType.Int).Value = activityId;
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =========================================================
        // PLACEMENT DRIVE - INSERT
        // =========================================================
        public int SavePlacementDrive(
            TPOSchedulerPlacementDriveM model,
            int tpoId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            INSERT INTO dbo.tblTPOSchedulerPlacementDrive
            (
                TPOID, CompanyName, JobPosition, JobType, Salary,
                JobLocation, RequiredSkills, JobDescription,
                EligibleCourse, TenthPercentage, TwelfthPercentage,
                MinimumPercentage, GraduationYear, EligibilityCriteria,
                StartDate, EndDate, StartTime, EndTime, Venue,
                ContactPerson, ContactEmail, ContactPhone,
                Participants, PublicationStatus, Instructions,
                CreatedDate, IsActive
            )
            OUTPUT INSERTED.PlacementDriveID
            VALUES
            (
                @TPOID, @CompanyName, @JobPosition, @JobType, @Salary,
                @JobLocation, @RequiredSkills, @JobDescription,
                @EligibleCourse, @TenthPercentage, @TwelfthPercentage,
                @MinimumPercentage, @GraduationYear, @EligibilityCriteria,
                @StartDate, @EndDate, @StartTime, @EndTime, @Venue,
                @ContactPerson, @ContactEmail, @ContactPhone,
                @Participants, @PublicationStatus, @Instructions,
                SYSDATETIME(), 1
            );";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                AddPlacementParameters(cmd, model, tpoId, includeId: false);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // =========================================================
        // PLACEMENT DRIVE - GET ALL FOR THE LOGGED-IN TPO
        // =========================================================
        public List<TPOSchedulerPlacementDriveM> GetPlacementDrives(int tpoId)
        {
            var drives = new List<TPOSchedulerPlacementDriveM>();

            const string query = @"
            SELECT
                PlacementDriveID, TPOID, CompanyName, JobPosition,
                JobType, Salary, JobLocation, RequiredSkills,
                JobDescription, EligibleCourse, TenthPercentage,
                TwelfthPercentage, MinimumPercentage, GraduationYear,
                EligibilityCriteria, StartDate, EndDate, StartTime,
                EndTime, Venue, ContactPerson, ContactEmail,
                ContactPhone, Participants, PublicationStatus, Instructions
            FROM dbo.tblTPOSchedulerPlacementDrive
            WHERE TPOID = @TPOID
              AND IsActive = 1
            ORDER BY StartDate, StartTime, PlacementDriveID;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drives.Add(new TPOSchedulerPlacementDriveM
                        {
                            PlacementDriveID = GetInt(reader, "PlacementDriveID"),
                            TPOID = GetInt(reader, "TPOID"),
                            CompanyName = GetString(reader, "CompanyName"),
                            JobPosition = GetString(reader, "JobPosition"),
                            JobType = GetString(reader, "JobType"),
                            Salary = GetNullableString(reader, "Salary"),
                            JobLocation = GetString(reader, "JobLocation"),
                            RequiredSkills = GetNullableString(reader, "RequiredSkills"),
                            JobDescription = GetString(reader, "JobDescription"),
                            EligibleCourse = GetNullableString(reader, "EligibleCourse"),
                            TenthPercentage = GetNullableDecimal(reader, "TenthPercentage"),
                            TwelfthPercentage = GetNullableDecimal(reader, "TwelfthPercentage"),
                            MinimumPercentage = GetNullableDecimal(reader, "MinimumPercentage"),
                            GraduationYear = GetNullableInt(reader, "GraduationYear"),
                            EligibilityCriteria = GetNullableString(reader, "EligibilityCriteria"),
                            StartDate = GetDateTime(reader, "StartDate"),
                            EndDate = GetDateTime(reader, "EndDate"),
                            StartTime = reader["StartTime"] == DBNull.Value
    ? (TimeSpan?)null
    : (TimeSpan)reader["StartTime"],

                            EndTime = reader["EndTime"] == DBNull.Value
    ? (TimeSpan?)null
    : (TimeSpan)reader["EndTime"],
                            Venue = GetNullableString(reader, "Venue"),
                            ContactPerson = GetNullableString(reader, "ContactPerson"),
                            ContactEmail = GetNullableString(reader, "ContactEmail"),
                            ContactPhone = GetNullableString(reader, "ContactPhone"),
                            Participants = GetNullableString(reader, "Participants"),
                            PublicationStatus = GetString(reader, "PublicationStatus"),
                            Instructions = GetNullableString(reader, "Instructions")
                        });
                    }
                }
            }

            return drives;
        }

        // =========================================================
        // PLACEMENT DRIVE - UPDATE
        // =========================================================
        public bool UpdatePlacementDrive(
            TPOSchedulerPlacementDriveM model,
            int tpoId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            UPDATE dbo.tblTPOSchedulerPlacementDrive
            SET
                CompanyName = @CompanyName,
                JobPosition = @JobPosition,
                JobType = @JobType,
                Salary = @Salary,
                JobLocation = @JobLocation,
                RequiredSkills = @RequiredSkills,
                JobDescription = @JobDescription,
                EligibleCourse = @EligibleCourse,
                TenthPercentage = @TenthPercentage,
                TwelfthPercentage = @TwelfthPercentage,
                MinimumPercentage = @MinimumPercentage,
                GraduationYear = @GraduationYear,
                EligibilityCriteria = @EligibilityCriteria,
                StartDate = @StartDate,
                EndDate = @EndDate,
                StartTime = @StartTime,
                EndTime = @EndTime,
                Venue = @Venue,
                ContactPerson = @ContactPerson,
                ContactEmail = @ContactEmail,
                ContactPhone = @ContactPhone,
                Participants = @Participants,
                PublicationStatus = @PublicationStatus,
                Instructions = @Instructions,
                ModifiedDate = SYSDATETIME()
            WHERE PlacementDriveID = @PlacementDriveID
              AND TPOID = @TPOID
              AND IsActive = 1;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                AddPlacementParameters(cmd, model, tpoId, includeId: true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =========================================================
        // PLACEMENT DRIVE - SOFT DELETE
        // =========================================================
        public bool DeletePlacementDrive(int placementDriveId, int tpoId)
        {
            const string query = @"
            UPDATE dbo.tblTPOSchedulerPlacementDrive
            SET IsActive = 0,
                ModifiedDate = SYSDATETIME()
            WHERE PlacementDriveID = @PlacementDriveID
              AND TPOID = @TPOID
              AND IsActive = 1;";

            using (SqlConnection cn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.Add("@PlacementDriveID", SqlDbType.Int).Value = placementDriveId;
                cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =========================================================
        // GENERAL ACTIVITY PARAMETERS
        // =========================================================
        private static void AddActivityParameters(
            SqlCommand cmd,
            TPOSchedulerActivityM model,
            int tpoId,
            bool includeId)
        {
            cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

            if (includeId)
                cmd.Parameters.Add("@ActivityID", SqlDbType.Int).Value = model.ActivityID;

            AddText(cmd, "@Title", model.Title, 200);
            AddText(cmd, "@Category", model.Category, 50);
            AddText(cmd, "@Description", model.Description, -1);

            cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value =
                model.StartDate.Date;

            cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value =
                model.EndDate.Date;

            cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value =
     model.StartTime;

            cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value =
                model.EndTime;

            AddText(cmd, "@Location", model.Location, 300);
            AddText(cmd, "@Participants", model.Participants, 50);
            AddText(cmd, "@Reminder", model.Reminder, 20);
            AddText(cmd, "@ReminderChannel", model.ReminderChannel, 30);
            AddText(cmd, "@Status", string.IsNullOrWhiteSpace(model.Status)
                ? "Upcoming" : model.Status, 30);
        }

        // =========================================================
        // PLACEMENT DRIVE PARAMETERS
        // =========================================================
        private static void AddPlacementParameters(
            SqlCommand cmd,
            TPOSchedulerPlacementDriveM model,
            int tpoId,
            bool includeId)
        {
            cmd.Parameters.Add("@TPOID", SqlDbType.Int).Value = tpoId;

            if (includeId)
            {
                cmd.Parameters.Add("@PlacementDriveID", SqlDbType.Int).Value =
                    model.PlacementDriveID;
            }

            AddText(cmd, "@CompanyName", model.CompanyName, 200);
            AddText(cmd, "@JobPosition", model.JobPosition, 200);
            AddText(cmd, "@JobType", model.JobType, 50);
            AddText(cmd, "@Salary", model.Salary, 100);
            AddText(cmd, "@JobLocation", model.JobLocation, 300);
            AddText(cmd, "@RequiredSkills", model.RequiredSkills, -1);
            AddText(cmd, "@JobDescription", model.JobDescription, -1);
            AddText(cmd, "@EligibleCourse", model.EligibleCourse, 300);

            AddDecimal(cmd, "@TenthPercentage", model.TenthPercentage);
            AddDecimal(cmd, "@TwelfthPercentage", model.TwelfthPercentage);
            AddDecimal(cmd, "@MinimumPercentage", model.MinimumPercentage);

            cmd.Parameters.Add("@GraduationYear", SqlDbType.Int).Value =
                (object)model.GraduationYear ?? DBNull.Value;

            AddText(cmd, "@EligibilityCriteria", model.EligibilityCriteria, -1);

            cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value =
                model.StartDate.Date;

            cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value =
                model.EndDate.Date;

            SqlParameter startTimeParam =
    cmd.Parameters.Add("@StartTime", SqlDbType.Time);

            startTimeParam.Value =
                (object?)model.StartTime ?? DBNull.Value;

            SqlParameter endTimeParam =
                cmd.Parameters.Add("@EndTime", SqlDbType.Time);

            endTimeParam.Value =
                (object?)model.EndTime ?? DBNull.Value;

            AddText(cmd, "@Venue", model.Venue, 300);
            AddText(cmd, "@ContactPerson", model.ContactPerson, 150);
            AddText(cmd, "@ContactEmail", model.ContactEmail, 254);
            AddText(cmd, "@ContactPhone", model.ContactPhone, 20);
            AddText(cmd, "@Participants", model.Participants, 50);
            AddText(cmd, "@PublicationStatus",
                string.IsNullOrWhiteSpace(model.PublicationStatus)
                    ? "Draft" : model.PublicationStatus, 30);
            AddText(cmd, "@Instructions", model.Instructions, -1);
        }

        // =========================================================
        // SQL PARAMETER HELPERS
        // =========================================================
        private static void AddText(
            SqlCommand cmd,
            string name,
            string value,
            int size)
        {
            SqlParameter parameter = size == -1
                ? cmd.Parameters.Add(name, SqlDbType.NVarChar, -1)
                : cmd.Parameters.Add(name, SqlDbType.NVarChar, size);

            parameter.Value = string.IsNullOrWhiteSpace(value)
                ? DBNull.Value
                : value.Trim();
        }

        private static void AddDecimal(
            SqlCommand cmd,
            string name,
            decimal? value)
        {
            SqlParameter parameter = cmd.Parameters.Add(name, SqlDbType.Decimal);
            parameter.Precision = 5;
            parameter.Scale = 2;
            parameter.Value = (object)value ?? DBNull.Value;
        }

       

        // =========================================================
        // SQL DATA READER HELPERS
        // =========================================================
        private static int GetInt(SqlDataReader reader, string column)
        {
            return Convert.ToInt32(reader[column]);
        }

        private static int? GetNullableInt(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? (int?)null
                : Convert.ToInt32(reader[column]);
        }

        private static string GetString(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? string.Empty
                : Convert.ToString(reader[column]) ?? string.Empty;
        }

        private static string GetNullableString(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : Convert.ToString(reader[column]);
        }

        private static DateTime GetDateTime(SqlDataReader reader, string column)
        {
            return Convert.ToDateTime(reader[column]);
        }

       

        private static decimal? GetNullableDecimal(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? (decimal?)null
                : Convert.ToDecimal(reader[column]);
        }
    }


}
