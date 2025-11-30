using System.Text;

namespace DeterministicFiniteAutomaton
{
    class RegexPostfixForm
    {
        static bool IsOperator(char c)
        {
            return c == '*' || c == '.' || c == '|';
        }

        static bool IsOperand(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   (c >= '0' && c <= '9');
        }

        static int Priority(char op)
        {
            switch (op)
            {
                case '*': return 3;
                case '.': return 2;
                case '|': return 1;
            }
            return 0;
        }

        static string AddConcatOperators(string str)
        {
            StringBuilder expr = new StringBuilder();

            for (int i = 1; i < str.Length; i++)
            {
                expr.Append(str[i - 1]);

                if ((IsOperand(str[i - 1]) || str[i - 1] == ')' || str[i - 1] == '*') &&
                    (IsOperand(str[i]) || str[i] == '('))
                {
                    expr.Append('.');
                }
            }

            expr.Append(str[str.Length - 1]);
            return expr.ToString();
        }

        static public string PostFixForm(string regex)
        {
            string expression = AddConcatOperators(regex);
            Stack<char> operators = new Stack<char>();
            StringBuilder output = new StringBuilder();

            foreach (char c in expression)
            {
                if (IsOperator(c))
                {
                    while (operators.Count > 0 && operators.Peek() != '(' &&
                           Priority(operators.Peek()) >= Priority(c))
                    {
                        output.Append(operators.Pop());
                    }
                    operators.Push(c);
                }
                else if (c == '(')
                {
                    operators.Push(c);
                }
                else if (c == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        output.Append(operators.Pop());
                    }
                    if (operators.Count > 0) operators.Pop();
                }
                else
                {
                    output.Append(c);
                }
            }

            while (operators.Count > 0)
            {
                output.Append(operators.Pop());
            }

            return output.ToString();
        }
    }
}
