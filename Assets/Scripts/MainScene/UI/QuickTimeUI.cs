using UnityEngine;
using UnityEngine.UI;

public class QuickTimeUI : MonoBehaviour
{
    [SerializeField] private Image qteMarker;
    [SerializeField] private Image qteTarget;
    [SerializeField] private Image qteTargetClose;
    [SerializeField] private RectTransform qteBar;

    private float qteBarWidth;
    private float qteTargetPosition = 0.5f; // Default target position, can be set from Player script

    private void Start()
    {
        qteBarWidth = qteBar.rect.width;
        SetQTEPosition(0f);
        SetQTargetPosition(qteTargetPosition);
    }

    public void UpdateQTE(float markerPosition)
    {
        SetQTEPosition(markerPosition);
    }

    public void SetQTargetPosition(float targetPosition)
    {
        qteTargetPosition = targetPosition;
        qteTarget.rectTransform.anchoredPosition = GetCenteredPosition(targetPosition, qteTarget.rectTransform); ;
        qteTargetClose.rectTransform.anchoredPosition = qteTarget.rectTransform.anchoredPosition; // Align to the same position
    }

    private void SetQTEPosition(float markerPosition)
    {
        qteMarker.rectTransform.anchoredPosition = GetCenteredPosition(markerPosition, qteMarker.rectTransform);
    }

    private Vector2 GetCenteredPosition(float normalizedPosition, RectTransform rectTransform)
    {
        var position = Mathf.Clamp01(normalizedPosition); // Ensure position is within [0, 1]
        var offsetX = position * (qteBar.rect.width - rectTransform.rect.width) - (qteBar.rect.width / 2);
        return new Vector2(offsetX, rectTransform.anchoredPosition.y);
    }
}