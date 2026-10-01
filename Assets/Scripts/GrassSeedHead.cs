using UnityEngine;

public static class GrassSeedHead
{
    private const float GrainAspect = 1.9f;
    private const float GrainStagger = 0.35f;
    private const float SpreadAngle = 13.0f;

    public static void Build(MeshBuffer mesh, GrassSettings settings, BladePlan plan, BladeTip tip, Color grainFill)
    {
        int grains = Mathf.Max(settings.seedHeadGrains, 1);
        float step = settings.seedHeadLength / grains;
        Vector3 cursor = tip.position;
        Vector3 heading = tip.tangent;
        Vector3 flank = tip.flank;

        for (int i = 0; i < grains; i++)
        {
            float along = i / (float)(grains - 1 == 0 ? 1 : grains - 1);
            float sag = settings.seedHeadDroop * along * along * step;
            heading = (heading - Vector3.up * sag).normalized;
            cursor += heading * step;

            float side = (i % 2 == 0) ? 1.0f : -1.0f;
            Vector3 outward = (Quaternion.AngleAxis(SpreadAngle * side, heading) * flank).normalized;
            Vector3 anchor = cursor + outward * (settings.seedHeadWidth * GrainStagger);
            Vector3 lengthwise = heading * (settings.seedHeadWidth * GrainAspect * (1.0f - along * 0.4f));
            Vector3 across = outward * settings.seedHeadWidth;
            Vector3 normal = Vector3.Cross(lengthwise, across).normalized;

            int first = mesh.VertexCount;
            mesh.AddVertex(anchor - lengthwise - across, -across, Vector4.zero, grainFill, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(normal));
            mesh.AddVertex(anchor + lengthwise - across, -across, Vector4.zero, grainFill, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(normal));
            mesh.AddVertex(anchor + lengthwise + across, across, Vector4.zero, grainFill, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(normal));
            mesh.AddVertex(anchor - lengthwise + across, across, Vector4.zero, grainFill, StrokeKind.Card,
                0.0f, settings.outlineWeight, settings.depthBias, Shading.Surface(normal));
            mesh.AddQuad(first, first + 1, first + 2, first + 3);
        }
    }
}
