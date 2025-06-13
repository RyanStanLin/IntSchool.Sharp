using System;
using System.ComponentModel.DataAnnotations;
using IntCopilot.Sniffer.StudentId.Models;

namespace IntCopilot.Sniffer.StudentId.Configuration
{
    public class SnifferConfiguration
    {
        [Required(ErrorMessage = "XToken is required to authenticate with the API.")]
        public string XToken { get; set; } = null!; // 新增Token字段

        [Required]
        public string InitialStudentId { get; set; } = null!;

        [Required]
        public string InitialStudentName { get; set; } = null!;

        public SchoolYearPreset SchoolYearPreset { get; set; } = SchoolYearPreset.Current;
        
        public TimeWindow TimeWindow { get; set; } = TimeWindow.ThisWeek;
        
        [Range(1, 100, ErrorMessage = "Rate limit must be between 1 and 100 requests per second.")]
        public int RateLimitPerSecond { get; set; } = 1;
        
        [Range(0, 10)]
        public int MaxRetries { get; set; } = 3;
        
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
        
        public TimeSpan StateUpdateInterval { get; set; } = TimeSpan.FromSeconds(1);
    }
}