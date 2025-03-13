using Petroineos.DayAheadPowerPositionReporting;
using Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .AddOptions<ReportingOptions>()
    .BindConfiguration("Reporting")
    .ValidateDataAnnotations();

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Petroineos Day Ahead Power Position Reporting Service";
});

builder.Services.AddSingleton<IFileSystem, LocalFileSystem>();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton<ILocalTimeZoneProvider, SystemLocalTimeZoneProvider>();

builder.Services.AddTransient<IReportGenerator, ReportGenerator>();
builder.Services.AddTransient<IReportEmitter, CsvReportEmitter>();

builder.Services.AddTransient<IPowerService, PowerService>();

var host = builder.Build();
host.Run();
