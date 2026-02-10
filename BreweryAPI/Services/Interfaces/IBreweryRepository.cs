using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Services.Interfaces
{
    public interface IBreweryRepository
    {
        Task<IReadOnlyList<OpenBreweryResponse>> GetAllAsync(CancellationToken ct);
    }
}



