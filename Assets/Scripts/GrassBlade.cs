using UnityEngine;

public struct BladePlan
{
    public Vector3 root;
    public float height;
    public float width;
    public float stiffness;
    public float droop;
    public float twist;
    public float lean;
    public Vector2 facing;
    public Color fill;
    public bool hasSeedHead;
}

public struct BladeTip
{
    public Vector3 position;
    public Vector3 tangent;
    public Vector3 flank;
}

public static class GrassBlade
{
    private const float SheathNarrowing = 0.72f;
    private const float MinimumWidth = 1e-5f;
    private const float RidgeShare = 0.5f;
    private const float RootShadeSpan = 0.35f;

    private static float SlopeProfile(float along)
    {
        return along * (2.0f - along);
    }

    private static float CurvatureWeight(float along)
    {
        return 2.0f * (1.0f - along);
    }

    private static float HalfWidth(GrassSettings settings, BladePlan plan, float along)
    {
        float taper = Mathf.Pow(Mathf.Max(1.0f - along, 0.0f), settings.taperPower);
        float sheath = Mathf.Lerp(SheathNarrowing, 1.0f, Mathf.Clamp01(along / Mathf.Max(settings.shoulder, MinimumWidth)));
        return Mathf.Max(plan.width * taper * sheath, MinimumWidth) * RidgeShare;
    }

    private static Color Tone(GrassSettings settings, BladePlan plan, float along)
    {
        float shade = Mathf.Lerp(settings.rootShade, 1.0f, Mathf.Clamp01(along / RootShadeSpan));
        float lift = 1.0f + settings.tipLift * along * along;
        float scale = shade * lift;
        return new Color(plan.fill.r * scale, plan.fill.g * scale, plan.fill.b * scale, 1.0f);
    }

    public static BladeTip Build(MeshBuffer mesh, GrassSettings settings, BladePlan plan, WindGust[] gusts, float seconds)
    {
        Vector3 heading = new Vector3(plan.facing.x, 0.0f, plan.facing.y).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, heading).normalized;

        float travel = Vector3.Dot(plan.root, heading);
        Vector2 wind = GrassWind.Bend(gusts, plan.height, plan.stiffness, settings.damping, travel, seconds);

        float totalSlope = plan.droop + plan.lean + wind.x;
        float totalSway = wind.y;

        int rings = settings.segments + 1;
        float step = plan.height / settings.segments;
        Vector3 cursor = plan.root;
        Vector3 tangent = Vector3.up;
        Vector3 lastFlank = side;
        int first = mesh.VertexCount;

        for (int i = 0; i < rings; i++)
        {
            float along = i / (float)settings.segments;

            if (i > 0)
            {
                float previous = (i - 1) / (float)settings.segments;
                float slice = along - previous;
                float pitch = totalSlope * CurvatureWeight(previous) * slice;
                float yaw = totalSway * CurvatureWeight(previous) * slice;
                tangent = (Quaternion.AngleAxis(pitch * Mathf.Rad2Deg, side)
                           * Quaternion.AngleAxis(yaw * Mathf.Rad2Deg, Vector3.up) * tangent).normalized;
                cursor += tangent * step;
            }

            float spin = plan.twist * SlopeProfile(along);
            Vector3 flank = (Quaternion.AngleAxis(spin, tangent) * side).normalized;
            Vector3 face = Vector3.Cross(flank, tangent).normalized;
            lastFlank = flank;
            float half = HalfWidth(settings, plan, along);
            Color tone = Tone(settings, plan, along);
            Vector3 ridge = cursor + face * (half * settings.fold);

            Vector3 leftNormal = Vector3.Cross(tangent, (ridge - (cursor - flank * half))).normalized;
            Vector3 rightNormal = Vector3.Cross(((cursor + flank * half) - ridge), tangent).normalized;

            mesh.AddVertex(cursor - flank * half, -flank, Vector4.zero, tone, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(leftNormal));
            mesh.AddVertex(ridge, face, Vector4.zero, tone, StrokeKind.Card,
                0.0f, 0.0f, settings.depthBias, Shading.Surface(face));
            mesh.AddVertex(cursor + flank * half, flank, Vector4.zero, tone, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(rightNormal));

            if (i > 0)
            {
                int here = first + i * 3;
                mesh.AddQuad(here - 3, here - 2, here + 1, here);
                mesh.AddQuad(here - 2, here - 1, here + 2, here + 1);
            }
        }

        return new BladeTip { position = cursor, tangent = tangent, flank = lastFlank };
    }
}
