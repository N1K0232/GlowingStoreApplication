using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GlowingStoreApplication.BusinessLayer.Diagnostics.HealthChecks;

public class SqlConnectionHealthCheck : IHealthCheck
{
    private readonly IConfiguration configuration;
    private readonly ILogger<SqlConnectionHealthCheck> logger;

    public SqlConnectionHealthCheck(IConfiguration configuration, ILogger<SqlConnectionHealthCheck> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("SqlConnection");
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("SELECT 1");

        try
        {
            logger.LogInformation("checking connection");

            await connection.OpenAsync(cancellationToken);
            command.Connection = connection;

            await command.ExecuteScalarAsync(cancellationToken);
            await connection.CloseAsync();

            logger.LogInformation("test succeeded");
            return HealthCheckResult.Healthy();
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "couldn't connect");
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "couldn't execute");
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
        finally
        {
            logger.LogInformation("stopping service");

            await connection.DisposeAsync();
            await command.DisposeAsync();
        }
    }
}