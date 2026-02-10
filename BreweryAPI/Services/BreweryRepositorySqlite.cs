using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Infrastructure.Persistence;
using BreweryAPI.Models;
using BreweryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Services
{
    public sealed class BreweryRepositorySqlite : IBreweryRepository
    {
        private readonly BreweryDbContext _db;
        private readonly HttpClient _http;
        private readonly ICacheProvider _cache;
        private readonly ILogger<BreweryRepositorySqlite> _logger;
        private const string CacheKey = "brewerydb_refresh";

        public BreweryRepositorySqlite(BreweryDbContext db, HttpClient http, ICacheProvider cache, ILogger<BreweryRepositorySqlite> logger)
        {
            _db = db; _http = http; _cache = cache; _logger = logger;
        }

        public async Task<IReadOnlyList<OpenBreweryResponse>> GetAllAsync(CancellationToken ct)
        {
            var url = BreweryRepository.BaseApiUrl + $"?per_page=200";
            // Ensure refresh at most every 10 minutes
            await _cache.GetOrCreateAsync(CacheKey, TimeSpan.FromMinutes(10), async () =>
            {
                var list = await _http.GetFromJsonAsync<List<OpenBreweryResponse>>(url, ct) ?? new();
                var entities = list.Select(b => new ItemDto
                {
                    Id = b.id,
                    Name = b.name,
                    City = b.city,
                    Phone = b.phone,
                    Latitude = b.latitude,
                    Longitude = b.longitude
                }).ToList();

                using var tx = await _db.Database.BeginTransactionAsync(ct);
                _db.Breweries.RemoveRange(_db.Breweries);
                await _db.SaveChangesAsync(ct);
                await _db.Breweries.AddRangeAsync(entities, ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                _logger.LogInformation("Refreshed {Count} breweries in SQLite", entities.Count);
                return true;
            });

            // Return mapped back to OpenBreweryResponse for service pipeline
            var all = await _db.Breweries.AsNoTracking().ToListAsync(ct);
            return all.Select(e => new OpenBreweryResponse
            {
                id = e.Id,
                name = e.Name,
                city = e.City,
                phone = e.Phone,
                latitude = e.Latitude,
                longitude = e.Longitude
            }).ToList();
        }
    }

}
