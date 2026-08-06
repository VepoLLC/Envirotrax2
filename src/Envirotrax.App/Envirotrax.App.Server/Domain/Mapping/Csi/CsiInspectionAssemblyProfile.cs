using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectionAssemblyProfile : Profile
{
    public CsiInspectionAssemblyProfile()
    {
        CreateMap<CsiInspectionVisuallyIdentifiedAssembly, CsiInspectionAssemblyDto>()
            .AfterMap((model, dto) =>
            {
                dto.Disapproved = model.Test?.Disapproved ?? false;
                dto.Rejected = model.Test?.Rejected ?? false;
            });
    }
}
