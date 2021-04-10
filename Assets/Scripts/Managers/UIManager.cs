using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private static UIManager _instance;

    public static UIManager Instance { get { return _instance; } }

    [Header("Debug")]
    public bool showDebug;

    [Header("UI")]
    public DebugUI debugUI;

    public static DebugUI DebugUI { get { return Instance?.debugUI; } }

    void Awake()
    {
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
}
