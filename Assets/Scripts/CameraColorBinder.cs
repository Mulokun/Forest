using UnityEngine;
using Argon.ColorPalette;

namespace Forest
{
    public class CameraColorBinder : MonoBehaviour, IHasDynamicColor
    {
        [SerializeField] private Camera bindCamera;
        public int ColorCount => 1;

        public void SetColor(int index, Color color)
        {
            bindCamera.backgroundColor = color;
        }
    }
}
