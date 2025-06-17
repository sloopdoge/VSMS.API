using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class CompaniesService(
    ILogger<CompaniesService> logger,
    CompaniesDbContext context) : ICompaniesService
{
    public async Task<CompanyDto> Create(CompanyDto model)
    {
        try
        {
            var newCompany = new Domain.Entities.Company
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                NormalizedTitle = model.Title.Normalize(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            
            var createResul = await context.Companies.AddAsync(newCompany);
            var result = await context.SaveChangesAsync();
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
            var existingCompany = await context.Companies.FindAsync(model.Id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(model.Id);

            existingCompany.Title = model.Title;
            existingCompany.NormalizedTitle = model.Title.Normalize();
            existingCompany.UpdatedAt = DateTime.UtcNow;

            context.Companies.Update(existingCompany);
            var result = await context.SaveChangesAsync();

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
            var existingCompany = await context.Companies.FindAsync(id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(id);
            
            context.Companies.Remove(existingCompany);
            var result = await context.SaveChangesAsync();
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
            var existingCompany = await context.Companies.FindAsync(id);

            if (existingCompany is null)
                throw new CompanyNotFoundException(id);

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

    public async Task<bool> IsTitleExists(string title)
    {
        try
        {
            var normalizedTitle = title.Normalize();
            var result = await context.Companies.Where(c => c.NormalizedTitle == normalizedTitle).FirstOrDefaultAsync();
            
            return result is not null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}