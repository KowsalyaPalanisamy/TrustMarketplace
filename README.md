# Secure Marketplace - E-Commerce Platform

## Project Overview

A **secure multi-vendor marketplace** web application demonstrating the complete secure software development lifecycle. Built for **National College of Ireland - Secure Web Development Module (CA2)**.

The application implements role-based access control (RBAC) with three user types:
- **Sellers** - Create, edit, and delete their own products
- **Buyers** - Browse and view products (read-only)
- **Admins** - Full control over all products and user management

---

## Security Features Implemented

| Control | Implementation | Status |
|---------|----------------|--------|
| Password Hashing | bcrypt via ASP.NET Core Identity | **Implemented** |
| SQL Injection Prevention | Entity Framework parameterized queries | **Implemented** |
| XSS Protection | Razor Pages auto-encoding | **Implemented** |
| CSRF Protection | Built-in anti-forgery tokens | **Implemented** |
| Role-Based Authorization | `[Authorize(Roles="...")]` attributes | **Implemented** |
| Secure Session Management | 20-min timeout, HttpOnly cookies | **Implemented** |
| Input Validation | Data annotations (`[Required]`, `[Range]`) | **Implemented** |
| Generic Error Handling | Custom error page, no stack traces | **Implemented** |

---

## Branch Strategy

This repository contains **two branches** demonstrating the security improvement process:

| Branch | Description | Purpose |
|--------|-------------|---------|
| **main** | **Secure version** - All vulnerabilities fixed | Final production-ready code |
| **vulnerability-code** | **Vulnerable version** - Contains 6 intentional vulnerabilities | Security testing & comparison |

### Vulnerabilities in "vulnerability-code" branch:

| # | Vulnerability | Location | Risk |
|---|---------------|----------|------|
| 1 | SQL Injection | Pages/Products/Search.cshtml.cs | Critical |
| 2 | XSS (Cross-Site Scripting) | Pages/Products/Index.cshtml | High |
| 3 | Missing Input Validation | Models/Product.cs | Medium |
| 4 | Hardcoded Credentials (Backdoor) | Pages/Account/Login.cshtml.cs | Critical |
| 5 | Missing Security Headers | Program.cs | Medium |
| 6 | IDOR (Insecure Direct Object Reference) | Pages/Products/Edit.cshtml.cs | High |

---

## Test Accounts

Use these accounts to test different roles:

### Main Branch (Secure Version)

| Role | Email | Password |
|------|-------|----------|
| **Seller** | seller@gmail.com | seller@123 |
| **Buyer** | buyer@gmail.com | buyer@123 |
| **Admin** | admin@marketplace.com | Admin@123 |

### Vulnerability-Code Branch (Vulnerable Version)

| Role | Email | Password | Backdoor Access |
|------|-------|----------|-----------------|
| **Seller** | seller@gmail.com | seller@123 | - |
| **Buyer** | buyer@gmail.com | buyer@123 | - |
| **Backdoor** | backdoor@hacker.com | hack123 | ✅ Bypasses auth |

---

## Installation & Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- [SQLite](https://sqlitebrowser.org/) (optional - for viewing database)

### Clone Repository

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/SecureMarketplace.git

# Navigate to project folder
cd SecureMarketplace

# Switch to main branch (secure version)
git checkout main

# OR switch to vulnerable branch (vulnerable code)
git checkout vulnerability-code
