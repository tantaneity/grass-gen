using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class GrassBuilder : MonoBehaviour
{
    public GrassSettings settings = new GrassSettings();

    private Mesh mesh;

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnValidate()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        if (mesh == null)
        {
            mesh = new Mesh { name = "Grass", hideFlags = HideFlags.DontSave };
        }

        MeshBuffer buffer = new MeshBuffer();
        GrassGeometry.Build(buffer, settings, GrassPalette.Meadow);
        buffer.WriteTo(mesh);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
