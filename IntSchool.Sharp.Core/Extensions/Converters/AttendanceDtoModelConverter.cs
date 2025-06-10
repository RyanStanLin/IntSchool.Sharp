using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Models.Dto;

namespace IntSchool.Sharp.Core.Extensions.Converters;

public static class AttendanceDtoModelConverter
{
    public static AttendanceDtoModel ToAttendanceDtoModel(this GetAttendanceResponseModel rawModel)
    {
        var result = new AttendanceDtoModel();
        foreach (var dailyStatic in rawModel.DailyStatistics)
        {
            var attendanceDay = new AttendanceDay()
            {
                Date = dailyStatic.Date.ToDateTimeFromUnixMilliseconds(),
                MorningAttendance = dailyStatic.Am.ToAttendanceOption()
            };
            foreach (var period in dailyStatic.ClassPeriods)
            {
                foreach (var coursePair in dailyStatic.Attendances)
                {
                    if (period.Description == coursePair.Key)
                    {
                        var coursesAttendance = new CourseSession()
                        {
Attendance = coursePair.Value.Status.ToAttendanceOption(),
CourseName = coursePair.Value.CourseName,
ClassRoom = coursePair.Value.ClassRoom,
StartTime = period.Start.ToDateTimeFromUnixMilliseconds(),
EndTime = period.End.ToDateTimeFromUnixMilliseconds()
                        };
                        attendanceDay.Courses.Add(coursesAttendance);
                    }
                }
            }
            result.Days.Add(attendanceDay);
        }
        return result;
    }
}