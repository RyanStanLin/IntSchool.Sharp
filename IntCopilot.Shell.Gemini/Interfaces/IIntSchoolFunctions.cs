using System.ComponentModel;
using CSharpToJsonSchema;
using GenerativeAI;
using IntCopilot.DataAccess.Postgres.Models;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Models.Dto;

namespace IntCopilot.Shell.Gemini.Interfaces;

[GenerateJsonSchema(GoogleFunctionTool = true)]
public interface IIntSchoolFunctions
{
    //includes in the header: current schoolYearId(call GetCurrentSchoolYearAsync), current timestamp, last week timestamp
    [Description("Use studentId to get the detailed information for a single student. It includes:Personal Data: `studentId`, `studentNum`, `name`, `enName`, `gender`, `birthday`, `avatarUrl`, `idType`, and `idNum`.Academic Information: `sectionName`, `className`, `enterYear`, `enterDate`, and `status`.Location & Contact: `email`, `countryName`, `provinceName`, `cityName`, `districtName`, and `address`.School-Related Details: `houseName` (for school houses), `boarding` status, `schoolBus` usage, bus route/site details, and dormitory information.Domicile Information: Details about the student's official place of residence.")]
    Task<GetStudentDetailResponseModel> GetStudentDetailAsync([Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);

    [Description("Using studentId and a startTimeStamp, a endTimeStamp, a schoolYearId to returns a student's curriculum or timetable.`ClassArranges`: A nested dictionary containing the detailed class schedule, organized by date and then by class period. Each entry is a `ClassArrange` object.`ClassPeriods`: A list of objects that define the time slots for classes (e.g., Period 1, Period 2).`DayOfArranged`: A list of days that have scheduled classes.Each `ClassArrange` object represents a single scheduled class and contains:`ClassArrangeId`: The unique ID for this specific class session.`ClassRoomId`: Details about the classroom, including its ID and code.`CourseId`: Detailed information about the course being taught.`TargetSections`: A list of student sections or groups attending this class.`StartTime` and `EndTime`: The start and end timestamps for the class.`Teachers`: A list of teachers for this specific session, indicating if they are a substitute.The `CourseId` object further details the course with:`CourseIdCourseId`: The unique ID for the course.`Description`: A description of the course.`Subject`: Subject details, including its ID, description, and a display color.`Students`: A list of all students enrolled in this course, with their ID and name.`Teacher`: A list of all teachers assigned to the course, including their ID, name, avatar URL, and various roles (class teacher, course teacher, etc.).")]
    Task<GetStudentCurriculumResponseModel> GetStudentCurriculumAsync(
        [Description("Target school year Id")]string schoolYearId,
        [Description("Target student's studentId")]long studentId,
        [Description("The starting time of the query range")]long startTimeStamp,
        [Description("The ending time of the query range")]long endTimeStamp,
        CancellationToken cancellationToken = default);

    [Description("Using a schoolYearId and a studentId to returns a list of reports, each including the student ID, grade period ID(can be use to qurey details), student's name, English name, school year, report type, request URL, and template ID.")]
    Task<List<GetReportListResponseModel>> GetReportListAsync(
        [Description("Target school year Id")]string schoolYearId,
        [Description("Target student's studentId")]long studentId,
        CancellationToken cancellationToken = default);
    
    [Description("Using target gradePeriodId(can be fetch by using GetReportListAsync) and studentId to return detailed information for a student report, including student and class details, a list of grade items for each course with teacher comments, effort, and attainment, house and conduct points, behavior events, attendance records, and overall comments from the tutor and head teacher.")]
    Task<GetReportDetailResponseModel> GetReportDetailAsync(
       [Description("Target gradePeriodId")]string gradePeriodId,
       [Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);
    
    [Description("Using a studentId to returns a list of parents, each including the parent ID, name, contact information (area code, mobile, email), relationship to the student, their work details, and flags indicating if they are a teacher, the primary contact, and their account status.")]
    Task<List<GetParentsResponseModel>> GetParentsAsync(
       [Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);
    
    [Description("Using target courseId, studentId and schoolYearId to returns a paginated list of marks, including pagination details and a list of items. Each item includes its taskStudentId(taskStudentId, can be use to qurey details), name, start and end dates, the student's score, the maximum score, the creator's name, type, and status flags indicating whether it has been read, its submission status, and if it is an online task.")]
    Task<GetMarkListResponseModel> GetMarkListAsync(
       [Description("Id of the course")]string courseId,
       [Description("Target school year Id")]string schoolYearId,
       [Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);

    [Description("Using taskStudentId and studentId to returns detailed information for a single mark or task, including its name, subject, course, description, due date, and maximum score, along with the student's submission status, content, score, teacher comments, and any associated resources.")]
    Task<GetMarkDetailResponseModel> GetMarkDetailAsync(
       [Description("Id of student task")]string taskStudentId,
       [Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);
    
    [Description("Using studentId, pageSize(30 recommanded), pageCurrent(indicating the page index fetching) Returns a paginated list of leave applications, each including details such as the application ID, start and end times, duration, type, reason, current status, auditor information, and any associated attachments.")]
    Task<GetLeavesListResponseModel> GetLeavesListAsync(
       [Description("Number of Item to retuen")]string pageSize,
       [Description("Index of pages to return")]string pageCurrent,
       [Description("Target student's studentId")]long studentId,
       CancellationToken cancellationToken = default);

    [Description("Using studentId and a startTimeStamp, a endTimeStamp, a schoolYearId to return a collection of daily attendance records. Each daily record includes the date, the morning attendance status, and a list of course sessions for that day. Each session provides details on the course name, classroom, start and end times, and the student's attendance status for that specific session.")]
    Task<AttendanceDtoModel> GetAttendance(
       [Description("Target school year Id")]string schoolYearId,
       [Description("Target student's studentId")]long studentId,
       [Description("The starting time of the query range")]long startTimeStamp,
       [Description("The ending time of the query range")]long endTimeStamp,
       CancellationToken cancellationToken = default);
    
    [Description("(Get StudentIds By Name)Using student name to return a collection of possible student with studentId, posibility of matching, and the full name.")]
    Task<IEnumerable<FuzzySearchResult>> GetStudentIdsByName([Description("The Name the needs to be search")]string studentName,
       CancellationToken cancellationToken = default);
}