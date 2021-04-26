using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// [ExecuteAlways]
public class PlayerColorManager : MonoBehaviour
{
    [Header("Colors")]
    public ColorPalette[] colorPalettes;
    public ColorPalette colorPalette;

    [Header("Materials")]
    public Material color1_Mat;
    public Material color2_Mat;
    public Material color3_Mat;
    public Material color4_Mat;

    [Header("GameObjects")]
    public Light2D playerLight;

    private int currentPaletteIndex = 0;
    private ColorPalette colorPaletteThisFrame;

    public void UpdateForward()
    {
        currentPaletteIndex++;
        if (currentPaletteIndex >= colorPalettes.Length)
        {
            currentPaletteIndex = 0;
        }
        print($"UpdateForward: {currentPaletteIndex}");

        colorPalette = colorPalettes[currentPaletteIndex];
        colorPaletteThisFrame.Copy(colorPalette);

        UpdateColors();
    }

    public void UpdateBackward()
    {
        currentPaletteIndex--;
        if (currentPaletteIndex < 0)
        {
            currentPaletteIndex = colorPalettes.Length - 1;
        }
        print($"UpdateBackward: {currentPaletteIndex}");

        colorPalette = colorPalettes[currentPaletteIndex];
        colorPaletteThisFrame.Copy(colorPalette);

        UpdateColors();
    }

    private void Awake()
    {
        Object[] colorPaletteObjects = Resources.LoadAll<ColorPalette>("ColorPalettes");
        colorPalettes = new ColorPalette[colorPaletteObjects.Length];
        colorPaletteObjects.CopyTo(colorPalettes, 0);
        colorPaletteThisFrame = ScriptableObject.CreateInstance<ColorPalette>();

        for (int i = 0; i < colorPalettes.Length; i++)
        {
            if (colorPalette.IsSame(colorPalettes[i]))
            {
                currentPaletteIndex = i;
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ColorPaletteHasChanged())
        {
            UpdateColors();
        }
    }

    bool ColorPaletteHasChanged()
    {
        if (!colorPalette.IsSame(colorPaletteThisFrame))
        {
            colorPaletteThisFrame.Copy(colorPalette);
            return true;
        }

        return false;
    }

    [ContextMenu("Update Colors")]
    void UpdateColors()
    {
        colorPalette = colorPalettes[currentPaletteIndex];

        if (playerLight)
        {
            playerLight.color = colorPalette.color1;
            playerLight.intensity = colorPalette.playerLightIntensity;
            playerLight.shadowIntensity = colorPalette.playerLightShadowIntensity;
        }

        if (color1_Mat)
        {
            color1_Mat.SetColor("_Color", colorPalette.color1);
        }

        if (color2_Mat)
        {
            color2_Mat.SetColor("_Color", colorPalette.color2);
        }

        if (color3_Mat)
        {
            color3_Mat.SetColor("_Color", colorPalette.color3);
        }

        if (color4_Mat)
        {
            color4_Mat.SetColor("_Color", colorPalette.color4);
        }

    }
}
