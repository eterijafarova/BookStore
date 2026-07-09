# 📚 Cheshire Shelf API

Backend API for **Cheshire Shelf** — a modern online bookstore inspired by the magical atmosphere of the Cheshire Cat. The API provides secure authentication, book management, shopping functionality, order processing, and user account management.

**Frontend:**
http://cheshire-shelf-front.s3-website.eu-north-1.amazonaws.com/

---

## Overview

Cheshire Shelf API is built with **ASP.NET Core 8** following a layered architecture and RESTful design principles. It serves as the backend for the Cheshire Shelf web application, handling business logic, authentication, database operations, email communication, and image management.

---

## Features

### Authentication & Authorization

* JWT-based authentication
* User registration and login
* Email confirmation
* Password recovery and reset
* Role-based authorization

### Books

* Browse books
* Search and filtering
* Book details
* Categories

### User Account

* Profile management
* Avatar upload
* Saved payment cards

### Shopping

* Shopping cart
* Wishlist
* Order creation
* Order history

### Reviews

* Add reviews
* Edit reviews
* Delete reviews
* Book ratings

### Administration

* Book management
* Category management
* Order management
* User management

### Additional Features

* Cloudinary image storage
* SMTP email service
* Swagger API documentation

---

# Technology Stack

| Category          | Technologies              |
| ----------------- | ------------------------- |
| Framework         | ASP.NET Core 8            |
| ORM               | Entity Framework Core     |
| Database          | MySQL (Amazon RDS)        |
| Authentication    | JWT Bearer Authentication |
| Object Mapping    | AutoMapper                |
| Email Service     | MailKit                   |
| Image Storage     | Cloudinary                |
| API Documentation | Swagger / OpenAPI         |
| Deployment        | AWS Elastic Beanstalk     |
| CI/CD             | GitHub Actions            |

---

# Project Structure

```text
BookShop.API
│
├── Controllers
├── DTOs
├── Entities
├── Extensions
├── Helpers
├── Interfaces
├── Mapping
├── Middleware
├── Migrations
├── Repositories
├── Services
├── wwwroot
└── Program.cs
```

---

# Getting Started

## Prerequisites

Before running the project, ensure the following software is installed:

* .NET 8 SDK
* MySQL Server
* Visual Studio 2022, Rider, or Visual Studio Code

---

## Clone the Repository

```bash
git clone https://github.com/your-username/CheshireShelf.git
cd CheshireShelf
```

---

## Configuration

Create an **appsettings.json** file and configure the required settings.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },

  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  },

  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },

  "EmailSettings": {
    "Email": "YOUR_EMAIL",
    "Password": "YOUR_PASSWORD",
    "Host": "YOUR_SMTP_HOST",
    "Port": 587
  }
}
```

---

## Apply Database Migrations

```bash
dotnet ef database update
```

---

## Run the Application

```bash
dotnet run
```

The API will be available locally at:

```
https://localhost:5001
```

Swagger documentation:

```
https://localhost:5001/swagger
```

---

# Authentication

The API uses **JWT Bearer Authentication**.

Authentication flow:

1. Register a new account
2. Confirm email address
3. Sign in
4. Receive a JWT access token
5. Include the token in the `Authorization` header

```http
Authorization: Bearer YOUR_ACCESS_TOKEN
```

---

# Email Services

The application supports:

* Email confirmation
* Password reset
* Transactional email notifications

SMTP functionality is implemented using **MailKit**.

---

# Image Management

Images are uploaded and stored securely using **Cloudinary**.

---

# Deployment

The application is deployed using Amazon Web Services.

| Service                  | Purpose                             |
| ------------------------ | ----------------------------------- |
| Amazon Elastic Beanstalk | Backend hosting                     |
| Amazon RDS               | MySQL database                      |
| GitHub Actions           | Continuous Integration & Deployment |

---

# Frontend

Frontend application:

http://cheshire-shelf-front.s3-website.eu-north-1.amazonaws.com/

---

# Future Improvements

* Payment gateway integration
* Recommendation system
* Inventory management
* Real-time notifications
* Analytics dashboard

---



# Author

**Eteri Jafarova**

