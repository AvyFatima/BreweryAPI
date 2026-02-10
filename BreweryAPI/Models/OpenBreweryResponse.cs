using System;
namespace BreweryAPI.Models
{
    public sealed class OpenBreweryResponse
    {
        public string id { get; set; } = default!;
        public string name { get; set; } = default!;
        public string city { get; set; } = default!;
        public string phone { get; set; } = default!;
        public double? latitude { get; set; } = default!;
        public double? longitude { get; set; } = default!;
    }
}
