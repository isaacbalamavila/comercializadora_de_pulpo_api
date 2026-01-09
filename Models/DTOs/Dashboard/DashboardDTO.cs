namespace comercializadora_de_pulpo_api.Models.DTOs.Dashboard
{
    public class DashboardDTO
    {
        public bool IsAdmin { get; set; }
        public int TodaySales { get; set; }
        public int TodayPurchases { get; set; }
        public int? DaysOfWork { get; set; }
        public int? MonthClients { get; set; }
        public int[] WeekPurchases { get; set; } = [];
        public int[] WeekSales { get; set; } = [];
    }
}
