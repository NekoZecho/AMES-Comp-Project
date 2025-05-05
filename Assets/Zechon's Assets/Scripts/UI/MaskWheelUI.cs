using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MaskWheelUI : MonoBehaviour
{
    [System.Serializable]
    public class MaskSegment
    {
        public string maskID;
        public Image image; // UI Image for the segment
    }

    [Header("Segment Settings")]
    public List<MaskSegment> maskSegments = new List<MaskSegment>();
    public float radius = 150f;
    public float startAngle = 0f;
    public bool clockwise = true;

    [Header("Highlight Colors")]
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;

    private RectTransform wheelCenter;
    private int currentSelectionIndex = -1;

    void Awake()
    {
        wheelCenter = GetComponent<RectTransform>();
        ArrangeSegments();
    }

    void Update()
    {
        HighlightHoveredSegment();
    }

    void ArrangeSegments()
    {
        int count = maskSegments.Count;
        if (count == 0) return;

        float angleStep = 360f / count;
        float direction = clockwise ? -1f : 1f;

        for (int i = 0; i < count; i++)
        {
            float angle = (startAngle + angleStep * i * direction) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            RectTransform rt = maskSegments[i].image.rectTransform;
            rt.anchoredPosition = pos;
            rt.localRotation = Quaternion.identity;
        }
    }

    void HighlightHoveredSegment()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 dir = (mousePos - (Vector2)wheelCenter.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        float anglePerSegment = 360f / maskSegments.Count;
        int hoveredIndex = Mathf.FloorToInt((angle - startAngle + 360f) % 360f / anglePerSegment);
        hoveredIndex = Mathf.Clamp(hoveredIndex, 0, maskSegments.Count - 1);

        if (hoveredIndex != currentSelectionIndex)
        {
            currentSelectionIndex = hoveredIndex;
            for (int i = 0; i < maskSegments.Count; i++)
            {
                maskSegments[i].image.color = (i == currentSelectionIndex) ? highlightColor : defaultColor;
            }
        }
    }

    public string GetSelectedMaskID()
    {
        if (currentSelectionIndex >= 0 && currentSelectionIndex < maskSegments.Count)
        {
            return maskSegments[currentSelectionIndex].maskID;
        }

        return null;
    }
}
