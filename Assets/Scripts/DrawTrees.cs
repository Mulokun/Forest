using UnityEngine;

namespace Forest
{
    public class DrawTrees : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Texture2D brushTexture;
        [SerializeField] private Color tintColor;
        [SerializeField] private Color backgroundColor;
        [SerializeField] private Gradient gradientColor;
        private Material mat;
        private Texture initialTexture = null;


        private float ratio => 600f * meshRenderer.transform.localScale.y / 10f /*camera size * 2*/;

        public virtual Texture DrawTexture
        {
            get
            {
                return meshRenderer.material.mainTexture;
            }

            set
            {
                meshRenderer.material.mainTexture = value;
            }
        }

        protected virtual void Awake()
        {
            CreateTexture();
        }

        public void CreateTexture()
        {
            meshRenderer.material.color = tintColor;
            initialTexture = DrawTexture;

            mat = new Material(Shader.Find("Hidden/DrawOnTexture"));
            mat.SetTexture("_BrushTexture", brushTexture);

            if (!initialTexture)
            {
                initialTexture = new Texture2D(800, 800);
                Texture2D tex2D = (Texture2D)initialTexture;
                Color[] fillColorArray = tex2D.GetPixels();

                for (int i = 0; i < fillColorArray.Length; ++i)
                {
                    fillColorArray[i] = backgroundColor;
                }

                tex2D.SetPixels(fillColorArray);
                tex2D.Apply();
            }

            RenderTexture rt = new RenderTexture(initialTexture.width, initialTexture.height, 0);
            RenderTexture.active = rt;
            Graphics.Blit(initialTexture, rt);

            DrawTexture = rt;
        }

        protected void Update()
        {
            // DrawRandomPosition();
        }

        public RenderTexture DrawScreenPosition(Vector2 position)
        {
            Vector2 p = new Vector2(position.x / ratio, position.y / ratio);
            Texture src = DrawTexture;
            int srcWidth = src.width;
            mat.SetVector("_BrushPosition", p);
            mat.SetFloat("_BrushSize", 60 / (float)srcWidth);
            mat.SetColor("_BrushColor", gradientColor.Evaluate(Random.value));

            RenderTexture copiedTexture = new RenderTexture(srcWidth, src.height, 32);
            Graphics.Blit(src, copiedTexture, mat);
            DestroyImmediate(src);

            DrawTexture = copiedTexture;
            return copiedTexture;
        }

        private RenderTexture DrawRandomPosition()
        {
            Texture src = DrawTexture;
            int srcWidth = src.width;
            mat.SetVector("_BrushPosition", new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            mat.SetFloat("_BrushSize", 60 / (float)srcWidth);
            mat.SetColor("_BrushColor", gradientColor.Evaluate(Random.value));

            RenderTexture copiedTexture = new RenderTexture(srcWidth, src.height, 32);
            Graphics.Blit(src, copiedTexture, mat);
            DestroyImmediate(src);

            DrawTexture = copiedTexture;
            return copiedTexture;
        }
    }
}
