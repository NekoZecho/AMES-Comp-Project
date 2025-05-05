using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskWheelUI : MonoBehaviour
{
    [System.Serializable]
    public class MaskSegment
    {
        public string maskID;
        public Image image;
        public TMP_Text label; // new: optional label
        [HideInInspector] public RectTransform rect;
        [HideInInspector] public Vector2 targetPos;
    }

    [Header("Segment Settings")]
    public List<MaskSegment> maskSegments = new List<MaskSegment>();
    public float radius = 150f;
    public float startAngle = 0f;
    public bool clockwise = true;

    [Header("Animation")]
    public float fadeDuration = 0.2f;
    public float slideInTime = 0.25f;
    public float startRadiusMultiplier = 2f;

    [Header("Highlighting")]
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color labelDefaultColor = Color.white;
    public Color labelHighlightColor = Color.yellow;

    private RectTransform wheelCenter;
    private CanvasGroup canvasGroup;
    private int currentSelectionIndex = -1;

    void Awake()
    {
        wheelCenter = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        foreach (var seg in maskSegments)
            seg.rect = seg.image.rectTransform;

        ArrangeSegments();
        HideInstant();
    }

    void Update()
    {
        if (canvasGroup.alpha < 1f) return;
        HighlightNearestSegment();
    }

    public void ShowWheel()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAndSlideIn());
    }

    public void HideWheel()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    void HideInstant()
    {
        canvasGroup.alpha = 0;
        foreach (var seg in maskSegments)
        {
            seg.rect.localScale = Vector3.zero;
            seg.rect.anchoredPosition = Vector2.zero;
        }
    }

    IEnumerator FadeAndSlideIn()
    {
        float t = 0f;
        canvasGroup.alpha = 0;
        foreach (var seg in maskSegments)
        {
            seg.rect.localScale = Vector3.zero;
            seg.rect.anchoredPosition = seg.targetPos * startRadiusMultiplier;
        }

        while (t < slideInTime)
        {
            t += Time.unscaledDeltaTime;
            float percent = Mathf.Clamp01(t / slideInTime);

            canvasGroup.alpha = percent;

            foreach (var seg in maskSegments)
            {
                seg.rect.anchoredPosition = Vector2.Lerp(seg.targetPos * startRadiusMultiplier, seg.targetPos, percent);
                seg.rect.localScale = Vector3.one * percent;
            }

            yield return null;
        }

        canvasGroup.alpha = 1f;
        foreach (var seg in maskSegments)
        {
            seg.rect.anchoredPosition = seg.targetPos;
            seg.rect.localScale = Vector3.one;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        foreach (var seg in maskSegments)
            seg.rect.localScale = Vector3.zero;
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

            maskSegments[i].targetPos = pos;
            maskSegments[i].rect.anchoredPosition = pos;
            maskSegments[i].rect.localRotation = Quaternion.identity;
        }
    }

    void HighlightNearestSegment()
    {
        Vector2 mouse = Input.mousePosition;
        float closest = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < maskSegments.Count; i++)
        {
            Vector2 segPos = RectTransformUtility.WorldToScreenPoint(null, maskSegments[i].rect.position);
            float dist = Vector2.Distance(mouse, segPos);

            if (dist < closest)
            {
                closest = dist;
                closestIndex = i;
            }
        }

        if (closestIndex != currentSelectionIndex)
        {
            currentSelectionIndex = closestIndex;
            for (int i = 0; i < maskSegments.Count; i++)
            {
                bool selected = i == currentSelectionIndex;
                maskSegments[i].image.color = selected ? highlightColor : defaultColor;
                if (maskSegments[i].label != null)
                    maskSegments[i].label.color = selected ? labelHighlightColor : labelDefaultColor;
            }
        }
    }

    public string GetSelectedMaskID()
    {
        if (currentSelectionIndex >= 0 && currentSelectionIndex < maskSegments.Count)
            return maskSegments[currentSelectionIndex].maskID;

        return null;
    }
}
