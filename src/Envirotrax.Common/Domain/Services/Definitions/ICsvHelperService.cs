
using System.Collections;

namespace Envirotrax.Common.Domain.Services.Defintions
{
    public interface ICsvHelperService
    {
        Task<string> WriteAsStringAsync(IEnumerable records, IDictionary<string, string> selectedColumns, CsvConfigurationDto configuration);
    }

    public class CsvConfigurationDto
    {
        public string? Delimiter { get; set; }
    }
}