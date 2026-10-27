# SPA Comments

**Recruitment assignment — Oleksandr Domanskyi**  
Live application: https://spa.austriaeast.cloudapp.azure.com

SPA Comments is a web application for posting comments and replies. Comments and user information are stored in PostgreSQL. A comment can include an image or a TXT attachment.

The project was built as a recruitment assignment using .NET 10 and React. The application is deployed on an Azure virtual machine and can be tested using the link above.

## Features

- Add comments and reply to existing comments. Replies can be nested to form discussion threads.
- Browse comments with sorting and pagination. Newest comments appear first by default.
- Register an account, sign in, and sign out. Comments can also be posted anonymously.
- Vote on comments and replies.
- Preview a comment before submitting it, without reloading the page.
- Format comment text using the permitted HTML tags: `<a>`, `<code>`, `<i>`, and `<strong>`. The editor includes buttons for inserting these tags.
- Attach JPG, PNG, GIF, or TXT files. Images larger than 320 × 240 pixels are resized proportionally; animated GIFs retain their animation. TXT files are limited to 100 KB.
- Open attachments in a preview.
- Receive new comment updates through SignalR without manually refreshing the page.

Posting a comment requires CAPTCHA verification. Form input is validated on both the frontend and backend. Comment HTML is checked and sanitized to prevent unsafe markup from being rendered.

## Technologies

**Backend:** .NET 10, ASP.NET Core Minimal API, Entity Framework Core, PostgreSQL, MediatR, FluentValidation, JWT authentication, SignalR, SkiaSharp, Magick.NET.

**Frontend:** React, TypeScript, Vite, CSS Modules, DOMPurify, SignalR client.

**Infrastructure:** Docker, Docker Compose, Nginx, Azure Blob Storage. The deployed version also uses Caddy for HTTPS with a Let's Encrypt certificate.

## Project structure

The backend is split into a comment module, a user module, shared code, and an integration for file storage. The modules separate API endpoints, application logic, domain models, and database access.

### Backend/SPA.API

The application's entry point. It configures dependency injection, API endpoints, authentication, Swagger, error handling, and the other backend modules.

### Backend/Modules/CommentModule

- **Comment.API** — HTTP endpoints for comments and real-time communication.
- **Comment.Application** — application logic, request handlers, DTOs, and validation.
- **Comment.Core** — comment domain models.
- **Comment.Infrastructure** — PostgreSQL access through EF Core, migrations, and attachment processing.

### Backend/Modules/UserModule

- **User.API** — endpoints for registration, login, logout, and user-related operations.
- **User.Application** — application logic for user operations.
- **User.Core** — user domain models.
- **User.Infrastructure** — identity management, persistence, and EF Core migrations.

### Backend/SharedModule

- **SPA.Captcha** — CAPTCHA generation and verification.
- **SPA.Database.Shared** — shared database components.
- **SPA.Shared** — common types, constants, and utilities, including image processing.

### Backend/Integration/AzureStorage

Integration with Azure Blob Storage for saving attachments.

### Frontend/spa-client

The React application. It contains the comment feed, forms, account interface, attachment previews, and code for communicating with the API.

## Database

The application uses PostgreSQL and Entity Framework Core. User and comment data are managed through separate contexts: `UserDbContext` and `CommentDbContext`.

A comment may reference a parent comment, which allows replies to be nested. Database changes are managed with EF Core migrations.

## Running with Docker

You need Git and Docker with Docker Compose. A valid Azure Blob Storage connection string is also required for attachments. The Compose configuration uses the backend and frontend images published to GitHub Container Registry.

### 1. Clone the repository

```bash
git clone https://github.com/Oleksandr-Domanskyi/Recruitment-task.git
cd Recruitment-task
```

### 2. Configure environment variables

Copy `env.example` to `.env` and supply your own values:

```dotenv
POSTGRES_DB=spa_comments
POSTGRES_USER=spa
POSTGRES_PASSWORD=your_database_password
JWT_SECRET=your_long_random_jwt_secret
AZURE_STORAGE_CONNECTION_STRING=your_azure_storage_connection_string
HTTP_PORT=8080
```

`POSTGRES_PASSWORD` is the password for the PostgreSQL container, `JWT_SECRET` is used for signing authentication tokens, and `AZURE_STORAGE_CONNECTION_STRING` is used to store attachments in Azure Blob Storage. Do not commit the `.env` file or real credentials.

### 3. Start PostgreSQL

```bash
docker compose up -d postgres
```

Check `docker compose ps` and wait until PostgreSQL reports `healthy` before applying migrations.

### 4. Apply database migrations

There are two EF Core contexts, so their migrations must be applied separately before starting the API for the first time.

From the repository root, start a temporary .NET 10 SDK container:

```bash
docker run --rm -it --network spa-comments_default --env-file .env -v "${PWD}:/src" -w /src mcr.microsoft.com/dotnet/sdk:10.0 bash
```

Inside that container, run:

```bash
dotnet tool install --global dotnet-ef --version 10.0.13
export PATH="$PATH:/root/.dotnet/tools"
export ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"

dotnet ef database update --context UserDbContext \
  --project Backend/Modules/UserModule/User.Infrastructure/User.Infrastructure.csproj \
  --startup-project Backend/SPA.API/SPA.API.csproj

dotnet ef database update --context CommentDbContext \
  --project Backend/Modules/CommentModule/Comment.Infrastructure/Comment.Infrastructure.csproj \
  --startup-project Backend/SPA.API/SPA.API.csproj

exit
```

### 5. Start the application

```bash
docker compose up -d
```

With the default `HTTP_PORT`, the frontend is available at http://localhost:8080.

Check the containers and API logs with:

```bash
docker compose ps
docker compose logs api --tail=100
```

The public deployment uses HTTPS. When testing authentication locally, note that cookies marked `Secure` require a secure connection for normal browser behavior. Use HTTPS if you need to test the complete login and refresh-token flow.

## Deployment

The application is running in Docker containers on an Ubuntu virtual machine in Microsoft Azure. Docker Compose manages PostgreSQL, the .NET API, and the React frontend.

Nginx serves the frontend and forwards `/api/` requests and `/hubs/` connections to the backend. Caddy sits in front of Nginx, handles HTTPS, and manages the TLS certificate. The server's Caddy configuration is separate from the base `compose.yaml` in the repository.


