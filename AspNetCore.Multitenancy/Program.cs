using AspNetCore.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Scalar;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TenantProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<WeatherContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), 
        b => b.EnableRetryOnFailure(3));
}, ServiceLifetime.Scoped);

builder.Services.AddDbContext<InitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), 
        b => b.EnableRetryOnFailure(3));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider
        .GetRequiredService<InitContext>();
    dbContext.Init();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
