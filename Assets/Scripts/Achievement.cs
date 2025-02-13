using System.Collections.Generic;
using Forest.ConditionSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Forest
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Forest/New Achievement")]
    public class Achievement : ScriptableObject, IUnlockable<GameContext>
    {
        public string Name;
        public string Description;
        [PropertySpace(SpaceAfter = 30, SpaceBefore = 0)]
        public Sprite Icon;
        [BoxGroup]
        public Condition<GameContext> UnlockCondition;
        public Condition<GameContext> UnlockingCondition => UnlockCondition;

        [Header("Modifiers")]
        public List<ModifierData> Modifiers;
    }
}
