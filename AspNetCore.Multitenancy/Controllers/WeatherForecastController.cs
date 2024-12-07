using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AspNetCore.Multitenancy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken = default)
        {
            return await _dbContext.WeatherForecasts
                .Include(w => w.Tenant)
                .ToListAsync(cancellationToken);
        }
    }
}
