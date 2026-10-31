using AutoMapper;
using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Data.Repositories.Definitions.TableSettings;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.TableSettings;
using Envirotrax.App.Server.Domain.Services.Definitions.TableSettings;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.TableSettings;

/// <summary>
/// Table settings belong to the logged-in user. Every method here reads or changes only that user's rows.
/// </summary>
public class TableSettingService : Service<TableSetting, TableSettingDto>, ITableSettingService
{
    private readonly ITableSettingRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public TableSettingService(
        IMapper mapper,
        ITableSettingRepository repository,
        IUserRepository userRepository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _repository = repository;
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<TableSettingDto>> GetAllMyAsync(TableSettingType tableType, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllByUserAsync(_authService.UserId, tableType, cancellationToken);

        return settings.Select(setting => MapToDto(setting)!);
    }

    /// <summary>
    /// UserId is a foreign key to WaterSupplierUsers, so only a user of this water supplier can own its settings.
    /// A master account's user who works in a sub account through the parent account isn't one.
    /// </summary>
    public Task<bool> CanAddMyAsync(CancellationToken cancellationToken)
    {
        return _userRepository.ExistsAsync(_authService.UserId, cancellationToken);
    }

    public async Task<TableSettingDto> AddMyAsync(TableSettingDto dto, CancellationToken cancellationToken)
    {
        if (!await CanAddMyAsync(cancellationToken))
        {
            throw new AppValidationException("Search templates can't be saved here because you aren't a user of this water supplier account.");
        }

        var setting = MapToModel(dto)!;

        setting.Id = 0;
        setting.UserId = _authService.UserId;
        setting.Name = setting.Name.Trim();

        var added = await _repository.AddAsync(setting);

        return MapToDto(added)!;
    }

    public async Task<TableSettingDto?> UpdateMyAsync(int id, TableSettingDto dto, CancellationToken cancellationToken)
    {
        var setting = await _repository.GetTrackedForUpdateAsync(id, cancellationToken);

        if (setting == null || setting.UserId != _authService.UserId)
        {
            return null;
        }

        // The owner and the table never change. Only the name and the saved filters do.
        setting.Name = dto.Name.Trim();
        setting.Filters = dto.Filters;

        await _repository.SaveChangesAsync();

        return MapToDto(setting);
    }

    public async Task<TableSettingDto?> DeleteMyAsync(int id, CancellationToken cancellationToken)
    {
        if (!await _repository.ExistsForUserAsync(id, _authService.UserId, cancellationToken))
        {
            return null;
        }

        return await DeleteAsync(id);
    }
}
