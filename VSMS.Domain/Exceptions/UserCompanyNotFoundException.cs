namespace VSMS.Domain.Exceptions;

public class UserCompanyNotFoundException(Guid userId) : Exception($"Company for User with ID: '{userId}' not found");