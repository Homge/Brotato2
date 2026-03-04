using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class HopOnTap : MonoBehaviour, IPointerDownHandler
{

    [Header(" Elements ")]
    private RectTransform rt;
    private Vector2 initialPosition;

    private void Awake() => rt = GetComponent<RectTransform>();
    private void Start() => initialPosition = rt.anchoredPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        float targetY = initialPosition.y + Screen.height / 50;

        LeanTween.cancel(gameObject);
        LeanTween.moveY(rt, targetY, .6f)
            .setEase(LeanTweenType.punch)
            .setIgnoreTimeScale(true);
    }
}
