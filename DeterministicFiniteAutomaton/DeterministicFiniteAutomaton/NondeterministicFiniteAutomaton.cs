namespace DeterministicFiniteAutomaton
{
    public record Transition(char? Symbol, int ToState);

    class NondeterministicFiniteAutomaton
    {
        public List<int> States { get; set; }
        public List<char> Alphabet { get; set; }
        public Dictionary<int, List<Transition>> Transitions { get; set; }
        public int StartingState { get; set; }
        public List<int> FinalStates { get; set; }

        public NondeterministicFiniteAutomaton()
        {
            States = new List<int>();
            Alphabet = new List<char>();
            Transitions = new Dictionary<int, List<Transition>>();
            StartingState = 0;
            FinalStates = new List<int>();
        }

        public NondeterministicFiniteAutomaton(string postfix)
        {
            Stack<NondeterministicFiniteAutomaton> stack = new Stack<NondeterministicFiniteAutomaton>();
            int counter = 0;

            foreach (char c in postfix)
            {
                if (RegexPostfixForm.IsOperand(c))
                {
                    List<int> states = new List<int> { counter, counter + 1 };
                    List<char> alphabet = new List<char>() { c };

                    Dictionary<int, List<Transition>> transitions = new Dictionary<int, List<Transition>>();
                    transitions[counter] = new List<Transition> { new Transition(c, counter + 1) };

                    List<int> finalStates = new List<int> { counter + 1 };

                    stack.Push(new NondeterministicFiniteAutomaton
                    {
                        States = states,
                        Alphabet = alphabet,
                        Transitions = transitions,
                        StartingState = counter,
                        FinalStates = finalStates
                    });

                    counter += 2;
                }
                else if (c == '|')
                {
                    var B = stack.Pop();
                    var A = stack.Pop();

                    List<int> states = B.States.Concat(A.States).ToList();
                    states.Add(counter);
                    states.Add(counter + 1);

                    List<char> alphabet = B.Alphabet.Union(A.Alphabet).ToList();

                    Dictionary<int, List<Transition>> transitions = new Dictionary<int, List<Transition>>(B.Transitions);
                    foreach (var kvp in A.Transitions)
                    {
                        transitions[kvp.Key] = kvp.Value;
                    }

                    transitions[counter] = new List<Transition>
                    {
                        new Transition(null, A.StartingState),
                        new Transition(null, B.StartingState)
                    };

                    if (!transitions.ContainsKey(A.FinalStates[0]))
                    {
                        transitions[A.FinalStates[0]] = new List<Transition>();
                    }

                    if (!transitions.ContainsKey(B.FinalStates[0]))
                    {
                        transitions[B.FinalStates[0]] = new List<Transition>();
                    }

                    transitions[A.FinalStates[0]].Add(new Transition(null, counter + 1));
                    transitions[B.FinalStates[0]].Add(new Transition(null, counter + 1));

                    stack.Push(new NondeterministicFiniteAutomaton
                    {
                        States = states,
                        Alphabet = alphabet,
                        Transitions = transitions,
                        StartingState = counter,
                        FinalStates = new List<int>() { counter + 1 }
                    });

                    counter += 2;
                }
                else if (c == '.')
                {
                    var B = stack.Pop();
                    var A = stack.Pop();

                    List<int> states = A.States.Concat(B.States).ToList();
                    List<char> alphabet = A.Alphabet.Union(B.Alphabet).ToList();

                    Dictionary<int, List<Transition>> transitions = new Dictionary<int, List<Transition>>();
                    foreach (var kvp in A.Transitions)
                        transitions[kvp.Key] = new List<Transition>(kvp.Value);
                    foreach (var kvp in B.Transitions)
                        transitions[kvp.Key] = new List<Transition>(kvp.Value);

                    foreach (int f in A.FinalStates)
                    {
                        if (!transitions.ContainsKey(f))
                            transitions[f] = new List<Transition>();
                        transitions[f].Add(new Transition(null, B.StartingState));
                    }

                    stack.Push(new NondeterministicFiniteAutomaton
                    {
                        States = states,
                        Alphabet = alphabet,
                        Transitions = transitions,
                        StartingState = A.StartingState,
                        FinalStates = new List<int>(B.FinalStates)
                    });
                }
                else if (c == '*')
                {
                    var A = stack.Pop();

                    List<int> states = new List<int>(A.States) { counter, counter + 1 };
                    List<char> alphabet = new List<char>(A.Alphabet);

                    Dictionary<int, List<Transition>> transitions = new Dictionary<int, List<Transition>>();
                    foreach (var kvp in A.Transitions)
                        transitions[kvp.Key] = new List<Transition>(kvp.Value);

                    transitions[counter] = new List<Transition> {
                        new Transition(null, A.StartingState),
                        new Transition(null, counter + 1)
                    };

                    foreach (int f in A.FinalStates)
                    {
                        if (!transitions.ContainsKey(f))
                            transitions[f] = new List<Transition>();
                        transitions[f].Add(new Transition(null, A.StartingState));
                        transitions[f].Add(new Transition(null, counter + 1));
                    }

                    stack.Push(new NondeterministicFiniteAutomaton
                    {
                        States = states,
                        Alphabet = alphabet,
                        Transitions = transitions,
                        StartingState = counter,
                        FinalStates = new List<int> { counter + 1 }
                    });

                    counter += 2;
                }
            }

            var nfa = stack.Pop();

            States = nfa.States;
            Alphabet = nfa.Alphabet;
            Transitions = nfa.Transitions;
            StartingState = nfa.StartingState;
            FinalStates = nfa.FinalStates;
        }
    }
}