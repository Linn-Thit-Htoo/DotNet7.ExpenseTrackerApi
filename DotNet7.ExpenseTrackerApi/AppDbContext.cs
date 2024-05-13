using DotNet7.ExpenseTrackerApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNet7.ExpenseTrackerApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<IncomeModel> Income {  get; set; }
        public DbSet<BalanceModel> Balance { get; set; }
    }
}
