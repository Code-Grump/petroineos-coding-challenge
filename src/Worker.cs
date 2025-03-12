namespace Petroineos.DayAheadPowerPositionReporting;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {

        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred.");

            // Terminate the process with a non-zero exit code to allow the Windows Service Managagement system
            // a signal that the service has failed and can attempt any configured recovery procedures.
            Environment.Exit(1);
        }
    }
}
