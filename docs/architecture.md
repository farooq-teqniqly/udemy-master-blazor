# IMS — Inventory Management System: Architecture

| | |
| --- | --- |
| **Last updated** | 2026-03-07 |
| **Based on commit** | `fa2429d0eff9b27d629c67289045e759528907d0` |
| **Commit date** | 2026-03-07 |

## Overview

The Inventory Management System (IMS) is a nascent Blazor Server application built on **.NET 10** following **Clean Architecture** (also known as the Ports & Adapters or Onion Architecture pattern). The solution is structured so that the innermost layers have zero knowledge of outer layers, and all dependencies point inward toward the domain.

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
| `Inventory` | Core domain entity representing a stocked item (Id, Name, Price, Quantity) |

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

#### Plugin interfaces / Repository ports (`PluginInterfaces/`)

| Interface | Purpose |
| --- | --- |
| `IInventoryRepository` | Persistence port — fetches inventories by name |

#### Use-case implementations (`Inventories/`)

| Class | Implements | Description |
| --- | --- | --- |
| `ViewInventoriesByNameUseCase` | `IViewInventoriesByNameUseCase` | Delegates to `IInventoryRepository`; returns a filtered collection of `Inventory` entities |

**Project dependencies:** `IMS.CoreBusiness`.

---

### 3. `IMS.Plugins.InMemory` — Infrastructure / Plugin Layer

Provides concrete *adapters* for the repository ports defined in `IMS.UseCases`. Currently ships a single in-memory implementation used during development and early testing.

| Class | Implements | Description |
| --- | --- | --- |
| `InventoryRepository` | `IInventoryRepository` | Hard-coded seed data (bike parts); filters by name using `OrdinalIgnoreCase` comparison |

Swapping this project for a different plugin (e.g., `IMS.Plugins.EFCore`) requires only a DI re-wiring in `Program.cs` — no changes to the domain or use-case layers.

**Project dependencies:** `IMS.UseCases` (and transitively `IMS.CoreBusiness`).

---

### 4. `IMS.WebApp` — Presentation Layer (Blazor Server)

ASP.NET Core Blazor application that renders the UI and wires the dependency injection container. Components interact exclusively with use-case interfaces — never with repositories directly.

#### Pages (`Components/Pages/`)

| Page | Route | Description |
| --- | --- | --- |
| `Home.razor` | `/` | Quick view: renders a plain list of all inventory items |
| `InventoryList.razor` | `/inventories` | Full inventory table via `InventoryListComponent` |
| `NotFound.razor` | — | 404 handler |
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

// Use-case bound to its interface
builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>();
```

**Project dependencies:** `IMS.UseCases`, `IMS.Plugins.InMemory`.

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

## Data Flow (View Inventories)

```text
Browser request
  → InventoryList.razor (page)
    → InventoryListComponent.razor (control)
      → IViewInventoriesByNameUseCase.ExecuteAsync()   [use-case interface]
        → ViewInventoriesByNameUseCase                 [use-case impl]
          → IInventoryRepository.GetInventoriesByNameAsync()  [plugin interface]
            → InventoryRepository (InMemory)           [plugin adapter]
              → List<Inventory>                        [domain entity]
          ← IReadOnlyCollection<Inventory>
        ← IEnumerable<Inventory>
      ← List<Inventory>
    → InventoryListItemComponent × N (one per row)
  → Rendered HTML table
```

---

## Technology Stack

| Concern | Technology |
| --- | --- |
| Runtime | .NET 10 |
| Web framework | ASP.NET Core Blazor Server (static SSR with `MapRazorComponents`) |
| UI styling | Bootstrap 5 |
| Persistence (current) | In-memory (`List<Inventory>` seed data) |
| Persistence (future) | Swappable via new `IMS.Plugins.*` project |

---

## Planned Extension Points

The architecture makes the following changes straightforward:

- **New persistence backend** — add `IMS.Plugins.EFCore` (or similar), implement `IInventoryRepository`, swap the DI binding.
- **New use cases** — add an interface to `IMS.UseCases/Interfaces/` and its implementation to `IMS.UseCases/<Aggregate>/`, then register it in `Program.cs`.
- **New entities** — add to `IMS.CoreBusiness` with no impact on the infrastructure.
- **API surface** — an `IMS.WebApi` project could expose the same use-case interfaces over HTTP without touching domain or application layers.
