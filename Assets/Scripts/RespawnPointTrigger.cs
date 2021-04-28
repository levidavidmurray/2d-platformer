using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointTrigger : MonoBehaviour
{
    private Transform spawnPoint;

    void Awake()
    {
        if (transform.childCount <= 0)
        {
            spawnPoint = transform;
        }
        else
        {
            spawnPoint = transform.GetChild(0);
        }
    }

    public Transform SpawnPoint { get { return spawnPoint; } }

}
