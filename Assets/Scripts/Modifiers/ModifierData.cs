using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Forest
{
    [CreateAssetMenu(fileName = "NewModifier", menuName = "Forest/Create Modifier")]
    public class ModifierData : ScriptableObject
    {
        [SuffixLabel("in seconds", true)]
        public double Duration;
        public List<Modifier> InstantModifier;
        public List<Modifier> OnActionModifier;
        public List<Modifier> OnTickModifier;
    }
}
