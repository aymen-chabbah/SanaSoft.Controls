using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SanaSoft.Controls.Core.Mvvm;
using SanaSoft.Controls.Samples.Pages;

namespace SanaSoft.Controls.Samples.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private SamplePage _selectedPage;
    private string _searchText = "";
    private string _selectedTheme = "System";

    public MainViewModel()
    {
        var home = new SamplePage(
            "Home",
            "Reusable WPF controls, styles and converters for SanaSoft applications.",
            "",
            () => new HomePage());

        // Register a page here for each new control.
        ControlPages =
        [
            new SamplePage(
                "NumericBox",
                "A text box for numbers, with spin buttons, arrow-key stepping and a clamped range.",
                "",
                () => new NumericBoxPage(),
                "ss:NumericBox"),
            new SamplePage(
                "Converters",
                "Value converters for common binding scenarios.",
                "",
                () => new ConvertersPage(),
                "ss:InverseBooleanToVisibilityConverter"),
        ];

        Pages = [home, .. ControlPages];
        PagesView = CollectionViewSource.GetDefaultView(Pages);
        PagesView.Filter = MatchesSearch;
        _selectedPage = home;

        NavigateCommand = new RelayCommand<SamplePage>(page => SelectedPage = page);
    }

    public IReadOnlyList<SamplePage> Pages { get; }

    public IReadOnlyList<SamplePage> ControlPages { get; }

    public ICollectionView PagesView { get; }

    public ICommand NavigateCommand { get; }

    public IReadOnlyList<string> Themes { get; } = ["System", "Light", "Dark"];

    public SamplePage SelectedPage
    {
        get => _selectedPage;
        set
        {
            // The list clears its selection when the search hides the current page; keep showing it.
            if (value is not null)
                SetProperty(ref _selectedPage, value);
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            PagesView.Refresh();
        }
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            SetProperty(ref _selectedTheme, value);

            // FluentBranding follows the theme change and switches the SanaSoft tokens and logo with it.
#pragma warning disable WPF0001 // ThemeMode is marked experimental.
            Application.Current.ThemeMode = new ThemeMode(value);
#pragma warning restore WPF0001
        }
    }

    private bool MatchesSearch(object item) =>
        string.IsNullOrWhiteSpace(SearchText)
        || ((SamplePage)item).Title.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase);
}
