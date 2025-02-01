using System.Collections.Generic;
using UnityEngine;
using Argon;
using Sirenix.Utilities;

namespace Forest
{
    [CreateAssetMenu(fileName = "InitialGameValue", menuName = "Forest/Create Game Initiale Values")]
    public class GameData : ScriptableObject
    {
        [NamedArray(typeof(GameVariables))]
        public List<double> GameInitialValues = new List<double>();

        protected void OnValidate()
        {
            GameInitialValues.SetLength((int)GameVariables.Count);
        }
    }
}
