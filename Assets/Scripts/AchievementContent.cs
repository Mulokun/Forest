using System.Collections.Generic;
using UnityEngine;

namespace Forest
{
    [CreateAssetMenu(fileName = "Achievements", menuName = "Forest/New Achievement Content")]
    public class AchievementContent : ScriptableObject
    {
        public List<Achievement> Achievements;
    }
}
