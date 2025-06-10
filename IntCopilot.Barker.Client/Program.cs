using IntCopilot.Barker.Worker;
using IntCopilot.Barker.Worker.Data;
using Microsoft.Extensions.Options;

namespace IntCopilot.Barker.Client;

public class Program
{
    public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<AttendanceWorkerSettings>(
                builder.Configuration.GetSection("AttendanceWorker"));

            /*builder.Services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<AttendanceWorkerSettings>>().Value;
                
                settings.Validate();
                
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("AttendanceWorker");

                try
                {
                    var attendanceWorker = AttendanceWorker.New(TimeSpan.FromSeconds(settings.PollingIntervalSeconds));
                    
                    foreach (var profileSetting in settings.Profiles)
                    {
                        try
                        {
                            var profile = new Profile
                            {
                                Description = profileSetting.Description,
                                StudentId = profileSetting.StudentId,
                                SchoolYearId = profileSetting.SchoolYearId,
                                TimeWindow = GetTimeWindowFromString(profileSetting.TimeWindow)
                            };
                            
                            var profileLogger = loggerFactory.CreateLogger($"AttendanceProfile.{profile.Id}");
                            profile.OnChanged.Subscribe((prev, current) =>
                            {
                                profileLogger.LogInformation(
                                    "ATTENDANCE CHANGE DETECTED for Profile '{Description}': " +
                                    "Course '{CourseName}' changed from '{PrevState}' to '{CurrentState}'.",
                                    profile.Description,
                                    current.CourseName,
                                    prev.Attendance?.ToString() ?? "N/A",
                                    current.Attendance?.ToString() ?? "N/A");
                            });

                            attendanceWorker.AddProfile(profile);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to configure profile: {Description}", profileSetting.Description);
                            throw;
                        }
                    }
                    
                    return attendanceWorker.Build();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to create AttendanceWorker");
                    throw;
                }
            });*/
            builder.Services.AddSingleton<AttendanceWorkerManager>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }

        private static TimeWindow GetTimeWindowFromString(string timeWindowStr)
        {
            return timeWindowStr.ToLowerInvariant() switch
            {
                "today" => TimeWindow.Today,
                "thisweek" => TimeWindow.ThisWeek,
                _ => throw new ArgumentException($"Unsupported TimeWindow value: {timeWindowStr}", nameof(timeWindowStr))
            };
        }
}