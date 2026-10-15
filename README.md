# SanaSoft.Controls

Shared WPF components and C# code for SanaSoft applications.

| Project | Target | Contents |
|---|---|---|
| `src/SanaSoft.Controls.Core` | `net10.0` | UI-independent code (MVVM base classes, helpers) |
| `src/SanaSoft.Controls` | `net10.0-windows` | WPF controls, theme tokens, converters |
| `samples/SanaSoft.Controls.Samples` | `net10.0-windows` | Gallery app that shows every control (not packed) |
| `tests/*` | | xUnit + FluentAssertions tests for each project |

## Architecture of `SanaSoft.Controls`

```
Properties/AssemblyInfo.cs     ThemeInfo + one XAML namespace (https://schemas.sanasoft.com/controls)
Controls/<Name>/<Name>.cs      control behaviour: dependency properties, commands, events
Controls/<Name>/<Name>.xaml    default style and template of that control
Converters/                    value converters
Themes/Generic.xaml            index only: merges the fallback tokens and every control's .xaml
Themes/Tokens/*.xaml           design tokens: Light/Dark palettes, Typography, Spacing
Themes/ResourceKeys.cs         public keys of the tokens
Themes/SanaSoftTheme.cs        dictionary applications merge to pick Light or Dark
```

Rules for every control:

- **Custom control, not UserControl.** Derive from `Control` (or a WPF base class). Put all looks in the template, so applications can restyle or retemplate it.
- **Template contract.** Declare named parts with `[TemplatePart]` (`PART_*`) and states with `[TemplateVisualState]`. The control must keep working when a part is missing.
- **Tokens only.** Templates use `{DynamicResource {x:Static themes:ResourceKeys.*}}` and never hard-coded colors, so light/dark switching works. Storyboards cannot use DynamicResource; animate the `Opacity` of an overlay that uses the token instead (see `NumericBox.xaml`).
- **Testable with UI Automation.** Give each control an `AutomationPeer` (without one, a custom control is invisible to UI tests and screen readers) and set `AutomationProperties.AutomationId` on the interactive template parts. These IDs are public contract, like `PART_*` names. For example, `NumericBox` is a `Spinner` with the RangeValue pattern, and its parts are `TextBox`, `IncreaseButton` and `DecreaseButton`.
- **Documented public API.** XML docs are required (the build fails without them) and ship in the package for IntelliSense.

### Adding a control

1. Create `Controls/<Name>/<Name>.cs` and `Controls/<Name>/<Name>.xaml` (copy `NumericBox` as a starting point).
2. Merge the `.xaml` into `Themes/Generic.xaml`.
3. Add tests in `tests/SanaSoft.Controls.Tests/Controls/`. Use `[StaFact]`. Call `BeginInit()`/`EndInit()` before `ApplyTemplate()` so WPF loads the default style.
4. Add a page in `samples/SanaSoft.Controls.Samples/Pages/` and register it in `MainViewModel`.

### Packaging

- The PDB is embedded and Source Link is on, so consumers can step into the library source.
- Package validation runs on `dotnet pack`. After 1.0.0 ships, set `PackageValidationBaselineVersion` in `Directory.Build.props` to fail the pack on breaking API changes.
- Projects under `tests/` and `samples/` set `IsPackable=false` before importing the root `Directory.Build.props`.

## Using the controls in an application

```xml
<Application xmlns:ss="https://schemas.sanasoft.com/controls" ...>
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ss:SanaSoftTheme Theme="Light" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

Default styles load automatically. Without `SanaSoftTheme`, the controls fall back to the light tokens. Set `Theme` at runtime to switch palettes. Use the tokens in your own XAML with `{DynamicResource {x:Static ss:ResourceKeys.AccentBrush}}`.

## Build and test

Requires the .NET 10 SDK on Windows.

```bash
dotnet build
dotnet test
```

## Installing the packages

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
