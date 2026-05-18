# Two-Factor Authentication

A full-stack web application implementing multi-factor authentication using TOTP (Time-based One-Time Password) compatible with Google Authenticator and similar apps, combined with email-based OTP verification.

## Overview

The project demonstrates a complete, production-ready authentication system with the following capabilities:

- **User registration** with email verification via OTP
- **JWT-based authentication** with access and refresh tokens
- **TOTP two-factor authentication** via QR code and authenticator apps (Google Authenticator, Authy, etc.)
- **Email OTP** as a secondary verification layer on each login
- **Password reset** flow with time-limited codes
- **Rate limiting** on all sensitive endpoints
- **Docker support** for streamlined local development and deployment

## Tech Stack

### Backend

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM (PostgreSQL / SQLite) |
| MediatR | 12.4.1 | CQRS / request-handler pipeline |
| Otp.NET | 1.4.1 | RFC 6238 TOTP implementation |
| FluentValidation | 12.1.1 | Input validation |
| BCrypt.Net-Next | 4.2.0 | Password and OTP hashing |
| DotNetEnv | 3.1.1 | `.env` file support |
| Brevo SMTP | — | Transactional email delivery |

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| React | 18.3.1 | UI framework |
| React Router | 6.28.1 | Client-side routing |
| React Hook Form | 7.54.2 | Form management |
| Axios | 1.7.9 | HTTP client with interceptors |
| Tailwind CSS | 3.4.19 | Utility-first styling |
| Radix UI | — | Accessible headless components |
| qrcode.react | 3.2.0 | QR code generation for TOTP setup |

### Infrastructure

- **Database:** PostgreSQL 16 (development via Docker Compose)
- **Containerization:** Multi-stage Docker build (Alpine Linux)
- **Testing:** xUnit + Moq + FluentAssertions

## Architecture

The backend follows a clean architecture with the **MediatR** CQRS pattern: controllers delegate all logic to request handlers, which keeps the business logic decoupled and testable.

Error handling avoids throwing domain exceptions; instead, errors are collected via a `NotificationContext` and formatted uniformly by a global action filter.

```
src/
├── WebApplication/             # ASP.NET Core Web API
│   ├── Controllers/            # Thin controllers (no business logic)
│   ├── Handlers/               # MediatR request handlers
│   ├── Managers/               # Domain logic (TOTP, OTP, passwords)
│   ├── Services/               # External service integrations (email)
│   ├── Entities/               # EF Core entities
│   ├── Jwt/                    # JWT generation and signing (RSA 2048-bit)
│   ├── Validators/             # FluentValidation rules
│   ├── Behaviors/              # MediatR pipeline behaviors
│   └── Notifications/          # Notification-based error handling
├── WebApplication.Tests/       # xUnit test suite
└── react-app/                  # React 18 frontend
```

## Authentication Flow

```
1. Register    → email + password → OTP sent to email → account verified
2. Login       → email + password → email OTP sent
3. 2FA setup   → scan QR code with authenticator app
4. Verify      → enter TOTP code → JWT access token + refresh token issued
5. Refresh     → HttpOnly cookie → new access token (no re-login needed)
```

## Security Highlights

- Passwords hashed with **BCrypt** (salted)
- JWT signed with **RSA 2048-bit** private key (auto-generated at build)
- Access tokens expire in **8 hours**; refresh tokens in **7 days** (HttpOnly cookie)
- Email OTP codes hashed with BCrypt before storage and expire in **10 minutes**
- Password reset codes expire in **15 minutes**
- Rate limiting on login, 2FA verification, token refresh, and registration
- CORS restricted to configured allowed origins

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth` | Authenticate with email and password |
| POST | `/api/auth/VerifyCode` | Verify TOTP code and receive JWT |
| POST | `/api/auth/AddTwoFactAuth` | Enable 2FA with authenticator app |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Revoke refresh token |
| POST | `/api/account` | Register new user |
| POST | `/api/account/verify-email` | Confirm registration OTP |
| POST | `/api/account/resend-verification` | Resend registration OTP |
| POST | `/api/emailotp/send` | Send email OTP |
| POST | `/api/emailotp/verify` | Verify email OTP |
| POST | `/api/password/forgot` | Request password reset |
| POST | `/api/password/verify-code` | Verify reset code |
| POST | `/api/password/reset` | Set new password |

## Running Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [Docker](https://www.docker.com) (optional, for the full stack with PostgreSQL)

### With Docker Compose

```bash
# Copy and fill in the environment file
cp src/WebApplication/.env.example src/WebApplication/.env

docker-compose up --build
```

The application will be available at `http://localhost:4000`.

### Manual Setup

**Backend:**

```bash
cd src/WebApplication
cp .env.example .env
# Edit .env with your database and email credentials
dotnet run
```

**Frontend:**

```bash
cd src/react-app
npm install
npm start
```

### Environment Variables

Create `src/WebApplication/.env` based on `.env.example`:

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=tfa;Username=postgres;Password=yourpassword
Email__FromName=Your App Name
Email__SenderEmail=noreply@yourdomain.com
Email__ApiKey=your-brevo-api-key
```

> Sensitive values (API keys, connection strings, secrets) must **never** be committed to the repository. Always use environment variables or the `.env` file.

## Running Tests

```bash
cd src/WebApplication.Tests
dotnet test
```

## References

**Autenticação e segurança**
- [Multi-factor Authentication in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/mfa?view=aspnetcore-8.0)
- [JSON Web Tokens in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-8.0)
- [Otp.NET — RFC 6238 TOTP implementation](https://github.com/kspearrin/Otp.NET)
- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)

**Arquitetura e padrões**
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net)
- [Não lance Exceptions em seu Domínio — use Notifications](https://medium.com/tableless/n%C3%A3o-lance-exceptions-em-seu-dom%C3%ADnio-use-notifications-70b31f7148d3)

**Dados e infraestrutura**
- [Entity Framework Core com PostgreSQL](https://www.npgsql.org/efcore/)
- [DotNetEnv — .env file support for .NET](https://github.com/tonerdo/dotnet-env)
- [Brevo API — transactional email](https://developers.brevo.com)
