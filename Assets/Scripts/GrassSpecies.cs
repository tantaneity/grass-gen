using UnityEngine;

public static class GrassSpecies
{
    public static readonly string[] Names = { "meadow", "timothy", "feather", "sedge", "fescue" };

    public static int Count => Names.Length;

    public static int IndexOf(string name)
    {
        for (int i = 0; i < Names.Length; i++)
        {
            if (Names[i] == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static GrassSettings Preset(int index, int seed)
    {
        GrassSettings settings = Tuned(Mathf.Clamp(index, 0, Count - 1));
        settings.seed = seed;
        return settings;
    }

    private static GrassSettings Tuned(int index)
    {
        switch (index)
        {
            case 1:
                return new GrassSettings
                {
                    height = 0.86f, heightJitter = 0.22f, width = 0.016f, taperPower = 1.0f,
                    droop = 0.52f, droopJitter = 0.20f, twist = 22.0f, fold = 0.30f,
                    stiffness = 0.74f, stiffnessJitter = 0.22f, damping = 0.26f,
                    seedHeadChance = 0.46f, seedHeadLength = 0.12f, seedHeadGrains = 34,
                    seedHeadWidth = 0.0038f, seedHeadDroop = 0.35f,
                    bladeCount = 4200, clumpBias = 0.42f, dryShare = 0.10f
                };
            case 2:
                return new GrassSettings
                {
                    height = 1.15f, heightJitter = 0.30f, width = 0.009f, taperPower = 2.0f,
                    droop = 1.65f, droopJitter = 0.34f, twist = 64.0f, fold = 0.20f,
                    stiffness = 0.24f, stiffnessJitter = 0.38f, damping = 0.13f,
                    seedHeadChance = 0.62f, seedHeadLength = 0.21f, seedHeadGrains = 38,
                    seedHeadWidth = 0.0028f, seedHeadDroop = 2.6f,
                    bladeCount = 3000, clumpBias = 0.68f, dryShare = 0.42f
                };
            case 3:
                return new GrassSettings
                {
                    height = 0.74f, heightJitter = 0.24f, width = 0.030f, taperPower = 1.15f,
                    droop = 0.95f, droopJitter = 0.22f, twist = 16.0f, fold = 0.58f,
                    stiffness = 0.56f, stiffnessJitter = 0.20f, damping = 0.30f,
                    seedHeadChance = 0.14f, seedHeadLength = 0.13f, seedHeadGrains = 16,
                    seedHeadWidth = 0.008f, seedHeadDroop = 1.4f,
                    bladeCount = 2600, clumpBias = 0.74f, dryShare = 0.08f
                };
            case 4:
                return new GrassSettings
                {
                    height = 0.40f, heightJitter = 0.38f, width = 0.006f, taperPower = 1.6f,
                    droop = 1.25f, droopJitter = 0.40f, twist = 52.0f, fold = 0.26f,
                    stiffness = 0.30f, stiffnessJitter = 0.34f, damping = 0.17f,
                    seedHeadChance = 0.05f, seedHeadLength = 0.07f, seedHeadGrains = 8,
                    seedHeadWidth = 0.004f, seedHeadDroop = 1.9f,
                    bladeCount = 8200, clumpBias = 0.82f, clumpCell = 0.22f, dryShare = 0.16f
                };
            default:
                return new GrassSettings();
        }
    }
}
