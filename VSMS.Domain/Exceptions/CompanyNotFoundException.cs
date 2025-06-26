namespace VSMS.Domain.Exceptions;

public class CompanyNotFoundException(Guid id) : Exception($"Company with ID '{id}' was not found.");