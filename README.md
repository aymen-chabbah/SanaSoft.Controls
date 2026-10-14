# Sanasoft.Shared

Shared WPF components and C# code for Sanasoft applications.

| Project | Target | Contents |
|---|---|---|
| `src/Sanasoft.Shared` | `net10.0` | UI-independent code (MVVM base classes, helpers) |
| `src/Sanasoft.Shared.Wpf` | `net10.0-windows` | WPF controls, styles, converters |
| `tests/*` | | xUnit + FluentAssertions tests for each project |

## Build and test

Requires the .NET 10 SDK on Windows.

```bash
dotnet build
dotnet test
```

## Using it in an application

Add this repository as a git submodule of your application:

```bash
git submodule add https://github.com/aymen-chabbah/Sanasoft.Shared.git external/Sanasoft.Shared
```

Then reference the projects you need from your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\external\Sanasoft.Shared\src\Sanasoft.Shared.Wpf\Sanasoft.Shared.Wpf.csproj" />
</ItemGroup>
```

Teammates cloning the application run `git submodule update --init` once.
To pick up newer shared code, run `git submodule update --remote` and commit the updated submodule.

## Contributing

1. Branch from `main` (`feature/<short-name>`).
2. Add the component under the matching folder (`Converters/`, `Controls/`, `Mvvm/`, ...) with tests next to it in the matching test project.
3. Open a pull request; CI builds and runs all tests on Windows.
