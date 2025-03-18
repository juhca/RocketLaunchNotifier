using Infrastructure.Entities;

namespace Infrastructure.Repositories.EmailRepository;

public class InMemoryEmailRepository : IEmailRepository
{
    private readonly Dictionary<int, EmailEntity> _emails;
    private int _nextEmailId = 1;

    public InMemoryEmailRepository()
    {
        _emails = new Dictionary<int, EmailEntity>();
        _emails.Add(
            _nextEmailId,
            new EmailEntity { Email = "tomas.stefe@gmail.com", EmailId = _nextEmailId++ }
        );
        _emails.Add(
            _nextEmailId,
            new EmailEntity { Email = "tomas.stefe@enerdat.com", EmailId = _nextEmailId++ }
        );
    }

    public InMemoryEmailRepository(IEnumerable<string> initialEmails)
    {
        _emails = new Dictionary<int, EmailEntity>();
        foreach (var email in initialEmails)
        {
            SaveEmail(email);
        }
    }

    public IEnumerable<string> GetAllEmails()
    {
        return _emails.Values.Select((entry) => entry.Email).ToList();
    }

    public EmailEntity? GetById(int emailId)
    {
        _emails.TryGetValue(emailId, out var emailEntry);

        return emailEntry;
    }

    public void SaveEmail(string email)
    {
        var newEmailEntry = new EmailEntity { Email = email, EmailId = _nextEmailId++ };
        _emails[newEmailEntry.EmailId] = newEmailEntry;
    }

    public EmailEntity? UpdateEmail(int emailId, string email)
    {
        if (_emails.ContainsKey(emailId))
        {
            var emailEntry = _emails[emailId];
            emailEntry.Email = email;
            return emailEntry;
        }

        return null;
    }

    public void DeleteEmail(int emailId)
    {
        _emails.Remove(emailId);
    }
}
