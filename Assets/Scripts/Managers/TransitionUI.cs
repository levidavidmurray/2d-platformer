using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    public RectTransform transitionImage;

    private Vector2 swipeLeftStartPos;
    private Vector2 swipeRightStartPos;

    private void Awake()
    {
        swipeLeftStartPos = transitionImage.anchoredPosition;
        swipeRightStartPos = -swipeLeftStartPos;
    }

    public void swipeInOut(float duration, float transitionDelay = 0f, float holdDelay = 0f)
    {
        transitionImage.anchoredPosition = swipeLeftStartPos;
        LeanTween.moveX(transitionImage, 0, duration).setDelay(transitionDelay).setOnComplete(() => {
            LeanTween.moveX(transitionImage, swipeLeftStartPos.x, duration).setDelay(holdDelay);
        });
    }

}
