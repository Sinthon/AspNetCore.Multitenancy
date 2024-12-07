namespace AspNetCore.Multitenancy
{
    public class WeatherForecast : AggregateRoot, IHasTenant
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
