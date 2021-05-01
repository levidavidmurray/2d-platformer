using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerTrackEffects : MonoBehaviour
{
    public TrailRenderer Trail { get; private set; }
    public Light2D Light { get; private set; }

    void Awake()
    {
        Trail = GetComponentInChildren<TrailRenderer>();
        Light = GetComponentInChildren<Light2D>();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
