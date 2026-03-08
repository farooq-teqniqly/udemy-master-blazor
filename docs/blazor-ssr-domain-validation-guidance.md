# Blazor SSR — Domain Modelling, Validation, and View Models: Engineering Guidance

| | |
| --- | --- |
| **Created** | 2026-03-07 |
| **Applies to** | ASP.NET Core Blazor applications using static Server-Side Rendering (SSR) |

---

## Context

This document captures guidance distilled from design discussions on the IMS project. It is intended as a reference for engineers starting a new Blazor SSR application of moderate or greater complexity, or for evolving an existing application that began with a simple anemic domain model.

The three concerns are tightly related and are best decided together:

1. How rich should the domain model be?
2. Where should validation rules live?
3. Should UI forms bind directly to domain entities or to dedicated view models?

---

## 1. The Anemic Domain Model Problem

An **anemic domain model** is an entity class whose only content is public getters and setters — a named property bag with no behaviour or invariants:

```csharp
public class Inventory
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public int Quantity { get; set; }
}
```

This is acceptable at the start of a project, but it creates pressure over time:

- Validation rules accumulate elsewhere (use-case layer, controllers, components), scattered across the codebase
- Nothing stops a caller from creating an `Inventory` with a null name or a negative price
- When a second consumer appears (e.g., a REST API alongside the Blazor UI), the invariants must be duplicated or enforced only by convention

The goal of a **rich domain model** is that the entity enforces its own invariants — it is impossible to construct or mutate it into an invalid state.

---

## 2. Validation: Where Should the Rules Live?

Validation rules exist at two distinct levels with different purposes:

| Level | Purpose | Where it lives | Example |
| --- | --- | --- | --- |
| **UI / application validation** | Give the user friendly per-field feedback before a round-trip to the server | View model or FluentValidation validator in the application layer | "Name is required", "Price must be positive" |
| **Domain invariants** | Enforce business rules that must be true regardless of which entry point is used | Domain entity | "A product cannot have a negative price", "A name cannot be empty" |

These levels coexist — they are not alternatives. UI validation is a UX concern; domain invariants are a correctness concern.

### 2a. Validation in the Application Layer (FluentValidation)

Placing FluentValidation validators in the use-cases project (`IMS.UseCases/Validators/`) and wiring them to the Blazor `EditForm` via a library such as **Blazilla** (`<FluentValidator>`) is a clean, practical approach for Blazor SSR:

- Rules are centralised in one class per entity
- The Blazor `EditContext` pipeline is respected: per-field `<ValidationMessage>` components work without extra code
- No domain-layer dependency on FluentValidation

**When this is sufficient:** simple CRUD forms where the application layer is the only consumer of the domain.

**When it starts to break down:** when a second consumer (REST API, background job, CLI) needs the same rules. The rules must either be duplicated or the application-layer validator must be called explicitly by every entry point — fragile.

### 2b. Domain Invariants via Factory Method (recommended for moderate+ complexity)

Rather than throwing from a constructor, prefer a **static factory method** that returns a result type:

```csharp
public sealed class Inventory
{
    // Private constructor — cannot be created in an invalid state externally
    private Inventory() { }

    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public double Price { get; private set; }
    public int Quantity { get; private set; }

    public static Result<Inventory> Create(string name, double price, int quantity)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name) || name.Length < 5)
            errors.Add("Name must be at least 5 characters.");
        if (name?.Length > 100)
            errors.Add("Name cannot exceed 100 characters.");
        if (price < 0)
            errors.Add("Price cannot be negative.");
        if (quantity < 0)
            errors.Add("Quantity cannot be negative.");

        if (errors.Count > 0)
            return Result<Inventory>.Failure(errors);

        return Result<Inventory>.Success(new Inventory
        {
            Id = $"INV-{Guid.CreateVersion7()}",
            Name = name!,
            Price = price,
            Quantity = quantity
        });
    }
}
```

The use case calls `Inventory.Create(...)`, handles the `Result`, and either persists the entity or surfaces errors. No consumer can bypass the invariants.

**Why not throw from the constructor?**

- Blazor SSR form binding creates objects via a **parameterless constructor and property setters**. A validating constructor is never invoked by the model binder.
- Exceptions from constructors are harder to handle gracefully than result types.
- Result types make the failure path explicit in the method signature.

---

