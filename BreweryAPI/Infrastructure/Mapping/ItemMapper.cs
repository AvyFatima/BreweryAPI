using System;
using BreweryAPI.Models;

namespace BreweryAPI.Infrastructure.Mapping
{
    public static class ItemMapper
    {
        public static ItemDto ToDto(OpenBreweryResponse src, double? originLat = null, double? originLon = null)
        {
            double? lat = src.latitude;
            double? lon = src.longitude;

            return new ItemDto
            {
                Id = src.id,
                Name = src.name,
                City = src.city,
                Phone = NormalizePhone(src.phone),
                Latitude = lat,
                Longitude = lon,
                DistanceKm = (originLat.HasValue && originLon.HasValue && lat.HasValue && lon.HasValue) ? HaversineKm(originLat.Value, originLon.Value, lat.Value, lon.Value) : null
            };
        }

        private static string NormalizePhone(string phone)
            => string.IsNullOrWhiteSpace(phone) ? "" : phone.Trim();

        // Haversine formula
        private static double HaversineKm(double lat1, double lon1,
                        double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);
            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return rad * c;
        }

    }

}
