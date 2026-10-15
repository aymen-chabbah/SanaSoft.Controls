using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace SanaSoft.Controls.Tests.Controls;

public class NumericBoxAutomationPeerTests
{
    [StaFact]
    public void Is_a_spinner_with_the_owner_automation_id()
    {
        var box = new NumericBox();
        AutomationProperties.SetAutomationId(box, "Quantity");

        var peer = UIElementAutomationPeer.CreatePeerForElement(box);

        using (new AssertionScope())
        {
            peer.Should().BeOfType<NumericBoxAutomationPeer>();
            peer.GetAutomationControlType().Should().Be(AutomationControlType.Spinner);
            peer.GetClassName().Should().Be("NumericBox");
            peer.GetAutomationId().Should().Be("Quantity");
        }
    }

    [StaFact]
    public void Exposes_the_range_value_pattern()
    {
        var box = new NumericBox { Minimum = 1, Maximum = 9, Increment = 2, Value = 5 };

        var range = (IRangeValueProvider)UIElementAutomationPeer.CreatePeerForElement(box).GetPattern(PatternInterface.RangeValue);

        using (new AssertionScope())
        {
            range.Value.Should().Be(5);
            range.Minimum.Should().Be(1);
            range.Maximum.Should().Be(9);
            range.SmallChange.Should().Be(2);
            range.IsReadOnly.Should().BeFalse();
        }
    }

    [StaFact]
    public void Sets_the_value_through_the_range_value_pattern()
    {
        var box = new NumericBox { Maximum = 10 };
        var range = (IRangeValueProvider)UIElementAutomationPeer.CreatePeerForElement(box).GetPattern(PatternInterface.RangeValue);

        range.SetValue(7);

        box.Value.Should().Be(7);
    }

    [StaFact]
    public void Refuses_to_set_a_read_only_box()
    {
        var box = new NumericBox { IsReadOnly = true };
        var range = (IRangeValueProvider)UIElementAutomationPeer.CreatePeerForElement(box).GetPattern(PatternInterface.RangeValue);

        var act = () => range.SetValue(3);

        act.Should().Throw<InvalidOperationException>();
    }

    [StaFact]
    public void Refuses_a_value_outside_the_range()
    {
        var box = new NumericBox { Maximum = 10 };
        var range = (IRangeValueProvider)UIElementAutomationPeer.CreatePeerForElement(box).GetPattern(PatternInterface.RangeValue);

        var act = () => range.SetValue(11);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
