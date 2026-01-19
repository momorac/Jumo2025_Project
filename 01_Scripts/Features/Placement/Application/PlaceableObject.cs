using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private Placeable type;
    public Placeable Type => type;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private Int2 cellSize;
    public Int2 CellSize => cellSize;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    public void Bind(Placeable placeable)
    {
        this.type = placeable;
    }

    public void SetPreviewColor(bool isAvailable, bool hasPlaced = false)
    {
        Color color = isAvailable ? Color.green : Color.red;
        if (hasPlaced)
        {
            color = Color.white;
        }

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            // 모든 머티리얼 인덱스에 대해 MaterialPropertyBlock 적용
            var materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block, i);

                if (mat != null)
                {
                    block.SetColor(BaseColorID, color);
                }

                renderer.SetPropertyBlock(block, i);
            }
        }
    }
}
