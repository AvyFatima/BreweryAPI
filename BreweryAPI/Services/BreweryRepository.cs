using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Models;
using BreweryAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Services
{
    public sealed class BreweryRepository : IBreweryRepository
    {
        private readonly HttpClient _http;
        private readonly ICacheProvider _cache;
        private readonly ILogger<BreweryRepository> _logger;
        private const string CacheKey = "brewerydb_all";
        public const string BaseApiUrl = "https://api.openbrewerydb.org/v1/breweries";

        public BreweryRepository(HttpClient http, ICacheProvider cache, ILogger<BreweryRepository> logger)
        {
            _http = http;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<OpenBreweryResponse>> GetAllAsync(CancellationToken ct)
        {
            return await _cache.GetOrCreateAsync(CacheKey, TimeSpan.FromMinutes(10), async () =>
            {
                try
                {
                    // Fetch paged results until exhausted (defensive: cap pages)
                    var results = new List<OpenBreweryResponse>();
                    int page = 1;
                    const int perPage = 50;
                    const int maxPages = 40; // safety cap

                    while (page <= maxPages)
                    {
                        var url = BaseApiUrl + $"?per_page={perPage}&page={page}";
                        var pageData = await _http.GetFromJsonAsync<List<OpenBreweryResponse>>(url, ct);
                        if (pageData is null || pageData.Count == 0)
                            break;
                        results.AddRange(pageData);
                        page++;
                    }

                    _logger.LogInformation("Fetched {Count} breweries from OpenBreweryDB", results.Count);
                    return results;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch breweries from OpenBreweryDB");
                    throw new InvalidOperationException("Source API unavailable. Please try again later.");
                }
            });
        }
    }

}
