using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class RocketLaunchEntity
{
    [Key]
    public string RockerLaunchEntryId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public DateTime LaunchDate { get; set; }
    public string LaunchStatus { get; set; } = string.Empty;
    public string LaunchDescription { get; set; } = string.Empty;
}
