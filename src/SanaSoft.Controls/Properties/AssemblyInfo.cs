using System.Windows;
using System.Windows.Markup;

// Default control styles live in Themes/Generic.xaml of this assembly; there are no per-OS theme dictionaries.
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

// One XAML namespace for everything public: xmlns:ss="https://schemas.sanasoft.com/controls"
[assembly: XmlnsDefinition("https://schemas.sanasoft.com/controls", "SanaSoft.Controls")]
[assembly: XmlnsDefinition("https://schemas.sanasoft.com/controls", "SanaSoft.Controls.Converters")]
[assembly: XmlnsDefinition("https://schemas.sanasoft.com/controls", "SanaSoft.Controls.Themes")]
[assembly: XmlnsPrefix("https://schemas.sanasoft.com/controls", "ss")]
