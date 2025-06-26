using System;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Configuration;
using IntSchool.Sharp.Core.LifeCycle; 
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntCopilot.Sniffer.StudentId.Infrastructure
{
    public class LiveApiClient : IApiClient
    {
        private readonly ILogger<LiveApiClient> _logger;
        private readonly string _xToken;

        public LiveApiClient(ILogger<LiveApiClient> logger, IOptions<SnifferConfiguration> options)
        {
            _logger = logger;
            
            _xToken = options.Value.XToken;
            if (string.IsNullOrWhiteSpace(_xToken))
            {
                throw new ArgumentException("XToken must be provided in the configuration.", nameof(options.Value.XToken));
            }

            _logger.LogInformation("LiveApiClient initialized.");
        }

        private void SetApiToken()
        {
            Api.Instance.XToken = _xToken;
        }

        public async Task<ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>> GetCurrentSchoolYearAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Calling real API: GetCurrentSchoolYearAsync");
            SetApiToken();
            var result = await Api.Instance.GetCurrentSchoolYearAsync(); 
            
            _logger.LogDebug("API call GetCurrentSchoolYearAsync completed. Success: {IsSuccess}", result.IsSuccess);
            return result;
        }

        public async Task<ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>> GetStudentCurriculumAsync(SharedStudentTimespanConfiguration config, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Calling real API: GetStudentCurriculumAsync for student {StudentId}", config.StudentId);
            SetApiToken();
            var result = await Api.Instance.GetStudentCurriculumAsync(config);
            
            _logger.LogDebug("API call GetStudentCurriculumAsync for student {StudentId} completed. Success: {IsSuccess}", config.StudentId, result.IsSuccess);
            return result;
        }
    }
}