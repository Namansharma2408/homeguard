# MonsterASP.NET single-site deployment

HomeGuard AI is deployed as one ASP.NET Core 10 website. The GitHub Actions workflow builds the Vite app, copies its files into the ASP.NET project's `wwwroot`, publishes the server project, and sends that one publish directory to the website through MonsterASP.NET's documented Web Deploy workflow. The deployable directory is also kept as a GitHub Actions artifact.

MonsterASP.NET's Control Panel `Build .NET app` documentation guarantees `dotnet publish`; it does not state that Node.js/npm is available to MSBuild targets. This repository therefore builds the frontend on the GitHub Actions Windows runner. Use Web Deploy from the workflow rather than relying on an undocumented Node toolchain in the Control Panel build environment.

## One-time hosting setup

1. Create one ASP.NET Core 10 website and one MSSQL database in MonsterASP.NET.
2. Enable Web Deploy for that website. Add these GitHub repository secrets from the Control Panel's Visual Studio access card: `WEBSITE_NAME`, `SERVER_COMPUTER_NAME`, `SERVER_USERNAME`, and `SERVER_PASSWORD`.
3. In the website's **Scripting → Environment variables**, add:

   | Name | Value |
   | --- | --- |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `Database__Provider` | `sqlserver` |
   | `ConnectionStrings__Default` | The database's **local** connection string from the hosting panel |
   | `Seed__Admin__Email` | Your initial administrator email |
   | `Seed__Admin__Password` | A secure, unique administrator password |
   | `Seed__DemoData` | `true` |

   These values belong in the Control Panel, never in source control or the workflow. Restart the application after setting them.
4. Push to `main` or start **Build and deploy HomeGuard AI to MonsterASP.NET** manually in GitHub Actions. Pull requests build, test, and publish the artifact but do not deploy.

The first application start applies the SQL Server migrations and then seeds the configured administrator and demo data. SQLite remains the local default and uses its own existing migration history. The SQL Server migration set is separate so provider-specific migration operations cannot be applied to SQLite.

The React API client already uses same-origin `/api` paths and includes browser credentials. The authentication cookie is HTTP-only, secure, and same-site lax. The SPA fallback serves extensionless client paths; unknown `/api`, `/scalar`, `/openapi`, `/health`, `/hubs`, `/hangfire`, and `/swagger` paths remain 404 responses.

## Local artifact build

Run these from the repository root on a machine with Node.js 22 and the .NET 10 SDK:

```powershell
Push-Location netforge.client
npm ci
npm run build
Pop-Location

New-Item -ItemType Directory -Force NetForge.Server/wwwroot | Out-Null
Copy-Item -Path netforge.client/dist/* -Destination NetForge.Server/wwwroot -Recurse -Force
dotnet restore NetForge.Server/NetForge.Server.csproj
dotnet publish NetForge.Server/NetForge.Server.csproj -c Release -o ./publish
```

`NetForge.Server/wwwroot` and `publish/` are generated and ignored by Git. Do not place database credentials in `appsettings*.json` or commit a local `.env` file.
