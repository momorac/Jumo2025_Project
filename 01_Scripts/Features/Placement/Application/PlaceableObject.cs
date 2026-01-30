using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private Placeable type;
    public Placeable Type => type;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private Int2 cellSize;
    public Int2 CellSize => cellSize;

    public void Bind(Placeable placeable)
    {
        this.type = placeable;
    }


    private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private Color lastPreviewColor;
    private Color[] colors = { new Color(0, 1, 0, 1), new Color(1, 0, 0, 1) };

    public void SetPreviewColor(bool isAvailable, bool hasPlaced = false)
    {
        Color color = isAvailable ? colors[0] : colors[1];

        // 같은 색이면 작업 생략
        if (lastPreviewColor == color) return;

        for (int r = 0; r < renderers.Length; r++)
        {
            var renderer = renderers[r];
            if (renderer == null) continue;

            if (hasPlaced)
            {
                renderer.SetPropertyBlock(null);
                continue;
            }

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor(OutlineColorID, color);
            mpb.SetFloat("_ID", 0);

            renderer.SetPropertyBlock(mpb);
        }

        lastPreviewColor = color;
    }
}
