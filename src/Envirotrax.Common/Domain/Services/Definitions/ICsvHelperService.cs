
using System.Collections;

namespace Envirotrax.Common.Domain.Services.Defintions
{
    public interface ICsvHelperService
    {
        Task<string> WriteAsStringAsync(IEnumerable records, IDictionary<string, string> selectedColumns);
    }
}