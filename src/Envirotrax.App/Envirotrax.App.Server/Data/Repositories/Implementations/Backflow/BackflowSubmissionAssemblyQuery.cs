using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public static class BackflowSubmissionAssemblyQuery
{
    public static IQueryable<BackflowTest> WhereSubmissionAssembly(this IQueryable<BackflowTest> tests, BackflowSubmissionSearchDto criteria)
    {
        var assemblies = tests.Where(test => test.DeletedTime == null
            && test.IsCurrent
            && !test.OutOfService
            && test.TransactionId != null
            && test.TransactionId != string.Empty);

        if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
        {
            var manufacturer = criteria.Manufacturer.Trim();

            assemblies = assemblies.Where(test => test.Manufacturer != null && test.Manufacturer.StartsWith(manufacturer));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SerialNumber))
        {
            var serialNumber = criteria.SerialNumber.Trim();

            assemblies = assemblies.Where(test => test.SerialNumber == serialNumber);
        }

        return assemblies;
    }

    public static bool HasAssemblyCriteria(BackflowSubmissionSearchDto criteria)
    {
        return !string.IsNullOrWhiteSpace(criteria.Manufacturer) || !string.IsNullOrWhiteSpace(criteria.SerialNumber);
    }
}
