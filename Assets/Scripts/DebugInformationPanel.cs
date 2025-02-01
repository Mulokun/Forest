using UnityEngine;
using TMPro;

namespace Forest
{
    public class DebugInformationPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text textNames;
        [SerializeField] private TMP_Text textValue;
        private GameContext context;

        public void Initialize(GameContext game)
        {
            context = game;

            for (int i = 0; i < context.Variables.Count; i++)
            {
                context.Variables[i].OnUpdate += UpdateFloat;
            }
        }

        private void UpdateFloat(double d1, double d2)
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            string n = string.Empty;
            string v = string.Empty;
            for (int i = 0; i < context.Variables.Count; i++)
            {
                n += $"{(GameVariables)i}\n";
                v += $"{context.Variables[i].ToFormatedString()}\n";
            }

            textNames.text = n;
            textValue.text = v;
        }
    }
}
