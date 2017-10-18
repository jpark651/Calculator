using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public enum Operat
    {
        add = 1,
        sub,
        div,
        mul,
        mod,
        exp,
        lparenth
    }

    public enum OriginMethod
    {
        Factorial = 1,
        Negate,
        NatLog,
        Log10
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private const double _pi = 3.14159265358979;

        private double[] _currentDisplay = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private double[] _storeDisplay = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private Operat[] _operat = new Operat[10];
        private double _memory = 0;
        private int _parenth = 0;
        private int _afterDecimal = 0;
        private bool _pauseDisplay = false;
        private bool _pauseOperat = false;

        private int CheckLength(double display)
        {
            int digits = 0;
            string displayString = display.ToString();
            int displayLength = displayString.Length;
            for (int i = 0; i < displayLength; i++)
            {
                if (displayString[i] != '-' && displayString[i] != '.')
                {
                    digits++;
                }
            }
            return digits;
        }

        private void NewDisplay(bool parenthStart = false, bool parenthEnd = false)
        {
            if (parenthStart == false && (parenthEnd == false || _operat[_parenth] != 0))
            {
                switch (_operat[_parenth])
                {
                    case Operat.add:
                        _currentDisplay[_parenth] = _storeDisplay[_parenth] + _currentDisplay[_parenth];
                        break;
                    case Operat.sub:
                        _currentDisplay[_parenth] = _storeDisplay[_parenth] - _currentDisplay[_parenth];
                        break;
                    case Operat.div:
                        _currentDisplay[_parenth] = _storeDisplay[_parenth] / _currentDisplay[_parenth];
                        break;
                    case Operat.mul:
                        _currentDisplay[_parenth] = _storeDisplay[_parenth] * _currentDisplay[_parenth];
                        break;
                    case Operat.mod:
                        _currentDisplay[_parenth] = _storeDisplay[_parenth] % _currentDisplay[_parenth];
                        break;
                    case Operat.exp:
                        _currentDisplay[_parenth] = Math.Pow(_storeDisplay[_parenth], _currentDisplay[_parenth]);
                        break;
                }
                _storeDisplay[_parenth] = _currentDisplay[_parenth];
            }
            if (Double.IsNaN(_storeDisplay[_parenth]))
            {
                if (_operat[_parenth] == Operat.div)
                {
                    displayBox.Text = "Result of function is undefined.";
                }
                else
                {
                    displayBox.Text = "Invalid input for function.";
                }
                _storeDisplay[_parenth] = 0;
            }
            else if (Double.IsInfinity(_storeDisplay[_parenth]) && _operat[_parenth] == Operat.div)
            {
                displayBox.Text = "Cannot divide by zero.";
                _storeDisplay[_parenth] = 0;
            }
            else if (Double.IsInfinity(_storeDisplay[_parenth]))
            {
                displayBox.Text = "Values are too large.";
                _storeDisplay[_parenth] = 0;
            }
            else
            {
                displayBox.Text = _storeDisplay[_parenth].ToString();
            }
            _currentDisplay[_parenth] = 0;
            _afterDecimal = 0;
        }

        private void AppendDigit(double number)
        {
            if (_pauseDisplay)
            {
                _currentDisplay[_parenth] = 0;
                _afterDecimal = 0;
                _pauseDisplay = false;
            }
            int displayLength = CheckLength(_currentDisplay[_parenth]);
            int decimalLength = CheckLength(Math.Truncate(_currentDisplay[_parenth]));
            decimalLength = displayLength - decimalLength;
            if (displayLength < 15 && displayLength + (_afterDecimal - decimalLength) < 16)
            {
                if (_afterDecimal == 0)
                {
                    if (_currentDisplay[_parenth] >= 0)
                    {
                        _currentDisplay[_parenth] = (_currentDisplay[_parenth] * 10) + number;
                    }
                    else
                    {
                        _currentDisplay[_parenth] = (_currentDisplay[_parenth] * 10) - number;
                    }
                    displayBox.Text = _currentDisplay[_parenth].ToString();
                }
                else
                {
                    if (_currentDisplay[_parenth] >= 0)
                    {
                        _currentDisplay[_parenth] += number / Math.Pow(10, _afterDecimal);
                    }
                    else
                    {
                        _currentDisplay[_parenth] -= number / Math.Pow(10, _afterDecimal);
                    }
                    displayBox.Text = _currentDisplay[_parenth].ToString("N" + _afterDecimal).Replace(",", "");
                    _afterDecimal++;
                }
                _pauseOperat = false;
            }
        }

        private void PreserveDisplayValue()
        {
            if (displayBox.Text != _currentDisplay[_parenth].ToString() && _afterDecimal == 0)
            {
                _currentDisplay[_parenth] = _storeDisplay[_parenth];
            }
        }

        private void ClearAllValues()
        {
            _parenth = 0;
            _afterDecimal = 0;
            _pauseDisplay = false;
            for (int i = 0; i < 10; i++)
            {
                _currentDisplay[i] = 0;
                _storeDisplay[i] = 0;
                _operat[i] = 0;
            }
            displayBox.Text = "0";
            parenthBox.Text = "";
        }

        private void PerformOperat(Operat operat)
        {
            if (_pauseOperat == false)
            {
                PreserveDisplayValue();
                NewDisplay();
            }
            _pauseOperat = true;
            _operat[_parenth] = operat;
        }

        private void CheckMemory()
        {
            if (_memory != 0)
            {
                memoryBox.Text = "M";
            }
            else
            {
                memoryBox.Text = "";
            }
        }

        private void InvalidInput()
        {
            displayBox.Text = "Invalid input for function.";
            _storeDisplay[_parenth] = 0;
            _currentDisplay[_parenth] = 0;
            _afterDecimal = 0;
        }

        private void StoreDisplayUsed (OriginMethod originMethod)
        {
            double parsedResult;
            bool parseTest = Double.TryParse(displayBox.Text, out parsedResult);
            bool storeDisplayUsed = false;
            bool inputError = false;
            if (parseTest == false)
            {
                parsedResult = _currentDisplay[_parenth];
            }
            else if (_currentDisplay[_parenth].ToString() != parsedResult.ToString())
            {
                parsedResult = _storeDisplay[_parenth];
                storeDisplayUsed = true;
            }
            switch (originMethod)
            {
                case OriginMethod.Factorial:
                    if (parsedResult % 1 == 0)
                    {
                        if (parsedResult > 16)
                        {
                            displayBox.Text = "Cannot be greater than 16.";
                            _storeDisplay[_parenth] = 0;
                            _currentDisplay[_parenth] = 0;
                            _afterDecimal = 0;
                            inputError = true;
                        }
                        else if (parsedResult < 1)
                        {
                            displayBox.Text = "Cannot be less than 1.";
                            _storeDisplay[_parenth] = 0;
                            _currentDisplay[_parenth] = 0;
                            _afterDecimal = 0;
                            inputError = true;
                        }
                        else
                        {
                            int modAnswer = 1;
                            for (int i = (int)parsedResult; i > 0; i--)
                            {
                                modAnswer *= i;
                            }
                            parsedResult = modAnswer;
                            _pauseDisplay = true;
                        }
                    }
                    else
                    {
                        InvalidInput();
                        inputError = true;
                    }
                    break;
                case OriginMethod.Negate:
                    parsedResult *= -1;
                    break;
                case OriginMethod.NatLog:
                    if (parsedResult > 0)
                    {
                        parsedResult = Math.Log(parsedResult);
                        _pauseDisplay = true;
                    }
                    else
                    {
                        InvalidInput();
                        inputError = true;
                    }
                    break;
                case OriginMethod.Log10:
                    if (parsedResult > 0)
                    {
                    parsedResult = Math.Log10(parsedResult);
                    _pauseDisplay = true;
                    }
                    else
                    {
                        InvalidInput();
                        inputError = true;
                    }
                    break;
            }
            if (storeDisplayUsed == true)
            {
                _storeDisplay[_parenth] = parsedResult;
            }
            else
            {
                _currentDisplay[_parenth] = parsedResult;
            }
            if (inputError == false)
            {
                displayBox.Text = parsedResult.ToString();
            }
        }

        private int PasteDisable()
        {
            menuPaste.IsEnabled = false;
            int afterDecimal = 0;
            double parsedResult;
            if (Double.TryParse(Clipboard.GetText(), out parsedResult))
            {
                int displayLength = CheckLength(parsedResult);
                int decimalLength = CheckLength(Math.Truncate(parsedResult));
                decimalLength = displayLength - decimalLength;
                if (decimalLength > 0)
                {
                    afterDecimal = decimalLength + 1;
                }
                if (displayLength < 16 && displayLength + (_afterDecimal - decimalLength) < 17)
                {
                    menuPaste.IsEnabled = true;
                }
            }
            return afterDecimal;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(1);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(2);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(3);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(4);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(5);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(6);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(7);
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(8);
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(9);
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            AppendDigit(0);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.add);
        }

        private void buttonSubtract_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.sub);
        }

        private void buttonDivide_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.div);
        }

        private void buttonMultiply_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.mul);
        }

        private void buttonEquals_Click(object sender, RoutedEventArgs e)
        {
            PreserveDisplayValue();
            NewDisplay();
            _operat[_parenth] = 0;
        }

        private void buttonModulus_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.mod);
        }

        private void buttonMemoryClear_Click(object sender, RoutedEventArgs e)
        {
            _memory = 0;
            memoryBox.Text = "";
        }

        private void buttonMemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            if (_memory != 0)
            {
                _currentDisplay[_parenth] = _memory;
                displayBox.Text = _memory.ToString();
                _pauseDisplay = true;
            }     
        }

        private void buttonMemorySet_Click(object sender, RoutedEventArgs e)
        {
            double parsedResult = 0;
            if (Double.TryParse(displayBox.Text, out parsedResult))
            {
                _memory = parsedResult;
            }
            else
            {
                _memory = _currentDisplay[_parenth];
            }
            CheckMemory();
            _pauseDisplay = true;
        }

        private void buttonMemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            double parsedResult = 0;
            if (Double.TryParse(displayBox.Text, out parsedResult))
            {
                _memory += parsedResult;
            }
            else
            {
                _memory += _currentDisplay[_parenth];
            }
            CheckMemory();
            _pauseDisplay = true;
        }

        private void buttonPi_Click(object sender, RoutedEventArgs e)
        {
            _currentDisplay[_parenth] = _pi;
            displayBox.Text = _pi.ToString();
            _afterDecimal = 15;
            _pauseDisplay = true;
        }

        private void buttonFactorial_Click(object sender, RoutedEventArgs e)
        {
            StoreDisplayUsed(OriginMethod.Factorial);
        }

        private void buttonExponent_Click(object sender, RoutedEventArgs e)
        {
            PerformOperat(Operat.exp);
        }

        private void buttonParenthStart_Click(object sender, RoutedEventArgs e)
        {
            if (_parenth < 9)
            {
                NewDisplay(true, false);
                _parenth++;
                _operat[_parenth] = Operat.lparenth;
                displayBox.Text = "";
                for (int i = 0; i < _parenth; i++)
                {
                    displayBox.Text += "(";
                }
            }
            parenthBox.Text = "(=" + _parenth;
        }

        private void buttonParenthEnd_Click(object sender, RoutedEventArgs e)
        {
            if (_parenth > 0)
            {
                NewDisplay(false, true);
                _currentDisplay[_parenth - 1] = _storeDisplay[_parenth];
                _currentDisplay[_parenth] = 0;
                _storeDisplay[_parenth] = 0;
                _operat[_parenth] = 0;
                _parenth--;
            }
            if (_parenth > 0)
            {
                parenthBox.Text = "(=" + _parenth;
            }
            else
            {
                parenthBox.Text = "";
            }
        }

        private void buttonNegate_Click(object sender, RoutedEventArgs e)
        {
            StoreDisplayUsed(OriginMethod.Negate);
        }

        private void buttonDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (_pauseDisplay)
            {
                _currentDisplay[_parenth] = 0;
                _afterDecimal = 0;
                _pauseDisplay = false;
            }
            if (_afterDecimal == 0)
            {
                if (displayBox.Text != _currentDisplay[_parenth].ToString())
                {
                    NewDisplay();
                    displayBox.Text = "0";
                }
                displayBox.Text += ".";
                _afterDecimal = 1;
            }
        }

        private void buttonBackspace_Click(object sender, RoutedEventArgs e)
        {
            double displayValue = 0;
            Double.TryParse(displayBox.Text, out displayValue);
            if (_pauseDisplay == false && _currentDisplay[_parenth].ToString() == displayValue.ToString())
            {
                if (displayBox.Text.Length > 2 || (displayBox.Text.Length > 1 && displayBox.Text[0] != '-'))
                {
                    double parsedResult;
                    string displayParse = displayBox.Text.Substring(0, displayBox.Text.Length - 1);
                    if (Double.TryParse(displayParse, out parsedResult))
                    {
                        displayBox.Text = displayParse;
                        _currentDisplay[_parenth] = parsedResult;
                    }
                    if (_afterDecimal > 0)
                    {
                        _afterDecimal--;
                    }
                }
                else
                {
                    _currentDisplay[_parenth] = 0;
                    displayBox.Text = "0";
                }
            }
        }

        private void buttonClearEntry_Click(object sender, RoutedEventArgs e)
        {
            _currentDisplay[_parenth] = 0;
            _afterDecimal = 0;
            displayBox.Text = "0";
        }

        private void buttonClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAllValues();
        }

        private void buttonNatLog_Click(object sender, RoutedEventArgs e)
        {
            StoreDisplayUsed(OriginMethod.NatLog);
        }

        private void buttonLog10_Click(object sender, RoutedEventArgs e)
        {
            StoreDisplayUsed(OriginMethod.Log10);
        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(displayBox.Text);
            PasteDisable();
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            int afterDecimal = PasteDisable();
            if (menuPaste.IsEnabled == true)
            {
                double parsedResult;
                Double.TryParse(Clipboard.GetText(), out parsedResult);
                _currentDisplay[_parenth] = parsedResult;
                displayBox.Text = _currentDisplay[_parenth].ToString();
                _afterDecimal = afterDecimal;
                _pauseOperat = false;
            }
        }

        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            PasteDisable();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.D1 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad1)
            {
                button1_Click(button1, e);
            }
            else if ((e.Key == Key.D2 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad2)
            {
                button2_Click(button2, e);
            }
            else if ((e.Key == Key.D3 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad3)
            {
                button3_Click(button3, e);
            }
            else if ((e.Key == Key.D4 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad4)
            {
                button4_Click(button4, e);
            }
            else if ((e.Key == Key.D5 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad5)
            {
                button5_Click(button5, e);
            }
            else if ((e.Key == Key.D6 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad6)
            {
                button6_Click(button6, e);
            }
            else if ((e.Key == Key.D7 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad7)
            {
                button7_Click(button7, e);
            }
            else if ((e.Key == Key.D8 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad8)
            {
                button8_Click(button8, e);
            }
            else if ((e.Key == Key.D9 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad9)
            {
                button9_Click(button9, e);
            }
            else if ((e.Key == Key.D0 && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.NumPad0)
            {
                button0_Click(button10, e);
            }
            else if ((e.Key == Key.OemPlus && Keyboard.Modifiers == ModifierKeys.Shift) || e.Key == Key.Add)
            {
                buttonAdd_Click(button11, e);
            }
            else if ((e.Key == Key.OemMinus && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.Subtract)
            {
                buttonSubtract_Click(button12, e);
            }
            else if ((e.Key == Key.OemQuestion && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.Divide)
            {
                buttonDivide_Click(button13, e);
            }
            else if (e.Key == Key.D8 || e.Key == Key.Multiply)
            {
                buttonMultiply_Click(button14, e);
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Enter)
            {
                buttonEquals_Click(button15, e);
            }
            else if (e.Key == Key.D9)
            {
                buttonParenthStart_Click(button24, e);
            }
            else if (e.Key == Key.D0)
            {
                buttonParenthEnd_Click(button25, e);
            }
            else if (e.Key == Key.Back)
            {
                buttonBackspace_Click(button27, e);
            }
            else if ((e.Key == Key.OemPeriod && Keyboard.Modifiers != ModifierKeys.Shift) || e.Key == Key.Decimal)
            {
                buttonDecimal_Click(button28, e);
            }
            else if (e.Key == Key.N)
            {
                buttonNatLog_Click(button31, e);
            }
            else if (e.Key == Key.L)
            {
                buttonLog10_Click(button32, e);
            }
            else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                menuCopy_Click(menuCopy, e);
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                menuPaste_Click(menuPaste, e);
            }
        }
    }
}