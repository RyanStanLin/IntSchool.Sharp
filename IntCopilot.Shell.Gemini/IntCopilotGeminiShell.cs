using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.DataAccess.Postgres.Models;
using IntCopilot.Shell.Gemini.Interfaces;
using IntSchool.Sharp.Core.LifeCycle;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Models.Dto;
using IntSchool.Sharp.Core.RequestConfigs;
using IntSchool.Sharp.Core.Extensions;
using IntSchool.Sharp.Core.Extensions.Converters;

namespace IntCopilot.Shell.Gemini;

public class IntCopilotGeminiShell(IStudentRepository studentRepository) : IIntSchoolFunctions
{
    public async Task<GetStudentDetailResponseModel> GetStudentDetailAsync(string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetStudentDetailAsync(studentId);
        return result.SuccessResult ?? new GetStudentDetailResponseModel();
    }

    public async Task<GetStudentCurriculumResponseModel> GetStudentCurriculumAsync(string schoolYearId, string studentId, long startTimeStamp, long endTimeStamp,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetStudentCurriculumAsync(new SharedStudentTimespanConfiguration(schoolYearId, studentId, startTimeStamp.ToDateTimeFromUnixMilliseconds(), endTimeStamp.ToDateTimeFromUnixMilliseconds()));
        return result.SuccessResult ?? new GetStudentCurriculumResponseModel();
    }

    public async Task<List<GetReportListResponseModel>> GetReportListAsync(string schoolYearId, string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetReportListAsync(schoolYearId, studentId);
        return result.SuccessResult ?? new List<GetReportListResponseModel>();
    }

    public async Task<GetReportDetailResponseModel> GetReportDetailAsync(string gradePeriodId, string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetReportDetailAsync(gradePeriodId, studentId);
        return result.SuccessResult ?? new GetReportDetailResponseModel();
    }

    public async Task<List<GetParentsResponseModel>> GetParentsAsync(string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetParentsAsync(studentId);
        return result.SuccessResult ?? new List<GetParentsResponseModel>();
    }

    public async Task<GetMarkListResponseModel> GetMarkListAsync(string courseId, string schoolYearId, string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetMarkListAsync(courseId,schoolYearId,studentId);
        return result.SuccessResult ?? new GetMarkListResponseModel();
    }

    public async Task<GetMarkDetailResponseModel> GetMarkDetailAsync(string taskStudentId, string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetMarkDetailAsync(taskStudentId,studentId);
        return result.SuccessResult ?? new GetMarkDetailResponseModel();
    }

    public async Task<GetLeavesListResponseModel> GetLeavesListAsync(string pageSize, string pageCurrent, string studentId,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetLeavesListAsync(studentId,new GetPageControlConfiguration(pageSize,pageCurrent));
        return result.SuccessResult ?? new GetLeavesListResponseModel();
    }

    public async Task<AttendanceDtoModel> GetAttendance(string schoolYearId, string studentId, long startTimeStamp, long endTimeStamp,
        CancellationToken cancellationToken = default)
    {
        var result = await Api.Instance.GetAttendanceAsync(new SharedStudentTimespanConfiguration(schoolYearId, studentId, startTimeStamp.ToDateTimeFromUnixMilliseconds(), endTimeStamp.ToDateTimeFromUnixMilliseconds()));
        return result.SuccessResult.ToAttendanceDtoModel() ?? new AttendanceDtoModel();
    }

    public async Task<IEnumerable<FuzzySearchResult>> GetStudentIdsByName(string studentName,
        CancellationToken cancellationToken = default)
    {
        var searchResults = await studentRepository.FuzzySearchByNameAsync(studentName,similarityThreshold:0.10f);
        return searchResults;
    }
}