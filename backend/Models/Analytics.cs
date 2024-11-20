

namespace HouseOs.Models;

public class AnalyticsPeriod
{
    public int Count { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

}

public class AnalyticsData
{
    public AnalyticsPeriod? Today { get; set; }
    public AnalyticsPeriod? ThisWeek { get; set; }
    public AnalyticsPeriod? ThisMonth { get; set; }
    public AnalyticsPeriod? ThisYear { get; set; }
    public AnalyticsPeriod? AllTime { get; set; }
}


public class PopularItems
{
    public string? Name { get; set; }
    public int Count { get; set; }
    public decimal AverageSpend { get; set; }
}

public class PriceTrend
{
    public DateTime? Date { get; set; }
    public decimal Price { get; set; }
}
