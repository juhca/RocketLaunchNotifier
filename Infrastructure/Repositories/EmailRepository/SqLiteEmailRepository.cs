using Infrastructure.Data;
using Infrastructure.Entities;

namespace Infrastructure.Repositories.EmailRepository;

public class SqLiteEmailRepository : IEmailRepository
{
    private readonly RocketLaunchDbContext _context;

    public SqLiteEmailRepository(RocketLaunchDbContext context)
    {
        _context = context;
    }

    public SqLiteEmailRepository(RocketLaunchDbContext context, string[] initialEmails)
    {
        _context = context;

        // Add initial emails to the database
        foreach (var email in initialEmails)
        {
            SaveEmail(email);
        }
    }

    public IEnumerable<string> GetAllEmails()
    {
        return _context.Emails.Select(e => e.Email).ToList();
    }

    public EmailEntity? GetById(int emailId)
    {
        return _context.Emails.Find(emailId);
    }

    public void SaveEmail(string email)
    {
        var newEmailEntry = new EmailEntity { Email = email };
        _context.Emails.Add(newEmailEntry);
        _context.SaveChanges();
    }

    public EmailEntity? UpdateEmail(int emailId, string email)
    {
        var emailEntry = _context.Emails.Find(emailId);
        if (emailEntry != null)
        {
            emailEntry.Email = email;
            _context.SaveChanges();
            return emailEntry;
        }
        return null;
    }

    public void DeleteEmail(int emailId)
    {
        var emailEntry = _context.Emails.Find(emailId);
        if (emailEntry != null)
        {
            _context.Emails.Remove(emailEntry);
            _context.SaveChanges();
        }
    }
}
