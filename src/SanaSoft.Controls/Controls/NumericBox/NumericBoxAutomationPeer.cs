using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace SanaSoft.Controls;

/// <summary>
/// Exposes <see cref="NumericBox"/> to UI Automation as a spinner with the RangeValue pattern,
/// so screen readers and UI tests can read and set its value.
/// </summary>
public class NumericBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    /// <summary>Creates the peer for <paramref name="owner"/>.</summary>
    public NumericBoxAutomationPeer(NumericBox owner)
        : base(owner)
    {
    }

    private NumericBox Box => (NumericBox)Owner;

    /// <inheritdoc />
    public double Value => Box.Value;

    /// <inheritdoc />
    public double Minimum => Box.Minimum;

    /// <inheritdoc />
    public double Maximum => Box.Maximum;

    /// <inheritdoc />
    public double SmallChange => Box.Increment;

    /// <inheritdoc />
    public double LargeChange => Box.Increment;

    /// <inheritdoc />
    public bool IsReadOnly => Box.IsReadOnly || !Box.IsEnabled;

    /// <inheritdoc />
    public void SetValue(double value)
    {
        if (!IsEnabled())
            throw new ElementNotEnabledException();

        if (Box.IsReadOnly)
            throw new InvalidOperationException("The NumericBox is read-only.");

        if (value < Minimum || value > Maximum)
            throw new ArgumentOutOfRangeException(nameof(value));

        Box.SetCurrentValue(NumericBox.ValueProperty, value);
    }

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface) =>
        patternInterface == PatternInterface.RangeValue ? this : base.GetPattern(patternInterface);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Spinner;

    /// <inheritdoc />
    protected override string GetClassNameCore() => nameof(NumericBox);

    internal void RaiseValueChanged(double oldValue, double newValue) =>
        RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
}
