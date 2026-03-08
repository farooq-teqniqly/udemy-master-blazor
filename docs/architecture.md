# IMS — Inventory Management System: Architecture

| | |
| --- | --- |
| **Last updated** | 2026-03-07 |
| **Based on commit** | `d193823737c60dc0d42cf67bee24aa31db72ace3` |
| **Commit date** | 2026-03-07 |

## Overview

The Inventory Management System (IMS) is a nascent Blazor application built on **.NET 10** following **Clean Architecture** (also known as the Ports & Adapters or Onion Architecture pattern). The solution is structured so that the innermost layers have zero knowledge of outer layers, and all dependencies point inward toward the domain.

The application runs in **static Server-Side Rendering (SSR)** mode — pages are rendered to plain HTML on the server and streamed to the browser with no persistent SignalR circuit or WebAssembly runtime. This has concrete implications for UI design: `@onclick` and other interactive Blazor event handlers are inert by default, so navigation (e.g., a Cancel button) must use plain `<a href>` links or server-triggered redirects via `NavigationManager.NavigateTo` inside form POST handlers.

---

## Clean Architecture Layers

```text
┌──────────────────────────────────────────────────────────────────┐
│  Frameworks & Drivers  (IMS.Plugins.InMemory, ASP.NET Core)      │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  Interface Adapters  (IMS.WebApp — Blazor Components)      │  │
│  │  ┌──────────────────────────────────────────────────────┐  │  │
│  │  │  Application / Use Cases  (IMS.UseCases)             │  │  │
│  │  │  ┌────────────────────────────────────────────────┐  │  │  │
│  │  │  │  Domain / Entities  (IMS.CoreBusiness)         │  │  │  │
│  │  │  └────────────────────────────────────────────────┘  │  │  │
│  │  └──────────────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

---

## Projects

### 1. `IMS.CoreBusiness` — Domain Layer

The innermost ring. Contains enterprise-wide business rules and entity definitions. Has **no references** to any other IMS project or external framework.

| Item | Description |
| --- | --- |
| `Inventory` | Core domain entity representing a stocked item (`Id`, `Name`, `Price`, `Quantity`). Implements `IEquatable<Inventory>` with equality by `Id` (case-insensitive); also overrides `==` / `!=` operators and `GetHashCode`. No data-annotation attributes — validation is handled by FluentValidation in the use-cases layer. |

**Project dependencies:** none.

---

### 2. `IMS.UseCases` — Application Layer

Orchestrates domain entities to fulfil discrete application use cases. This layer owns two kinds of abstractions:

- **Use-case interfaces** (consumed by the UI) — defined in `Interfaces/`
- **Plugin interfaces** (consumed by infrastructure) — defined in `PluginInterfaces/`

These interfaces form the *ports* in the Ports & Adapters sense, keeping the application layer free of concrete I/O concerns.

#### Use-case interfaces (`Interfaces/`)

| Interface | Purpose |
| --- | --- |
| `IViewInventoriesByNameUseCase` | Retrieve inventories, optionally filtered by name |
| `IAddInventoryUseCase` | Add a new inventory item |

#### Plugin interfaces / Repository ports (`PluginInterfaces/`)

| Interface | Method | Purpose |
| --- | --- | --- |
| `IInventoryRepository` | `GetInventoriesByNameAsync` | Persistence port — fetches inventories filtered by name |
| `IInventoryRepository` | `AddInventoryAsync` | Persistence port — persists a new inventory item |

#### Use-case implementations (`Inventories/`)

| Class | Implements | Description |
| --- | --- | --- |
| `ViewInventoriesByNameUseCase` | `IViewInventoriesByNameUseCase` | Delegates to `IInventoryRepository`; returns a filtered collection of `Inventory` entities |
| `AddInventoryUseCase` | `IAddInventoryUseCase` | Assigns a `Guid.CreateVersion7`-based `Id` then delegates to `IInventoryRepository.AddInventoryAsync` |

#### Validators (`Validators/`)

| Class | Validates | Rules |
| --- | --- | --- |
| `InventoryValidator` | `Inventory` | `Name`: 5–100 chars, not empty; `Price`: 0–1,000,000; `Quantity`: 0–100,000 |

Uses **FluentValidation** (`AbstractValidator<T>`).

#### Composition helpers (`Extensions/`)

`ServiceCollectionExtensions` exposes two extension methods on `IServiceCollection` that are called from `Program.cs`:

| Method | Registers |
| --- | --- |
| `AddUseCases()` | `IViewInventoriesByNameUseCase` → `ViewInventoriesByNameUseCase` (transient); `IAddInventoryUseCase` → `AddInventoryUseCase` (transient) |
| `AddValidators()` | `IValidator<Inventory>` → `InventoryValidator` (singleton) |

**Project dependencies:** `IMS.CoreBusiness`. NuGet: `FluentValidation`, `Microsoft.Extensions.DependencyInjection.Abstractions`.

---

### 3. `IMS.Plugins.InMemory` — Infrastructure / Plugin Layer

Provides concrete *adapters* for the repository ports defined in `IMS.UseCases`. Currently ships a single in-memory implementation used during development and early testing.

| Class | Implements | Description |
| --- | --- | --- |
| `InventoryRepository` | `IInventoryRepository` | Seed data (four bike parts) stored in a `HashSet<Inventory>`; supports name-filter retrieval and add (duplicate guard via `HashSet`) |

Swapping this project for a different plugin (e.g., `IMS.Plugins.EFCore`) requires only a DI re-wiring in `Program.cs` — no changes to the domain or use-case layers.

**Project dependencies:** `IMS.UseCases` (and transitively `IMS.CoreBusiness`).

---

### 4. `IMS.WebApp` — Presentation Layer (Blazor, static SSR)

ASP.NET Core Blazor application that renders the UI and wires the dependency injection container. Components interact exclusively with use-case interfaces — never with repositories directly.

#### Pages (`Components/Pages/`)

| Page | Route | Description |
| --- | --- | --- |
| `Home.razor` | `/` | Quick view: renders a plain list of all inventory items |
| `Inventories/InventoryList.razor` | `/inventories` | Full inventory table via `InventoryListComponent`; links to Add Inventory |
| `Inventories/AddInventory.razor` | `/add-inventory` | `EditForm` validated by `<FluentValidator>` (Blazilla); redirects to `/inventories` on success. Cancel is an `<a href>` link (not `@onclick`) because the page runs under static SSR. |
| `Weather.razor` | `/weather` | Demo page using `@attribute [StreamRendering]`; simulates async data load and renders a 5-day weather forecast table |
| `NotFound.razor` | `/not-found` | 404 handler (wired via `UseStatusCodePagesWithReExecute`) |
| `Error.razor` | `/Error` | Unhandled-exception fallback |

#### Reusable controls (`Components/Controls/`)

| Component | Description |
| --- | --- |
| `InventoryListComponent` | Injects `IViewInventoriesByNameUseCase`; loads data on init; renders a `<table>` using `InventoryListItemComponent` per row |
| `InventoryListItemComponent` | Stateless; accepts a single `Inventory` parameter and renders one `<tr>` (name, quantity, formatted price, Edit button stub) |

#### Layout (`Components/Layout/`)

| Component | Description |
| --- | --- |
| `MainLayout.razor` | Root shell with sidebar + body slot |
| `NavMenu.razor` | Side-navigation links: Home, Inventories, Weather |

#### Composition root (`Program.cs`)

DI registrations that complete the dependency inversion:

```csharp
// Plugin adapter bound to the repository port
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();

