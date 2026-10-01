using UnityEngine;

public static class GrassTerrain
{
    private const int ReliefChannel = 53;
    private const float Lacunarity = 2.0f;
    private const float Gain = 0.5f;
    private const float SlopeStep = 0.04f;

    private static float Smooth(float edge)
    {
        return edge * edge * (3.0f - 2.0f * edge);
    }

    private static float Lattice(Vector2 place, int octave, int seed)
    {
        int cellX = Mathf.FloorToInt(place.x);
        int cellZ = Mathf.FloorToInt(place.y);
        float fadeX = Smooth(place.x - cellX);
        float fadeZ = Smooth(place.y - cellZ);
        int channel = ReliefChannel + octave;

        float lowerLeft = GrassField.Hash01(cellX, cellZ, channel, seed);
        float lowerRight = GrassField.Hash01(cellX + 1, cellZ, channel, seed);
        float upperLeft = GrassField.Hash01(cellX, cellZ + 1, channel, seed);
        float upperRight = GrassField.Hash01(cellX + 1, cellZ + 1, channel, seed);

        return Mathf.Lerp(Mathf.Lerp(lowerLeft, lowerRight, fadeX),
                          Mathf.Lerp(upperLeft, upperRight, fadeX), fadeZ);
    }

    public static float Height(Vector2 place, GrassSettings settings)
    {
        float cell = Mathf.Max(settings.reliefCell, 1e-3f);
        float sum = 0.0f;
        float weight = 0.0f;
        float amplitude = 1.0f;
        float frequency = 1.0f / cell;

        for (int octave = 0; octave < Mathf.Max(settings.reliefOctaves, 1); octave++)
        {
            sum += Lattice(place * frequency, octave, settings.seed) * amplitude;
            weight += amplitude;
            amplitude *= Gain;
            frequency *= Lacunarity;
        }

        return (sum / weight - 0.5f) * 2.0f * settings.reliefHeight;
    }

    public static Vector3 Normal(Vector2 place, GrassSettings settings)
    {
        float east = Height(place + Vector2.right * SlopeStep, settings);
        float west = Height(place - Vector2.right * SlopeStep, settings);
        float north = Height(place + Vector2.up * SlopeStep, settings);
        float south = Height(place - Vector2.up * SlopeStep, settings);
        return new Vector3(west - east, 2.0f * SlopeStep, south - north).normalized;
    }

    public static Vector3 Seat(Vector3 place, GrassSettings settings)
    {
        Vector2 flat = new Vector2(place.x, place.z);
        return new Vector3(place.x, Height(flat, settings), place.z);
    }
}
