
using AutoMapper;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;

namespace Envirotrax.App.Server.Domain.Mapping.PublicSearch;

public class PublicSearchProfile : Profile
{
    public PublicSearchProfile()
    {
        CreateMap<PublicSearchCriteriaDto, PublicSearchCriteria>();

        CreateMap<PublicSearchWaterSupplier, PublicSearchWaterSupplierDto>();

        CreateMap<PublicBackflowTestResult, PublicBackflowTestResultDto>();

        CreateMap<PublicCsiInspectionResult, PublicCsiInspectionResultDto>();
    }
}
