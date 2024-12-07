﻿namespace AspNetCore.Multitenancy;

public interface IHasTenant
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
}

public class Tenant : AggregateRoot
{
    public required string Name { get; set; }
}
