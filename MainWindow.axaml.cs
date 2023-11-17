using Avalonia.Controls;
using System.Collections.Generic;
using System;
using Avalonia.Interactivity;

namespace Calculator
{
    public partial class MainWindow : Window
    {
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
        static bool error = false;
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
        //get expression, seperate components and know their priorities
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
            //create the "!" component to search its index in the expression
            int index = FindIndex(components, new Component("!"));
            //check if the previous compent is number
            if (int.TryParse(components[index - 1].Content, out int n))
            {
                double result = 1;
                //calculate the result of factorial operation
                for (int i = 1; i <= n; i++)
                {
                    result *= i;
                }
                //remove the old number and the factorial sign then inssert the result of this operation
                components.RemoveAt(index - 1);
                components.RemoveAt(index - 1);
                components.Insert(index - 1, new Component(result.ToString()));
            }
            else
                error = true;
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
                        error = true;
                }
                else
                    error = true;
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
                    error = true;
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
                    error = true;
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
                        error = true;
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
                    error = true;
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
                        error = true;
                        return;
                    }
                }
                else
                {
                    error = true;
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
                    error = true;
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
                    error = true;
                    return;
                }
            }
        }
        static void Evaluate(List<Component> components)
        {
            int i;
            while (components.Count > 1 && !error)
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
            if (error)
            {
                return;
            }
        }
        static void RemoveParenthesis(List<Component> components)
        {
            //find the indexes of two parenthesis
            int openParenthesisIndex, closeParenthesisIndex;
            openParenthesisIndex = FindLastIndex(components, new Component("("));
            if (openParenthesisIndex != -1)
            {
                closeParenthesisIndex = FindIndex(components, new Component(")"), openParenthesisIndex);
                //if one of them is missing
                if (closeParenthesisIndex == -1)
                {
                    error = true;
                    return;
                }
                else
                {
                    //get subexpression in the parenthesises and count the components
                    int count = 0;
                    List<Component> newComponents = new List<Component>();
                    for (int i = openParenthesisIndex + 1; i < closeParenthesisIndex; i++)
                    {
                        newComponents.Add(components[i]);
                        count++;
                    }
                    //evaluate it
                    Evaluate(newComponents);
                    //remove the subexpression and two parenthesis
                    for (int i = 0; i < count + 2; i++)
                    {
                        components.RemoveAt(openParenthesisIndex);
                    }
                    //push the result back to the main expression
                    components.Insert(openParenthesisIndex, newComponents[0]);
                }

            }
            else
                error = true;
        }
        static void Execute(List<Component> components)
        {
            while (components.Count > 1 && !error)
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
            if (error)
            {
                Console.WriteLine("Error");
                return;
            }
            if (components.Count == 1)
            {
                Console.WriteLine(components[0].Content);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        bool isDecimal = false;
        private void AllClear_Click(object sender, RoutedEventArgs e)
        {
            ExpressionBox.Text = string.Empty;
            ResultBox.Text = string.Empty;
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
                    if (!char.IsDigit(ExpressionBox.Text[ExpressionBox.Text.Length - 1]))
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
                error = false;
                components.Clear();
                Input(components, ExpressionBox.Text);
                Execute(components);
                if (!error)
                {
                    ResultBox.Text = components[0].Content;
                }
                else
                {
                    ResultBox.Text = "Error";
                }
            }
        }
    }
}