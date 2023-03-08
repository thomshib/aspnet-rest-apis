
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IDbConnectionFactory connectionFactory,ILogger<DatabaseHealthCheck> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
    CancellationToken cancellationToken = new())
    {
        try{
            _  = await _connectionFactory.CreateConnectionAsnc(cancellationToken);
            return HealthCheckResult.Healthy();

        }catch(Exception ex){
            _logger.LogError("Database is unhealthy", ex);
            return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
        }
    }
}