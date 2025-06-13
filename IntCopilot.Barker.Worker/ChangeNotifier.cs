using System;
using IntCopilot.Barker.Worker.Data;
using IntSchool.Sharp.Core.Data;
using IntSchool.Sharp.Core.Models.Dto;

namespace IntCopilot.Barker.Worker
{
    public class ChangeNotifier
    {
        private readonly Profile _getProfile;
        public Profile GetProfile() => _getProfile;

        public ChangeNotifier(Profile profile)
        {
            _getProfile = profile ?? throw new ArgumentNullException(nameof(profile));
        }

        public ChangeNotifier Subscribe(Action<CourseSession, CourseSession> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _getProfile.Subscriptions.Add(new Subscription { Action = action });
            return this;
        }

        public ChangeNotifier Subscribe(Filter filter, Action<CourseSession, CourseSession> action)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(action);
            _getProfile.Subscriptions.Add(new Subscription { Filter = filter.Predicate, Action = action });
            return this;
        }

        public ChangeNotifier Subscribe(Presets preset, Action<CourseSession, CourseSession> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            Func<CourseSession, CourseSession, bool> filter = (prev, curr) =>
            {
                var prevValue = prev.Attendance;
                var currValue = curr.Attendance;
                return preset == Presets.ImportanceIncreased ? currValue > prevValue : currValue < prevValue;
            };

            _getProfile.Subscriptions.Add(new Subscription { Filter = filter, Action = action });
            return this;
        }
    }
}