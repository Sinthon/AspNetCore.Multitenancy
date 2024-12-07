using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace AspNetCore.Multitenancy;

public class WeatherContext : DbContext
{
    private readonly Guid _tenantId;
    public WeatherContext(DbContextOptions<WeatherContext> options, TenantProvider tenantProvider) 
        : base(options) 
    {
        _tenantId = tenantProvider.GetTenantId();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherForecast>(builder =>
        {
            builder.ConfigureTanantEntity(p => p.TenantId == _tenantId);
        });
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
}

public class InitContext : DbContext
{
    public InitContext(DbContextOptions<InitContext> options): base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherForecast>(builder =>
        {
            builder.ConfigureTanantEntity();
        });
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
}

public static class InitContextExntensions
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static void Init(this InitContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Tenants.Any())
        {
            context.Tenants.Add(new Tenant()
            {
                Name = "Distirbutor 1"
            });
            context.SaveChanges();
        }

        if (!context.WeatherForecasts.Any())
        {
            var tenant = context.Tenants
                .Single();

            var weathers = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                TenantId = tenant.Id,
                Tenant = tenant
            })
            .ToArray();

            context.WeatherForecasts.AddRange(weathers);
            context.SaveChanges();
        }
    }


    public static EntityTypeBuilder<T> ConfigureTanantEntity<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, bool>> hasQueryFilter)
        where T : class, ITenantEntity
    {
        builder.HasIndex(x => x.TenantId);
        builder.HasQueryFilter(hasQueryFilter);
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureTanantEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, ITenantEntity
    {
        builder.HasIndex(x => x.TenantId);
        return builder;
    }
}
