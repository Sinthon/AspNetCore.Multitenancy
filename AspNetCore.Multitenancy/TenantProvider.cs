using Microsoft.AspNetCore.Http;

namespace AspNetCore.Multitenancy;

public class TenantProvider
{
    private const string TenantHeaderName = "X-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var tenantIdHeader = _httpContextAccessor
            .HttpContext?
            .Request.Headers[TenantHeaderName];

        if (!tenantIdHeader.HasValue || 
            !Guid.TryParse(tenantIdHeader, out Guid tenantId))
        {
            throw new ApplicationException("Tenant Id is not present.");
        }

        return tenantId;
    }
}
