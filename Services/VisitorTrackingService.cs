
using Microsoft.Data.SqlClient;
using System.Data;

namespace ErJobPortal.Services
{
    public class VisitorTrackingService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VisitorTrackingService> _logger;

        public VisitorTrackingService(
            IConfiguration configuration,
            ILogger<VisitorTrackingService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task TrackVisitAsync(
            HttpContext context,
            string panelType,
            string pageName,
            int? userId = null,
            string? userType = null,
            bool isLoggedIn = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection is missing.");

            string? ipAddress =
                context.Connection.RemoteIpAddress?.ToString();

            if (context.Connection.RemoteIpAddress?.IsIPv4MappedToIPv6 == true)
            {
                ipAddress = context.Connection.RemoteIpAddress
                    .MapToIPv4()
                    .ToString();
            }

            string userAgent =
                context.Request.Headers.UserAgent.ToString();

            string browserName = "Unknown";
            string browserVersion = "";
            string operatingSystem = "Unknown";
            string deviceType = "Desktop";

            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
            {
                browserName = "Edge";
                browserVersion = GetVersion(userAgent, "Edg/");
            }
            else if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
            {
                browserName = "Chrome";
                browserVersion = GetVersion(userAgent, "Chrome/");
            }
            else if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
            {
                browserName = "Firefox";
                browserVersion = GetVersion(userAgent, "Firefox/");
            }
            else if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
            {
                browserName = "Safari";
                browserVersion = GetVersion(userAgent, "Version/");
            }

            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                operatingSystem = "Windows";
            else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                operatingSystem = "Android";
            else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                     userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                operatingSystem = "iOS";
            else if (userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
                operatingSystem = "macOS";
            else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                operatingSystem = "Linux";

            if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
                deviceType = "Mobile";
            else if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) ||
                     userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                deviceType = "Tablet";

            string? referrerUrl = context.Request.Headers.Referer.ToString();

            if (string.IsNullOrWhiteSpace(referrerUrl))
                referrerUrl = null;

            string? language =
                context.Request.Headers.AcceptLanguage.ToString();

            if (string.IsNullOrWhiteSpace(language))
                language = null;
            else
                language = language.Split(',')[0];

            try
            {
                const string sql = @"
INSERT INTO dbo.tblLoginPageVisits
(
    IPAddress,
    ISP,
    Country,
    Region,
    City,
    Latitude,
    Longitude,
    TimeZone,
    SessionID,
    PanelType,
    UserID,
    UserType,
    BrowserName,
    BrowserVersion,
    OperatingSystem,
    DeviceType,
    Language,
    UserAgent,
    ReferrerUrl,
    RequestPath,
    PageName,
    VisitDateTime,
    IsLoggedIn
)
VALUES
(
    @IPAddress,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    @SessionID,
    @PanelType,
    @UserID,
    @UserType,
    @BrowserName,
    @BrowserVersion,
    @OperatingSystem,
    @DeviceType,
    @Language,
    @UserAgent,
    @ReferrerUrl,
    @RequestPath,
    @PageName,
    SYSUTCDATETIME(),
    @IsLoggedIn
);";

                await using var connection =
                    new SqlConnection(connectionString);

                await using var command =
                    new SqlCommand(sql, connection);

                command.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 45)
                    .Value = DbValue(ipAddress);

                command.Parameters.Add("@SessionID", SqlDbType.NVarChar, 100)
                    .Value = DbValue(context.Session.Id);

                command.Parameters.Add("@PanelType", SqlDbType.NVarChar, 30)
                    .Value = panelType;

                command.Parameters.Add("@UserID", SqlDbType.Int)
                    .Value = DbValue(userId);

                command.Parameters.Add("@UserType", SqlDbType.NVarChar, 30)
                    .Value = DbValue(userType);

                command.Parameters.Add("@BrowserName", SqlDbType.NVarChar, 100)
                    .Value = browserName;

                command.Parameters.Add("@BrowserVersion", SqlDbType.NVarChar, 50)
                    .Value = browserVersion;

                command.Parameters.Add("@OperatingSystem", SqlDbType.NVarChar, 100)
                    .Value = operatingSystem;

                command.Parameters.Add("@DeviceType", SqlDbType.NVarChar, 30)
                    .Value = deviceType;

                command.Parameters.Add("@Language", SqlDbType.NVarChar, 30)
                    .Value = DbValue(language);

                command.Parameters.Add("@UserAgent", SqlDbType.NVarChar, 500)
                    .Value = DbValue(Truncate(userAgent, 500));

                command.Parameters.Add("@ReferrerUrl", SqlDbType.NVarChar, 500)
                    .Value = DbValue(Truncate(referrerUrl, 500));

                command.Parameters.Add("@RequestPath", SqlDbType.NVarChar, 300)
                    .Value = Truncate(context.Request.Path.Value, 300);

                command.Parameters.Add("@PageName", SqlDbType.NVarChar, 100)
                    .Value = Truncate(pageName, 100);

                command.Parameters.Add("@IsLoggedIn", SqlDbType.Bit)
                    .Value = isLoggedIn;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to track visit for panel {PanelType}, page {PageName}",
                    panelType,
                    pageName);

                throw;
            }
        }

        private static object DbValue(object? value)
        {
            return value ?? DBNull.Value;
        }

        private static string GetVersion(string userAgent, string marker)
        {
            int start = userAgent.IndexOf(
                marker,
                StringComparison.OrdinalIgnoreCase);

            if (start < 0)
                return "";

            start += marker.Length;

            int end = userAgent.IndexOf(' ', start);

            return end < 0
                ? userAgent[start..]
                : userAgent[start..end];
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (value == null)
                return null;

            return value.Length <= maxLength
                ? value
                : value[..maxLength];
        }
    }
}
