using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Transform flagTransform;
    public float riseToY = 1f;
    public float riseDuration = 0.75f;

    private int riseTweenId = 0;
    private Vector2 startPos;

    void Awake()
    {
        startPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) {
            if (riseTweenId > 0 && LeanTween.isTweening(riseTweenId))
            {
                LeanTween.cancel(riseTweenId);
            }

            riseTweenId = LeanTween.moveLocalY(flagTransform.gameObject, riseToY, riseDuration).id;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) {
            if (riseTweenId > 0 && LeanTween.isTweening(riseTweenId))
            {
                LeanTween.cancel(riseTweenId);
            }

            riseTweenId = LeanTween.moveLocalY(flagTransform.gameObject, 0, riseDuration).id;
        }
    }
}
