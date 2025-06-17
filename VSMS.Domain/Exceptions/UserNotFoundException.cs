namespace VSMS.Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid id)
        : base($"User with ID '{id}' was not found.") { }

    public UserNotFoundException(string email)
        : base($"User with email '{email}' was not found.") { }
}
