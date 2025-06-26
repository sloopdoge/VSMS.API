namespace VSMS.Domain.Exceptions;

public class StockNotFoundException(Guid id) : Exception($"Company with ID '{id}' was not found.");