using System.Runtime.CompilerServices;
using System.Windows;

namespace SanaSoft.Controls.Tests;

internal static class WpfTestSetup
{
    // Application's static constructor registers the pack:// URI scheme that WPF uses to load XAML resources.
    // A real app always has it; the test host does not, so run it once without creating an Application.
    [ModuleInitializer]
    internal static void RegisterPackUriScheme() => RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
}
