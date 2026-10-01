using UnityEngine;

public static class GrassGround
{
    private const int RimSteps = 64;
    private const float Overhang = 1.18f;
    private const float FullTurn = Mathf.PI * 2.0f;
    private const float Sink = -0.004f;

    public static void Build(MeshBuffer mesh, GrassSettings settings, GrassPalette palette)
    {
        float radius = settings.fieldRadius * Overhang;
        Vector3 middle = new Vector3(0.0f, Sink, 0.0f);
        Vector3 up = Vector3.up;
        int centre = mesh.VertexCount;

        mesh.AddVertex(middle, Vector3.zero, Vector4.zero, palette.earth, StrokeKind.Card,
            0.0f, Outline.None, settings.depthBias, Shading.Surface(up));

        for (int i = 0; i < RimSteps; i++)
        {
            float angle = FullTurn * i / RimSteps;
            Vector3 rim = middle + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * radius;
            mesh.AddVertex(rim, Vector3.zero, Vector4.zero, palette.rim, StrokeKind.Card,
                0.0f, Outline.None, settings.depthBias, Shading.Surface(up));
            mesh.AddTriangle(centre, centre + 1 + i, centre + 1 + (i + 1) % RimSteps);
        }
    }
}
