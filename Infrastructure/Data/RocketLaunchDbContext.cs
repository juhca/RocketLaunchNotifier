using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class RocketLaunchDbContext : DbContext
{
    public RocketLaunchDbContext(DbContextOptions<RocketLaunchDbContext> options)
        : base(options) { }

    public DbSet<RocketLaunchEntity> RocketLaunches { get; set; }
    public DbSet<EmailEntity> Emails { get; set; }
    public DbSet<ApiCallTrackingEntity> ApiCallTracking { get; set; }
}
