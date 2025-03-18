using Infrastructure.Data;
using Infrastructure.Entities;

namespace Infrastructure.Repositories.ApiCallTrackingRepository;

public class SqLiteApiCallTrackingRepository : IApiCallTrackingRepository
{
    private readonly RocketLaunchDbContext _context;

    public SqLiteApiCallTrackingRepository(RocketLaunchDbContext context)
    {
        _context = context;
    }

    public DateTime? GetLastSuccessfulCall()
    {
        var entry = _context.ApiCallTracking.FirstOrDefault();
        return entry?.LastSuccessfulCall;
    }

    public void SetLastSuccessfulCall(DateTime lastCallTime)
    {
        var entry = _context.ApiCallTracking.FirstOrDefault();
        if (entry == null)
        {
            _context.ApiCallTracking.Add(
                new ApiCallTrackingEntity { LastSuccessfulCall = lastCallTime }
            );
        }
        else
        {
            entry.LastSuccessfulCall = lastCallTime;
        }
        _context.SaveChanges();
    }
}
