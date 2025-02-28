# CRUD Application

A full-stack CRUD (Create, Read, Update, Delete) application built with ASP.NET Core Web API backend and React frontend.

## Features

- Create new items with title and description
- Read/display list of items
- Update existing items
- Delete items
- Responsive design
- Real-time feedback during operations
- Form validation
- Error handling

## Tech Stack

### Backend

- ASP.NET Core 8.0
- Entity Framework Core
- MySQL Database
- DotNetEnv (for environment variables)

### Frontend

- React 18
- CSS Modules
- Fetch API

## Prerequisites

- .NET 8.0 SDK
- Node.js and npm
- MySQL Server
- Git

## Getting Started

### Backend Setup

1. Clone the repository
2. Navigate to the Backend directory:

   ```bash
   cd Backend
   ```

3. Create a `.env` file in the Backend directory using `.env.example` as a template:

   ```bash
   cp .env.example .env
   ```

4. Update the `.env` file with your MySQL database credentials

5. Install required .NET packages:

   ```bash
   dotnet restore
   ```

6. Run database migrations:

   ```bash
   dotnet ef database update
   ```

7. Start the backend server:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7241`

### Frontend Setup

1. Navigate to the frontend directory:

   ```bash
   cd frontend
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```
   The application will open in your default browser at `http://localhost:3000`

## Features

- Create, Read, Update and Delete operations for managing items
- Responsive design that works on desktop and mobile
- Form validation
- Error handling
- Environment variable configuration
- Database migrations
