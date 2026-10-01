using UnityEngine;

public struct TrampleSpot
{
    public Vector2 at;
    public float radius;
    public float strength;
}

public static class GrassTrample
{
    private const float Falloff = 1.7f;
    private const float MinimumRadius = 1e-3f;
    private const float FlattenShare = 0.55f;

    public static Vector2 Push(TrampleSpot[] spots, Vector3 root)
    {
        Vector2 total = Vector2.zero;
        if (spots == null)
        {
            return total;
        }

        Vector2 place = new Vector2(root.x, root.z);
        foreach (TrampleSpot spot in spots)
        {
            Vector2 away = place - spot.at;
            float reach = Mathf.Max(spot.radius, MinimumRadius);
            float distance = away.magnitude;
            if (distance >= reach)
            {
                continue;
            }

            float weight = Mathf.Pow(1.0f - distance / reach, Falloff) * spot.strength;
            Vector2 direction = distance > MinimumRadius ? away / distance : Vector2.right;
            total += direction * weight;
        }

        return total;
    }

    public static float Flatten(Vector2 push)
    {
        return 1.0f / (1.0f + push.magnitude * FlattenShare);
    }

    public static TrampleSpot[] Parse(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return null;
        }

        string[] groups = raw.Split(';');
        TrampleSpot[] spots = new TrampleSpot[groups.Length];
        for (int i = 0; i < groups.Length; i++)
        {
            string[] parts = groups[i].Split(',');
            if (parts.Length != 4)
            {
                throw new System.FormatException($"trample spot '{groups[i]}' needs x,z,radius,strength");
            }

            spots[i] = new TrampleSpot
            {
                at = new Vector2(Number(parts[0]), Number(parts[1])),
                radius = Number(parts[2]),
                strength = Number(parts[3])
            };
        }

        return spots;
    }

    private static float Number(string raw)
    {
        return float.Parse(raw.Trim(), System.Globalization.CultureInfo.InvariantCulture);
    }
}
