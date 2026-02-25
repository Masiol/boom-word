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

    [Header("Distances")]
    public float uiSlideDistance = 800f;
    public float jumpHeight = 300f;
    public float landingY = 0f;
    public float panelSlideDistance = 1200f;

    [Header("Timings")]
    public float uiSlideTime = 0.4f;
    public float riseTime = 0.3f;
    public float fallTime = 0.5f;
    public float panelSlideTime = 0.5f;

    [Header("Easing")]
    public Ease uiEase = Ease.InBack;
    public Ease riseEase = Ease.OutExpo;
    public Ease fallEase = Ease.InExpo;
    public Ease panelEase = Ease.OutCubic;

    Vector2 bombStartPos;
    Vector2 textStartPos;
    Vector2 buttonsStartPos;
    Vector2 panelStartPos;

    Sequence startSequence;
    Sequence settingsSequence;

    Tween bombIdleScale;
    Tween bombIdleRotate;

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

        startSequence.Join(
            logoText.DOAnchorPosX(textStartPos.x + uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        startSequence.Join(
            buttonsParent.DOAnchorPosX(buttonsStartPos.x - uiSlideDistance, uiSlideTime)
                .SetEase(uiEase)
        );

        startSequence.Append(
            logoBomb.DOScale(new Vector3(1.15f, 0.75f, 1f), 0.12f)
        );

        startSequence.Append(
            logoBomb.DOScale(new Vector3(0.9f, 1.2f, 1f), 0.15f)
        );

        startSequence.Join(
            logoBomb.DOAnchorPosY(bombStartPos.y + jumpHeight, riseTime)
                .SetEase(riseEase)
        );

        startSequence.Append(
            logoBomb.DOAnchorPosY(landingY, fallTime)
                .SetEase(fallEase)
        );

        startSequence.Join(
            logoBomb.DOScale(Vector3.one, fallTime)
        );

        startSequence.Join(
            packagePanel.DOAnchorPosX(panelStartPos.x, panelSlideTime)
                .SetEase(panelEase)
        );
    }

    public void PlayStart()
    {
        StopBombIdle();

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

        startSequence.PlayForward();
    }

    public void ReverseStart()
    {
        startSequence.PlayBackwards();

        if (startCanvasGroup != null)
        {
            startCanvasGroup.DOKill();

            startCanvasGroup.interactable = false;
            startCanvasGroup.blocksRaycasts = false;

            startCanvasGroup
                .DOFade(0f, 0.2f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    startCanvasGroup.gameObject.SetActive(false);
                });
        }

        startSequence.OnRewind(() =>
        {
            StartBombIdle();
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
                cg.interactable = false;
                cg.blocksRaycasts = false;
            });

            settingsSequence.Append(
                settingsPanel.DOScale(1f, 0.35f)
                    .SetEase(Ease.OutBack)
            );

            settingsSequence.Join(
                cg.DOFade(1f, 0.3f)
            );

            settingsSequence.AppendCallback(() =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });
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
                settingsPanel.gameObject.SetActive(false);
            }

            StartBombIdle();
        });
    }

    // =========================
    // IDLE
    // =========================

    void StartBombIdle()
    {
        bombIdleScale = logoBomb
            .DOScale(0.9f, 1.2f)
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

    public void OnStartClicked()
    {
        PlayStart();
    }

    public void OnBackFromStart()
    {
        ReverseStart();
    }

    public void OnSettingsClicked()
    {
        PlaySettings();
    }

    public void OnBackFromSettings()
    {
        ReverseSettings();
    }
}