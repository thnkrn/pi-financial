using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.User;

public class User : IAggregateRoot
{
    public User(Guid id, Guid iamUserId, string firstName, string lastName, string email)
    {
        Id = id;
        IamUserId = iamUserId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public Guid Id { get; set; }
    public Guid IamUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
