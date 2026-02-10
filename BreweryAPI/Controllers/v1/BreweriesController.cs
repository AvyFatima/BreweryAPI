
// Controllers/v1/BreweriesController.cs
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Models;
using BreweryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/breweries")]
    //[Route("api/[controller]")]
    public sealed class BreweriesController : ControllerBase
    {
        private readonly IBreweryService _service;
        private readonly ILogger<BreweriesController> _logger;

        public BreweriesController(IBreweryService service, ILogger<BreweriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ItemDto>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? city,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] double? originLat,
            [FromQuery] double? originLon,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            try
            {
                var query = new QueryParams
                {
                    Search = search,
                    City = city,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    OriginLat = originLat,
                    OriginLon = originLon,
                    Page = page,
                    PageSize = Math.Clamp(pageSize, 1, 200)
                };

                var result = await _service.GetBreweriesAsync(query, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Upstream error");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected server error" });
            }
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult<IReadOnlyList<string>>> Autocomplete(
            [FromQuery] string term,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { error = "term is required" });
            limit = Math.Clamp(limit, 1, 50);

            try
            {
                var result = await _service.AutocompleteAsync(term, limit, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Autocomplete error");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected server error" });
            }
        }


    }
}

