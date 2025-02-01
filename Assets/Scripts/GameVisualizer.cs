using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Forest
{
    public class GameVisualizer : MonoBehaviour
    {
        private GameContext game = null;

        [SerializeField] private TMP_Text treesTextValue;
        [SerializeField] private TMP_Text seedsTextValue;
        [SerializeField] private TMP_Text seedsGainTextValue;
        [SerializeField] private Image tickGauge;
        [SerializeField] private TMP_Text tooltip;

        public void Initialize(GameContext context)
        {
            game = context;
            game[GameVariables.Trees].OnUpdate += UpdateTrees;
            game[GameVariables.Seeds].OnUpdate += UpdateSeeds;

            UpdateTrees(game[GameVariables.Trees].ModifiedValue, 0);
            UpdateSeeds(game[GameVariables.Seeds].ModifiedValue, 0);
        }

        private void UpdateTrees(double newValue, double previousValue)
        {
            int l = game[GameVariables.MaxTrees].ModifiedValue.ToString().Length;
            treesTextValue.text = Utilities.GrayZeros(newValue.ToFormatedString(l));
        }

        private void UpdateSeeds(double newValue, double previousValue)
        {
            int l = game[GameVariables.MaxSeeds].ModifiedValue.ToString().Length;
            seedsTextValue.text = Utilities.GrayZeros(newValue.ToFormatedString(l));
        }
    }
}
