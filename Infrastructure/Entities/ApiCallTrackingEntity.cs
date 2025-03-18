using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ApiCallTrackingEntity
{
    [Key]
    public int Id { get; set; } = 1;
    public DateTime? LastSuccessfulCall { get; set; }
}
