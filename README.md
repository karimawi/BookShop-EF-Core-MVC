# BookShop - .NET 8 MVC Application

A modern, feature-rich book category management system built with ASP.NET Core 8 MVC and Entity Framework Core, featuring a clean N-Tier architecture and beautiful dark/light theme interface.

## 🏗️ Architecture

The application follows a clean N-Tier architecture pattern:

```
BookShop/
├── BookShop.Models/          # Domain entities and configurations
├── BookShop.DataAccess/      # Data access layer with DbContext
└── BookShop.Web/            # Presentation layer (MVC)
```

### Technology Stack
- **Framework**: .NET 8
- **ORM**: Entity Framework Core 9.0.7
- **Database**: SQL Server Express
- **Frontend**: Bootstrap 5, Font Awesome 6.4.0, SweetAlert2
- **Styling**: CSS Variables for theming, Inter font family

## 📋 Prerequisites

- .NET 8 SDK or later
- SQL Server Express LocalDB
- Visual Studio 2022 or Visual Studio Code
- Git (for version control)

## 🛠️ Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/karimawi/BookShop-EF-Core-MVC.git
   cd BookShop-EF-Core-MVC
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Open `BookShop.Web/appsettings.json`
   - Modify the connection string if needed (default uses LocalDB)

4. **Apply database migrations**
   ```bash
   cd BookShop.Web
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Open your browser and navigate to `https://localhost:5097`

## 🗃️ Database Schema

The application uses a simple but effective database structure:

### Categories Table (MasterSchema.Categories)
- `Id` (int, Primary Key, Identity)
- `CatName` (nvarchar(100), Required)
- `CatOrder` (int, Required)
- `CreatedDate` (datetime2, Default: Current timestamp)
- `IsDeleted` (bit, Default: false)

### Project Structure
```
BookShop.Web/
├── Controllers/
│   ├── CategoryController.cs    # CRUD operations
│   ├── ErrorController.cs       # Error handling
│   └── HomeController.cs        # Landing page
├── Views/
│   ├── Category/               # Category management views
│   ├── Shared/                 # Layout and shared components
│   └── Home/                   # Home page views
├── wwwroot/
│   ├── css/                    # Custom stylesheets
│   ├── js/                     # JavaScript files
│   └── lib/                    # Third-party libraries
└── Migrations/                 # EF Core migrations
```

### Key Components

#### CategoryController
- **Index**: List all active categories with search and sorting
- **Create**: Add new categories with validation
- **Edit**: Update existing categories
- **Details**: View category information
- **Delete**: Soft delete with confirmation
- **Activate/Deactivate**: Toggle category activity status

## 🚀 Deployment

### Local Development
```bash
dotnet run --project BookShop.Web
```