namespace VSMS.Domain.Exceptions;

public class ValidationException(string fieldName, string validationDescription) 
    : Exception($"{fieldName} is not valid. {fieldName} - must be {validationDescription}.");