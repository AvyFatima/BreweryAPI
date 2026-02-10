using System;
using BreweryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BreweryAPI.Infrastructure.Persistence
{
    public sealed class BreweryDbContext : DbContext
    {
        public BreweryDbContext(DbContextOptions<BreweryDbContext> options) : base(options)
        {

        }

        public DbSet<ItemDto> Breweries => Set<ItemDto>();
    }

}

