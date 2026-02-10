using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Infrastructure.Mapping;
using BreweryAPI.Models;
using BreweryAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Services
{
    public sealed class BreweryService : IBreweryService
    {
        private readonly IBreweryRepository _repo;
        private readonly ILogger<BreweryService> _logger;

        public BreweryService(IBreweryRepository repo, ILogger<BreweryService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<PagedResult<ItemDto>> GetBreweriesAsync(QueryParams query, CancellationToken ct)
        {
            var all = await _repo.GetAllAsync(ct);

            // Transform
            var dtos = all.Select(b => ItemMapper.ToDto(b, query.OriginLat, query.OriginLon));

            // Filter
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim().ToLowerInvariant();
                dtos = dtos.Where(d =>
                    d.Name.ToLowerInvariant().Contains(term) ||
                    d.City.ToLowerInvariant().Contains(term) ||
                    (!string.IsNullOrEmpty(d.Phone) && d.Phone.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                var city = query.City.Trim().ToLowerInvariant();
                dtos = dtos.Where(d => d.City.ToLowerInvariant() == city);
            }

            // Sort
            bool desc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            dtos = query.SortBy?.ToLowerInvariant() switch
            {
                "city" => desc ? dtos.OrderByDescending(d => d.City).ThenBy(d => d.Name)
                               : dtos.OrderBy(d => d.City).ThenBy(d => d.Name),
                "distance" => desc ? dtos.OrderByDescending(d => d.DistanceKm ?? double.MaxValue)
                               : dtos.OrderBy(d => d.DistanceKm ?? double.MaxValue),
                _ => desc ? dtos.OrderByDescending(d => d.Name)
                          : dtos.OrderBy(d => d.Name)
            };

            // Pagination
            int total = dtos.Count();
            int skip = Math.Max(0, (query.Page - 1) * query.PageSize);
            var pageItems = dtos.Skip(skip).Take(query.PageSize).ToList();

            _logger.LogDebug("Returning {Count} breweries (page {Page})", pageItems.Count, query.Page);

            return new PagedResult<ItemDto>
            {
                Items = pageItems,
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total
            };
        }

        public async Task<IReadOnlyList<string>> AutocompleteAsync(string term, int limit, CancellationToken ct)
        {
            var all = await _repo.GetAllAsync(ct);
            var t = term.Trim().ToLowerInvariant();

            var names = all.Select(b => b.name)
                           .Where(n => !string.IsNullOrWhiteSpace(n) && n.ToLowerInvariant().Contains(t))
                           .Distinct()
                           .OrderBy(n => n)
                           .Take(limit)
                           .ToList();

            return names;
        }
    }

}
