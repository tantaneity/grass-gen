using System.Collections.Generic;
using UnityEngine;

public static class GrassField
{
    private const int ClumpChannel = 17;
    private const int BareChannel = 29;
    private const float FullTurn = Mathf.PI * 2.0f;
    private const float MinimumStiffness = 0.05f;

    public static float Hash01(int cellX, int cellZ, int channel, int seed)
    {
        uint hashed = (uint)(cellX * 73856093) ^ (uint)(cellZ * 19349663) ^ (uint)(channel * 83492791)
                      ^ (uint)(seed * 2654435761);
        hashed ^= hashed >> 13;
        hashed *= 0x85EBCA6B;
        hashed ^= hashed >> 16;
        return (hashed & 0xFFFFFF) / (float)0xFFFFFF;
    }

    private static float ClumpDensity(Vector3 place, GrassSettings settings)
    {
        float cell = Mathf.Max(settings.clumpCell, 1e-3f);
        int cellX = Mathf.FloorToInt(place.x / cell);
        int cellZ = Mathf.FloorToInt(place.z / cell);
        float draw = Hash01(cellX, cellZ, ClumpChannel, settings.seed);
        return Mathf.Lerp(1.0f, draw, settings.clumpBias);
    }

    private static bool IsBare(Vector3 place, GrassSettings settings)
    {
        float cell = Mathf.Max(settings.clumpCell, 1e-3f);
        int cellX = Mathf.FloorToInt(place.x / cell);
        int cellZ = Mathf.FloorToInt(place.z / cell);
        return Hash01(cellX, cellZ, BareChannel, settings.seed) < settings.bareChance;
    }

    public static float Cover(Vector3 place, GrassSettings settings)
    {
        return IsBare(place, settings) ? 0.0f : ClumpDensity(place, settings);
    }

    public static List<BladePlan> Sow(GrassSettings settings, GrassPalette palette)
    {
        System.Random dice = new System.Random(settings.seed);
        List<BladePlan> blades = new List<BladePlan>(settings.bladeCount);
        Vector2 heading = settings.windHeading.normalized;

        int attempts = settings.bladeCount * 3;
        while (blades.Count < settings.bladeCount && attempts-- > 0)
        {
            float angle = (float)dice.NextDouble() * FullTurn;
            float reach = Mathf.Sqrt((float)dice.NextDouble()) * settings.fieldRadius;
            Vector3 place = GrassTerrain.Seat(
                new Vector3(Mathf.Cos(angle) * reach, 0.0f, Mathf.Sin(angle) * reach), settings);

            if (IsBare(place, settings) || dice.NextDouble() > ClumpDensity(place, settings))
            {
                continue;
            }

            float spin = (float)dice.NextDouble() * FullTurn;
            float age = (float)dice.NextDouble();
            float dryness = (float)dice.NextDouble() < settings.dryShare ? (float)dice.NextDouble() : 0.0f;
            float shade = 1.0f + ((float)dice.NextDouble() - 0.5f) * settings.shadeJitter;

            blades.Add(new BladePlan
            {
                root = place,
                height = settings.height * (1.0f + ((float)dice.NextDouble() - 0.5f) * settings.heightJitter * 2.0f),
                width = settings.width * (1.0f + ((float)dice.NextDouble() - 0.5f) * settings.widthJitter * 2.0f),
                stiffness = Mathf.Max(settings.stiffness
                                      * (1.0f + ((float)dice.NextDouble() - 0.5f) * settings.stiffnessJitter * 2.0f),
                                      MinimumStiffness),
                droop = settings.droop * (1.0f + ((float)dice.NextDouble() - 0.5f) * settings.droopJitter * 2.0f),
                twist = settings.twist + ((float)dice.NextDouble() - 0.5f) * settings.twistJitter * 2.0f,
                lean = settings.lean,
                slope = GrassTerrain.Normal(new Vector2(place.x, place.z), settings),
                facing = Vector2.Lerp(new Vector2(Mathf.Cos(spin), Mathf.Sin(spin)), heading, 0.45f).normalized,
                fill = palette.Blend(Mathf.Lerp(settings.youngShare, 1.0f, age), dryness, shade),
                hasSeedHead = dice.NextDouble() < settings.seedHeadChance
            });
        }

        return blades;
    }

    public static WindGust[] Gusts(GrassSettings settings)
    {
        WindGust[] gusts = new WindGust[Mathf.Max(settings.gustCount, 1)];
        for (int i = 0; i < gusts.Length; i++)
        {
            float step = Mathf.Pow(settings.gustSpread, i);
            gusts[i] = new WindGust
            {
                frequency = settings.gustFrequency * step,
                strength = settings.windStrength * Mathf.Pow(settings.gustFalloff, i),
                wavelength = settings.gustWavelength / step,
                phase = Hash01(i, 0, 7, settings.seed) * FullTurn
            };
        }

        return gusts;
    }
}
