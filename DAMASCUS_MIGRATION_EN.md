# TRRCMS — Damascus Administrative Data: Setup & Run Guide

## Summary

The system's seeded administrative reference data is **Damascus governorate** (code `01`): 1 district, 1 sub-district, 2 communities (Damascus / Yarmuk), and 100 neighborhoods with GIS boundaries. This guide covers running the application locally in two supported ways, plus connecting to the database with pgAdmin or Adminer.

---

## 1) Prerequisites

- Docker Desktop (Windows), running.
- .NET 8 SDK — only required for **Option B** (running the API outside Docker).
- Project checked out at, for example: `C:\Users\....\backend`

---

## 2) Option A — Full Docker (database + API both containerized)

This builds the API image and runs everything (`db`, `api`, `adminer`) as containers.

```powershell
cd C:\Users\....\backend
docker compose up --build
```

What happens:
- `db` starts first and is only considered ready once `pg_isready` succeeds (PostgreSQL 16 + PostGIS 3.4).
- `api` only starts after `db` is confirmed healthy, builds the application image, then applies all database migrations automatically on startup.
- `adminer` starts alongside `db`.

Once the API log shows it is listening, open:
```
Swagger:  http://localhost:8080/swagger
Adminer:  http://localhost:8081
```

**Stop (keeps data):**
```powershell
docker compose stop
```

**Full teardown, removes containers, keeps data volume:**
```powershell
docker compose down
```

**Note:** building the API image downloads NuGet packages over the network. On a slow or unstable connection this step can occasionally time out; if it does, simply re-run `docker compose up --build` — it resumes using already-downloaded layers and usually succeeds on retry.

---

## 3) Option B — Database only in Docker, API runs directly on the host

Use this if building the API image via Docker is impractical (slow network) or for faster local iteration (no image rebuild needed on every code change).

**Step 1 — Start only the database and Adminer:**
```powershell
cd C:\Users\....\backend
docker compose up -d db adminer
```

**Step 2 — Create/verify `src/TRRCMS.WebAPI/appsettings.Development.json`** (local file, not committed to source control) with:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TRRCMS_Dev;Username=postgres;Password=ChangeThisPassword"
  }
}
```
(The password must match the `POSTGRES_PASSWORD` / `DB_PASSWORD` value configured in `docker-compose.yml` — `ChangeThisPassword` is that file's default when no override is set.)

**Step 3 — Run the API** (use a dedicated terminal window; this command occupies it):
```powershell
cd C:\Users\....\backend\src\TRRCMS.WebAPI
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --urls "http://localhost:5031"
```

Wait for `Now listening on: http://localhost:5031`, then open:
```
Swagger:  http://localhost:5031/swagger
Adminer:  http://localhost:8081
```

**Stop the API:**
- If its terminal window is open: press `Ctrl+C`.
- Otherwise, find and stop the process listening on port 5031:
```powershell
$conn = Get-NetTCPConnection -LocalPort 5031 -State Listen -ErrorAction SilentlyContinue
if ($conn) { Stop-Process -Id $conn.OwningProcess -Force; Write-Host "Stopped." }
else { Write-Host "Nothing running on port 5031." }
```

**Stop the database (keeps data):**
```powershell
docker compose stop db adminer
```

---

## 4) Connecting with Adminer (bundled, no install needed)

Open `http://localhost:8081` and fill in:

| Field | Value |
|---|---|
| System | PostgreSQL |
| Server | `db` (Option A) or `localhost` (Option B) |
| Username | `postgres` |
| Password | `ChangeThisPassword` (matches `docker-compose.yml`'s default) |
| Database | `TRRCMS_Dev` |

---

## 5) Connecting with pgAdmin (separate desktop install)

pgAdmin is a plain database client — it connects the same way regardless of whether PostgreSQL is running natively or inside Docker; it only needs a reachable host and port.

1. Open pgAdmin.
2. Right-click **Servers** → **Register** → **Server...**
3. **General** tab — Name: any label, e.g. `TRRCMS Local`.
4. **Connection** tab:

| Field | Value |
|---|---|
| Host name/address | `localhost` |
| Port | `5432` |
| Maintenance database | `TRRCMS_Dev` |
| Username | `postgres` |
| Password | `ChangeThisPassword` |

5. Save. Browse under **Servers → TRRCMS Local → Databases → TRRCMS_Dev → Schemas → public → Tables**, or use the **Query Tool** to run SQL directly.

> Register this as a **new, separate** server entry — do not reuse or overwrite any pre-existing PostgreSQL connection you may already have configured for a different project.

---

## 6) Application login (Swagger / API)

Default accounts are seeded automatically on first startup. Every account requires a **mandatory password change on first login**.

| Username | Default password | Role |
|---|---|---|
| `admin` | `Admin@123` | Administrator |
| `datamanager` | `Data@123` | DataManager |
| `clerk` | `Clerk@123` | OfficeClerk |
| `collector` | `Field@123` | FieldCollector |
| `supervisor` | `Super@123` | FieldSupervisor |
| `analyst` | `Analyst@123` | Analyst |

**First login flow (via Swagger):**

1. `POST /api/v1/Auth/login` with `{"username": "admin", "password": "Admin@123"}`. The response includes `mustChangePassword: true`, a restricted `accessToken`, and `userId`.
2. Authorize Swagger with that token (top-right **Authorize** button, value `Bearer <accessToken>`).
3. `POST /api/v1/Auth/change-password` with:
   ```json
   {
     "userId": "<userId from step 1>",
     "currentPassword": "Admin@123",
     "newPassword": "<new password>",
     "confirmPassword": "<new password>"
   }
   ```
   The new password must satisfy the active policy: minimum 8 characters, at least one uppercase letter, one lowercase letter, one digit, and one special character.
4. `POST /api/v1/Auth/login` again with the new password to obtain a full, unrestricted `accessToken`.
5. Re-authorize Swagger with the new token and proceed to use the API.

**Note:** the failed-login lockout policy allows 5 attempts before a 15-minute lockout — enter credentials carefully.

---

## 7) Quick verification checklist

- `GET /api/v1/administrative-divisions/governorates` → returns Damascus (code `01`) only.
- `GET /api/v1/administrative-divisions/communities?governorateCode=01` → returns 2 communities.
- `GET /api/v1/Neighborhoods?governorateCode=01&districtCode=00&subDistrictCode=00&communityCode=001` → returns neighborhoods with valid `boundaryWkt`.
