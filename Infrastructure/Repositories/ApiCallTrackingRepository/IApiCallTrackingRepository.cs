using System;

namespace Infrastructure.Repositories.ApiCallTrackingRepository;

public interface IApiCallTrackingRepository
{
    DateTime? GetLastSuccessfulCall();
    void SetLastSuccessfulCall(DateTime lastCallTime);
}
