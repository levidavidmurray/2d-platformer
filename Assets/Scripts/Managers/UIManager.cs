using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coordinates
{
    public readonly float x;
    public readonly float y;

    public Coordinates(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "{ x: " + this.x.ToString() + ", y: " + this.y.ToString() + " }";
    }
}

public struct ScreenBounds
{
    public readonly Coordinates min;
    public readonly Coordinates max;

    public ScreenBounds(float xMin, float yMin, float xMax, float yMax)
    {
        this.min = new Coordinates(xMin, yMin);
        this.max = new Coordinates(xMax, yMax);
    }
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance { get { return _instance; } }

    public static DebugUI DebugUI { get { return Instance.debugUI; } }

    public static TransitionUI TransitionUI { get { return Instance.transitionUI; } }

    public static ScreenBounds Screen { get { return Instance.screenBounds; } }

    [Header("Debug")]
    public bool showDebug;

    [Header("UI")]
    public DebugUI debugUI;
    public TransitionUI transitionUI;

    public ScreenBounds screenBounds { get; private set; }

    void Awake()
    {
        this.SetScreenBounds();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (debugUI.gameObject.activeInHierarchy != showDebug)            
        {
            debugUI.gameObject.SetActive(showDebug);
        }
    }

    private void SetScreenBounds() {
        Camera camera = Camera.main;

        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        this.screenBounds = new ScreenBounds(
            -halfWidth,
            -halfHeight,
            halfWidth,
            halfHeight
        );
    }
}
