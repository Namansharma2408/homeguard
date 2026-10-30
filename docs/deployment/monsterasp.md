# MonsterASP.NET Git deployment

HomeGuard AI deploys as one ASP.NET Core 10 website. Build the React frontend locally before pushing; its generated files are committed under `NetForge.Server/wwwroot`. The server project deliberately does not reference `netforge.client.esproj`, so MonsterASP.NET's **Build .NET app** publish does not run Node.js or npm.

## Prepare and push a deployment

Run this from the repository root on a machine with Node.js/npm and the .NET 10 SDK:

```sh
./scripts/prepare-monsterasp.sh
dotnet restore NetForge.Server/NetForge.Server.csproj
dotnet publish NetForge.Server/NetForge.Server.csproj -c Release -o ./publish-test
```

Review and commit the changed files under `NetForge.Server/wwwroot`, along with `scripts/.monsterasp-vite-manifest`. The manifest lets the preparation script remove stale files from a previous Vite build without deleting unrelated server-owned static files. `node_modules`, `dist`, `obj`, `bin`, `.env` files, and local databases remain ignored.

In MonsterASP.NET, connect the GitHub repository, select **Build .NET app**, and choose `NetForge.Server/NetForge.Server.csproj`. Build and publish the selected branch. Because the frontend is already present under `wwwroot`, the server publish needs only the .NET SDK.

## Hosting configuration

Create one ASP.NET Core 10 website and one MSSQL database. In the site's **Scripting → Environment variables**, configure:

| Name | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `Database__Provider` | `sqlserver` |
| `ConnectionStrings__Default` | The database's local connection string from the hosting panel |
| `Seed__Admin__Email` | Your initial administrator email |
| `Seed__Admin__Password` | A secure, unique administrator password |
| `Seed__DemoData` | `true` |

Keep these values in the hosting control panel, never in source control. Restart the application after changing them. The application applies the SQL Server migration and seeds the configured administrator and demo data on startup. SQLite remains the local default and uses its separate migration history.

## Static frontend and routing

ASP.NET Core serves the committed Vite files from `wwwroot` through `MapStaticAssets`. The root URL serves `index.html`; the extensionless SPA fallback serves React Router paths. Unmatched `/api`, `/scalar`, `/openapi`, `/health`, `/hubs`, `/hangfire`, and `/swagger` paths remain 404 responses. Existing API endpoints, authentication, authorization, and same-origin `/api` requests continue to use the same site and cookie.

Run `scripts/prepare-monsterasp.sh` and commit its `wwwroot` changes each time the frontend changes. Do not commit credentials or local `.env` files.
