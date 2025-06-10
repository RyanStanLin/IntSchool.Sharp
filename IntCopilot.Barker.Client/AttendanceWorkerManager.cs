using BarkerSharper.Data;
using BarkerSharper.Model;
using IntCopilot.Barker.Worker;
using IntCopilot.Barker.Worker.Data;
using IntCopilot.Barker.Worker.Extensions;
using IntSchool.Sharp.Core.Data;
using Microsoft.Extensions.Options;

namespace IntCopilot.Barker.Client;

public class AttendanceWorkerManager : IDisposable
    {
        private readonly ILogger<AttendanceWorkerManager> _logger;
        private readonly IOptionsMonitor<AttendanceWorkerSettings> _settingsMonitor;
        private readonly ILoggerFactory _loggerFactory;
        private AttendanceWorker? _currentWorker;
        private IDisposable? _settingsChangeToken;
        private readonly object _lock = new();

        public AttendanceWorkerManager(
            ILogger<AttendanceWorkerManager> logger,
            IOptionsMonitor<AttendanceWorkerSettings> settingsMonitor,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _settingsMonitor = settingsMonitor;
            _loggerFactory = loggerFactory;

            // 初始化并监听配置更改
            _settingsChangeToken = _settingsMonitor.OnChange(OnSettingsChanged);
            OnSettingsChanged(_settingsMonitor.CurrentValue);
        }

        private void OnSettingsChanged(AttendanceWorkerSettings newSettings)
        {
            lock (_lock)
            {
                try
                {
                    _logger.LogInformation("Reloading AttendanceWorker configuration...");

                    // 停止并释放当前的 Worker
                    _currentWorker?.Stop();
                    _currentWorker?.Dispose();

                    // 创建新的 Worker
                    /*var testBuilder = AttendanceWorker.New(TimeSpan.FromSeconds(10))
                        .AddProfile(new Profile()
                        {
                            Description = "1",
                            SchoolYearId = "42",
                            StudentId = "11012",
                            TimeWindow = TimeWindow.Today,
                        }.OnChanged
                            .Subscribe(
                                filter:new Filter((prev,curr) => curr.Attendance.HigherThanInclusive(AttendanceOptions.Late)),
                                action: (prev, curr) =>
                                    {
                                        
                                    })
                            .Subscribe(
                                preset: Presets.ImportanceIncreased,
                                action: (prev, curr) =>
                                {
                                    
                                }).Profile);*/
                    var newWorkerBuilder = AttendanceWorker.New(TimeSpan.FromSeconds(newSettings.PollingIntervalSeconds));
                    
                    foreach (var profileSetting in newSettings.Profiles)
                    {
                        var profile = new Profile
                        {
                            Description = profileSetting.Description,
                            StudentId = profileSetting.StudentId,
                            SchoolYearId = profileSetting.SchoolYearId,
                            TimeWindow = GetTimeWindowFromString(profileSetting.TimeWindow)
                        };

                        // 为每个 profile 创建专门的日志记录器
                        var profileLogger = _loggerFactory.CreateLogger($"AttendanceProfile.{profile.Id}");
                        profile.OnChanged.Subscribe((prev, current) =>
                        {
                            profileLogger.LogInformation(
                                "ATTENDANCE CHANGE DETECTED for Profile '{Description}': " +
                                "Course '{CourseName}' changed from '{PrevState}' to '{CurrentState}'.",
                                profile.Description,
                                current.CourseName,
                                prev.Attendance.GetDescription() ?? "N/A",
                                current.Attendance.GetDescription() ?? "N/A");
                            foreach (var barkToken in profileSetting.BarkTokens)
                            {
                                BarkerSharper.Barker barker = new BarkerSharper.Barker(new BarkConfiguration(
                                    baseUrl: new Uri("https://api.day.app"),
                                    deviceKey: barkToken));
                                barker.Bark(new BarkNotificationExtendedModel()
                                {
                                    Body = @$"Attendance updated from ""{prev.Attendance.GetDescription()}"" to ""{current.Attendance.GetDescription()}"".",
                                    Title = $"{current.CourseName}",
                                    Sound = NotificationSound.Shake,
                                    Group = "Attendance",
                                    Icon = new Uri("https://i.hd-r.icu/398ce6efe83da649bd6e5be4954ba8af.png"),
                                    Url = "pcd.intschool.com"
                                });
                            }
                        }).Subscribe(filter:new Filter((prev,curr) => curr.Attendance.HigherThanInclusive(AttendanceOptions.Late)),
                            action: (prev, curr) =>
                            {
                                profileLogger.LogInformation(
                                    "ATTENDANCE UPDATE DETECTED for Profile '{Description}': " +
                                    "Course '{CourseName}' changed from '{PrevState}' to '{CurrentState}'.",
                                    profile.Description,
                                    curr.CourseName,
                                    prev.Attendance.GetDescription() ?? "N/A",
                                    curr.Attendance.GetDescription() ?? "N/A");
                                foreach (var barkToken in profileSetting.BarkTokens)
                                {
                                    BarkerSharper.Barker barker = new BarkerSharper.Barker(new BarkConfiguration(
                                        baseUrl: new Uri("https://api.day.app"),
                                        deviceKey: barkToken));
                                    barker.Bark(new BarkNotificationExtendedModel()
                                    {
                                        Body = @$"Attendance updated from ""{prev.Attendance.GetDescription()}"" to ""{curr.Attendance.GetDescription()}"". You should be at {curr.ClassRoom}.",
                                        Title = $"{curr.CourseName}",
                                        Sound = NotificationSound.Shake,
                                        Level = NotificationLevel.Critical,
                                        Group = "Attendance",
                                        Icon = new Uri("https://i.hd-r.icu/398ce6efe83da649bd6e5be4954ba8af.png"),
                                        Url = "pcd.intschool.com"
                                    });
                                }
                            });

                        newWorkerBuilder.AddProfile(profile);
                    }

                    // 启动新 Worker
                    _currentWorker = newWorkerBuilder.Build();
                    _currentWorker.Start();

                    _logger.LogInformation("AttendanceWorker reloaded successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reload AttendanceWorker configuration.");
                }
            }
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

        public void Dispose()
        {
            lock (_lock)
            {
                _currentWorker?.Dispose();
                _settingsChangeToken?.Dispose();
            }
        }
    }