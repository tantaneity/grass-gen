using UnityEngine;

public static class GrassGround
{
    private const int Spokes = 72;
    private const int Rings = 20;
    private const int ShadowTaps = 3;
    private const float Overhang = 1.18f;
    private const float FullTurn = Mathf.PI * 2.0f;
    private const float Sink = -0.004f;
    private const float TrampleShade = 0.55f;

    private static float Shadow(Vector3 place, GrassSettings settings)
    {
        Vector2 sunward = settings.sunHeading.normalized * settings.shadowReach;
        float cover = 0.0f;
        for (int tap = 0; tap < ShadowTaps; tap++)
        {
            float step = (tap + 1) / (float)ShadowTaps;
            Vector3 sampled = place + new Vector3(sunward.x, 0.0f, sunward.y) * step;
            cover += GrassField.Cover(sampled, settings);
        }

        float floor = 1.0f - Mathf.Max(settings.clumpBias, 1e-3f);
        float stretched = Mathf.InverseLerp(floor, 1.0f, cover / ShadowTaps);
        float pressed = GrassTrample.Push(settings.tramples, place).magnitude * TrampleShade;
        return Mathf.Clamp01(stretched + pressed) * settings.shadowDepth;
    }

    private static Color Tone(GrassSettings settings, GrassPalette palette, Vector3 place, float reach)
    {
        Color soil = Color.Lerp(palette.earth, palette.rim, reach);
        return Color.Lerp(soil, palette.shadow, Shadow(place, settings));
    }

    public static void Build(MeshBuffer mesh, GrassSettings settings, GrassPalette palette)
    {
        float radius = settings.fieldRadius * Overhang;
        Vector3 up = Vector3.up;
        int centre = mesh.VertexCount;
        Vector3 middle = new Vector3(0.0f, Sink, 0.0f);

        mesh.AddVertex(middle, Vector3.zero, Vector4.zero, Tone(settings, palette, middle, 0.0f),
            StrokeKind.Card, 0.0f, Outline.None, settings.depthBias, Shading.Surface(up));

        for (int ring = 1; ring <= Rings; ring++)
        {
            float reach = ring / (float)Rings;
            for (int spoke = 0; spoke < Spokes; spoke++)
            {
                float angle = FullTurn * spoke / Spokes;
                Vector3 place = middle + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * (radius * reach);
                mesh.AddVertex(place, Vector3.zero, Vector4.zero, Tone(settings, palette, place, reach),
                    StrokeKind.Card, 0.0f, Outline.None, settings.depthBias, Shading.Surface(up));
            }
        }

        for (int spoke = 0; spoke < Spokes; spoke++)
        {
            mesh.AddTriangle(centre, centre + 1 + spoke, centre + 1 + (spoke + 1) % Spokes);
        }

        for (int ring = 1; ring < Rings; ring++)
        {
            int inner = centre + 1 + (ring - 1) * Spokes;
            int outer = inner + Spokes;
            for (int spoke = 0; spoke < Spokes; spoke++)
            {
                int next = (spoke + 1) % Spokes;
                mesh.AddQuad(inner + spoke, inner + next, outer + next, outer + spoke);
            }
        }
    }
}
