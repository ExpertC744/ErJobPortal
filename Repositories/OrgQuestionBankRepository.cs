using ErJobPortal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ErJobPortal.Repositories
{
    public class OrgQuestionBankRepository
    {
        private readonly IConfiguration _configuration;

        public OrgQuestionBankRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==========================================
        // 1. INSERT QUESTION BANK
        // ==========================================
        public async Task<int> InsertAsync(OrgQuestionBank model)
        {
            const string sql = @"
                INSERT INTO dbo.tblOrgQuestionBank
                (
                    OrgID, Title, Category, StreamDomain, Course,
                    Subject, Topics, Description,
                    OriginalFileName, StoredFileName, FileExtension,
                    ContentType, FileSizeBytes, FilePath,
                    AudienceType, IsPublished, IsDeleted, CreatedDate
                )
                VALUES
                (
                    @OrgID, @Title, @Category, @StreamDomain, @Course,
                    @Subject, @Topics, @Description,
                    @OriginalFileName, @StoredFileName, @FileExtension,
                    @ContentType, @FileSizeBytes, @FilePath,
                    @AudienceType, @IsPublished, 0, SYSDATETIME()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            await using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@OrgID", SqlDbType.Int).Value =
                model.OrgID;

            command.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value =
                model.Title;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 100).Value =
                model.Category;

            command.Parameters.Add("@StreamDomain", SqlDbType.NVarChar, 100).Value =
                model.StreamDomain;

            command.Parameters.Add("@Course", SqlDbType.NVarChar, 150).Value =
                string.IsNullOrWhiteSpace(model.Course)
                    ? DBNull.Value : model.Course;

            command.Parameters.Add("@Subject", SqlDbType.NVarChar, 150).Value =
                model.Subject;

            command.Parameters.Add("@Topics", SqlDbType.NVarChar, 1000).Value =
                model.Topics;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 2000).Value =
                model.Description;

            command.Parameters.Add("@OriginalFileName", SqlDbType.NVarChar, 255).Value =
                model.OriginalFileName!;

            command.Parameters.Add("@StoredFileName", SqlDbType.NVarChar, 255).Value =
                model.StoredFileName!;

            command.Parameters.Add("@FileExtension", SqlDbType.NVarChar, 10).Value =
                model.FileExtension!;

            command.Parameters.Add("@ContentType", SqlDbType.NVarChar, 150).Value =
                string.IsNullOrWhiteSpace(model.ContentType)
                    ? DBNull.Value : model.ContentType;

            command.Parameters.Add("@FileSizeBytes", SqlDbType.BigInt).Value =
                model.FileSizeBytes;

            command.Parameters.Add("@FilePath", SqlDbType.NVarChar, 1000).Value =
                model.FilePath!;

            command.Parameters.Add("@AudienceType", SqlDbType.NVarChar, 30).Value =
                model.AudienceType;

            command.Parameters.Add("@IsPublished", SqlDbType.Bit).Value =
                model.IsPublished;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        // ==========================================
        // 2. GET QUESTION BANK LIST WITH FILTERS
        // ==========================================
        public async Task<List<OrgQuestionBank>> GetQuestionBanksAsync(
            int orgId,
            string? searchText = null,
            string? category = null,
            string? streamDomain = null,
            string? status = null)
        {
            var questionBanks = new List<OrgQuestionBank>();

            const string sql = @"
                SELECT
                    QuestionBankID,
                    OrgID,
                    Title,
                    Category,
                    StreamDomain,
                    Course,
                    Subject,
                    Topics,
                    Description,
                    OriginalFileName,
                    StoredFileName,
                    FileExtension,
                    ContentType,
                    FileSizeBytes,
                    FilePath,
                    AudienceType,
                    IsPublished,
                    IsDeleted,
                    CreatedDate,
                    UpdatedDate
                FROM dbo.tblOrgQuestionBank
                WHERE OrgID = @OrgID
                  AND IsDeleted = 0
                  AND
                  (
                      @SearchText IS NULL
                      OR Title LIKE '%' + @SearchText + '%'
                      OR Subject LIKE '%' + @SearchText + '%'
                      OR Topics LIKE '%' + @SearchText + '%'
                  )
                  AND (@Category IS NULL OR Category = @Category)
                  AND (@StreamDomain IS NULL OR StreamDomain = @StreamDomain)
                  AND
                  (
                      @Status IS NULL
                      OR (@Status = 'Published' AND IsPublished = 1)
                      OR (@Status = 'Draft' AND IsPublished = 0)
                  )
                ORDER BY CreatedDate DESC;";

            await using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@OrgID", SqlDbType.Int).Value = orgId;

            command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(searchText)
                    ? DBNull.Value
                    : searchText.Trim();

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(category)
                    ? DBNull.Value
                    : category.Trim();

            command.Parameters.Add("@StreamDomain", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(streamDomain)
                    ? DBNull.Value
                    : streamDomain.Trim();

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value =
                string.IsNullOrWhiteSpace(status) || status == "All"
                    ? DBNull.Value
                    : status;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                questionBanks.Add(new OrgQuestionBank
                {
                    QuestionBankID = reader.GetInt32(
                        reader.GetOrdinal("QuestionBankID")),

                    OrgID = reader.GetInt32(
                        reader.GetOrdinal("OrgID")),

                    Title = reader.GetString(
                        reader.GetOrdinal("Title")),

                    Category = reader.GetString(
                        reader.GetOrdinal("Category")),

                    StreamDomain = reader.GetString(
                        reader.GetOrdinal("StreamDomain")),

                    Course = reader["Course"] == DBNull.Value
                        ? null
                        : Convert.ToString(reader["Course"]),

                    Subject = reader.GetString(
                        reader.GetOrdinal("Subject")),

                    Topics = reader.GetString(
                        reader.GetOrdinal("Topics")),

                    Description = reader.GetString(
                        reader.GetOrdinal("Description")),

                    OriginalFileName = reader.GetString(
                        reader.GetOrdinal("OriginalFileName")),

                    StoredFileName = reader.GetString(
                        reader.GetOrdinal("StoredFileName")),

                    FileExtension = reader.GetString(
                        reader.GetOrdinal("FileExtension")),

                    ContentType = reader["ContentType"] == DBNull.Value
                        ? null
                        : Convert.ToString(reader["ContentType"]),

                    FileSizeBytes = Convert.ToInt64(
                        reader["FileSizeBytes"]),

                    FilePath = reader.GetString(
                        reader.GetOrdinal("FilePath")),

                    AudienceType = reader.GetString(
                        reader.GetOrdinal("AudienceType")),

                    IsPublished = reader.GetBoolean(
                        reader.GetOrdinal("IsPublished")),

                    IsDeleted = reader.GetBoolean(
                        reader.GetOrdinal("IsDeleted")),

                    CreatedDate = reader.GetDateTime(
                        reader.GetOrdinal("CreatedDate")),

                    UpdatedDate = reader["UpdatedDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["UpdatedDate"])
                });
            }

            return questionBanks;
        }

        // ==========================================
        // 3. GET QUESTION BANK BY ID
        //    Restricted to the owning organization
        // ==========================================
        public async Task<OrgQuestionBank?> GetQuestionBankByIdAsync(
            int questionBankId,
            int orgId)
        {
            const string sql = @"
                SELECT TOP (1)
                    QuestionBankID,
                    OrgID,
                    Title,
                    OriginalFileName,
                    StoredFileName,
                    FileExtension,
                    ContentType,
                    FileSizeBytes,
                    FilePath,
                    IsPublished,
                    IsDeleted
                FROM dbo.tblOrgQuestionBank
                WHERE QuestionBankID = @QuestionBankID
                  AND OrgID = @OrgID
                  AND IsDeleted = 0;";

            await using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@QuestionBankID", SqlDbType.Int).Value =
                questionBankId;

            command.Parameters.Add("@OrgID", SqlDbType.Int).Value =
                orgId;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new OrgQuestionBank
            {
                QuestionBankID = reader.GetInt32(
                    reader.GetOrdinal("QuestionBankID")),

                OrgID = reader.GetInt32(
                    reader.GetOrdinal("OrgID")),

                Title = reader.GetString(
                    reader.GetOrdinal("Title")),

                OriginalFileName = reader.GetString(
                    reader.GetOrdinal("OriginalFileName")),

                StoredFileName = reader.GetString(
                    reader.GetOrdinal("StoredFileName")),

                FileExtension = reader.GetString(
                    reader.GetOrdinal("FileExtension")),

                ContentType = reader["ContentType"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["ContentType"]),

                FileSizeBytes = Convert.ToInt64(
                    reader["FileSizeBytes"]),

                FilePath = reader.GetString(
                    reader.GetOrdinal("FilePath")),

                IsPublished = reader.GetBoolean(
                    reader.GetOrdinal("IsPublished")),

                IsDeleted = reader.GetBoolean(
                    reader.GetOrdinal("IsDeleted"))
            };
        }
    }
}