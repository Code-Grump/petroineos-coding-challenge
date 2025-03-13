using System.Diagnostics;

namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// A background worker that generates a report at a specified interval.
/// </summary>
/// <param name="generator">The generator to invoke to produce the report.</param>
/// <param name="logger">A logger to use to write diagnostic information.</param>
public class Worker(IReportGenerator generator, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // The service must produce a report when it starts and then every interval,
            // as specified by the service configuration.
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Report generation initiated");

                // TODO: Establish delay from configuration.
                var interval = TimeSpan.FromMinutes(30);
                var stopwatch = Stopwatch.StartNew();

                await generator.GenerateReportAsync(stoppingToken);

                logger.LogInformation(
                    "Report generation completed in {ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);

                // Delay the next report being generated until the next interval.
                var elapsed = stopwatch.Elapsed;
                var delay = interval - elapsed;

                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during report generation");

            // Terminate the process with a non-zero exit code to allow the Windows Service Managagement system
            // a signal that the service has failed and can attempt any configured recovery procedures.
            Environment.Exit(1);
        }
    }
}
