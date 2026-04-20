using UnityEngine;

[ExecuteAlways]
public class StretchToParent : MonoBehaviour
{
    [SerializeField] private RectTransform target;

    private void Update()
    {
        if (target == null)
            return;

        var rect = target;

        rect.anchorMin = Vector2.zero; // left, bottom = 0
        rect.anchorMax = Vector2.one;  // right, top = 1
        rect.offsetMin = Vector2.zero; // left/bottom margin = 0
        rect.offsetMax = Vector2.zero; // right/top margin = 0

        // (opcjonalnie)
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localScale = Vector3.one;
    }
}
