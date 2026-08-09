using AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.App.Server.Domain.Mapping.Logs;

public class RecordLogProfile : Profile
{
    public RecordLogProfile()
    {
        CreateMap<RecordLog, RecordLogDto>();
    }
}
