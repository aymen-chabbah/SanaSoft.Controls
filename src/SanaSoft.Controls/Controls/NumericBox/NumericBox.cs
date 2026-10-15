using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SanaSoft.Controls;

/// <summary>
/// Text box for a number, with spin buttons, keyboard stepping and a clamped range.
/// </summary>
/// <remarks>
/// Text is parsed and formatted with <see cref="CultureInfo.CurrentCulture"/>. Typed text is committed
/// on Enter or when focus leaves the control; invalid text is reverted to the current <see cref="Value"/>.
/// Up and Down arrows step by <see cref="Increment"/>.
/// </remarks>
[TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
[TemplateVisualState(GroupName = CommonStatesGroup, Name = NormalState)]
[TemplateVisualState(GroupName = CommonStatesGroup, Name = MouseOverState)]
[TemplateVisualState(GroupName = CommonStatesGroup, Name = DisabledState)]
[TemplateVisualState(GroupName = FocusStatesGroup, Name = FocusedState)]
[TemplateVisualState(GroupName = FocusStatesGroup, Name = UnfocusedState)]
public class NumericBox : Control
{
    private const string TextBoxPartName = "PART_TextBox";
    private const string CommonStatesGroup = "CommonStates";
    private const string NormalState = "Normal";
    private const string MouseOverState = "MouseOver";
    private const string DisabledState = "Disabled";
    private const string FocusStatesGroup = "FocusStates";
    private const string FocusedState = "Focused";
    private const string UnfocusedState = "Unfocused";

    private TextBox? _textBox;

    /// <summary>Increases <see cref="Value"/> by <see cref="Increment"/>.</summary>
    public static RoutedCommand IncreaseCommand { get; } = new(nameof(IncreaseCommand), typeof(NumericBox));

    /// <summary>Decreases <see cref="Value"/> by <see cref="Increment"/>.</summary>
    public static RoutedCommand DecreaseCommand { get; } = new(nameof(DecreaseCommand), typeof(NumericBox));

    /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(NumericBox),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue),
        IsFinite);

    /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(double), typeof(NumericBox),
        new FrameworkPropertyMetadata(double.MinValue, OnMinimumChanged),
        IsFinite);

    /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(double), typeof(NumericBox),
        new FrameworkPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum),
        IsFinite);

    /// <summary>Identifies the <see cref="Increment"/> dependency property.</summary>
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
        nameof(Increment), typeof(double), typeof(NumericBox),
        new FrameworkPropertyMetadata(1d),
        value => IsFinite(value) && (double)value > 0);

    /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
    public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
        nameof(StringFormat), typeof(string), typeof(NumericBox),
        new FrameworkPropertyMetadata("G", (d, _) => ((NumericBox)d).UpdateText()));

    /// <summary>Identifies the <see cref="IsReadOnly"/> dependency property.</summary>
    public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
        typeof(NumericBox),
        new FrameworkPropertyMetadata(false, (_, _) => CommandManager.InvalidateRequerySuggested()));

    /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(NumericBox));

    static NumericBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(typeof(NumericBox)));
        IsEnabledProperty.OverrideMetadata(typeof(NumericBox), new UIPropertyMetadata((d, _) => ((NumericBox)d).UpdateVisualStates(true)));

        CommandManager.RegisterClassCommandBinding(typeof(NumericBox), new CommandBinding(
            IncreaseCommand,
            (sender, _) => ((NumericBox)sender).Step(+1),
            (sender, e) => e.CanExecute = ((NumericBox)sender).CanStep(+1)));
        CommandManager.RegisterClassCommandBinding(typeof(NumericBox), new CommandBinding(
            DecreaseCommand,
            (sender, _) => ((NumericBox)sender).Step(-1),
            (sender, e) => e.CanExecute = ((NumericBox)sender).CanStep(-1)));
    }

    /// <summary>The current number, always between <see cref="Minimum"/> and <see cref="Maximum"/>. Binds two-way by default.</summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>The smallest allowed <see cref="Value"/>.</summary>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>The largest allowed <see cref="Value"/>. Never less than <see cref="Minimum"/>.</summary>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>The step used by the spin buttons and arrow keys. Must be greater than zero.</summary>
    public double Increment
    {
        get => (double)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    /// <summary>The .NET numeric format used to display <see cref="Value"/>, for example <c>"N2"</c> or <c>"C"</c>.</summary>
    public string StringFormat
    {
        get => (string)GetValue(StringFormatProperty);
        set => SetValue(StringFormatProperty, value);
    }

    /// <summary>When <see langword="true"/>, the value can be read and selected but not changed by the user.</summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>Occurs when <see cref="Value"/> changes.</summary>
    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (_textBox is not null)
        {
            _textBox.LostKeyboardFocus -= OnTextBoxLostKeyboardFocus;
        }

        base.OnApplyTemplate();

        // A retemplated control may omit the text box; the spin buttons and keyboard stepping keep working.
        _textBox = GetTemplateChild(TextBoxPartName) as TextBox;
        if (_textBox is not null)
        {
            _textBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
        }

        UpdateText();
        UpdateVisualStates(false);
    }

    /// <summary>Raises <see cref="ValueChanged"/> and notifies UI Automation clients.</summary>
    protected virtual void OnValueChanged(double oldValue, double newValue)
    {
        if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged)
            && UIElementAutomationPeer.FromElement(this) is NumericBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldValue, newValue);
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new NumericBoxAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        // Preview, because the inner TextBox would otherwise consume these keys for caret movement.
        switch (e.Key)
        {
            case Key.Up:
                e.Handled = TryExecute(IncreaseCommand);
                break;
            case Key.Down:
                e.Handled = TryExecute(DecreaseCommand);
                break;
            case Key.Enter:
                CommitText();
                e.Handled = true;
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        UpdateVisualStates(true);
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        UpdateVisualStates(true);
    }

    /// <inheritdoc />
    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsKeyboardFocusWithinChanged(e);
        UpdateVisualStates(true);
    }

    /// <inheritdoc />
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);

        // Tabbing onto the control itself forwards focus to the editable part.
        if (ReferenceEquals(e.NewFocus, this) && _textBox is not null)
        {
            _textBox.Focus();
            _textBox.SelectAll();
            e.Handled = true;
        }
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var box = (NumericBox)d;
        box.UpdateText();
        CommandManager.InvalidateRequerySuggested();
        box.OnValueChanged((double)e.OldValue, (double)e.NewValue);
    }

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        var box = (NumericBox)d;
        return Math.Clamp((double)baseValue, box.Minimum, box.Maximum);
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(MaximumProperty);
        d.CoerceValue(ValueProperty);
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        d.CoerceValue(ValueProperty);

    private static object CoerceMaximum(DependencyObject d, object baseValue) =>
        Math.Max((double)baseValue, ((NumericBox)d).Minimum);

    private static bool IsFinite(object value) => double.IsFinite((double)value);

    private bool CanStep(int direction) =>
        IsEnabled && !IsReadOnly && (direction > 0 ? Value < Maximum : Value > Minimum);

    private void Step(int direction)
    {
        CommitText();
        SetCurrentValue(ValueProperty, Value + direction * Increment);
    }

    private bool TryExecute(RoutedCommand command)
    {
        if (!command.CanExecute(null, this))
            return false;

        command.Execute(null, this);
        _textBox?.SelectAll();
        return true;
    }

    private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => CommitText();

    private void CommitText()
    {
        if (_textBox is null || IsReadOnly)
            return;

        if (double.TryParse(_textBox.Text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out var parsed)
            && double.IsFinite(parsed))
        {
            SetCurrentValue(ValueProperty, parsed);
        }

        // Always reformat: the text may have been invalid, out of range, or differently formatted.
        UpdateText();
    }

    private void UpdateText()
    {
        if (_textBox is not null)
        {
            _textBox.Text = Value.ToString(StringFormat, CultureInfo.CurrentCulture);
        }
    }

    private void UpdateVisualStates(bool useTransitions)
    {
        var commonState = !IsEnabled ? DisabledState : IsMouseOver ? MouseOverState : NormalState;
        VisualStateManager.GoToState(this, commonState, useTransitions);
        VisualStateManager.GoToState(this, IsKeyboardFocusWithin ? FocusedState : UnfocusedState, useTransitions);
    }
}
