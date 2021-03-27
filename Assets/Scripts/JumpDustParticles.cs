using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDustParticles : MonoBehaviour
{
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
