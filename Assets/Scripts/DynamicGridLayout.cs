using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class DynamicGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private bool autoFit = true;         // Automatyczne dopasowanie liczby kolumn/wierszy
    [SerializeField] private int columns = 2;             // Używane tylko, jeśli autoFit = false
    [SerializeField] private Vector2 spacing = new Vector2(10, 10);
    [SerializeField] private Vector2 padding = new Vector2(10, 10);
    [SerializeField, Range(0f, 0.01f)] private float scaleThreshold = 0.001f; // Skalę poniżej tej wartości ignorujemy

    private RectTransform rectTransform;
    private readonly List<RectTransform> children = new List<RectTransform>();
    private int lastChildCount = -1;
    private readonly Dictionary<RectTransform, Vector3> lastScales = new Dictionary<RectTransform, Vector3>();

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        CacheChildren();
        UpdateLayout();
    }

    private void Update()
    {
        bool layoutNeedsUpdate = false;

        // 🔹 Sprawdź, czy zmieniła się liczba dzieci
        if (transform.childCount != lastChildCount)
        {
            CacheChildren();
            layoutNeedsUpdate = true;
        }

        // 🔹 Sprawdź, czy zmieniła się skala któregokolwiek dziecka
        foreach (var child in children)
        {
            if (child == null) continue;

            Vector3 currentScale = child.localScale;
            if (!lastScales.TryGetValue(child, out Vector3 prevScale))
            {
                lastScales[child] = currentScale;
                layoutNeedsUpdate = true;
                continue;
            }

            // Jeśli skala się zmieniła znacząco, aktualizujemy layout
            if ((currentScale - prevScale).sqrMagnitude > 0.0001f)
            {
                lastScales[child] = currentScale;
                layoutNeedsUpdate = true;
            }
        }

#if UNITY_EDITOR
        // 🔹 W edytorze zawsze aktualizujemy dla podglądu
        if (!Application.isPlaying)
            layoutNeedsUpdate = true;
#endif

        // 🔹 Aktualizacja layoutu, jeśli coś się zmieniło
        if (layoutNeedsUpdate)
            UpdateLayout();
    }

    private void CacheChildren()
    {
        children.Clear();
        lastScales.Clear();
        foreach (Transform t in transform)
        {
            if (t is RectTransform rt)
            {
                children.Add(rt);
                lastScales[rt] = rt.localScale;
            }
        }
        lastChildCount = transform.childCount;
    }

    public void UpdateLayout()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // 🔹 Wybieramy tylko aktywne i widoczne (skala > 0) elementy
        List<RectTransform> visibleChildren = new List<RectTransform>();
        foreach (var child in children)
        {
            if (child == null || !child.gameObject.activeSelf)
                continue;

            Vector3 s = child.localScale;
            if (Mathf.Abs(s.x) < scaleThreshold ||
                Mathf.Abs(s.y) < scaleThreshold ||
                Mathf.Abs(s.z) < scaleThreshold)
                continue;

            visibleChildren.Add(child);
        }

        if (visibleChildren.Count == 0)
            return;

        // 🔹 Wylicz kolumny i wiersze
        int cols, rows;
        if (autoFit)
        {
            cols = Mathf.CeilToInt(Mathf.Sqrt(visibleChildren.Count));
            rows = Mathf.CeilToInt(visibleChildren.Count / (float)cols);
        }
        else
        {
            cols = Mathf.Max(1, columns);
            rows = Mathf.CeilToInt(visibleChildren.Count / (float)cols);
        }

        // 🔹 Oblicz przestrzeń
        float totalWidth = rectTransform.rect.width - padding.x * 2;
        float totalHeight = rectTransform.rect.height - padding.y * 2;

        float cellWidth = (totalWidth - spacing.x * (cols - 1)) / cols;
        float cellHeight = (totalHeight - spacing.y * (rows - 1)) / rows;

        // 🔹 Ustawienie rozmiaru i pozycji
        for (int i = 0; i < visibleChildren.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            float x = padding.x + col * (cellWidth + spacing.x);
            float y = -(padding.y + row * (cellHeight + spacing.y));

            RectTransform child = visibleChildren[i];
            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(0, 1);
            child.pivot = new Vector2(0, 1);
            child.sizeDelta = new Vector2(cellWidth, cellHeight);
            child.anchoredPosition = new Vector2(x, y);
        }
    }
}
