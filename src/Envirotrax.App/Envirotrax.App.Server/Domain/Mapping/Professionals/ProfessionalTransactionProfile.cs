using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalTransactionProfile : Profile
{
    public ProfessionalTransactionProfile()
    {
        CreateMap<ProfessionalTransaction, ProfessionalTransactionDto>()
            .ForMember(dto => dto.TransactionId, opt => opt.MapFrom(transaction => transaction.TransactionId ?? string.Empty));
    }
}
