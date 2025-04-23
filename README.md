[![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)

## Project Quality and Analysis

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)

### Ratings

[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Capyborrow_capyborrowProject&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Capyborrow_capyborrowProject)

## Repository Information

![GitHub Created At](https://img.shields.io/github/created-at/Capyborrow/capyborrowProject)
![GitHub Contributors](https://img.shields.io/github/contributors/Capyborrow/capyborrowProject)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Capyborrow/capyborrowProject/dotnet-desktop.yml)
![GitHub Repo Size](https://img.shields.io/github/repo-size/Capyborrow/capyborrowProject)

# Documentation

# Table of Contents

- [Introduction](#introduction)
- [Getting started](#getting-started)
- [Project structure](#project-stucture)
- [AuthController Overview](#authcontroller-overview)
- [Services Overview](#services-overview)
- [Controllers Overview](#controllers-overview)
- [Error Handling](#error-handling)
- [Roles & Authorization](#roles--authorization)
- [Swagger](#swagger)
- [Testing](#testing)
- [Conclusion](#conclusion)

# Introduction

The capyborrowProject repository was created to save the backend code for the project, the idea of which is _to create a universal digital platform that will combine all the key functions of the educational process in one convenient interface, simplifying the educational process for students, teachers and administration of the educational institution_.

# Getting started

To get started with capyborrowProject, follow these steps:

### 1. Clone the repository

Clone the project to your local machine using Git:

```
git clone https://github.com/Capyborrow/capyborrowProject.git
cd capyborrowProject
```
or do it using Visual Studio:
1. Copy the [link](https://github.com/Capyborrow/capyborrowProject.git).
2. Open Visual Studio and select "Clone a repository".
3. Insert the copied link and proceed.

### 2. Restore dependencies

If needed, restore the NuGet packages required for the project. In Visual Studio, this can be done by right-clicking on the solution in the Solution Explorer and selecting "Restore NuGet Packages."

# Project stucture 

For now, Solution 'capyborrowProject' consists of two projects: _capyborrowProject_ and _TestProject_.

- **capyborrowProject**: contains the main application code.
    - **Controllers folder**: contains API controllers that handle user authentication, user profiles, assignments, groups, lessons, and related operations. Notable controllers include `AuthController`, `StudentController`, `TeacherController`, `AssignmentController`, and `TimetableController`.
    - **Data folder**: contains the `ApplicationDbContext` class that acts as a bridge between the application and the database using Entity Framework Core.
    - **Extensions folder**: contains utility extensions for setting up the application pipeline and services (`HostExtensions.cs`).
    - **Hubs folder**: includes SignalR hubs used for real-time notifications, such as `NotificationHub.cs`.
    - **Migrations folder**: contains Entity Framework Core migration files that define the database schema evolution, including both initial and incremental changes.
    - **Models folder**: contains entity classes and supporting data structures, organized into subfolders:
        - **AuthModels**: models related to authentication and identity management.
        - **CsvFilesModels**: models used for CSV import/export.
        - **DTOs**: data transfer objects used to decouple API contracts from domain models.
        - Other key models include `Student`, `Teacher`, `Assignment`, `Lesson`, `Group`, `Subject`, `Attendance`, `RefreshToken`, and `SubmissionFile`.
    - **Service folder**: contains service classes responsible for business logic and infrastructure concerns:
        - `JwtService` handles access and refresh token generation and validation.
        - `EmailService` sends emails via Azure Communication Services.
        - `BlobStorageService` manages file storage using Azure Blob Storage.
        - `NotificationService` handles delivery of in-app notifications.
    - **appsettings.json**: the main configuration file containing connection strings, secrets, and service-specific settings.
    - **capyborrowProject.http**: REST client file used for testing API endpoints via Visual Studio's built-in HTTP runner.
    - **Program.cs**: application entry point, responsible for configuring middleware and starting the web host.
- **capyborrowProject.Tests**: contains unit and integration tests to ensure application correctness and validate business logic in isolation.


# AuthController Overview

The `AuthController` handles user authentication and authorization within the application. It exposes endpoints for registration, login, token management, password recovery, and email confirmation. The controller is built using ASP.NET Core Identity and JWT for secure authentication.

### Dependencies

- `ApplicationDbContext`: database context for user data.
- `UserManager<ApplicationUser>`: manages user creation and password validation.
- `RoleManager<IdentityRole>`: handles user roles.
- `JwtService`: generates and validates JWT tokens.
- `EmailService`: sends email notifications via Azure.

### Functionality

- Register new users with roles (`student`, `teacher`)
- Authenticate users and return access tokens
- Refresh access tokens using stored refresh tokens
- Logout by invalidating tokens
- Reset passwords via email link
- Confirm and re-send confirmation emails
- Check email confirmation status

### Key Endpoints

- `POST /api/Auth/Register` – register new user
- `POST /api/Auth/Login` – login with email and password
- `POST /api/Auth/RefreshAccessToken` – refresh access token via cookie
- `POST /api/Auth/Logout` – logout user
- `POST /api/Auth/ForgotPassword` – request password reset
- `POST /api/Auth/ResetPassword` – reset password using token
- `POST /api/Auth/ResendConfirmationEmail` – resend confirmation email
- `POST /api/Auth/ConfirmEmail` – confirm email using token
- `POST /api/Auth/CheckEmailConfirmation` – check if email is confirmed

> **Note**:  
> Refresh tokens are stored in the database and delivered in secure HTTP-only cookies.  
> Claims include the user's role and email.  
> Console logging is used for debugging during development and should be disabled in production.

# Services Overview

## JwtService
The `JwtService` class provides functionality for generating and validating **JSON Web Tokens (JWT)** used for user authentication and authorization in the application. It supports the creation and verification of both access tokens and refresh tokens, utilizing configuration settings provided through the JwtSettings section in the app’s configuration.
### Methods
`string GenerateAccessToken(IEnumerable<Claim> claims)`
Generates a JWT access token using the provided claims and access token settings (secret and expiration).
This token is typically short-lived and used for authenticating user requests.

`string GenerateRefreshToken(IEnumerable<Claim> claims)`
Generates a JWT refresh token using the provided claims and refresh token settings.
This token has a longer lifespan and is used to issue new access tokens after the previous one expires.

`string GenerateJwtToken(IEnumerable<Claim> claims, string secret, TimeSpan expiresIn)`
Handles the core logic of generating a JWT with the given claims, signing secret, and expiration duration.
Used internally by both GenerateAccessToken and GenerateRefreshToken.

`ClaimsPrincipal? ValidateAccessToken(string token)`
Validates a given access token string using the access token secret.
Returns a ClaimsPrincipal if the token is valid, otherwise returns null.

`ClaimsPrincipal? ValidateRefreshToken(string token)`
Validates a given refresh token string using the refresh token secret.
Returns a ClaimsPrincipal if the token is valid, otherwise returns null.

`ClaimsPrincipal? ValidateJwtToken(string token, string secret)`
Handles the core logic of validating a JWT using the appropriate signing secret.
Used internally by both ValidateAccessToken and ValidateRefreshToken.
## EmailService
The `EmailService` class provides a method to send emails using **Azure Communication Services (ACS)**. It simplifies the process of composing and dispatching email messages within the application by leveraging Azure’s secure and scalable email infrastructure.
### Methods
`Task SendEmailAsync(string recipientEmail, string subject, string body)`
Sends an email message asynchronously using Azure Communication Services.
## BlobStorageService 
The `BlobStorageService` class provides an abstraction layer for uploading, downloading, and deleting files using **Azure Blob Storage**. It supports different file types such as assignment files, submission files, and user profile pictures, organizing them into separate containers for better structure and manageability.
### Methods
`Task<string> UploadFileAsync(Stream fileStream, string fileName, string fileType, string contentType)`
Uploads a file stream (assignment or submission) to the corresponding blob container. Returns the public URI of the uploaded file.

`Task<string> UploadProfilePictureAsync(IFormFile file, string userId)`
Uploads a user's profile picture to the profile picture container. Returns the public URI of the uploaded profile picture.

`Task<bool> DeleteProfilePictureAsync(string profilePictureUrl)`
Deletes a profile picture from blob storage given its public URL. Returns true if deletion succeeded; false otherwise.

`Task<Stream?> DownloadFileAsync(string fileName, string fileType)`
Downloads an assignment or submission file as a stream. Returns a readable Stream if the file exists; null otherwise.

`Task<bool> DeleteFileAsync(string fileName, string fileType)`
Deletes an assignment or submission file from the corresponding container. Returns true if the file was deleted; false if not found.


# Controllers Overview

The Controllers folder contains the API controllers responsible for handling various aspects of the application’s functionality. Each controller corresponds to a specific domain or feature and exposes RESTful endpoints to interact with that domain.
- **AuthController**: handles authentication-related operations including user registration, login, logout, email confirmation, password reset, and access token refresh.
- **ApplicationUserController**: provides endpoints for retrieving user profile information and filtering users by roles.
- **StudentController**: manages student-related operations such as viewing profile details, submitting assignments, and accessing personal schedule information.
- **TeacherController**: responsible for managing teacher data, including assigned subjects, lessons, and created assignments.
- **AssignmentController**: allows teachers to create, update, and manage assignments, and enables students to view them.
- **AttachmentFileController**: supports file upload and download for assignment-related documents using Azure Blob Storage.
- **CsvImportController**: enables importing structured data (e.g., students, subjects, schedules) from CSV files to simplify bulk data entry.
- **GroupController**: manages academic groups, including group creation, student assignments, and lesson scheduling per group.
- **LessonController**: provides endpoints for managing lessons, including creation, updating, and association with groups and subjects.
- **SubjectController**: handles subject creation and assignment to groups and teachers.
- **TimetableController**: returns structured timetable data for students and teachers, based on their roles and associated lessons.

# Error Handling

The backend handles errors explicitly within controller actions. When an operation fails due to invalid input, authentication failure, or business logic constraints, the controller returns a meaningful HTTP status code and a structured error message in the response body.

### Error Response Format

Most error responses are returned in the following JSON format:

```json
{
  "message": "User not found.",
  "errors": {
    "email": ["Email field is required."],
    "password": ["Password must contain at least one uppercase letter."]
  }
}
```

### Validation

Request model validation is handled using [ApiController] attribute and ModelState.IsValid checks. When validation fails, the API returns:
- `400 Bad Request`  —  for invalid input or failed validation.
- `401 Unauthorized` —  for incorrect credentials or unverified users.
- `403 Forbidden`    —  for invalid tokens or denied access.
- `404 Not Found`    —  if the requested resource does not exist.

### Common Status Codes

| Status Code               | Meaning                                       |
|---------------------------|-----------------------------------------------|
| 200 OK                    | The request was successful                    |
| 201 Created               | A new resource was created                    |
| 400 Bad Request           | The request was invalid or failed validation  |
| 401 Unauthorized          | Authentication is required or failed          |
| 403 Forbidden             | Access is denied due to invalid token or role |
| 404 Not Found             | The requested resource was not found          |
| 500 Internal Server Error | Unhandled server error (not standardized)     |

### Notes

- Error messages are returned in a consistent format to allow frontend parsing.
- Token-related errors (expired, missing, or invalid tokens) are logged via OnAuthenticationFailed, OnChallenge, and OnForbidden events in the JwtBearer authentication configuration.
- There is currently no global exception handler; all error handling is implemented at the controller level.


# Roles & Authorization

The application uses role-based authorization to control access to protected resources. Each user is assigned a role during registration, and access to specific endpoints is restricted using ASP.NET Core’s `[Authorize]` attributes and custom policies.

### Available Roles

- `student`: users registered as students.
- `teacher`: users registered as teachers.
- `admin`: system administrators (can be created manually via seeding or DB).

### Role Assignment

A role is assigned to a user at the moment of registration (via the `role` field in the request body). Internally, the system uses `UserManager.AddToRoleAsync` to attach the role and add corresponding claims.

### Role-Based Access Control

Controllers and endpoints are protected using the `[Authorize]` attribute and named policies defined in `Program.cs`. Example:

```csharp
[Authorize(Roles = "teacher")]
public class AssignmentController : ControllerBase
```

Additionally, authorization policies are defined during app startup:
```csharp
  options.AddPolicy("StudentOrAdmin", policy =>
     policy.RequireRole("student", "admin"));
```

### Defined Policies

| Policy Name    | Description                                    |
|----------------|------------------------------------------------|
| StudentOrAdmin | Accessible to users with student or admin role |
| TeacherOrAdmin | Accessible to users with teacher or admin role |
| AdminOnly	     | Restricted to admin users only                 |

These policies are used in combination with [Authorize(Policy = "...")] to restrict access to specific endpoints or actions.

# Swagger

The backend includes auto-generated interactive documentation using **Swagger (OpenAPI)**, which allows testing and exploring all available endpoints directly from the browser.

### Swagger UI

After running the backend locally in development mode, Swagger UI is available at:
`https://localhost:{PORT}/swagger`


This interface provides:
- Descriptions of all endpoints grouped by controller
- Required request parameters and expected responses
- Support for authentication using JWT Bearer tokens

>  Make sure to authorize using a valid token via the "Authorize" button in the Swagger UI before accessing protected endpoints.

### JWT Token Setup in Swagger

To authorize Swagger requests:
1. Click the **Authorize** button.
2. Enter the token in the following format:  
   `Bearer YOUR_ACCESS_TOKEN_HERE`
3. Click **Authorize** and close the modal.

# Testing

The `capyborrowProject.Tests` project includes automated tests for backend controllers using **NUnit**.

Currently, it contains:

- **Controllers/AuthControllerTests.cs**  
  Tests the behavior of the `AuthController` in failure scenarios such as:
  - Registration with already existing email
  - Login with unknown user
  - Forgot password for nonexistent email
  - Email confirmation with invalid token

Test suite uses:
- **NUnit** — as the test framework (`[TestFixture]`, `[Test]`)
- **AutoFixture** with **AutoMoq** — for easy setup of inputs and mocked dependencies
- **Moq** — for mocking services like `UserManager`, `JwtService`, `EmailService`

Tests focus on controller-level logic and response validation (e.g., returning `BadRequestObjectResult`).

# Conclusion

This documentation outlines the backend of capyborrowProject — its structure, functionality, and testing approach.
