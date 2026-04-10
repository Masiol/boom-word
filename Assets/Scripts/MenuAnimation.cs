using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    [Header("Logo")]
    public RectTransform logoBomb;
    public RectTransform logoText;

    [Header("Buttons Parent")]
    public RectTransform buttonsParent;

    [Header("Panels")]
    public RectTransform packagePanel;
    public RectTransform settingsPanel;

    [Header("Start Panel (Fade)")]
    public CanvasGroup startCanvasGroup;

    [Header("Premium Button")]
    public RectTransform premiumButton;
    public float premiumIdleScale = 0.04f;
    public float premiumIdleSpeed = 1.6f;

    [Header("Distances")]
    public float uiSlideDistance = 800f;
    public float jumpHeight = 300f;
    public float landingY = 0f;
    public float panelSlideDistance = 1200f;

    [Header("Bomb Jump Settings")]
    public float anticipationDown = 40f;
    public float anticipationTime = 0.06f;
    public float jumpPower = 1.15f;
    public float jumpUpTime = 0.22f;
    public float jumpDownTime = 0.38f;
    public float landSquashScale = 1.2f;
    public float landSquashTime = 0.08f;

    [Header("Timings")]
    public float uiSlideTime = 0.4f;
    public float panelSlideTime = 0.5f;

    [Header("Easing")]
    public Ease uiEase = Ease.InBack;
    public Ease panelEase = Ease.OutCubic;

    Vector2 bombStartPos;
    Vector2 textStartPos;
    Vector2 buttonsStartPos;
    Vector2 panelStartPos;

    Sequence startSequence;
    Sequence settingsSequence;

    Tween bombIdleScale;
    Tween bombIdleRotate;
    Tween premiumIdleTween;

    public Ease bombEaseIn;
    public Ease bombEaseOut;

    void Awake()
    {
        SaveInitialPositions();
        PreparePanels();
        BuildStartSequence();
        BuildSettingsSequence();
    }

    void Start()
    {
        StartBombIdle();
        StartPremiumIdle();
        
    }

    // =========================
    // SAVE POSITIONS
    // =========================

    void SaveInitialPositions()
    {
        bombStartPos = logoBomb.anchoredPosition;
        textStartPos = logoText.anchoredPosition;
        buttonsStartPos = buttonsParent.anchoredPosition;
        panelStartPos = packagePanel.anchoredPosition;
    }

    void PreparePanels()
    {
        packagePanel.anchoredPosition += Vector2.right * panelSlideDistance;

        if (settingsPanel != null)
        {
            settingsPanel.localScale = Vector3.zero;
            CanvasGroup cg = GetOrAddCanvasGroup(settingsPanel);
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        if (startCanvasGroup != null)
        {
            startCanvasGroup.alpha = 0f;
            startCanvasGroup.interactable = false;
            startCanvasGroup.blocksRaycasts = false;
            startCanvasGroup.gameObject.SetActive(false);
        }
    }

    // =========================
    // START TRANSITION
    // =========================

    void BuildStartSequence()
    {
        startSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .Pause();

        // UI slide
        startSequence.Join(
            logoText.DOAnchorPosX(textStartPos.x + uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        startSequence.Join(
            buttonsParent.DOAnchorPosX(buttonsStartPos.x - uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        // 🔥 ANTICIPATION
        startSequence.Join(
            logoBomb.DOAnchorPosY(bombStartPos.y - anticipationDown, anticipationTime)
                .SetEase(Ease.InQuad)
        );

        startSequence.Join(
            logoBomb.DOScale(new Vector3(1.2f, 0.8f, 1f), anticipationTime)
        );

        // 🔥 WYSTRZAŁ
        startSequence.Append(
            logoBomb.DOScale(new Vector3(0.85f, jumpPower, 1f), 0.1f)
        );

        startSequence.Join(
            logoBomb.DOAnchorPosY(bombStartPos.y + jumpHeight, jumpUpTime)
                .SetEase(Ease.OutCubic)
        );

        // 🔥 OPADANIE
        startSequence.Append(
            logoBomb.DOAnchorPosY(landingY, jumpDownTime)
                .SetEase(Ease.InCubic)
        );

        startSequence.Join(
            logoBomb.DOScale(Vector3.one, jumpDownTime)
        );

        // 🔥 LANDING SQUASH
        startSequence.Append(
            logoBomb.DOScale(new Vector3(landSquashScale, 0.85f, 1f), landSquashTime)
        );

        startSequence.Append(
            logoBomb.DOScale(Vector3.one, 0.12f)
                .SetEase(Ease.OutBack)
        );

        // Panel slide
        startSequence.Insert(0.4f, (
            packagePanel.DOAnchorPosX(panelStartPos.x, panelSlideTime)
                .SetEase(panelEase)
        ));
    }

    public void PlayStart()
    {
        StopBombIdle();

        startSequence.Restart();

        if (startCanvasGroup != null)
        {
            startCanvasGroup.gameObject.SetActive(true);
            startCanvasGroup.DOKill();

            startCanvasGroup.alpha = 0f;
            startCanvasGroup.interactable = false;
            startCanvasGroup.blocksRaycasts = false;

            startCanvasGroup
                .DOFade(1f, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    startCanvasGroup.interactable = true;
                    startCanvasGroup.blocksRaycasts = true;
                });
        }
    }

    public void ReverseStart()
    {
        StopBombIdle();

        Sequence reverse = DOTween.Sequence();

        // 🔥 Bomba znika OD RAZU (bardzo szybko)
        reverse.Join(
            logoBomb.DOScale(Vector3.zero, 0.08f)
                .SetEase(bombEaseIn)
        );

        // 🔥 UI wraca równolegle
        reverse.Join(
            logoText.DOAnchorPosX(textStartPos.x, 0.35f)
                .SetEase(Ease.OutCubic)
        );

        reverse.Join(
            buttonsParent.DOAnchorPosX(buttonsStartPos.x, 0.35f)
                .SetEase(Ease.OutCubic)
        );

        reverse.Join(
            packagePanel.DOAnchorPosX(panelStartPos.x + panelSlideDistance, 0.35f)
                .SetEase(Ease.InBack)
        );

        // 🔥 Reset pozycji po zniknięciu
        reverse.Insert(0.08f, DOTween.Sequence().AppendCallback(() =>
        {
            logoBomb.anchoredPosition = bombStartPos;
            logoBomb.localRotation = Quaternion.identity;
        }));

        // 🔥 Pojawienie z popem po powrocie UI
        reverse.Insert(0.1f,
            logoBomb.DOScale(1.15f, 0.18f)
                .SetEase(bombEaseOut)
        );

        reverse.Append(
            logoBomb.DOScale(1f, 0.1f)
        );

        // Fade panel
        if (startCanvasGroup != null)
        {
            startCanvasGroup.DOKill();
            startCanvasGroup.interactable = false;
            startCanvasGroup.blocksRaycasts = false;

            startCanvasGroup
                .DOFade(0f, 0.15f)
                .OnComplete(() =>
                {
                    startCanvasGroup.gameObject.SetActive(false);
                });
        }

        reverse.OnComplete(() =>
        {
            StartBombIdle();
            StartPremiumIdle();
        });
    }

    // =========================
    // SETTINGS TRANSITION
    // =========================

    void BuildSettingsSequence()
    {
        settingsSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .Pause();

        settingsSequence.Join(
            logoText.DOAnchorPosX(textStartPos.x + uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        settingsSequence.Join(
            logoBomb.DOAnchorPosX(bombStartPos.x + uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        settingsSequence.Join(
            buttonsParent.DOAnchorPosX(buttonsStartPos.x - uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        if (settingsPanel != null)
        {
            CanvasGroup cg = GetOrAddCanvasGroup(settingsPanel);

            settingsSequence.AppendCallback(() =>
            {
                settingsPanel.gameObject.SetActive(true);
                cg.alpha = 0f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });

            settingsSequence.Append(
                settingsPanel.DOScale(1f, 0.35f)
                    .SetEase(Ease.OutBack)
            );

            settingsSequence.Join(
                cg.DOFade(1f, 0.3f)
            );
        }
    }

    public void PlaySettings()
    {
        StopBombIdle();
        settingsSequence.PlayForward();
    }

    public void ReverseSettings()
    {
        settingsSequence.PlayBackwards();

        settingsSequence.OnRewind(() =>
        {
            if (settingsPanel != null)
            {
                CanvasGroup cg = GetOrAddCanvasGroup(settingsPanel);
                cg.interactable = false;
                cg.blocksRaycasts = false;
               // settingsPanel.gameObject.SetActive(false);
            }

            StartBombIdle();
            StartPremiumIdle();
        });
    }

    // =========================
    // IDLE
    // =========================

    void StartBombIdle()
    {
        bombIdleScale = logoBomb
            .DOScale(1.1f, 1.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        bombIdleRotate = logoBomb
            .DORotate(new Vector3(0, 0, 3f), 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void StopBombIdle()
    {
        bombIdleScale?.Kill();
        bombIdleRotate?.Kill();
       // premiumIdleTween?.Kill();
    }

    void StartPremiumIdle()
    {
        if (premiumButton == null) return;

        premiumIdleTween?.Kill();

      /*  premiumButton.localScale = Vector3.one * 1.05f;

        premiumIdleTween = premiumButton
            .DOScale(1.1f, premiumIdleSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);*/
    }

    // =========================
    // HELPERS
    // =========================

    CanvasGroup GetOrAddCanvasGroup(RectTransform target)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    // =========================
    // UI BUTTON HOOKS
    // =========================

    public void OnStartClicked() => PlayStart();
    public void OnBackFromStart() => ReverseStart();
    public void OnSettingsClicked() => PlaySettings();
    public void OnBackFromSettings() => ReverseSettings();
}