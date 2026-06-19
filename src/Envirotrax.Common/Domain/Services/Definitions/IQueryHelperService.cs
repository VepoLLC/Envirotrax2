
using System.Collections.Specialized;
using DeveloperPartners.SortingFiltering;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IQueryHelperService
{
    NameValueCollection BuildQuery(PageInfo pageInfo, Query query);
}