// Use-cases and validators via extension methods defined in IMS.UseCases
builder.Services.AddUseCases();
builder.Services.AddValidators();
```

**Project dependencies:** `IMS.UseCases`, `IMS.Plugins.InMemory`. NuGet: `Blazilla` (provides `<FluentValidator>` Blazor component), `FluentValidation.DependencyInjectionExtensions`.

---

## Dependency Graph

```text
IMS.WebApp
  ├── IMS.UseCases
  │     └── IMS.CoreBusiness
  └── IMS.Plugins.InMemory
        └── IMS.UseCases
              └── IMS.CoreBusiness
```

The **Dependency Rule** is upheld throughout: source-code dependencies point exclusively inward. `IMS.CoreBusiness` and `IMS.UseCases` are unaware of Blazor, ASP.NET Core, or any persistence technology.

---

## Data Flows

### View Inventories

```text
Browser request
  → Inventories/InventoryList.razor (page)
    → InventoryListComponent.razor (control)
      → IViewInventoriesByNameUseCase.ExecuteAsync()        [use-case interface]
        → ViewInventoriesByNameUseCase                      [use-case impl]
          → IInventoryRepository.GetInventoriesByNameAsync() [plugin interface]
            → InventoryRepository (InMemory)                [plugin adapter]
              → HashSet<Inventory>                          [domain entity]
          ← IReadOnlyCollection<Inventory>
        ← IEnumerable<Inventory>
      ← List<Inventory>
    → InventoryListItemComponent × N (one per row)
  → Rendered HTML table
```

### Add Inventory

```text
User submits AddInventory form (HTTP POST)
  → Inventories/AddInventory.razor (page — EditForm / OnValidSubmit)
    FluentValidator runs server-side before OnValidSubmit fires
    → IAddInventoryUseCase.ExecuteAsync(inventory)          [use-case interface]
      → AddInventoryUseCase                                 [use-case impl]
        assigns Id = $"INV-{Guid.CreateVersion7()}"
        → IInventoryRepository.AddInventoryAsync(inventory) [plugin interface]
          → InventoryRepository (InMemory)                  [plugin adapter]
            → HashSet<Inventory>.Add()                      [domain entity]
  → NavigationManager.NavigateTo("/inventories")
```

---

## Technology Stack

| Concern | Technology |
| --- | --- |
| Runtime | .NET 10 |
| Web framework | ASP.NET Core Blazor (static SSR with `MapRazorComponents`) |
| UI styling | Bootstrap 5 |
| Form validation | FluentValidation + Blazilla (`<FluentValidator>` component) |
| Persistence (current) | In-memory (`HashSet<Inventory>` seed data) |
| Persistence (future) | Swappable via new `IMS.Plugins.*` project |

---

## Planned Extension Points

The architecture makes the following changes straightforward:

- **New persistence backend** — add `IMS.Plugins.EFCore` (or similar), implement `IInventoryRepository`, swap the DI binding.
- **New use cases** — add an interface to `IMS.UseCases/Interfaces/` and its implementation to `IMS.UseCases/<Aggregate>/`, then register it in `Program.cs`.
- **New entities** — add to `IMS.CoreBusiness` with no impact on the infrastructure.
- **API surface** — an `IMS.WebApi` project could expose the same use-case interfaces over HTTP without touching domain or application layers.
