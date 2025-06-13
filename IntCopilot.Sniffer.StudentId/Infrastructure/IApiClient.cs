using System.Threading;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;

namespace IntCopilot.Sniffer.StudentId.Infrastructure
{
    // 抽象出API调用，便于测试和替换
    public interface IApiClient
    {
        Task<ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>> GetCurrentSchoolYearAsync(CancellationToken cancellationToken = default);
        Task<ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>> GetStudentCurriculumAsync(SharedStudentTimespanConfiguration config, CancellationToken cancellationToken = default);
    }
}