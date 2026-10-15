using System.Windows;
using SanaSoft.Controls.Samples.Themes;

namespace SanaSoft.Controls.Samples;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        FluentBranding.Apply(this);
    }
}
