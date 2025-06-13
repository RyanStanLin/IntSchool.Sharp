using System;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Configuration;
using IntSchool.Sharp.Core.LifeCycle; // 需要访问SnifferConfiguration来获取Token
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntCopilot.Sniffer.StudentId.Infrastructure
{
    // 这个类是您提供的Api类的真实包装器
    public class LiveApiClient : IApiClient
    {
        private readonly ILogger<LiveApiClient> _logger;
        private readonly string _xToken;

        // Api.Instance 是一个静态单例，所以我们不需要注入它
        // 但是我们需要注入配置来获取Token
        public LiveApiClient(ILogger<LiveApiClient> logger, IOptions<SnifferConfiguration> options)
        {
            _logger = logger;
            
            // 从配置中获取Token
            _xToken = options.Value.XToken;
            if (string.IsNullOrWhiteSpace(_xToken))
            {
                throw new ArgumentException("XToken must be provided in the configuration.", nameof(options.Value.XToken));
            }

            _logger.LogInformation("LiveApiClient initialized.");
        }

        private void SetApiToken()
        {
            // 每次调用前都设置Token，确保它是最新的（如果未来Token会刷新）
            // 如果Token是静态的，这也可以在构造函数中只设置一次
            Api.Instance.XToken = _xToken;
        }

        public async Task<ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>> GetCurrentSchoolYearAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Calling real API: GetCurrentSchoolYearAsync");
            SetApiToken();
            
            // 注意：您提供的API方法名可能是 GetCurrentSchoolYearAsync
            // 如果不是，请替换成实际的方法名
            var result = await Api.Instance.GetCurrentSchoolYearAsync(); 
            
            _logger.LogDebug("API call GetCurrentSchoolYearAsync completed. Success: {IsSuccess}", result.IsSuccess);
            return result;
        }

        public async Task<ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>> GetStudentCurriculumAsync(SharedStudentTimespanConfiguration config, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Calling real API: GetStudentCurriculumAsync for student {StudentId}", config.StudentId);
            SetApiToken();
            
            // 注意：您提供的API方法名可能是 GetStudentCurriculumAsync
            // 如果不是，请替换成实际的方法名
            var result = await Api.Instance.GetStudentCurriculumAsync(config);
            
            _logger.LogDebug("API call GetStudentCurriculumAsync for student {StudentId} completed. Success: {IsSuccess}", config.StudentId, result.IsSuccess);
            return result;
        }
    }
}