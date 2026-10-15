using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SanaSoft.Controls.Tests.Controls;

public class NumericBoxTests
{
    public NumericBoxTests() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    [StaFact]
    public void Clamps_the_value_to_the_range()
    {
        var sut = new NumericBox { Minimum = 0, Maximum = 10 };

        sut.Value = 42;

        sut.Value.Should().Be(10);
    }

    [StaFact]
    public void Re_clamps_the_value_when_the_range_shrinks()
    {
        var sut = new NumericBox { Value = 8 };

        sut.Maximum = 5;

        sut.Value.Should().Be(5);
    }

    [StaFact]
    public void Keeps_the_maximum_at_or_above_the_minimum()
    {
        var sut = new NumericBox { Maximum = 5 };

        sut.Minimum = 10;

        using (new AssertionScope())
        {
            sut.Maximum.Should().Be(10);
            sut.Value.Should().Be(10);
        }
    }

    [StaFact]
    public void Rejects_a_non_positive_increment()
    {
        var sut = new NumericBox();

        var act = () => sut.Increment = 0;

        act.Should().Throw<ArgumentException>();
    }

    [StaFact]
    public void Rejects_a_non_finite_value()
    {
        var sut = new NumericBox();

        var act = () => sut.Value = double.NaN;

        act.Should().Throw<ArgumentException>();
    }

    [StaFact]
    public void Increase_command_steps_by_the_increment()
    {
        var sut = new NumericBox { Value = 1, Increment = 0.5 };

        NumericBox.IncreaseCommand.Execute(null, sut);

        sut.Value.Should().Be(1.5);
    }

    [StaFact]
    public void Increase_command_cannot_execute_at_the_maximum()
    {
        var sut = new NumericBox { Maximum = 3, Value = 3 };

        NumericBox.IncreaseCommand.CanExecute(null, sut).Should().BeFalse();
    }

    [StaFact]
    public void Decrease_command_cannot_execute_when_read_only()
    {
        var sut = new NumericBox { Value = 3, IsReadOnly = true };

        NumericBox.DecreaseCommand.CanExecute(null, sut).Should().BeFalse();
    }

    [StaFact]
    public void Raises_value_changed_with_old_and_new_values()
    {
        var sut = new NumericBox { Value = 1 };
        RoutedPropertyChangedEventArgs<double>? raised = null;
        sut.ValueChanged += (_, e) => raised = e;

        sut.Value = 4;

        using (new AssertionScope())
        {
            raised.Should().NotBeNull();
            raised!.OldValue.Should().Be(1);
            raised.NewValue.Should().Be(4);
        }
    }

    [StaFact]
    public void Default_template_provides_the_text_box_part()
    {
        var sut = Initialized(new NumericBox { Value = 12.5, StringFormat = "N2" });

        sut.ApplyTemplate().Should().BeTrue();

        var textBox = sut.Template.FindName("PART_TextBox", sut).Should().BeOfType<TextBox>().Subject;
        textBox.Text.Should().Be("12.50");
    }

    [StaFact]
    public void Commits_typed_text_on_enter_and_clamps_it()
    {
        var sut = Initialized(new NumericBox { Maximum = 100 });
        sut.ApplyTemplate();
        var textBox = (TextBox)sut.Template.FindName("PART_TextBox", sut);

        textBox.Text = "250";
        CommitWithEnter(sut);

        using (new AssertionScope())
        {
            sut.Value.Should().Be(100);
            textBox.Text.Should().Be("100");
        }
    }

    [StaFact]
    public void Reverts_invalid_text()
    {
        var sut = Initialized(new NumericBox { Value = 7 });
        sut.ApplyTemplate();
        var textBox = (TextBox)sut.Template.FindName("PART_TextBox", sut);

        textBox.Text = "abc";
        CommitWithEnter(sut);

        using (new AssertionScope())
        {
            sut.Value.Should().Be(7);
            textBox.Text.Should().Be("7");
        }
    }

    // The XAML parser calls BeginInit/EndInit; EndInit is where WPF fetches the default style from Generic.xaml.
    private static NumericBox Initialized(NumericBox sut)
    {
        sut.BeginInit();
        sut.EndInit();
        return sut;
    }

    // The control steps and commits through PreviewKeyDown; raise it directly since there is no window or keyboard device.
    private static void CommitWithEnter(NumericBox sut)
    {
        var source = new System.Windows.Interop.HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);
        try
        {
            sut.RaiseEvent(new System.Windows.Input.KeyEventArgs(
                System.Windows.Input.Keyboard.PrimaryDevice, source, 0, System.Windows.Input.Key.Enter)
            {
                RoutedEvent = UIElement.PreviewKeyDownEvent,
            });
        }
        finally
        {
            source.Dispose();
        }
    }
}
