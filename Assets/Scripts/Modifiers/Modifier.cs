using System;

namespace Forest
{
    public enum Operations
    {
        Add,
        Multiply
    }

    [Serializable]
    public class Modifier
    {
        public GameVariables Variable;
        public Operations Operation;
        public double Value;

        public Modifier(GameVariables v, Operations op, double value)
        {
            Variable = v;
            Operation = op;
            Value = value;
        }
    }
}
