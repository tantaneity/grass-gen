using UnityEngine;

public sealed class GrassSettings
{
    public int seed = 1;

    public int bladeCount = 2400;
    public float fieldRadius = 2.6f;
    public float clumpCell = 0.34f;
    public float clumpBias = 0.55f;
    public float bareChance = 0.12f;

    public int segments = 9;
    public float height = 0.62f;
    public float heightJitter = 0.34f;
    public float width = 0.012f;
    public float widthJitter = 0.22f;
    public float shoulder = 0.18f;
    public float taperPower = 1.35f;
    public float fold = 0.34f;
    public float twist = 38.0f;
    public float twistJitter = 24.0f;
    public float droop = 0.44f;
    public float droopJitter = 0.26f;
    public float lean = 0.12f;

    public float stiffness = 0.42f;
    public float stiffnessJitter = 0.30f;
    public float damping = 0.22f;
    public float windPhase = 0.0f;
    public Vector2 windHeading = new Vector2(1.0f, 0.25f);
    public float windStrength = 0.55f;
    public float gustFrequency = 0.42f;
    public float gustWavelength = 3.4f;
    public int gustCount = 4;
    public float gustSpread = 1.72f;
    public float gustFalloff = 0.58f;

    public float seedHeadChance = 0.09f;
    public float seedHeadLength = 0.11f;
    public int seedHeadGrains = 11;
    public float seedHeadWidth = 0.0055f;
    public float seedHeadDroop = 1.8f;

    public float shadeJitter = 0.16f;
    public float youngShare = 0.28f;
    public float dryShare = 0.10f;
    public float outlineWeight = Outline.Small;
    public float depthBias = 0.0f;

    public GrassSettings Copy()
    {
        return (GrassSettings)MemberwiseClone();
    }
}
