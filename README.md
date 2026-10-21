# BreweryAPI

A versioned ASP.NET Core REST API that retrieves brewery data from [OpenBreweryDB](https://www.openbrewerydb.org/documentation), maps it to a consistent application model, and exposes searchable, sortable, paginated results.

## Technology stack

- **C# / ASP.NET Core 3.1** web API
- **Entity Framework Core 3.1** with **SQLite** persistence
- ASP.NET Core **dependency injection** and typed `HttpClient`
- `System.Net.Http.Json` for upstream JSON deserialization
- ASP.NET Core **in-memory caching** (`IMemoryCache`)
- URL-segment **API versioning** with `Microsoft.AspNetCore.Mvc.Versioning`
- **MSTest**, **Moq**, and **coverlet** for automated tests and coverage collection
- Console and Debug logging through `Microsoft.Extensions.Logging`

## Features

- `GET /api/v1/breweries` endpoint for brewery results
- `GET /api/v1/breweries/autocomplete` endpoint for brewery-name suggestions
- Search across brewery name, city, and phone number
- Filter by city
- Sort by brewery name, city, or calculated distance
- Ascending and descending sort order
- Optional origin latitude and longitude for distance calculations in kilometers
- Pagination with a configurable page size capped at 200 results
- Mapping from OpenBreweryDB responses to the API's generic `ItemDto` model
- Ten-minute caching to limit calls to the upstream brewery service
- Optional SQLite-backed repository that refreshes persisted brewery data at most once every ten minutes
- Cancellation-token support for asynchronous API and data-access operations
- Structured error handling for upstream failures and unexpected server errors
- Separation of controllers, services, repositories, interfaces, models, mapping, and persistence concerns
- Service-level test coverage for distance calculation and distance sorting

## API examples

### List breweries

```http
GET /api/v1/breweries?page=1&pageSize=25
```

### Search and filter

```http
GET /api/v1/breweries?search=coffee&city=Portland
```

### Sort by distance

```http
GET /api/v1/breweries?sortBy=distance&sortOrder=asc&originLat=45.5152&originLon=-122.6784
```

### Autocomplete

```http
GET /api/v1/breweries/autocomplete?term=stone&limit=10
```

## Data source

Brewery data is provided by [OpenBreweryDB](https://www.openbrewerydb.org/documentation). The API requests brewery records, transforms the source response, and returns the fields required by the application, including brewery name, city, phone number, coordinates, and calculated distance when an origin is supplied.

## Project structure

- `BreweryAPI/Controllers` — versioned HTTP endpoints
- `BreweryAPI/Services` — business logic, repositories, caching, and service interfaces
- `BreweryAPI/Models` — request, response, and DTO models
- `BreweryAPI/Infrastructure/Persistence` — EF Core SQLite context
- `BreweryTest` — MSTest unit tests using Moq

## Running locally

The solution targets `netcoreapp3.1` and contains the API and test projects.

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project BreweryAPI/BreweryAPI.csproj
```

The SQLite connection string is configured in `BreweryAPI/appsettings.json` and uses `brewery.db` as the local database file.
