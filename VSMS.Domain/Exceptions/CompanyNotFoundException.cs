﻿namespace VSMS.Domain.Exceptions;

public class CompanyNotFoundException : Exception
{
    public CompanyNotFoundException(Guid id)
        : base($"Company with ID '{id}' was not found.") { }
}