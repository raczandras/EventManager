# Event Manager API

This is a simple **ASP.NET Core Web API** & **React** project for managing events.

---

## Getting Started

1. Clone the repository.
2. Install [MS SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) if you don't have it installed.
3. Install [Node.js](https://nodejs.org/en/download) if you don't have it installed.
4. Update the connection string in [`appsettings.Development.json`](./EventManager.Server/appsettings.Development.json) to match your local SQL Server instance if needed:

```json
"ConnectionStrings": {
  "DbConnection": "Server=localhost;Database=EventManager;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

5. If you are running it in Visual Studio, make sure the startup project is set to `EventManager.Server`
6. Run the project.
7. If you are getting errors in the client console saying that packages cannot be found, then run the `npm install` command from the `eventManager.client` directory
8. The swagger UI will be available at: `/swagger/index.html`.

## Available Endpoints

### Authentication

| Method | Endpoint                     | Description                                                            |
| ------ | ---------------------------- | ---------------------------------------------------------------------- |
| POST   | `/api/authorization/login`   | Authenticate a user and return an access token + refresh token.        |
| POST   | `/api/authorization/refresh` | Exchange a valid refresh token for a new access token + refresh token. |
| POST   | `/api/authorization/logout`  | Logout a user and invalidate the refresh token                         |

### Events

| Method | Endpoint          | Description                                                                                       |
| ------ | ----------------- | ------------------------------------------------------------------------------------------------- |
| GET    | `/api/event/{id}` | Get an event by ID. Returns 404 if not found.                                                     |
| GET    | `/api/event`      | Get all events. Supports optional pagination and sorting.                                         |
| POST   | `/api/event`      | Create a new event. Returns the created event. 400 if a parameter is bad.                         |
| PUT    | `/api/event`      | Update an existing event (by ID in body). Returns 404 if not found, or 400 if a parameter is bad. |
| DELETE | `/api/event/{id}` | Delete an event by ID. Returns 204 if deleted, 404 if not found.                                  |

## Pagination & Sorting

The `GET /api/event` endpoint supports optional pagination and sorting using query parameters:

- `page` → Page number (must be ≥ 1)
- `pageSize` → Number of items per page (must be ≥ 1)
- `sortBy` → Property name to sort by (e.g. `Name`, `Location`, `Capacity`)
- `descending` → `true` for descending sort order, `false` (default) for ascending

---

## Hardcoded Users

On startup, the following users are seeded into the database:

| Username | Email           | Password | Role  |
| -------- | --------------- | -------- | ----- |
| `admin`  | admin@admin.com | Admin123 | Admin |
| `user`   | user@user.com   | User123  | User  |


