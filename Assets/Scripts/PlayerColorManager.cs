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
    public Material platformMat;
    public Material playerMat;
    public Material backgroundMat;

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
            playerLight.color = colorPalette.playerColor;
            playerLight.intensity = colorPalette.playerLightIntensity;
            playerLight.shadowIntensity = colorPalette.playerLightShadowIntensity;
        }

        if (playerMat)
        {
            playerMat.SetColor("_Color", colorPalette.playerColor);
        }

        if (backgroundMat)
        {
            backgroundMat.SetColor("_Color", colorPalette.backgroundColor);
        }

        if (platformMat)
        {
            platformMat.SetColor("_Color", colorPalette.platformColor);
        }

    }
}
