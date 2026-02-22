using WeFood.Domain.Enums;

namespace WeFood.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public UserType UserType { get; private set; }
    public bool IsActive { get; private set; }

    protected User() { }

    public User(Guid id, string fullName, UserType userType)
    {
        Id = id;
        FullName = fullName;
        UserType = userType;
        IsActive = true;
    }
}