# Document Management Web API
 
A backend document management system built with ASP.NET Core and Entity Framework Core.
 
This project provides a REST API for uploading, managing, versioning, and controlling access to PDF documents.
 
## Features
 
- Upload PDF documents
- Download documents
- Document versioning
- Retrieve latest or specific document versions
- Role-based access control
- Ownership transfer
- Access management (grant / update / revoke permissions)
- Swagger API documentation
- SQLite database storage
 
---
 
## Tech Stack
 
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- C#
 
---
 
## API Overview
 
### Documents
 
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/document` | Upload a new document |
| GET | `/api/document` | Get accessible documents |
| GET | `/api/document/download/latest` | Download latest document |
| GET | `/api/document/download` | Download specific document |
 
---
 
### Versions
 
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/document/{documentId}/versions` | Upload new document version |
| GET | `/api/document/{documentId}/versions` | Get all versions |
| GET | `/api/document/{documentId}/versions/latest` | Get latest version |
| GET | `/api/document/{documentId}/version/{versionNumber}` | Get specific version |
| GET | `/api/document/{documentId}/version/download/latest` | Download latest version |
| GET | `/api/document/{documentId}/version/download/{versionNumber}` | Download specific version |
 
---
 
### Access Management
 
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/document/{documentId}/access` | Get document access list |
| POST | `/api/document/{documentId}/access` | Grant access |
| PUT | `/api/document/{documentId}/access` | Update access role |
| DELETE | `/api/document/{documentId}/access` | Revoke access |
 
---
 
### Ownership
 
| Method | Endpoint | Description |
|--------|----------|-------------|
| PUT | `/api/document/{documentId}/owner` | Transfer ownership |
 
---
 
## Roles
 
Supported access roles:
 
- Owner
- Editor
- Viewer
 
---

Responsibilities
Controllers → HTTP endpoints / request handling
Services → business logic / access validation
DTOs → API request / response models
EF Core Models → database entities

Example Use Cases
Internal document archive
Controlled document sharing
Versioned PDF storage
Role-based document collaboration
