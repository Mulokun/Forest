using Argon.ColorPalette;
using UnityEngine;

namespace Forest
{
    public class ProceduralImageColorBinder : MonoBehaviour, IHasDynamicColor
    {
        [SerializeField] private UnityEngine.UI.ProceduralImage.ProceduralImage image;
        public int ColorCount => 1;

        public void SetColor(int index, Color color)
        {
            image.color = color;
        }
    }
}
