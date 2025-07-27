# BookShop - .NET 8 MVC Application

A modern, feature-rich book management system built with ASP.NET Core 8 MVC and Entity Framework Core, featuring complete product and category management with real-time search, interactive tooltips, and a beautiful dark/light theme interface.

## 📸 Screenshots

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://files.catbox.moe/ycpkvi.png">
  <source media="(prefers-color-scheme: light)" srcset="https://files.catbox.moe/4pg8a2.png">
  <img alt="BookShop Application Interface" src="https://files.catbox.moe/4pg8a2.png" width="100%">
</picture>

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
- **UI Features**: Real-time search, interactive tooltips, responsive design
- **Styling**: CSS Variables for theming, Inter font family, dark/light mode toggle

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

The application uses a comprehensive database structure for book management:

### Categories Table (MasterSchema.Categories)
- `Id` (int, Primary Key, Identity)
- `CatName` (nvarchar(50), Required)
- `CatOrder` (int, Required)
- `CreatedDate` (datetime2, Default: Current timestamp)
- `IsDeleted` (bit, Default: false)
- `IsActive` (bit, Default: true)

### Products Table (MasterSchema.Products)
- `Id` (int, Primary Key, Identity)
- `Title` (nvarchar(50), Required)
- `Description` (nvarchar(250), Optional)
- `Author` (nvarchar(50), Required)
- `BookPrice` (float, Required)
- `CategoryId` (int, Foreign Key to Categories)

### Project Structure
```
BookShop.Web/
├── Controllers/
│   ├── CategoryController.cs    # Category CRUD operations
│   ├── ProductController.cs     # Product CRUD operations
│   ├── ErrorController.cs       # Error handling
│   └── HomeController.cs        # Landing page
├── Views/
│   ├── Category/               # Category management views
│   ├── Product/                # Product management views
│   ├── Shared/                 # Layout and shared components
│   └── Home/                   # Home page views
├── wwwroot/
│   ├── css/                    # Custom stylesheets with dark/light theme
│   ├── js/                     # JavaScript files for interactivity
│   └── lib/                    # Third-party libraries
└── Migrations/                 # EF Core migrations
```

### Key Components

#### CategoryController
- **Index**: List all active categories with real-time search, sorting, and category filtering
- **Create**: Add new categories with validation
- **Edit**: Update existing categories
- **Details**: View category information
- **Delete**: Soft delete with confirmation
- **Activate/Deactivate**: Toggle category activity status

#### ProductController
- **Index**: Display products with advanced search, category filtering, and interactive tooltips
- **Create**: Add new products with category selection
- **Edit**: Update existing products
- **Details**: View detailed product information
- **Delete**: Remove products with confirmation
- **Reset**: Development utility to reset database

#### UI Features
- **Real-time Search**: Instant filtering without page refreshes
- **Interactive Tooltips**: Hover to view product descriptions in positioned modals
- **Category Filtering**: Dropdown filter for products by category
- **Dark/Light Theme**: Toggle between themes with persistent preference
- **Responsive Design**: Mobile-friendly interface with Bootstrap 5

## 🚀 Deployment

### Local Development
```bash
dotnet run --project BookShop.Web
```