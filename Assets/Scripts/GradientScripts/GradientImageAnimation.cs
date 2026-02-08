using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GradientImageAnimation : MonoBehaviour
{
    public Gradient gradient;
    public float duration = 2f;

    private Image image;
    private Color startColor;
    private Color endColor;

    [SerializeField] private float delay;


    private void Start()
    {
        image = GetComponent<Image>();

        Invoke("AnimateImageGradient", delay);
    }

    private void AnimateImageGradient()
    {
        startColor = gradient.Evaluate(0f);
        endColor = gradient.Evaluate(1f);

        image.color = startColor;

        DOTween.To(() => image.color, x => image.color = x, endColor, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .OnUpdate(() =>
            {
                image.color = image.color;
            });
    }
}