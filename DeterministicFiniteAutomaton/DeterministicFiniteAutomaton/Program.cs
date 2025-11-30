namespace DeterministicFiniteAutomaton
{
    class App
    {
        static DeterministicFiniteAutomaton RegexToDFA(string regex)
        {
            var postfix = RegexPostfixForm.PostFixForm(regex);
            var nfa = new NondeterministicFiniteAutomaton(postfix);
            return new DeterministicFiniteAutomaton(nfa);
        }

        static void Main(string[] args)
        {
            string inputFile = "regex.txt";
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("File regex.txt not found!");
                return;
            }

            string regex = File.ReadAllText(inputFile).Trim();
            string postfix = RegexPostfixForm.PostFixForm(regex);
            var dfa = RegexToDFA(regex);

            while (true)
            {
                Console.WriteLine("\nMeniu:");
                Console.WriteLine("1. Afiseaza forma postfixata");
                Console.WriteLine("2. Afiseaza automatul");
                Console.WriteLine("3. Verifica cuvinte");
                Console.WriteLine("0. Iesire");
                Console.Write("Optiune: ");
                string opt = Console.ReadLine();

                switch (opt)
                {
                    case "1":
                        Console.WriteLine("Postfix: " + postfix);
                        break;
                    case "2":
                        Console.WriteLine("Automat DFA:");
                        dfa.PrintAutomaton();
                        using (StreamWriter sw = new StreamWriter("dfa_output.txt"))
                        {
                            sw.WriteLine("States: " + string.Join(",", dfa.States));
                            sw.WriteLine("Alphabet: " + string.Join(",", dfa.Alphabet));
                            sw.WriteLine("StartingState: " + dfa.StartingState);
                            sw.WriteLine("FinalStates: " + string.Join(",", dfa.FinalStates));
                            sw.WriteLine("Transitions:");
                            foreach (var kvp in dfa.Transitions)
                            {
                                foreach (var t in kvp.Value)
                                {
                                    string sym = t.Symbol == null ? "lambda" : t.Symbol.ToString();
                                    sw.WriteLine($"p({kvp.Key}, {sym}) -> {t.ToState}");
                                }
                            }
                        }
                        Console.WriteLine("Automatul a fost scris in fisierul dfa_output.txt");
                        break;
                    case "3":
                        Console.Write("Introduceti cuvintele separate prin spatiu: ");
                        string[] words = Console.ReadLine().Split(' ');
                        foreach (string w in words)
                        {
                            bool ok = dfa.CheckWord(w);
                            Console.WriteLine($"{w}: {(ok ? "acceptat" : "respins")}");
                        }
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }
    }
}