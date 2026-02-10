using System;

namespace BreweryAPI.Models
{
    public sealed class ItemDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; } // computed when origin provided
    }
}

//This is added to support init which is available only for c# 9.0 and above
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
