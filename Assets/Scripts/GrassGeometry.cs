using System.Collections.Generic;
using UnityEngine;

public static class GrassGeometry
{
    public static void Build(MeshBuffer mesh, GrassSettings settings, GrassPalette palette)
    {
        mesh.SetInk(palette.ink);
        WindGust[] gusts = GrassField.Gusts(settings);
        List<BladePlan> blades = GrassField.Sow(settings, palette);

        foreach (BladePlan plan in blades)
        {
            BladeTip tip = GrassBlade.Build(mesh, settings, plan, gusts, settings.windPhase);
            if (plan.hasSeedHead)
            {
                GrassSeedHead.Build(mesh, settings, plan, tip, palette.seedHead);
            }
        }
    }
}
