namespace DeterministicFiniteAutomaton
{
    class DeterministicFiniteAutomaton
    {
        public List<int> States { get; set; }
        public List<char> Alphabet { get; set; }
        public Dictionary<int, List<Transition>> Transitions { get; set; }
        public int StartingState { get; set; }
        public List<int> FinalStates { get; set; }

        public DeterministicFiniteAutomaton()
        {
            States = new List<int>();
            Alphabet = new List<char>();
            Transitions = new Dictionary<int, List<Transition>>();
            StartingState = 0;
            FinalStates = new List<int>();
        }

        public DeterministicFiniteAutomaton(NondeterministicFiniteAutomaton nfa)
        {
            Alphabet = new List<char>(nfa.Alphabet);
            States = new List<int>();
            Transitions = new Dictionary<int, List<Transition>>();
            FinalStates = new List<int>();

            Dictionary<string, int> stateMap = new Dictionary<string, int>();
            Queue<HashSet<int>> unprocessed = new Queue<HashSet<int>>();

            HashSet<int> startClosure = new HashSet<int> { nfa.StartingState };
            Stack<int> stack = new Stack<int>();
            stack.Push(nfa.StartingState);

            while (stack.Count > 0)
            {
                int s = stack.Pop();
                if (nfa.Transitions.ContainsKey(s))
                {
                    foreach (var t in nfa.Transitions[s])
                    {
                        if (t.Symbol == null && !startClosure.Contains(t.ToState))
                        {
                            startClosure.Add(t.ToState);
                            stack.Push(t.ToState);
                        }
                    }
                }
            }

            string startKey = string.Join(",", startClosure.OrderBy(x => x));
            stateMap[startKey] = 0;
            States.Add(0);
            StartingState = 0;
            unprocessed.Enqueue(startClosure);
            int dfaCounter = 1;

            while (unprocessed.Count > 0)
            {
                HashSet<int> current = unprocessed.Dequeue();
                string currentKey = string.Join(",", current.OrderBy(x => x));
                int currentDfaState = stateMap[currentKey];

                foreach (char symbol in Alphabet)
                {
                    HashSet<int> moveSet = new HashSet<int>();
                    foreach (int state in current)
                    {
                        if (nfa.Transitions.ContainsKey(state))
                        {
                            foreach (var t in nfa.Transitions[state])
                            {
                                if (t.Symbol == symbol)
                                    moveSet.Add(t.ToState);
                            }
                        }
                    }

                    HashSet<int> closure = new HashSet<int>(moveSet);
                    Stack<int> closureStack = new Stack<int>(moveSet);
                    while (closureStack.Count > 0)
                    {
                        int s = closureStack.Pop();
                        if (nfa.Transitions.ContainsKey(s))
                        {
                            foreach (var t in nfa.Transitions[s])
                            {
                                if (t.Symbol == null && !closure.Contains(t.ToState))
                                {
                                    closure.Add(t.ToState);
                                    closureStack.Push(t.ToState);
                                }
                            }
                        }
                    }

                    if (closure.Count == 0) continue;

                    string closureKey = string.Join(",", closure.OrderBy(x => x));
                    if (!stateMap.ContainsKey(closureKey))
                    {
                        stateMap[closureKey] = dfaCounter;
                        States.Add(dfaCounter);
                        unprocessed.Enqueue(closure);
                        dfaCounter++;
                    }

                    int toState = stateMap[closureKey];
                    if (!Transitions.ContainsKey(currentDfaState))
                        Transitions[currentDfaState] = new List<Transition>();
                    Transitions[currentDfaState].Add(new Transition(symbol, toState));
                }
            }

            foreach (var key in stateMap.Keys)
            {
                string[] parts = key.Split(',');
                foreach (string p in parts)
                {
                    int s = int.Parse(p);
                    if (nfa.FinalStates.Contains(s))
                    {
                        FinalStates.Add(stateMap[key]);
                        break;
                    }
                }
            }
        }

        public bool VerifyAutomation()
        {
            if (!States.Contains(StartingState))
            {
                Console.WriteLine("Invalid: starting state not in States.");
                return false;
            }

            foreach (int f in FinalStates)
            {
                if (!States.Contains(f))
                {
                    Console.WriteLine("Invalid: final state " + f + " not in States.");
                    return false;
                }
            }

            foreach (int state in Transitions.Keys)
            {
                if (!States.Contains(state))
                {
                    Console.WriteLine("Invalid: state " + state + " in Transitions not in States.");
                    return false;
                }

                foreach (Transition t in Transitions[state])
                {
                    if (!States.Contains(t.ToState))
                    {
                        Console.WriteLine("Invalid: transition to state " + t.ToState + " not in States.");
                        return false;
                    }

                    if (t.Symbol != null && !Alphabet.Contains((char)t.Symbol))
                    {
                        Console.WriteLine("Invalid: transition symbol " + t.Symbol + " not in Alphabet.");
                        return false;
                    }
                }
            }

            return true;
        }

        public void PrintAutomaton()
        {
            Console.WriteLine("States: " + string.Join(", ", States));
            Console.WriteLine("Alphabet: " + string.Join(", ", Alphabet));
            Console.WriteLine("Starting State: " + StartingState);
            Console.WriteLine("Final States: " + string.Join(", ", FinalStates));
            Console.WriteLine("Transitions:");

            foreach (int state in States)
            {
                if (Transitions.ContainsKey(state))
                {
                    foreach (Transition t in Transitions[state])
                    {
                        string sym = t.Symbol == null ? "lambda" : t.Symbol.ToString();
                        Console.WriteLine($"  p({state}, {sym}) -> {t.ToState}");
                    }
                }
            }
        }

        public bool CheckWord(string word)
        {
            int currentState = StartingState;

            foreach (char c in word)
            {
                bool found = false;
                if (Transitions.ContainsKey(currentState))
                {
                    foreach (Transition t in Transitions[currentState])
                    {
                        if (t.Symbol != null && t.Symbol == c)
                        {
                            currentState = t.ToState;
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return FinalStates.Contains(currentState);
        }
    }
}
