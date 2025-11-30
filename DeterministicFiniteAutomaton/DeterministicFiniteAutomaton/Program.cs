namespace DeterministicFiniteAutomaton
{
    class App
    {
        static DeterministicFiniteAutomaton RegexToDFA(string regex)
        {
            var postfix = RegexPostfixForm.PostFixForm(regex);
            return new DeterministicFiniteAutomaton();
        }

        static void Main(string[] args)
        {
            var dfa = RegexToDFA("a(b|c)*d|e*");
        }
    }
}