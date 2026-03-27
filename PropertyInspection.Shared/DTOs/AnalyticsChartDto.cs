using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class AnalyticsChartDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public List<int> Data { get; set; } = new();
    }

    public class AnalyticsChartDto
    {
        public List<string> Labels { get; set; } = new();
        public List<AnalyticsChartDatasetDto> Datasets { get; set; } = new();
    }
}
