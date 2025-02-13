using UnityEngine;

namespace Forest
{
    public class AchievementManager : MonoBehaviour
    {
        private GameController controller;

        [SerializeField] private AchievementContent achievements;
        private UnlockManager<Achievement, GameContext> achievementUnlock;

        public void Initialize(GameController game)
        {
            controller = game;

            achievementUnlock = new(achievements.Achievements, controller.Game);
            achievementUnlock.OnUnlock += UnlockAchievement;

            for (int i = 0; i < controller.Game.Variables.Count; i++)
            {
                controller.Game.Variables[i].OnUpdate += CheckUnlocks;
            }
        }

        private void CheckUnlocks(double _, double __)
        {
            achievementUnlock.CheckUnlocks();
        }

        private void UnlockAchievement(Achievement a)
        {
            Debug.Log("Achievmeent Unlocked " + a.Name);
        }
    }
}
