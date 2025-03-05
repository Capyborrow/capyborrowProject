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

## Table of Contents
- [Introduction](#introduction)
- [Getting started](#getting-started)
- [Project structure](#project-stucture)
- [AuthController Overview](#authcontroller-overview)
- [Useful links](#useful-links)

## Introduction

The capyborrowProject repository was created to save the backend code for the project, the idea of which is _to create a universal digital platform that will combine all the key functions of the educational process in one convenient interface, simplifying the educational process for students, teachers and administration of the educational institution_.

## Getting started

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

## Project stucture 
For now, Solution 'capyborrowProject' consists of two projects: _cappyborrowProject_ and _TestProject_.

- **capyborrowProject**: contains the main application code.
    - **Controllers folder**: contains three API controllers for Teacher and Student models and for handling authentication.
    - **Data folder**: contains a database context class, the purpose of which is to act as a bridge between the application and the database, enabling the app to interact with the database in an object-oriented way.
    - **Migrations folder**: contains automatically generated files that create tables in the database based on the code written for models.
    - **Models folder**: contains classes for entities, folders for predefined tables, join tables and authentication models.
    - **Service folder**: contains utility classes (JwtService and EmailService) that handle authentication (JWT generation and validation) and email communication using Azure's Email Communication Service.
- **TestProject**: contains unit tests for the application.

## AuthController Overview
The `AuthController` is responsible for handling user authentication and authorization in the application. It provides endpoints for user registration, login, access token refresh, logout, password recovery, and email confirmation. This controller leverages ASP.NET Core Identity and JWT authentication for secure user management.
### Dependencies
- `ApplicationDbContext`: Database context for managing user data.
- `UserManager<ApplicationUser>`: Manages user-related operations (creation, password validation, etc.).
- `RoleManager<IdentityRole>`: Handles user roles.
- `JwtService`: Generates and validates JWT tokens.
- `EmailService`: Sends email notifications.
### Endpoints
**Register User** (`POST /api/Auth/Register`)
  - **Description**: Registers a new user (either Student or Teacher).
  - **Request Body**:
    ```
    {
      "firstName": "John",
      "middleName": "A.",
      "lastName": "Doe",
      "email": "johndoe@example.com",
      "password": "SecurePassword123!",
      "role": "student"
    }
  - **Responses**:
    - `201 Created`: User registered successfully.
    - `400 Bad Request`: Invalid data or user already exists.

**Login User** (`POST /api/Auth/Login`)
  - **Description**: Authenticates a user and returns an access token.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com",
      "password": "SecurePassword123!"
    }
  - **Responses**:
    - `200 OK`: Returns an access token.
    - `400 Bad Request`: Invalid credentials.
    - `401 Unauthorized`: Password incorrect.

**Refresh Access Token** (`POST /api/Auth/RefreshAccessToken`)
  - **Description**: Refreshes the user's access token using the stored refresh token.
  - **Responses**:
    - `200 OK`: Returns a new access token.
    - `401 Unauthorized`: Missing or invalid refresh token.

**Logout User** (`POST /api/Auth/Logout`)
  - **Description**: Logs out a user by invalidating the refresh token.
  - **Responses**:
    - `200 OK`: User logged out.
    - `403 Forbidden`: No valid refresh token found.

**Forgot Password** (`POST /api/Auth/ForgotPassword`)
  - **Description**: Sends a password reset link to the user's email.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com"
    }
   - **Responses**:
     - `200 OK`: Reset link sent.
     - `400 Bad Request`: User not found.

**Reset Password** (`POST /api/Auth/ResetPassword`)
  - **Description**: Resets the user's password using a token.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com",
      "token": "reset-token",
      "newPassword": "NewSecurePassword123!"
    }
   - **Responses**:
     - `200 OK`: Password reset successful.
     - `400 Bad Request`: Invalid token or request.

**Resend Confirmation Email** (`POST /api/Auth/ResendConfirmationEmail`)
  - **Description**: Resends the email confirmation link.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com"
    }
   - **Responses**:
     - `200 OK`: Confirmation email resent.
     - `400 Bad Request`: Email already confirmed or user not found.

**Confirm Email** (`POST /api/Auth/ConfirmEmail`)
  - **Description**: Confirms the user's email using a token.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com",
      "token": "confirmation-token"
    }
   - **Responses**:
     - `200 OK`: Email confirmed.
     - `400 Bad Request`: Invalid token or request.

**Check Email Confirmation** (`POST /api/Auth/CheckEmailConfirmation`)
  - **Description**: Checks whether a user's email has been confirmed.
  - **Request Body**:
    ```
    {
      "email": "johndoe@example.com"
    }
   - **Responses**:
     - `200 OK`: Returns `{ "Confirmed": true }` if email is confirmed, otherwise `{ "Confirmed": false }`.
     - `400 Bad Request`: User not found or invalid request.

> [!NOTE]
> Refresh tokens are stored in the database and secured in HTTP-only cookies.  
> Claims are used to store user role and email information.  
> Console logging is used for debugging purposes (should be removed in production).

## Useful links
- [For writing documentation](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#lists)
- [A short CRUD course](https://youtu.be/b8fFRX0T38M?si=lBDJx2gsc41vuBC_)
- [An API Tutorial](https://youtu.be/sdlt3-ptt9g?si=Iqdk6i4Njr5m23cn)
