# HomeGuard AI

AI-assisted property intelligence and protection platform. Helps homeowners and property managers
monitor property health, identify potential risks, manage incidents, and organize preventive maintenance.

## Features

- Executive dashboard with KPIs, risk distribution, recent events and incidents
- Property management (CRUD, search/filter, health score)
- Deterministic C# risk engine with configurable thresholds
- Simulated telemetry ingestion and sample data generation
- Incident management with lifecycle tracking
- Role-based permissions, auth, settings (from NetForge platform)

## Stack

ASP.NET Core 10, C#, EF Core 10, React 19, TypeScript, Vite, Tailwind CSS 4, shadcn/ui,
PostgreSQL/SQLite, TanStack Query, Zustand.

## Prerequisites

- .NET 10 SDK
- Node.js 20+

## Setup

Backend:
```
cd NetForge.Server
dotnet restore
dotnet run
```

Frontend (dev):
```
cd netforge.client
npm install
npm run dev
```

Database: SQLite by default (App_Data/netforge.db). Migrations applied on boot.

## Environment variables

- `ConnectionStrings:Default` (SQLite path)
- `Database:Provider` (sqlite)
- `Seed:Admin:Email`, `Seed:Admin:Password`
- `Seed:DemoData` (true to seed demo users/content)

## Testing

Backend:
```
dotnet test tests/Tests.Unit/NetForge.Tests.Unit.csproj
```

Frontend:
```
cd netforge.client && npm run build
```

## API docs

Scalar UI: http://localhost:5000/scalar (development)

## Demo data

8 properties, sensors/readings, 36+ risk events, 6 incidents seeded in Development.

## Demo credentials

- Email: admin@homeguard.local
- Password: Admin123!$

## Licensing

MIT. Builds upon NetForge Community; see THIRD_PARTY_NOTICES.md and LICENSE.