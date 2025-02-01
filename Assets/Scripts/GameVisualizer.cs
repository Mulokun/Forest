using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Forest
{
    public class GameVisualizer : MonoBehaviour
    {
        private GameContext game = null;

        [SerializeField] private Camera base_camera;
        [SerializeField] private TMP_Text treesTextValue;
        [SerializeField] private TMP_Text seedsTextValue;
        [SerializeField] private TMP_Text seedsGainTextValue;
        [SerializeField] private Image tickGauge;
        [SerializeField] private TMP_Text tooltip;
        [SerializeField] private ParticleHandler prefabLeafParticle;

        private ParticlePooler particlePooler;

        public void Initialize(GameContext context)
        {
            particlePooler = new(prefabLeafParticle, 10);

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

        public void Action(Vector2 position)
        {
            Vector3 pos = base_camera.ScreenToWorldPoint(position);
            pos.z = 0;

            ParticleHandler p = particlePooler.Borrow();
            p.transform.position = pos;
        }
    }
}
