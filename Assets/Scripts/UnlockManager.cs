
using System.Collections.Generic;
using Forest.ConditionSystem;

namespace Forest
{
    public interface IUnlockable<T> where T : ISolvingData
    {
        public Condition<T> UnlockingCondition { get; }
    }

    public class UnlockManager<T, U> where T : IUnlockable<U> where U : ISolvingData
    {
        public delegate void UnlockedEvent(T unlockedObject);
        public event UnlockedEvent OnUnlock;

        private List<T> unlockedElements = new List<T>();
        private List<T> unlockableElements;

        private U conditionSolver;

        public UnlockManager(List<T> unlockablesList, U conditionSolver)
        {
            this.conditionSolver = conditionSolver;
            unlockableElements = new List<T>(unlockablesList);
        }

        public void CheckUnlocks()
        {
            for (int i = 0; i < unlockableElements.Count; i++)
            {
                if (unlockableElements[i].UnlockingCondition != null && unlockableElements[i].UnlockingCondition.GetResult(conditionSolver))
                {
                    ForceUnlock(unlockableElements[i]);
                }
            }
        }

        public void ForceUnlock(T elementToUnlock)
        {
            if (unlockableElements.Remove(elementToUnlock))
            {
                unlockedElements.Add(elementToUnlock);
                OnUnlock?.Invoke(elementToUnlock);
            }
        }

        public bool IsUnlocked(T elementToCheck)
        {
            return unlockedElements.Contains(elementToCheck);
        }
    }
}
