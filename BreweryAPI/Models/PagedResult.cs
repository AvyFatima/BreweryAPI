using System;
using System.Collections.Generic;

namespace BreweryAPI.Models
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
