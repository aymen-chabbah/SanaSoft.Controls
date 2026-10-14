# SanaSoft.Controls

Shared WPF components and C# code for SanaSoft applications.

| Project | Target | Contents |
|---|---|---|
| `src/SanaSoft.Controls.Core` | `net10.0` | UI-independent code (MVVM base classes, helpers) |
| `src/SanaSoft.Controls` | `net10.0-windows` | WPF controls, styles, converters |
| `tests/*` | | xUnit + FluentAssertions tests for each project |

## Build and test

Requires the .NET 10 SDK on Windows.

```bash
dotnet build
dotnet test
```

## Using the packages in an application

The packages `SanaSoft.Controls.Core` and `SanaSoft.Controls` are published to GitHub Packages.
GitHub Packages always requires sign-in to restore, even for a public repository.

1. Create a GitHub personal access token (classic) with the `read:packages` scope.
2. Add the feed once per machine:

   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/aymen-chabbah/index.json --name sanasoft --username <your-github-username> --password <your-token>
   ```

3. Reference the package from your application:

   ```bash
   dotnet add package SanaSoft.Controls
   ```

`SanaSoft.Controls` brings in `SanaSoft.Controls.Core` automatically.

## Releasing a new version

Versions follow [Semantic Versioning](https://semver.org). Merge to `main`, then tag the commit:

```bash
git tag v1.2.0
git push origin v1.2.0
```

CI builds, runs the tests, and publishes both packages with version `1.2.0`.

## Contributing

1. Branch from `main` (`feature/<short-name>`).
2. Add the component under the matching folder (`Converters/`, `Controls/`, `Mvvm/`, ...) with tests next to it in the matching test project.
3. Open a pull request; CI builds and runs all tests on Windows.
