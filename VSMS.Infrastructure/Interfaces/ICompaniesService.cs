using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompaniesService
{
    /// <summary>
    /// Creates a new company based on the provided data transfer object and
    /// returns the resulting company representation.
    /// </summary>
    /// <param name="model">Information about the company to be created.</param>
    /// <returns>The newly created <see cref="CompanyDto"/>.</returns>
    Task<CompanyDto> Create(CompanyDto model);

    /// <summary>
    /// Applies changes from the specified model to an existing company and
    /// returns the updated representation.
    /// </summary>
    /// <param name="model">The company data with the values to apply.</param>
    /// <returns>The updated <see cref="CompanyDto"/>.</returns>
    Task<CompanyDto> Update(CompanyDto model);

    /// <summary>
    /// Permanently removes the company identified by the given id.
    /// </summary>
    /// <param name="id">Unique identifier of the company.</param>
    /// <returns><c>true</c> when the company was removed; otherwise <c>false</c>.</returns>
    Task<bool> DeleteById(Guid id);

    /// <summary>
    /// Retrieves company details for the specified identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the company.</param>
    /// <returns>The corresponding <see cref="CompanyDto"/> if found.</returns>
    Task<CompanyDto> GetById(Guid id);

    /// <summary>
    /// Returns a list of all companies currently stored in the system.
    /// </summary>
    /// <returns>Collection of <see cref="CompanyDto"/> objects.</returns>
    Task<List<CompanyDto>> GetAll();

    /// <summary>
    /// Determines whether a company with the specified title already exists.
    /// </summary>
    /// <param name="title">Title of the company to check.</param>
    /// <returns><c>true</c> if a company with the given title is present.</returns>
    Task<bool> IsTitleExists(string title);
}