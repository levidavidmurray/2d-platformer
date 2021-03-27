using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PlayerColorManager : MonoBehaviour
{
    [Header("Colors")]
    public ColorPalette colorPalette;

    [Header("Game Objects")]
    public SpriteRenderer playerRenderer;
    public SpriteRenderer scarfRenderer;
    public TrailRenderer trailRenderer;
    public Material platformMaterial;

    private void Update()
    {
        UpdateColors();
    }

    [ContextMenu("Update Colors")]
    void UpdateColors()
    {
        if (playerRenderer && scarfRenderer && trailRenderer)
        {
            playerRenderer.color = colorPalette.playerColor;
            scarfRenderer.color = colorPalette.scarfColor;
            trailRenderer.startColor = colorPalette.scarfColor;
            trailRenderer.endColor = colorPalette.scarfColor;
        }

        if (platformMaterial)
        {
            platformMaterial.SetColor("_Color", colorPalette.platformColor);
        }

        Camera.main.backgroundColor = colorPalette.backgroundColor;
    }
}
