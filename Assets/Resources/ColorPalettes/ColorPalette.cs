using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{

    [Header("Colors")]
    public Color backgroundColor;
    public Color platformColor;
    public Color playerColor;

    [Header("Player Light")]
    public float playerLightIntensity = 0.4f;
    public float playerLightShadowIntensity = 0.3f;

    public bool IsSame(ColorPalette other)
    {
        if (!other)
            return false;

        return other.backgroundColor.Equals(backgroundColor) &&
            other.platformColor.Equals(platformColor) &&
            other.playerColor.Equals(playerColor) &&
            other.playerLightIntensity.Equals(playerLightIntensity) &&
            other.playerLightShadowIntensity.Equals(playerLightShadowIntensity);
    }

    public void Copy(ColorPalette other)
    {
        backgroundColor = other.backgroundColor;
        platformColor = other.platformColor;
        playerColor = other.playerColor;
        playerLightIntensity = other.playerLightIntensity;
        playerLightShadowIntensity = other.playerLightShadowIntensity;
    }
}