## 3. View Models: Separating Form Data from Domain Entities

### Should you bind the form directly to the domain entity?

| Scenario | Recommendation |
| --- | --- |
| Simple CRUD, domain entity is a POCO, early project | Binding directly to the entity is acceptable |
| Domain entity has private setters or a factory method | **Must** use a view model — SSR binding requires a parameterless constructor and public setters |
| Form shape differs from entity shape (e.g., different field names, computed values, multi-step wizard) | **Must** use a view model |
| Multiple forms target the same entity (Add vs Edit) | View model per form is strongly preferred |
| Application has multiple entry points (Blazor UI + REST API) | View model (or a shared DTO record) per entry point |

### View model pattern

The view model lives in `IMS.WebApp` (the Interface Adapters layer). It is purely a UI concern.

```csharp
// IMS.WebApp/Models/AddInventoryViewModel.cs
public sealed class AddInventoryViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public double Price { get; set; }

    [Range(0, 100_000)]
    public int Quantity { get; set; }
}
```

The page/component maps the view model to the domain object before calling the use case. This mapping belongs in the **Interface Adapters layer** (the page/component), not in the use case:

```csharp
// AddInventory.razor @code block
private async Task SaveAsync()
{
    var result = Inventory.Create(_viewModel.Name, _viewModel.Price, _viewModel.Quantity);

    if (!result.IsSuccess)
    {
        // surface errors to the UI — result.Errors
        return;
    }

    await AddInventoryUseCase.ExecuteAsync(result.Value);
    NavigationManager.NavigateTo("/inventories");
}
```

**The use case interface never accepts a view model.** The use case works with domain entities or primitive/record input types — never with UI-layer types.

### Using FluentValidation with a view model

If you prefer FluentValidation over data annotations on the view model, define a separate validator for the view model in `IMS.WebApp` (or in `IMS.UseCases` if it mirrors the domain rules and you want to reuse it):

```csharp
// Validator lives in IMS.WebApp — it validates UI input, not the domain entity
public sealed class AddInventoryViewModelValidator : AbstractValidator<AddInventoryViewModel>
{
    public AddInventoryViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(100);
        RuleFor(x => x.Price).InclusiveBetween(0, 1_000_000);
        RuleFor(x => x.Quantity).InclusiveBetween(0, 100_000);
    }
}
```

---

## 4. Decision Flowchart

```text
Starting a new Blazor SSR feature:

Is this a simple CRUD form with a POCO entity and only one consumer?
├── Yes → Bind directly to entity + FluentValidation in use-cases layer. Revisit if complexity grows.
└── No ↓

Does the domain entity enforce its own invariants (private setters / factory method)?
├── Yes → Must use a view model. Map view model → domain in the page/component.
└── No  → Consider introducing a factory method if there are multiple consumers.

Does the form shape differ from the entity (different fields, computed values, multi-step)?
└── Yes → Must use a view model regardless of domain model style.

Where should validation rules live?
├── UI feedback (per-field messages, empty-field checks) → View model validator in WebApp layer
├── Business invariants (always true, all consumers) → Domain factory method / domain methods
└── Both levels are needed for moderate+ complexity apps — they serve different purposes.
```

---

## 5. Summary of Recommendations

| Concern | Simple / Early project | Moderate+ complexity |
| --- | --- | --- |
| Domain model | Anemic POCO is acceptable | Enrich with factory method, private setters, equality by Id |
| Validation location | FluentValidation in use-cases layer | Both: domain factory + view model validator |
| Form binding target | Domain entity directly | Dedicated view model per form |
| Mapping | No mapping needed | View model → domain entity in the page/component (Interface Adapters layer) |
| Use case interface | Accepts domain entity | Accepts domain entity (never a view model or UI type) |

---

## 6. Key Constraints Specific to Blazor SSR

- **Blazor SSR model binding** creates objects via a parameterless constructor and public property setters. Any domain design that removes these (private setters, required constructor parameters) **requires a view model** as an intermediary.
- **`@onclick` handlers do not fire** in static SSR. Navigation on cancel must use `<a href>` links or `NavigationManager.NavigateTo` inside an `OnValidSubmit` handler (which runs server-side on form POST).
- **`[SupplyParameterFromForm]`** can set the bound property to `null` during a form POST if binding fails. Guard against this with a null-coalescing setter or a backing field.
