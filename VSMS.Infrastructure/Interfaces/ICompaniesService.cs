using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompaniesService
{
    /// <summary>
    /// Creates a new company using the provided model.
    /// </summary>
    /// <param name="model">Data for the company to create.</param>
    /// <returns>The created <see cref="CompanyDto"/>.</returns>
    Task<CompanyDto> Create(CompanyDto model);

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="model">The company data with updated values.</param>
    /// <returns>The updated <see cref="CompanyDto"/>.</returns>
    Task<CompanyDto> Update(CompanyDto model);

    /// <summary>
    /// Deletes a company by identifier.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    /// <returns><c>true</c> if the company was deleted; otherwise <c>false</c>.</returns>
    Task<bool> DeleteById(Guid id);

    /// <summary>
    /// Retrieves a company by identifier.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    /// <returns>The <see cref="CompanyDto"/> if found.</returns>
    Task<CompanyDto> GetById(Guid id);

    /// <summary>
    /// Retrieves all companies.
    /// </summary>
    /// <returns>The list of <see cref="CompanyDto"/> if found.</returns>
    Task<List<CompanyDto>> GetAll();

    /// <summary>
    /// Checks whether a company title already exists.
    /// </summary>
    /// <param name="title">Company title to check.</param>
    /// <returns><c>true</c> if a company with the title exists.</returns>
    Task<bool> IsTitleExists(string title);
}