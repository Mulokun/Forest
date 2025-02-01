using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Forest
{
    [CreateAssetMenu(fileName = "NewModifier", menuName = "Forest/Create Modifier")]
    public class ModifierData : ScriptableObject
    {
        [ToggleLeft]
        public bool IsInstant = true;
        [SuffixLabel("in seconds", true)]
        [HideIf(nameof(IsInstant))]
        public double Duration;
        public List<Modifier> InstantModifier;
        [HideIf(nameof(IsInstant))]
        public List<Modifier> OnActionModifier;
        [HideIf(nameof(IsInstant))]
        public List<Modifier> OnTickModifier;
    }
}
