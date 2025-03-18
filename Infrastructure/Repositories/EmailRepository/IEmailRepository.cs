using Infrastructure.Entities;

namespace Infrastructure.Repositories.EmailRepository;

public interface IEmailRepository
{
    IEnumerable<string> GetAllEmails();
    EmailEntity? GetById(int emailId);
    void SaveEmail(string email);
    EmailEntity? UpdateEmail(int emailId, string email);
    void DeleteEmail(int emailId);
}
