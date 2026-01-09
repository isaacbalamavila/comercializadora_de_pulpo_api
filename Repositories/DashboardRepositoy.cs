using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Dashboard;
using comercializadora_de_pulpo_api.Models.DTOs.User;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class DashboardRepositoy(ComercializadoraDePulpoContext context) : IDashboardRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<DashboardDTO> GetDashboardByUserAsync(User user)
        {
            bool isAdmin = user.RoleId == 1;
            DateTime today = DateTime.Now.Date;
            DateTime monthStart = new(today.Year, today.Month, 1);
            DateTime monthEnd = new(today.Year, today.Month + 1, 1);

            int daysToSubtract = ( int )today.DayOfWeek - ( int )DayOfWeek.Monday;
            if (daysToSubtract < 0)
                daysToSubtract += 7;
            DateTime weekStart = today.Date.AddDays(-daysToSubtract);

            // Crear todos los días de la semana hasta hoy
            var daysInWeek = Enumerable.Range(0, ( today - weekStart ).Days + 1)
                .Select(offset => weekStart.AddDays(offset))
                .ToList();

            if (isAdmin)
            {
                var monthClients = await _context
                    .Clients.Where(c =>
                        c.CreatedAt >= monthStart && c.CreatedAt < monthEnd
                    )
                    .CountAsync();

                var purchasesPerDay = await _context
                    .Purchases.Where(p =>
                        p.CreatedAt >= weekStart && p.CreatedAt < today.AddDays(1)
                    )
                    .GroupBy(p => p.CreatedAt.Date)
                    .Select(g => new { Fecha = g.Key, Total = g.Count() })
                    .ToListAsync();

                // Combinar todos los días con los datos reales
                int [] weekPurchases = daysInWeek
                    .Select(day => purchasesPerDay.FirstOrDefault(p => p.Fecha == day)?.Total ?? 0)
                    .ToArray();

                var salesPerDay = await _context
                    .Sales.Where(s => s.SaleDate >= weekStart && s.SaleDate < today.AddDays(1))
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new { Fecha = g.Key, Total = g.Count() })
                    .ToListAsync();

                int [] weekSales = daysInWeek
                    .Select(day => salesPerDay.FirstOrDefault(s => s.Fecha == day)?.Total ?? 0)
                    .ToArray();

                var todaySales = await _context
                    .Sales.Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1))
                    .CountAsync();
                var todayPurchases = await _context
                    .Purchases.Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1))
                    .CountAsync();

                return new DashboardDTO
                {
                    IsAdmin = isAdmin,
                    MonthClients = monthClients,
                    TodayPurchases = todayPurchases,
                    TodaySales = todaySales,
                    WeekPurchases = weekPurchases,
                    WeekSales = weekSales,
                };
            }
            else
            {
                TimeSpan workDays = today - user.CreatedAt;

                var purchasesPerDay = await _context
                    .Purchases.Where(p =>
                        p.CreatedAt >= weekStart && p.CreatedAt < today.AddDays(1) && p.UserId == user.Id
                    )
                    .GroupBy(p => p.CreatedAt.Date)
                    .Select(g => new { Fecha = g.Key, Total = g.Count() })
                    .ToListAsync();

                int [] weekPurchases = daysInWeek
                    .Select(day => purchasesPerDay.FirstOrDefault(p => p.Fecha == day)?.Total ?? 0)
                    .ToArray();

                var salesPerDay = await _context
                    .Sales.Where(s => s.SaleDate >= weekStart && s.SaleDate < today.AddDays(1) && s.EmployeeId == user.Id)
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new { Fecha = g.Key, Total = g.Count() })
                    .ToListAsync();

                int [] weekSales = daysInWeek
                    .Select(day => salesPerDay.FirstOrDefault(s => s.Fecha == day)?.Total ?? 0)
                    .ToArray();

                var todaySales = await _context
                    .Sales.Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1) && s.EmployeeId == user.Id)
                    .CountAsync();
                var todayPurchases = await _context
                    .Purchases.Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1) && p.UserId == user.Id)
                    .CountAsync();

                return new DashboardDTO
                {
                    IsAdmin = isAdmin,
                    DaysOfWork = workDays.Days,
                    TodayPurchases = todayPurchases,
                    TodaySales = todaySales,
                    WeekPurchases = weekPurchases,
                    WeekSales = weekSales,
                };
            }
        }
    }
}
