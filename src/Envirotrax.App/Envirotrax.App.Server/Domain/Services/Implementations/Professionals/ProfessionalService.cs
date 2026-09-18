
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalService : Service<Professional, ProfessionalDto>, IProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;
    private readonly IAuthorizeNetPaymentService _authorizeNetPaymentService;

    public ProfessionalService(
        IMapper mapper,
        IProfessionalRepository repository,
        IProfessionalUserRepository professionalUserRepository,
        IAuthService authService,
        IAuthorizeNetPaymentService authorizeNetPaymentService)
        : base(mapper, repository)
    {
        _professionalRepository = repository;
        _professionalUserRepository = professionalUserRepository;
        _authService = authService;
        _authorizeNetPaymentService = authorizeNetPaymentService;
    }

    public async Task<IPagedData<ProfessionalDto>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);

        var professionals = await _professionalRepository.GetAllMyAsync(pageInfo, query, cancellationToken);
        var professionalsDto = Mapper.Map<IEnumerable<Professional>, IEnumerable<ProfessionalDto>>(professionals);

        return professionalsDto.ToPagedData(pageInfo);
    }

    public async Task<ProfessionalDto?> GetLoggedInProfessionalAsync()
    {
        return await GetLoggedInProfessionalAsync(CancellationToken.None);
    }

    public async Task<ProfessionalDto?> GetLoggedInProfessionalAsync(CancellationToken cancellationToken)
    {
        return await GetAsync(_authService.ProfessionalId, cancellationToken);
    }

    public async Task<IReadOnlyList<ProfessionalDto>> GetSubAccountsAsync(CancellationToken cancellationToken)
    {
        var subAccounts = await _professionalRepository.GetSubAccountsAsync(cancellationToken);
        return [.. Mapper.Map<IEnumerable<ProfessionalDto>>(subAccounts)];
    }

    public async Task<ProfessionalDto> UpdateMyAccountBalanceAsync(ProfessionalAccountBalanceDto dto, CancellationToken cancellationToken)
    {
        var professional = await _professionalRepository.GetTrackedForUpdateAsync(_authService.ProfessionalId, cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");

        if (dto.AmountToAdd > 0)
        {
            var chargeResult = await _authorizeNetPaymentService.ChargeAsync(
                dto.DataDescriptor,
                dto.DataValue,
                dto.AmountToAdd,
                new AuthorizeNetBillingInfo
                {
                    FirstName = dto.BillingFirstName,
                    LastName = dto.BillingLastName,
                    Address = dto.BillingAddress,
                    City = dto.BillingCity,
                    State = dto.BillingState.Code,
                    Zip = dto.BillingZipCode
                },
                cancellationToken);

            if (!chargeResult.IsApproved)
            {
                throw new AppValidationException($"Your card was declined: {chargeResult.ErrorMessage}");
            }

            professional.AccountBalance += dto.AmountToAdd;
        }

        var professionalUser = await _professionalUserRepository.GetTrackedForUpdateAsync(_authService.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Professional user not found.");

        professionalUser.BillingFirstName = dto.BillingFirstName;
        professionalUser.BillingLastName = dto.BillingLastName;
        professionalUser.BillingAddress = dto.BillingAddress;
        professionalUser.BillingCity = dto.BillingCity;
        professionalUser.BillingStateId = dto.BillingState.Id;
        professionalUser.BillingZipCode = dto.BillingZipCode;

        await _professionalRepository.SaveChangesAsync();

        return MapToDto(professional)!;
    }

    public async Task<ProfessionalDto> AddMyAsync(CreateProfessionalDto createProfessional)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var added = await AddAsync(createProfessional.Professional);

            await _professionalUserRepository.AddAsync(new()
            {
                ProfessionalId = added.Id,
                UserId = _authService.UserId,
                ContactName = createProfessional.User.ContactName,
                JobTitle = createProfessional.User.JobTitle,
                IsAdmin = true,
                IsBackflowTester = createProfessional.Professional.HasBackflowTesting,
                IsCsiInspector = createProfessional.Professional.HasCsiInspection,
                IsFogInspector = createProfessional.Professional.HasFogInspection,
                IsFogTransporter = createProfessional.Professional.HasFogTransportation,
                IsWiseGuy = createProfessional.Professional.HasWiseGuys
            });

            scope.Complete();
            return added;
        }
    }
}