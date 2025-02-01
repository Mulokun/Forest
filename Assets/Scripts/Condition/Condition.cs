using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forest.ConditionSystem
{
    [Serializable]
    public class Condition<T> where T : ISolvingData
    {
        [SerializeReference] public BaseCondition ConditionValue;

        public bool GetResult(T conditionSolver)
        {
            return ConditionValue.Result(conditionSolver);
        }
    }

    [Serializable]
    public abstract class BaseCondition
    {
        public bool IntendedResult = true;

        public bool Result(ISolvingData conditionSolver)
        {
            return ComputeResult(conditionSolver) == IntendedResult;
        }

        protected abstract bool ComputeResult(ISolvingData conditionSolver);
    }

    public class OrCondition : BaseCondition
    {
        [SerializeReference] private BaseCondition[] orComparedConditions = new BaseCondition[2];

        protected override bool ComputeResult(ISolvingData conditionSolver)
        {
            bool result = false;
            foreach (BaseCondition condition in orComparedConditions)
            {
                result |= condition.Result(conditionSolver);
                if (result)
                {
                    break;
                }
            }
            return result;
        }
    }

    public class AndCondition : BaseCondition
    {
        [SerializeReference] private BaseCondition[] andComparedConditions = new BaseCondition[2];

        protected override bool ComputeResult(ISolvingData conditionSolver)
        {
            bool result = true;
            foreach (BaseCondition condition in andComparedConditions)
            {
                result &= condition.Result(conditionSolver);
                if (!result)
                {
                    break;
                }
            }
            return result;
        }
    }

    public interface ISolvingData
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredSolvingDataAttribute : Attribute
    {
        private List<Type> dataTypes = new List<Type>();

        public RequiredSolvingDataAttribute(Type contextType, params Type[] additionalContextTypes)
        {
            dataTypes.Add(contextType);
            for (int i = 0; i < additionalContextTypes.Length; i++)
            {
                dataTypes.Add(additionalContextTypes[i]);
            }
        }

        public bool IsCompatible(Type t)
        {
            return dataTypes.Contains(t);
        }
    }
}
