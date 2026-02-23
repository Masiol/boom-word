using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;

    [Header("Positions")]
    [SerializeField] private float shownY = 0f;
    [SerializeField] private float hiddenY = -1200f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    private bool isOpen = false;
    private bool isAnimating = false;

    private Tween moveTween;
    private Tween fadeTween;
    private Tween scaleTween;

    [SerializeField] private float openDelay = 0.2f;

    private void Awake()
    {
        // Stan pocz¹tkowy panelu
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, hiddenY);
        panel.localScale = Vector3.one * 0.9f;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Automatyczne podpiêcie przycisków
        if (settingsButton != null)
            settingsButton.onClick.AddListener(Open);

        if (backButton != null)
            backButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        if (isAnimating || isOpen) return;

        isAnimating = true;
        isOpen = true;

        KillTweens();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(openDelay);

        seq.Join(panel
            .DOAnchorPosY(shownY, duration)
            .SetEase(showEase)); 

        seq.Join(canvasGroup
            .DOFade(1f, duration)).SetDelay(openDelay); ;

        seq.Join(panel
            .DOScale(1f, duration)
            .SetEase(showEase).SetDelay(openDelay));

        seq.OnComplete(() =>
        {
            isAnimating = false;
        });
    }

    public void Close()
    {
        if (isAnimating || !isOpen) return;

        isAnimating = true;
        isOpen = false;

        KillTweens();

        moveTween = panel
            .DOAnchorPosY(hiddenY, duration / 2)
            .SetEase(hideEase);

        fadeTween = canvasGroup
            .DOFade(0f, duration);

        scaleTween = panel
            .DOScale(0.9f, duration)
            .SetEase(hideEase);

        moveTween.OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isAnimating = false;
        });

        FindObjectOfType<MenuAnimation>().PlayBackFromSettings();
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    private void KillTweens()
    {
        moveTween?.Kill();
        fadeTween?.Kill();
        scaleTween?.Kill();
    }
}