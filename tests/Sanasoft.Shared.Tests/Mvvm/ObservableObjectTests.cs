using System.ComponentModel;
using Sanasoft.Shared.Mvvm;

namespace Sanasoft.Shared.Tests.Mvvm;

public class ObservableObjectTests
{
    [Fact]
    public void Stores_the_new_value()
    {
        var sut = CreateSut();

        sut.Name = "Pump A";

        sut.Name.Should().Be("Pump A");
    }

    [Fact]
    public void Raises_PropertyChanged_with_the_property_name_when_the_value_changes()
    {
        var sut = CreateSut();
        var raised = RecordPropertyChanges(sut);

        sut.Name = "Pump A";

        raised.Should().Equal(nameof(Person.Name));
    }

    [Fact]
    public void Does_not_raise_PropertyChanged_when_the_value_is_unchanged()
    {
        var sut = CreateSut(name: "Pump A");
        var raised = RecordPropertyChanges(sut);

        sut.Name = "Pump A";

        raised.Should().BeEmpty();
    }

    [Fact]
    public void Raises_PropertyChanged_when_the_value_changes_to_null()
    {
        var sut = CreateSut(name: "Pump A");
        var raised = RecordPropertyChanges(sut);

        sut.Name = null;

        raised.Should().Equal(nameof(Person.Name));
    }

    [Fact]
    public void Does_not_raise_PropertyChanged_when_null_is_set_again()
    {
        var sut = CreateSut(name: null);
        var raised = RecordPropertyChanges(sut);

        sut.Name = null;

        raised.Should().BeEmpty();
    }

    [Fact]
    public void Raises_PropertyChanged_for_a_value_type_property()
    {
        var sut = CreateSut();
        var raised = RecordPropertyChanges(sut);

        sut.Age = 42;

        raised.Should().Equal(nameof(Person.Age));
    }

    [Fact]
    public void Raises_PropertyChanged_with_the_given_name_when_notified_explicitly()
    {
        var sut = CreateSut();
        var raised = RecordPropertyChanges(sut);

        sut.NotifyDisplayNameChanged();

        raised.Should().Equal(nameof(Person.DisplayName));
    }

    [Fact]
    public void Does_not_throw_when_no_one_listens()
    {
        var sut = CreateSut();

        var act = () => sut.Name = "Pump A";

        act.Should().NotThrow();
    }

    private static Person CreateSut(string? name = null) => new() { Name = name };

    private static List<string?> RecordPropertyChanges(INotifyPropertyChanged sut)
    {
        var raised = new List<string?>();
        sut.PropertyChanged += (_, args) => raised.Add(args.PropertyName);
        return raised;
    }

    private sealed class Person : ObservableObject
    {
        private string? _name;
        private int _age;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public string DisplayName => $"{Name} ({Age})";

        public void NotifyDisplayNameChanged() => OnPropertyChanged(nameof(DisplayName));
    }
}
