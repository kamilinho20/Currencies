using Currencies.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Currencies.DataAccess;
public class AppDbContext : DbContext
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> Exchanges { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Currency>()
            .HasKey(c => c.Code);

        modelBuilder.Entity<Currency>()
            .HasMany(c => c.BaseCurrencyRates)
            .WithOne(ex => ex.BaseCurrency)
            .HasForeignKey(c => c.BaseCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Currency>()
            .HasMany(c => c.ExchangeCurrencyRates)
            .WithOne(ex => ex.ExchangeCurrency)
            .HasForeignKey(ex => ex.ExchangeCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRate>()
            .HasKey(ex => new { ex.BaseCurrencyCode, ex.ExchangeCurrencyCode, ex.ExDate });

        modelBuilder.Entity<ExchangeRate>()
            .Property(ex => ex.BidRate)
            .HasPrecision(18, 8);

        modelBuilder.Entity<ExchangeRate>()
            .Property(ex => ex.AskRate)
            .HasPrecision(18, 8);

    }
}
