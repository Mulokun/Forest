using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using Forest.ConditionSystem;

namespace Forest
{
    [Serializable, InlineProperty]
    public struct CostFormula
    {
        [HorizontalGroup("formula"), HideLabel, SuffixLabel(" * ( ")]
        public double Base;
        [HorizontalGroup("formula"), HideLabel, SuffixLabel(" ^ Rank_Level )")]
        public double Multiplier;

        public readonly double Value(double level = 1)
        {
            return Base * Math.Pow(Multiplier, level);
        }
    }

    [Serializable, InlineProperty]
    public class Cost
    {
        [HorizontalGroup("cost"), HideLabel]
        [ShowIf(nameof(IsCustom)), SuffixLabel("$CustomSuffixLabel", true)]
        public double CustomCost;
        [HorizontalGroup("cost"), HideLabel]
        [ReadOnly, HideIf(nameof(IsCustom))]
        public double FormulaCost;
        [HorizontalGroup("cost", 16), HideLabel]
        public bool IsCustom;

        private string customSuffixLabel => "Custom Cost (Formula: " + FormulaCost.ToFormatedString() + ")";
    }

    [CreateAssetMenu(fileName = "ShopItem", menuName = "Forest/New Shop Item")]
    public class ShopItem : ScriptableObject
    {
        public string Name;
        public string Description;
        [PropertySpace(SpaceAfter = 30, SpaceBefore = 0)]
        public Sprite Icon;
        [BoxGroup]
        public Condition<GameContext> UnlockCondition;

        [Header("Cost")]
        [Range(1, 250)]
        public int MaximumRank = 1;
        public CostFormula Formula;
        [ListDrawerSettings(ShowIndexLabels = true, HideRemoveButton = true, DraggableItems = false)]
        public List<Cost> Costs;

        [Header("Modifiers")]
        public List<Modifier> Modifiers;

        public double GetPrice(double rank)
        {
            return Formula.Value(rank);
        }

        protected void OnValidate()
        {
            Costs.SetLength(MaximumRank);
            for (int i = 0; i < MaximumRank; i++)
            {
                Costs[i].FormulaCost = Formula.Value(i);
            }
        }
    }
}
