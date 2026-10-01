using UnityEngine;

public sealed class GrassPalette
{
    public Color young;
    public Color mature;
    public Color dry;
    public Color seedHead;
    public Color ink;
    public Color background;
    public Color earth;
    public Color rim;
    public Color shadow;

    public static GrassPalette Meadow => new GrassPalette
    {
        young = Hex(0x8aa84e),
        mature = Hex(0x5f7f3c),
        dry = Hex(0xb0a25c),
        seedHead = Hex(0x9c8a52),
        ink = Hex(0x2b2a22),
        background = Hex(0xf6f3ec),
        earth = Hex(0x56523d),
        rim = Hex(0x8d886f),
        shadow = Hex(0x343a28)
    };

    public static GrassPalette Autumn => new GrassPalette
    {
        young = Hex(0xa8a455),
        mature = Hex(0x8d7c42),
        dry = Hex(0xc0a768),
        seedHead = Hex(0xa98f55),
        ink = Hex(0x2f271d),
        background = Hex(0xf6f3ec),
        earth = Hex(0x61532f),
        rim = Hex(0x9a8e6c),
        shadow = Hex(0x453a22)
    };

    public Color Blend(float age, float dryness, float shade)
    {
        Color living = Color.Lerp(young, mature, age);
        Color tinted = Color.Lerp(living, dry, dryness);
        return new Color(tinted.r * shade, tinted.g * shade, tinted.b * shade, 1.0f);
    }

    public static Color Hex(uint value)
    {
        float red = ((value >> 16) & 0xff) / 255.0f;
        float green = ((value >> 8) & 0xff) / 255.0f;
        float blue = (value & 0xff) / 255.0f;
        return new Color(red, green, blue, 1.0f);
    }
}
