using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{

    [Header("Colors")]
    [FormerlySerializedAs("playerColor")]
    public Color color1;
    [FormerlySerializedAs("backgroundColor")]
    public Color color2;
    [FormerlySerializedAs("platformColor")]
    public Color color3;
    [FormerlySerializedAs("spikeColor")]
    public Color color4 = Color.white;

    [Header("Player Light")]
    public float playerLightIntensity = 0.4f;
    public float playerLightShadowIntensity = 0.3f;

    public bool IsSame(ColorPalette other)
    {
        if (!other)
            return false;

        return other.color2.Equals(color2) &&
            other.color3.Equals(color3) &&
            other.color1.Equals(color1) &&
            other.color4.Equals(color4) &&
            other.playerLightIntensity.Equals(playerLightIntensity) &&
            other.playerLightShadowIntensity.Equals(playerLightShadowIntensity);
    }

    public void Copy(ColorPalette other)
    {
        color1 = other.color1;
        color2 = other.color2;
        color3 = other.color3;
        color4 = other.color4;
        playerLightIntensity = other.playerLightIntensity;
        playerLightShadowIntensity = other.playerLightShadowIntensity;
    }
}
