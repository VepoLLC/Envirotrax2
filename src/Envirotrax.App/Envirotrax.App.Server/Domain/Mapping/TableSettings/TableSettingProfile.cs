using AutoMapper;
using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Domain.DataTransferObjects.TableSettings;

namespace Envirotrax.App.Server.Domain.Mapping.TableSettings;

public class TableSettingProfile : Profile
{
    public TableSettingProfile()
    {
        CreateMap<TableSetting, TableSettingDto>()
            .ReverseMap()
            .ForMember(setting => setting.UserId, opt => opt.Ignore())
            .ForMember(setting => setting.User, opt => opt.Ignore());
    }
}
