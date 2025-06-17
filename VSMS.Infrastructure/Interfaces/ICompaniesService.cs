using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompaniesService
{
    Task<CompanyDto> Create(CompanyDto model);
    Task<CompanyDto> Update(CompanyDto model);
    Task<bool> DeleteById(Guid id);
    Task<CompanyDto> GetById(Guid id);
    Task<bool> IsTitleExists(string title);
}