
using AutoMapper;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
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

        CreateMap<BackflowTestDto, PublicBackflowTestDetailsDto>();

        CreateMap<CsiInspectionDto, PublicCsiInspectionDetailsDto>();
    }
}
