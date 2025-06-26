using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Barker.Worker.Data;
using IntSchool.Sharp.Core.Extensions.Converters;
using IntSchool.Sharp.Core.LifeCycle;
using IntSchool.Sharp.Core.Models.Dto;
using IntSchool.Sharp.Core.RequestConfigs;

namespace IntCopilot.Barker.Worker
{
    public class AttendanceWorker : IDisposable
    {
        private readonly object _lockObject = new();
        private Timer? _timer;
        private readonly List<Profile> _profiles = new();
        private readonly Dictionary<string, AttendanceDtoModel> _lastStates = new();
        private readonly TimeSpan _pollingInterval;
        private bool _isRunning;
        private bool _isDisposed;

        private AttendanceWorker(TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
        }

        public static Builder New(TimeSpan pollingInterval) => new(pollingInterval);

        public void Start()
        {
            ThrowIfDisposed();
            lock (_lockObject)
            {
                if (_isRunning) return;
                _isRunning = true;
                _timer = new Timer(PollAttendance, null, TimeSpan.Zero, _pollingInterval);
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (!_isRunning) return;
                _timer?.Dispose();
                _timer = null;
                _isRunning = false;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            Stop();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AttendanceWorker));
            }
        }

        private void PollAttendance(object? state)
        {
            if (!_isRunning) return;

            List<Profile> profilesToPoll;
            lock (_lockObject)
            {
                profilesToPoll = _profiles.ToList();
            }

            try
            {
                Parallel.ForEach(profilesToPoll, profile =>
                {
                    try
                    {
                        ProcessProfile(profile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing profile {profile.Id}: {ex}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PollAttendance loop: {ex}");
            }
        }

        private void ProcessProfile(Profile profile)
        {
            var config = new SharedStudentTimespanConfiguration
            {
                StartTime = profile.TimeWindow.StartTime,
                EndTime = profile.TimeWindow.EndTime,
                StudentId = profile.StudentId,
                SchoolYearId = profile.SchoolYearId
            };
            
            //TODOshould be delete when its in production state
            Api.Instance.XToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJ3ZWJUb2tlbiIsInNjaG9vbElkcyI6WzhdLCJpc3MiOiJkaXBvbnQtbmoiLCJleHAiOjE3NDk5OTMyMzYsInVzZXJOYW1lIjoi5p6X55-l6L-cIiwiaWF0IjoxNzQ3NDAxMjM2LCJ1c2VySWQiOjE1NDM1LCJqdGkiOiI0NWU0NDZjYy0yYTg3LTRkYWEtOWQyZS03ODEyNmUwOWJhOTIifQ.iCuNIx4A-BDaNXmnEmcn63vIye4BeDtPfTTO31H3H-8";
            
            var response = Api.Instance.GetAttendance(config);
            
            if (response.IsSuccess == false) return;
            Console.WriteLine("DEBUG - Request sent and success");
            var currentState = response.SuccessResult.ToAttendanceDtoModel();
            var profileKey = $"{profile.StudentId}_{profile.SchoolYearId}";

            AttendanceDtoModel? previousState;
            lock (_lastStates)
            {
                _lastStates.TryGetValue(profileKey, out previousState);
            }

            if (previousState != null)
            {
                CompareAndNotify(previousState, currentState, profile);
            }

            lock (_lastStates)
            {
                _lastStates[profileKey] = currentState;
            }
        }

        private void CompareAndNotify(AttendanceDtoModel prev, AttendanceDtoModel current, Profile profile)
        {
            foreach (var currentDay in current.Days)
            {
                var prevDay = prev.Days.FirstOrDefault(d => d.Date.Date == currentDay.Date.Date);
                if (prevDay == null) continue;

                CompareAndNotifyMorningAttendance(prevDay, currentDay, profile);
                CompareAndNotifyCourses(prevDay, currentDay, profile);
            }
        }

        private void CompareAndNotifyMorningAttendance(AttendanceDay prevDay, AttendanceDay currentDay, Profile profile)
        {
            if (prevDay.MorningAttendance != currentDay.MorningAttendance)
            {
                var prevSession = new CourseSession { Attendance = prevDay.MorningAttendance, CourseName = "Morning Attendance", StartTime = currentDay.Date.Date, EndTime = currentDay.Date.Date.AddHours(1) };
                var currentSession = new CourseSession { Attendance = currentDay.MorningAttendance, CourseName = "Morning Attendance", StartTime = currentDay.Date.Date, EndTime = currentDay.Date.Date.AddHours(1) };
                NotifySubscribers(profile, prevSession, currentSession);
            }
        }

        private void CompareAndNotifyCourses(AttendanceDay prevDay, AttendanceDay currentDay, Profile profile)
        {
            foreach (var currentCourse in currentDay.Courses)
            {
                var prevCourse = prevDay.Courses.FirstOrDefault(c => c.CourseName == currentCourse.CourseName && c.StartTime == currentCourse.StartTime);
                if (prevCourse != null && prevCourse.Attendance == currentCourse.Attendance) continue;
                NotifySubscribers(profile, prevCourse ?? new CourseSession(), currentCourse);
            }
        }

        private void NotifySubscribers(Profile profile, CourseSession prev, CourseSession current)
        {
            foreach (var subscription in profile.Subscriptions.ToList())
            {
                try
                {
                    if (subscription.ShouldNotify(prev, current))
                    {
                        subscription.Action.Invoke(prev, current);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in subscription for profile {profile.Id}: {ex}");
                }
            }
        }

        public class Builder
        {
            private readonly AttendanceWorker _worker;

            internal Builder(TimeSpan pollingInterval)
            {
                if (pollingInterval <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Polling interval must be positive.", nameof(pollingInterval));
                }
                _worker = new AttendanceWorker(pollingInterval);
            }

            public Builder AddProfile(Profile profile)
            {
                ArgumentNullException.ThrowIfNull(profile);
                lock (_worker._lockObject)
                {
                    _worker._profiles.Add(profile);
                }
                return this;
            }

            public AttendanceWorker Build()
            {
                if (!_worker._profiles.Any())
                {
                    throw new InvalidOperationException("At least one profile must be added before building");
                }
                return _worker;
            }

            public AttendanceWorker Start()
            {
                var worker = Build();
                worker.Start();
                return worker;
            }
        }
    }
}