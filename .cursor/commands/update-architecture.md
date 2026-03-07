Update `docs/architecture.md` to accurately reflect the current state of the codebase. (Short form: `/ua`)

Steps:
1. Read `docs/architecture.md` to understand what is already documented.
2. Read every `.cs`, `.razor`, and `.csproj` file under `IMS/` that is relevant to the architecture (entities, use-case interfaces, use-case implementations, plugin interfaces, plugin adapters, DI wiring in `Program.cs`, and Blazor pages/components/layout).
3. Compare what you found against the existing document. Identify any additions, removals, or changes in: projects, layers, entities, use-case interfaces, plugin interfaces, implementations, DI registrations, pages, or components.
4. Update only the sections of `docs/architecture.md` that need to change. Do not rewrite sections that are already accurate.
5. Update the **Last updated** field in the header table to today's date, and update **Based on commit** and **Commit date** by running `git log -1 --format="%H %cd" --date=format:"%Y-%m-%d"` in the repo root.
6. Verify the file has no markdown lint errors before finishing.
