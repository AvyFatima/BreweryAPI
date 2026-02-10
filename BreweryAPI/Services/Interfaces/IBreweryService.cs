using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Services.Interfaces
{
    public interface IBreweryService
    {
        Task<PagedResult<ItemDto>> GetBreweriesAsync(QueryParams query, CancellationToken ct);
        Task<IReadOnlyList<string>> AutocompleteAsync(string term, int limit, CancellationToken ct);
    }
}
