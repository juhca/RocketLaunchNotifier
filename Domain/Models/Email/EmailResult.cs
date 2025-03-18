namespace Domain.Models.Email;

public record EmailResult(bool Success, string? Message = null, Exception? Exception = null);
