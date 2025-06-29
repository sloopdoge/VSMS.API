using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class CompaniesService(
    ILogger<CompaniesService> logger,
    ApplicationRepository repository,
    UserManager<ApplicationUser> userManager,
    ICompanyUsersService companyUsersService) : ICompaniesService
{
    public async Task<CompanyDto> Create(CompanyDto model)
    {
        try
        {
            var newCompany = new Company
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                NormalizedTitle = model.Title.Normalize(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            
            var createResul = await repository.Companies.AddAsync(newCompany);
            var result = await repository.SaveChangesAsync();
            if (result < 1)
                throw new Exception($"Company: {newCompany.Title} - was not created");

            return new CompanyDto
            {
                Id = createResul.Entity.Id,
                Title = createResul.Entity.Title,
                CreatedAt = createResul.Entity.CreatedAt,
                UpdatedAt = createResul.Entity.UpdatedAt,
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<CompanyDto> Update(CompanyDto model)
    {
        try
        {
            var existingCompany = await repository.Companies.FindAsync(model.Id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(model.Id);

            existingCompany.Title = model.Title;
            existingCompany.NormalizedTitle = model.Title.Normalize();
            existingCompany.UpdatedAt = DateTime.UtcNow;

            repository.Companies.Update(existingCompany);
            var result = await repository.SaveChangesAsync();

            if (result < 1)
                throw new Exception($"Company: {model.Id} - was not updated");

            return new CompanyDto
            {
                Id = existingCompany.Id,
                Title = existingCompany.Title,
                CreatedAt = existingCompany.CreatedAt,
                UpdatedAt = existingCompany.UpdatedAt,
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
    
    public async Task<bool> DeleteById(Guid id)
    {
        try
        {
            var existingCompany = await repository.Companies.FindAsync(id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(id);
            
            repository.Companies.Remove(existingCompany);
            var result = await repository.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<CompanyDto> GetById(Guid id)
    {
        try
        {
            var existingCompany = await repository.Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(id);

            var result = new CompanyDto
            {
                Id = existingCompany.Id,
                Title = existingCompany.Title,
                CreatedAt = existingCompany.CreatedAt,
                UpdatedAt = existingCompany.UpdatedAt,
                UserProfiles = await companyUsersService.GetAllUsersInCompany(existingCompany.Id)
            };

            return result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<CompanyDto>> GetAll()
    {
        try
        {
            var companies = await repository.Companies
                .ToListAsync();

            var result = companies.Select(c => new CompanyDto
            {
                Id = c.Id,
                Title = c.Title,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            }).ToList();

            var tasks = result.Select(async company =>
            {
                company.UserProfiles = await companyUsersService.GetAllUsersInCompany(company.Id);
            }).ToList();

            await Task.WhenAll(tasks);

            return result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> IsTitleExists(string title)
    {
        try
        {
            var normalizedTitle = title.Normalize();
            var result = await repository.Companies.Where(c => c.NormalizedTitle == normalizedTitle).FirstOrDefaultAsync();
            
            return result is not null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}