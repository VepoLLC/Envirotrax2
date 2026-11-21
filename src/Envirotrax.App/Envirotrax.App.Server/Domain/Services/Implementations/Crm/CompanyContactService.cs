using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Data.Repositories.Definitions.Crm;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.App.Server.Domain.Services.Definitions.Crm;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Crm;

public class CompanyContactService : Service<CompanyContact, CompanyContactDto>, ICompanyContactService
{
    private readonly ICompanyContactRepository _contactRepository;
    private readonly ICompanyRepository _companyRepository;

    public CompanyContactService(
        IMapper mapper,
        ICompanyContactRepository repository,
        ICompanyRepository companyRepository)
        : base(mapper, repository)
    {
        _contactRepository = repository;
        _companyRepository = companyRepository;
    }

    public async Task<IPagedData<CompanyContactDto>> GetAllByCompanyAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<CompanyContact, CompanyContactDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<CompanyContact, CompanyContactDto>(Mapper);

        var contacts = await _contactRepository.GetAllByCompanyAsync(companyId, pageInfo, query, cancellationToken);

        return contacts.Select(contact => MapToDto(contact)!).ToPagedData(pageInfo);
    }

    public async Task<CompanyContactDto?> AddForCompanyAsync(int companyId, CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetNoIncludesAsync(companyId, cancellationToken);

        if (company == null)
        {
            return null;
        }

        var model = MapToModel(contact)!;

        // The key to the company includes its water supplier, so the contact takes the company's.
        model.Id = 0;
        model.CompanyId = companyId;
        model.WaterSupplierId = company.WaterSupplierId;

        var added = await _contactRepository.AddAsync(model);

        return MapToDto(added);
    }

    public async Task<CompanyContactDto?> UpdateForCompanyAsync(int companyId, int contactId, CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var model = MapToModel(contact)!;

        model.Id = contactId;

        var updated = await _contactRepository.UpdateForCompanyAsync(companyId, model, cancellationToken);

        return MapToDto(updated);
    }

    public async Task<CompanyContactDto?> DeleteForCompanyAsync(int companyId, int contactId, CancellationToken cancellationToken)
    {
        if (!await _contactRepository.ExistsForCompanyAsync(companyId, contactId, cancellationToken))
        {
            return null;
        }

        return await DeleteAsync(contactId);
    }
}
