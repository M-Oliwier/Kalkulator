using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpCalculator;

public partial class MainWindow : Window
{
    private enum State { Number, Operator }
    private State _state = State.Number;
    private CultureInfo _culture = CultureInfo.GetCultureInfo("en-EN");
    private double firstNumber = 0;
    private double secondNumber = 0;
    private string operatorSymbol = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void UpdateEquationDisplay()
    {
        if (_state == State.Operator)
            EquationDisplay.Content = $"{firstNumber.ToString(_culture)} {operatorSymbol}";
        else if (!string.IsNullOrEmpty(operatorSymbol))
            EquationDisplay.Content = $"{firstNumber.ToString(_culture)} {operatorSymbol} {Display.Content}";
        else
            EquationDisplay.Content = string.Empty;
    }

    private void Number_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            if (_state == State.Operator || Display.Content.ToString() == "0")
            {
                Display.Content = btn.Content.ToString();
                _state = State.Number;
            }
            else
            {
                Display.Content += btn.Content.ToString();
            }
            UpdateEquationDisplay();
        }
    }

    private void Dot_Click(object sender, RoutedEventArgs e)
    {
        string text = Display.Content.ToString();
        if (!text.Contains(_culture.NumberFormat.NumberDecimalSeparator))
        {
            Display.Content = text + _culture.NumberFormat.NumberDecimalSeparator;
            _state = State.Number;
        }
        UpdateEquationDisplay();
    }

    private void Operator_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(Display.Content.ToString(), NumberStyles.Any, _culture, out double val))
        {
            firstNumber = val;
            _state = State.Operator;
            operatorSymbol = ((Button)sender).Content.ToString();
            UpdateEquationDisplay();
        }
    }

    private void Equals_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(Display.Content.ToString(), NumberStyles.Any, _culture, out double val))
        {
            secondNumber = val;
            double result;
            switch (operatorSymbol)
            {
                case "+": result = firstNumber + secondNumber; break;
                case "-": result = firstNumber - secondNumber; break;
                case "×": result = firstNumber * secondNumber; break;
                case "÷": result = secondNumber != 0 ? firstNumber / secondNumber : 0; break;
                default: result = val; break;
            }
            EquationDisplay.Content = $"{firstNumber.ToString(_culture)} {operatorSymbol} {secondNumber.ToString(_culture)} =";
            Display.Content = result.ToString(_culture);
            _state = State.Number;
        }
    }

    private void Percent_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(Display.Content.ToString(), NumberStyles.Any, _culture, out double val))
        {
            double pct = val / 100.0;
            if (firstNumber != 0 && !string.IsNullOrEmpty(operatorSymbol))
            {
                pct = firstNumber * pct;
                Display.Content = pct.ToString(_culture);
                UpdateEquationDisplay();
            }
            else
            {
                Display.Content = pct.ToString(_culture);
            }
            _state = State.Number;
        }
    }

    private void PlusMinus_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(Display.Content.ToString(), NumberStyles.Any, _culture, out double val))
        {
            val = -val;
            Display.Content = val.ToString(_culture);
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        firstNumber = 0;
        secondNumber = 0;
        operatorSymbol = "";
        Display.Content = "0";
        
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Button btn && btn.Background is SolidColorBrush brush)
        {
            btn.Tag = brush;
            var col = brush.Color;
            var lighter = Color.FromRgb(
                (byte)Math.Min(255, col.R + 30),
                (byte)Math.Min(255, col.G + 30),
                (byte)Math.Min(255, col.B + 30));
            btn.Background = new SolidColorBrush(lighter);
        }
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Button btn && btn.Tag is SolidColorBrush original)
            btn.Background = original;
    }
}
