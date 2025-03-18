using Infrastructure.Entities;

namespace Infrastructure.Repositories.ApiCallTrackingRepository;

public class InMemoryApiCallTrackingRepository : IApiCallTrackingRepository
{
    private ApiCallTrackingEntity _lastSuccessfulCallEntity;

    public InMemoryApiCallTrackingRepository()
    {
        _lastSuccessfulCallEntity = new ApiCallTrackingEntity();
    }

    public DateTime? GetLastSuccessfulCall()
    {
        return _lastSuccessfulCallEntity.LastSuccessfulCall;
    }

    public void SetLastSuccessfulCall(DateTime lastCallTime)
    {
        _lastSuccessfulCallEntity.LastSuccessfulCall = lastCallTime;
    }
}
