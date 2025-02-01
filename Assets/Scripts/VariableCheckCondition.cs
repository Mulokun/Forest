using UnityEngine;
using Forest.ConditionSystem;
using Sirenix.OdinInspector;

namespace Forest
{
    [RequiredSolvingData(typeof(GameContext))]
    public class VariableCheckCondition : BaseCondition
    {
        public enum ConditionOperation
        {
            GreaterThan,
            EqualTo,
        }

        [HorizontalGroup("cnd"), HideLabel]
        [SerializeField] private GameVariables variableID;

        [HorizontalGroup("cnd"), HideLabel]
        [SerializeField] private ConditionOperation operation;

        [HorizontalGroup("cnd"), HideLabel]
        [SerializeField] private double value;

        protected override bool ComputeResult(ISolvingData solver)
        {
            if (solver is GameContext c)
            {
                switch (operation)
                {
                    case ConditionOperation.EqualTo:
                        return c[variableID].BaseValue == value;
                    case ConditionOperation.GreaterThan:
                        return c[variableID].BaseValue >= value;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
