using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{
    public Color backgroundColor;
    public Color platformColor;
    public Color scarfColor;
    public Color playerColor;

    public bool IsSame(ColorPalette other)
    {
        if (!other)
            return false;

        return other.backgroundColor.Equals(backgroundColor) &&
            other.platformColor.Equals(platformColor) &&
            other.scarfColor.Equals(scarfColor) &&
            other.playerColor.Equals(playerColor);
    }
}
