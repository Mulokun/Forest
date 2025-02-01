using System;
using System.Collections.Generic;
using Forest.ConditionSystem;

namespace Forest
{
    public class GameContext : ISolvingData
    {
        private GameData initialData = null;

        public List<Variable> Variables = new();

        private List<Modifier> modifiers = new();
        private List<Modifier> onTick = new();
        private List<Modifier> onAction = new();

        public GameContext(GameData initialData)
        {
            this.initialData = initialData;
            Initialize();
        }

        protected void Initialize()
        {
            modifiers.Clear();
            onTick.Clear();
            onAction.Clear();

            Variables.Clear();
            for (int i = 0; i < initialData.GameInitialValues.Count; i++)
            {
                GameVariables game_variable = (GameVariables)i;
                double init_balue = initialData.GameInitialValues[i];

                Variable v = new Variable(init_balue);
                v.OnUpdateBase += (_, _) => ComputeModifiedValue(game_variable);
                Variables.Add(v);
            }

            /// Min & Max
            Variables[(int)GameVariables.Seeds].MinMax = (v) => Math.Clamp(v, 0, Variables[(int)GameVariables.MaxSeeds].ModifiedValue);
            Variables[(int)GameVariables.Trees].MinMax = (v) => Math.Clamp(v, 0, Variables[(int)GameVariables.MaxTrees].ModifiedValue);
        }

        public Variable this[GameVariables variable]
        {
            get
            {
                return Variables[(int)variable];
            }
            set
            {
                Variables[(int)variable] = value;
            }
        }

        public double ApplyInstant(Modifier modifier)
        {
            Variable v = this[modifier.Variable];
            double r = v.BaseValue;
            switch (modifier.Operation)
            {
                case Operations.Add:
                    r += modifier.Value;
                    break;
                case Operations.Multiply:
                    r *= modifier.Value;
                    break;
            }
            v.BaseValue = r;
            return v.BaseValue;
        }

        public double AddModifier(Modifier m)
        {
            modifiers.Add(m);
            return ComputeModifiedValue(m.Variable);
        }

        public double RemoveModifier(Modifier m)
        {
            modifiers.Remove(m);
            return ComputeModifiedValue(m.Variable);
        }

        public void AddTick(Modifier m)
        {
            onTick.Add(m);
        }

        public void RemoveTick(Modifier m)
        {
            onTick.Remove(m);
        }

        public void Tick()
        {
            foreach (Modifier t in onTick)
            {
                ApplyInstant(t);
            }
        }

        public void AddAction(Modifier m)
        {
            onAction.Add(m);
        }

        public void RemoveAction(Modifier m)
        {
            onAction.Remove(m);
        }

        public void Action()
        {
            foreach (Modifier a in onAction)
            {
                ApplyInstant(a);
            }
        }

        private double ComputeModifiedValue(GameVariables v)
        {
            double add_value = 0;
            double mul_value = 1;

            foreach (Modifier m in modifiers)
            {
                if (m.Variable == v)
                {
                    if (m.Operation == Operations.Add)
                    {
                        add_value += m.Value;
                    }
                    else if (m.Operation == Operations.Multiply)
                    {
                        mul_value += m.Value;
                    }
                }
            }

            this[v].ModifiedValue = ComputeValue(this[v].BaseValue, add_value, mul_value);
            return this[v].ModifiedValue;
        }

        private double ComputeValue(double value, double add, double mul)
        {
            return (value + add) * mul;
        }
    }
}
