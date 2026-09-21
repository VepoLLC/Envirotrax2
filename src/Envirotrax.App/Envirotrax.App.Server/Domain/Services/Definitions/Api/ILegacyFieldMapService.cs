using Envirotrax.App.Server.Domain.DataTransferObjects.Api;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Api;

public interface ILegacyFieldMapService
{
    IReadOnlyCollection<LegacyFieldDescriptor> GetFields(LegacyApiTable table);

    IReadOnlyCollection<LegacyCriterionDescriptor> GetCriteria(LegacyApiTable table);

    LegacyFieldDescriptor? FindField(LegacyApiTable table, string wireName);

    LegacyFieldDescriptor? FindOrderByField(LegacyApiTable table, string wireName);

    LegacyCriterionDescriptor? FindCriterion(LegacyApiTable table, string parameterName);
}
