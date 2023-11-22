using Avalonia.Controls;
using System.Collections.Generic;
using System;
using Avalonia.Interactivity;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Avalonia;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        //Data structures and algorithms
        //Calculator
        class Component
        {
            private string content;
            public string Content
            {
                get { return content; }
                set { content = value; }
            }
            private int priority;
            public int Priority
            {
                get { return priority; }
                set { priority = value; }
            }
            public Component(string content)
            {
                this.content = content;
                priority = content switch
                {
                    "(" or ")" => 5,
                    "!" => 4,
                    "^" or "√" => 3,
                    "*" or "/" or "%" => 2,
                    "+" or "-" => 1,
                    _ => 0,
                };
            }
            public bool Equals(Component other)
            {
                return content.Equals(other.content);
            }
        }
        static readonly List<Component> components = new List<Component>();
        static string error = string.Empty;
        static int FindIndex(List<Component> components, Component component)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (component.Equals((components[i])))
                    return i;
            }
            return -1;
        }
        static int FindIndex(List<Component> components, Component component, int startIndex)
        {
            for (int i = startIndex; i < components.Count; i++)
            {
                if (component.Equals((components[i])))
                    return i;
            }
            return -1;
        }
        static int FindLastIndex(List<Component> components, Component component)
        {
            int lastIndex = -1;
            for (int i = 0; i < components.Count; i++)
            {
                if (component.Equals((components[i])))
                    lastIndex = i;
            }
            return lastIndex;
        }
        static void Input(List<Component> components, string expression)
        {
            string temp = string.Empty;
            for (int i = 0; i < expression.Length; i++)
            {
                if (!char.IsDigit(expression[i]))
                {
                    components.Add(new Component(expression[i].ToString()));
                }
                else
                {
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == ','))
                    {
                        temp += expression[i];
                        i++;
                    }
                    components.Add(new Component(temp));
                    temp = string.Empty;
                    i--;
                }
            }
        }
        static int MaxPriority(List<Component> components)
        {
            int max = 0;
            foreach (var component in components)
            {
                if (component.Priority > max)
                {
                    max = component.Priority;
                }
            }
            return max;
        }
        static void ApplyFactirial(List<Component> components)
        {
            int index = FindIndex(components, new Component("!"));
            if (int.TryParse(components[index - 1].Content, out int n))
            {
                double result = 1;
                for (int i = 1; i <= n; i++)
                {
                    result *= i;
                }
                components.RemoveAt(index - 1);
                components.RemoveAt(index - 1);
                components.Insert(index - 1, new Component(result.ToString()));
            }
            else
                error = "Error";
        }
        static void ApplySquareRoot(List<Component> components)
        {
            int index = FindLastIndex(components, new Component("√"));
            if (index >= 0)
            {
                if (double.TryParse(components[index + 1].Content, out double n))
                {
                    if (n >= 0)
                    {
                        double result = Math.Sqrt(n);
                        components.RemoveAt(index);
                        components.RemoveAt(index);
                        components.Insert(index, new Component(result.ToString()));
                    }
                    else
                        error = "Error";
                }
                else
                    error = "Error";
            }
        }
        static void ApplyExponentiation(List<Component> components)
        {
            int index = FindIndex(components, new Component("^"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    double result = Math.Pow(a, b);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.Insert(index - 1, new Component(result.ToString()));
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void ApplyMultiplication(List<Component> components)
        {
            int index = FindIndex(components, new Component("*"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    double result = a * b;
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.Insert(index - 1, new Component(result.ToString()));
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void ApplyDivision(List<Component> components)
        {
            int index = FindIndex(components, new Component("/"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    if (b == 0)
                    {
                        error = "Divided by 0";
                        return;
                    }
                    double result = a / b;
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.Insert(index - 1, new Component(result.ToString()));
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void ApplyModulo(List<Component> components)
        {
            int index = FindIndex(components, new Component("%"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    if (a >= b)
                    {
                        double result = a % b;
                        components.RemoveAt(index - 1);
                        components.RemoveAt(index - 1);
                        components.RemoveAt(index - 1);
                        components.Insert(index - 1, new Component(result.ToString()));
                    }
                    else
                    {
                        error = "Error";
                        return;
                    }
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void ApplyAddition(List<Component> components)
        {
            int index = FindIndex(components, new Component("+"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    double result = a + b;
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.Insert(index - 1, new Component(result.ToString()));
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void ApplySubtraction(List<Component> components)
        {
            int index = FindIndex(components, new Component("-"));
            if (index >= 0)
            {
                if (double.TryParse(components[index - 1].Content, out double a) && double.TryParse(components[index + 1].Content, out double b))
                {
                    double result = a - b;
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.RemoveAt(index - 1);
                    components.Insert(index - 1, new Component(result.ToString()));
                }
                else
                {
                    error = "Error";
                    return;
                }
            }
        }
        static void Evaluate(List<Component> components)
        {
            int i;
            while (components.Count > 1 && string.IsNullOrEmpty(error))
            {
                int maxPriority = MaxPriority(components);
                switch (maxPriority)
                {
                    case 4:
                        ApplyFactirial(components);
                        break;
                    case 3:
                        for (i = 0;i<components.Count;i++)
                        {
                            if (components[i].Priority == maxPriority)
                                break;
                        }
                        if(components[i].Content== "√")
                            ApplySquareRoot(components);
                        else
                        ApplyExponentiation(components);
                        break;
                    case 2:
                        for (i = 0; i < components.Count; i++)
                        {
                            if (components[i].Priority == maxPriority)
                                break;
                        }
                        if (components[i].Content == "*")
                            ApplyMultiplication(components);
                        else if (components[i].Content == "/")
                            ApplyDivision(components);
                        else
                            ApplyModulo(components);
                        break;
                    case 1:
                        for (i = 0; i < components.Count; i++)
                        {
                            if (components[i].Priority == maxPriority)
                                break;
                        }
                        if (components[i].Content == "+")
                            ApplyAddition(components);
                        ApplySubtraction(components);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(error))
            {
                return;
            }
        }
        static void RemoveParenthesis(List<Component> components)
        {
            int openParenthesisIndex, closeParenthesisIndex;
            openParenthesisIndex = FindLastIndex(components, new Component("("));
            if (openParenthesisIndex != -1)
            {
                closeParenthesisIndex = FindIndex(components, new Component(")"), openParenthesisIndex);
                if (closeParenthesisIndex == -1)
                {
                    error = "Missing parenthesis";
                    return;
                }
                else
                {
                    int count = 0;
                    List<Component> newComponents = new List<Component>();
                    for (int i = openParenthesisIndex + 1; i < closeParenthesisIndex; i++)
                    {
                        newComponents.Add(components[i]);
                        count++;
                    }
                    Evaluate(newComponents);
                    for (int i = 0; i < count + 2; i++)
                    {
                        components.RemoveAt(openParenthesisIndex);
                    }
                    components.Insert(openParenthesisIndex, newComponents[0]);
                }

            }
            else
                error = "Missing parenthesis";
        }
        static void Execute(List<Component> components)
        {
            while (components.Count > 1 && string.IsNullOrEmpty(error))
            {
                int maxPriority = MaxPriority(components);
                switch (maxPriority)
                {
                    case 5:
                        RemoveParenthesis(components);
                        break;
                    default:
                        Evaluate(components);
                        break;
                }
            }
        }
        //Converter
        static int SymbolToValue(char digit)
        {
            switch (digit)
            {
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                default:
                    return 0;
            }
        }
        static char ValueToSymbol(int value)
        {
            switch(value)
            {
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'C';
                case 13:
                    return 'D';
                case 14:
                    return 'E';
                case 15:
                    return 'F';
                default:
                    return '0';
            }
        }
        static int GetBaseNumber(string basenumbersystem)
        {
            switch(basenumbersystem)
            {
                case "Binary":
                    return 2;
                case "Octal":
                    return 8;
                case "Hexadecimal":
                    return 16;
                default:
                    return 10;
            }
        }
        static string ConvertToDecimal(string number, string numeralSystem)
        {
            double result = 0;
            int baseNumber = GetBaseNumber(numeralSystem);
            for (int i = 0; i < number.Length; i++)
            {
                result += SymbolToValue(number[number.Length - i - 1]) * Math.Pow(baseNumber, i);
            }
            return result.ToString();
        }
        static string ConvertFromDecimal(string number, string numeralSystem)
        {
            int baseNumber = GetBaseNumber(numeralSystem), temp = int.Parse(number);
            List<char> remainers = new List<char>();
            if (temp == 0) return "0";
            while (temp != 0)
            {
                remainers.Add(ValueToSymbol(temp % baseNumber));
                temp /= baseNumber;
            }
            string result = string.Empty;
            for (int i = 0; i < remainers.Count; i++)
            {
                result += remainers[remainers.Count - i - 1];
            }
            return result;
        }
        static string Convert(string number, string inputBaseSystem, string outputBaseSystem)
        {
            int basenumber = GetBaseNumber(inputBaseSystem);
            foreach (char item in number)
            {
                if (SymbolToValue(item) >= basenumber)
                    return "Invalid Input";
            }
            if (inputBaseSystem == outputBaseSystem)
                return number;
            else if (inputBaseSystem == "Decimal")
            {
                return ConvertFromDecimal(number, outputBaseSystem);
            }
            else if (outputBaseSystem == "Decimal")
            {
                return ConvertToDecimal(number, inputBaseSystem);
            }
            else
            {
                return ConvertFromDecimal(ConvertToDecimal(number, inputBaseSystem), outputBaseSystem);
            }
        }
        //Inpector
        static List<int> divisors = new List<int>();
        static void GetDivisors(int number)
        {
            divisors.Clear();
            divisors.Add(1);
            for(int i =2;i<number;i++)
            {
                if(number%i==0)
                { 
                    divisors.Add(i);
                }
            }
            divisors.Add(number);
        }
        static string IsPrimeOrComposite()
        {
            if (divisors.Count == 2)
                return "prime";
            return "composite";
        }
        static bool IsSquare(int number)
        {
            int s = (int)Math.Sqrt(number);
            return (s * s == number);
        }
        static bool IsCube(int number)
        {
            int c = (int)Math.Cbrt(number);
            return (c * c * c == number);
        }
        static bool IsPerfect(int number)
        {
            int sum = 0;
            for (int i = 0; i < divisors.Count - 1; i++)
            {
                sum += divisors[i];
            }
            if (sum == number)
                return true;
            return false;
        }
        static bool IsTriangular(int number)
        {
            int sum = 0;
            for (int i = 1; sum < number; i++)
            {
                sum += i;
            }
            if(sum == number)
                return true;
            return false;
        }
        static bool IsFibonacci(int n)
        {
            return IsSquare(5 * n * n + 4) || IsSquare(5 * n * n - 4);
        }
        static string IsAbundantOrDeficient(int number)
        {
            if (IsPrimeOrComposite() == "prime")
                return "deficient";
            else
            {
                int sum = 0;
                for (int i = 0; i < divisors.Count - 1; i++)
                {
                    sum += divisors[i];
                }
                if (sum < number)
                    return "deficient";
                return "abundant";
            }
        }
        static string Inpect(int number)
        {
            string result = string.Empty;
            GetDivisors(number);
            result += ("- " +number+ " is " + IsPrimeOrComposite() + ".\n");
            result += ("- Divisors: ");
            for (int i = 0;i< divisors.Count - 1;i++)
            {
                result += (divisors[i].ToString() + ", ");
            }
            result += (divisors[divisors.Count - 1].ToString()+".\n");
            if (IsSquare(number))
            {
                result += ("- " +number+ " is square.\n");
            }
            if (IsCube(number))
            {
                result +=( "- " +number+ " is cube.\n");
            }
            if (IsPerfect(number))
            {
                result += ("- " +number+ " is perfect.\n");
            }
            if (IsTriangular(number))
            {
                result += ("- " +number+ " is triangular.\n");
            }
            if (IsFibonacci(number))
            {
                result += ("- " +number+ " is fibonacci.\n");
            }
            result += ("- " +number+ " is " + IsAbundantOrDeficient(number) + ".\n");
            return result;
        }
        //Initalize Window
        public MainWindow()
        {
            InitializeComponent();
        }
        //Events Handlers
        //Calculator
        bool isDecimal = false;
        private void AllClear_Click(object sender, RoutedEventArgs e)
        {
            ExpressionBox?.Clear();
            ResultBox?.Clear();
        }
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content.Equals(","))
            {
                if (!string.IsNullOrEmpty(ExpressionBox.Text))
                {
                    if (isDecimal == false && char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]))
                    {
                        ExpressionBox.Text += button.Content;
                        isDecimal = true;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ExpressionBox.Text))
                {
                    ExpressionBox.Text += button.Content;
                    isDecimal = false;
                }
                else if (ExpressionBox.Text[ExpressionBox.Text.Length - 1] != ')' && ExpressionBox.Text[ExpressionBox.Text.Length - 1] != '!')
                {
                    if (!char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]) && ExpressionBox.Text[ExpressionBox.Text.Length - 1]!=',')
                        isDecimal = false;
                    ExpressionBox.Text += button.Content;
                }
            }
        }
        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Content)
            {
                case "!":
                    if (!string.IsNullOrEmpty(ExpressionBox.Text))
                    {
                        if (char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]) || ExpressionBox.Text[ExpressionBox.Text.Length - 1] == ')')
                        {
                            ExpressionBox.Text += button.Content;
                        }
                    }
                    break;
                case "√":
                    if (string.IsNullOrEmpty(ExpressionBox.Text))
                    {
                        ExpressionBox.Text += button.Content;
                    }
                    else if (ExpressionBox.Text[ExpressionBox.Text.Length - 1] != ')' && ExpressionBox.Text[ExpressionBox.Text.Length - 1] != '!' && ExpressionBox.Text[ExpressionBox.Text.Length - 1] != '.' && !char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]))
                    {
                        ExpressionBox.Text += button.Content;
                    }
                    break;
                case "^":
                case "*":
                case "/":
                case "%":
                case "+":
                case "-":
                    if (!string.IsNullOrEmpty(ExpressionBox.Text))
                    {
                        if (char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]) || ExpressionBox.Text[ExpressionBox.Text.Length - 1] == ')' || ExpressionBox.Text[ExpressionBox.Text.Length - 1] == '!')
                        {
                            ExpressionBox.Text += button.Content;
                        }
                    }
                    break;
            }
        }
        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content.Equals("("))
            {
                if (string.IsNullOrEmpty(ExpressionBox.Text))
                {
                    ExpressionBox.Text += button.Content;
                }
                else if (!char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]))
                {
                    ExpressionBox.Text += button.Content;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ExpressionBox.Text))
                {
                    if (char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]))
                    {
                        ExpressionBox.Text += button.Content;
                    }
                }
            }
        }
        private void BackSpaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ExpressionBox.Text))
                ExpressionBox.Text = ExpressionBox.Text.Remove(ExpressionBox.Text.Length - 1);
        }
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ExpressionBox.Text))
            {
                error = string.Empty;
                components.Clear();
                Input(components, ExpressionBox.Text);
                if (components[components.Count-1].Priority==0|| components[components.Count - 1].Content==")"|| components[components.Count - 1].Content=="!") 
                {
                    Execute(components);
                    if (string.IsNullOrEmpty(error))
                    {
                        ResultBox.Text = components[0].Content;
                    }
                    else
                    {
                        ResultBox.Text = error;
                    }
                    return;
                }
                ResultBox.Text = "Missing Operand";
            }
        }
        //Converter
        private void NumberButton_Click2(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int baseNumber = GetBaseNumber((InputBaseSystemBox.SelectedItem as ComboBoxItem).Content.ToString());
            if (SymbolToValue(button.Content.ToString()[0]) < baseNumber)
            {
                if (InputNumberBox.Text == "0")
                {
                    InputNumberBox.Text = button.Content.ToString();
                }
                else
                {
                    InputNumberBox.Text += button.Content.ToString();
                }
            }
        }
        private void ClearAndEraseButton_Click(object sender, RoutedEventArgs e)
        {
            InputNumberBox?.Clear();
            OutputNumberBox?.Clear();
        }
        private void BackSpaceButton_Click2(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputNumberBox.Text))
                InputNumberBox.Text = InputNumberBox.Text.Remove(InputNumberBox.Text.Length - 1);
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputNumberBox.Text))
            {
                OutputNumberBox.Text = Convert(InputNumberBox.Text, (InputBaseSystemBox.SelectedItem as ComboBoxItem).Content.ToString(), (OutputBaseSystemBox.SelectedItem as ComboBoxItem).Content.ToString());
            }
        }
        private void NumberButton_Click3(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (string.IsNullOrEmpty(NumberBox.Text))
            {
                if (button.Content != "0")
                    NumberBox.Text += button.Content;
            }
            else
            {
                NumberBox.Text += button.Content;
            }
        }
        private void BackSpaceButton_Click3(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NumberBox.Text))
                NumberBox.Text = NumberBox.Text.Remove(NumberBox.Text.Length - 1);
        }
        private void InspectButton_Click(object sender, RoutedEventArgs e)
        {
            InformationBox.Clear();
            if(!string.IsNullOrEmpty(NumberBox.Text))
                InformationBox.Text = Inpect(int.Parse(NumberBox.Text));
        }
    }
}