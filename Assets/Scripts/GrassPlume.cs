using UnityEngine;

public static class GrassPlume
{
    private const int SpineNodes = 26;
    private const int HairsPerNode = 4;
    private const float HairTaper = 0.45f;
    private const float CurveProfile = 1.6f;
    private const float RollPerNode = 47.0f;
    private const float BareRun = 0.16f;
    private const float HairSag = 0.22f;
    private const float FullTurn = 360.0f;

    private static float HairSpan(float along)
    {
        float arch = Mathf.Sin(Mathf.Clamp01((along - BareRun) / (1.0f - BareRun)) * Mathf.PI);
        return Mathf.Pow(Mathf.Max(arch, 0.0f), HairTaper);
    }

    public static void Build(MeshBuffer mesh, GrassSettings settings, BladeTip tip, Color fill)
    {
        float step = settings.plumeLength / SpineNodes;
        Vector3 cursor = tip.position;
        Vector3 heading = tip.tangent;
        Vector3 flank = tip.flank;

        for (int i = 0; i < SpineNodes; i++)
        {
            float along = i / (float)(SpineNodes - 1);
            float sag = settings.plumeCurve * Mathf.Pow(along, CurveProfile) * step;
            heading = (heading - Vector3.up * sag).normalized;
            cursor += heading * step;
            flank = Vector3.Cross(heading, Vector3.Cross(flank, heading)).normalized;

            float span = HairSpan(along) * settings.plumeHairLength;
            if (span <= 0.0f)
            {
                continue;
            }

            Vector3 binormal = Vector3.Cross(heading, flank).normalized;
            for (int hair = 0; hair < HairsPerNode; hair++)
            {
                float roll = (hair * FullTurn / HairsPerNode + i * RollPerNode) * Mathf.Deg2Rad;
                Vector3 radial = flank * Mathf.Cos(roll) + binormal * Mathf.Sin(roll);
                float tilt = settings.plumeHairAngle * Mathf.Deg2Rad;
                Vector3 outward = (heading * Mathf.Cos(tilt) + radial * Mathf.Sin(tilt)).normalized;
                Vector3 reach = (outward - Vector3.up * HairSag).normalized * span;
                Vector3 thick = Vector3.Cross(reach, radial).normalized * settings.plumeHairWidth;
                Vector3 normal = Vector3.Cross(reach, thick).normalized;

                int first = mesh.VertexCount;
                mesh.AddVertex(cursor - thick, -radial, Vector4.zero, fill, StrokeKind.Card,
                    0.0f, Outline.None, settings.depthBias, Shading.Surface(normal));
                mesh.AddVertex(cursor + thick, radial, Vector4.zero, fill, StrokeKind.Card,
                    0.0f, Outline.None, settings.depthBias, Shading.Surface(normal));
                mesh.AddVertex(cursor + reach + thick * HairTaper, radial, Vector4.zero, fill, StrokeKind.Card,
                    0.0f, Outline.None, settings.depthBias, Shading.Surface(normal));
                mesh.AddVertex(cursor + reach - thick * HairTaper, -radial, Vector4.zero, fill, StrokeKind.Card,
                    0.0f, Outline.None, settings.depthBias, Shading.Surface(normal));
                mesh.AddQuad(first, first + 1, first + 2, first + 3);
            }
        }
    }
}
