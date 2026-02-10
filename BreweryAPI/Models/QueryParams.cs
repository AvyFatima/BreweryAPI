using System;
namespace BreweryAPI.Models
{
    public sealed class QueryParams
    {
        public string? Search { get; set; }
        public string? City { get; set; }
        public string? SortBy { get; set; } // name|city|distance
        public string? SortOrder { get; set; } // asc|desc
        public double? OriginLat { get; set; }
        public double? OriginLon { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
