using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    [Header("Logo Elements")]
    public RectTransform logoBomb;
    public RectTransform logoText;

    [Header("Buttons")]
    public RectTransform startButton;
    public RectTransform settingsButton;
    public RectTransform howToPlayButton;

    [Header("Intro")]
    public float introDelay = 0.3f;

    [Header("Start Transition Settings")]

    [Space(10)]
    public float shrinkScale = 0.8f;
    public float shrinkTime = 0.15f;

    [Space(10)]
    public float jumpHeight = 300f;
    public float riseTime = 0.3f;
    public Ease riseEase = Ease.OutCubic;

    [Header("Landing Position")]
    public float landingY = 0f;

    [Space(10)]
    public float fallTime = 0.5f;
    public Ease fallEase = Ease.InQuad;

    [Space(10)]
    public float landPunchPower = 0.15f;
    public float landPunchTime = 0.25f;

    [Space(10)]
    public float uiSlideDistance = 800f;
    public float uiSlideTime = 0.4f;
    public Ease uiSlideEase = Ease.InBack;

    Vector2 bombStartPos;

    [Header("Settings Transition")]

    public RectTransform settingsPanel;

    public float settingsDuration = 0.35f;
    public Ease settingsEase = Ease.InBack;
    public float panelIntroTime = 0.45f;
    public Ease panelDropEase = Ease.OutBack;



    void Awake()
    {
        logoBomb.localScale = Vector3.zero;
        logoText.localScale = Vector3.zero;
        logoBomb.localRotation = Quaternion.identity;

        PrepareButton(startButton);
        PrepareButton(settingsButton);
        PrepareButton(howToPlayButton);

        bombStartPos = logoBomb.anchoredPosition;

        PrepareSettingsPanel();
    }

    void Start()
    {
        PlayIntro();
        SetupButton(startButton);
        SetupButton(settingsButton);
        SetupButton(howToPlayButton);

        startButton.GetComponent<Button>().onClick.AddListener(PlayStartTransition);
        settingsButton.GetComponent<Button>().onClick.AddListener(PlaySettingsTransition);

    }


    void PrepareSettingsPanel()
    {
        if (settingsPanel == null) return;

        CanvasGroup cg = GetOrAddCanvasGroup(settingsPanel);

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        settingsPanel.localScale = Vector3.zero;
    }

    void ShowSettingsPanel()
    {
        if (settingsPanel == null) return;

        CanvasGroup cg = GetOrAddCanvasGroup(settingsPanel);

        settingsPanel.gameObject.SetActive(true);

        // Stan początkowy
        settingsPanel.localScale = Vector3.zero;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            settingsPanel.DOScale(1f, panelIntroTime)
                .SetEase(panelDropEase)
        );

        seq.Join(
            cg.DOFade(1f, panelIntroTime)
        );

        seq.OnComplete(() =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        });
    }



    void PrepareButton(RectTransform button)
    {
        if (button == null) return;

        button.localScale = Vector3.one * 0.8f;

        CanvasGroup cg = button.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = button.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0;
    }

    // =========================
    // INTRO
    // =========================
    void PlayIntro()
    {
        Sequence intro = DOTween.Sequence().SetDelay(introDelay);

        intro.Append(
            DOTween.To(() => 0f, x =>
            {
                logoBomb.localScale = Vector3.one * x;
                logoText.localScale = Vector3.one * x;
            }, 1.1f, 0.5f).SetEase(Ease.OutBack)
        );

        intro.Append(
            DOTween.To(() => 1.1f, x =>
            {
                logoBomb.localScale = Vector3.one * x;
                logoText.localScale = Vector3.one * x;
            }, 1f, 0.15f)
        );

        intro.OnComplete(() =>
        {
            StartBombIdle();
            PlayButtonsIntro();
        });
    }

    void StartBombIdle()
    {
        logoBomb.DOScale(0.9f, 1.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        logoBomb.DORotate(new Vector3(0, 0, 3f), 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void PlayButtonsIntro()
    {
        AnimateButtonIntro(startButton, 0.2f);
        AnimateButtonIntro(settingsButton, 0.35f);
        AnimateButtonIntro(howToPlayButton, 0.5f);
    }

    void AnimateButtonIntro(RectTransform button, float delay)
    {
        CanvasGroup cg = button.GetComponent<CanvasGroup>();

        cg.DOFade(1f, 0.4f).SetDelay(delay);
        button.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetDelay(delay);
    }

    // =========================
    // START TRANSITION
    // =========================
    void PlayStartTransition()
    {
        logoBomb.DOKill();

        Sequence seq = DOTween.Sequence();

        // 🔹 UI zaczyna uciekać razem z pierwszym ruchem bomby
        logoText.DOAnchorPosX(logoText.anchoredPosition.x + uiSlideDistance, uiSlideTime)
            .SetEase(uiSlideEase);

        startButton.DOAnchorPosX(startButton.anchoredPosition.x - uiSlideDistance, uiSlideTime)
            .SetEase(uiSlideEase);

        settingsButton.DOAnchorPosX(settingsButton.anchoredPosition.x - uiSlideDistance, uiSlideTime)
            .SetEase(uiSlideEase);

        howToPlayButton.DOAnchorPosX(howToPlayButton.anchoredPosition.x - uiSlideDistance, uiSlideTime)
            .SetEase(uiSlideEase);

        // 1️⃣ Squash przed wybiciem
        seq.Append(
            logoBomb.DOScale(new Vector3(1.15f, 0.75f, 1f), 0.12f)
            .SetEase(Ease.OutQuad)
        );

        // 2️⃣ Stretch przy starcie
        seq.Append(
            logoBomb.DOScale(new Vector3(0.9f, 1.2f, 1f), 0.15f)
            .SetEase(Ease.OutQuad)
        );

        // 3️⃣ Wznoszenie
        seq.Join(
            logoBomb.DOAnchorPosY(bombStartPos.y + jumpHeight, riseTime)
            .SetEase(Ease.OutExpo)
        );

        // 4️⃣ Spadanie
        seq.Append(
            logoBomb.DOAnchorPosY(landingY, fallTime)
            .SetEase(Ease.InExpo)
        );

        // Powrót do normalnej skali w trakcie spadania
        seq.Join(
            logoBomb.DOScale(Vector3.one, fallTime)
            .SetEase(Ease.InCubic)
        );

        // 5️⃣ Mocny squash przy lądowaniu
        seq.Append(
            logoBomb.DOScale(new Vector3(1.25f, 0.75f, 1f), 0.1f)
        );

        // 🔥 Shake bomby
        seq.AppendCallback(() =>
        {
            logoBomb.DOShakeAnchorPos(
                0.18f,
                new Vector2(15f, 8f),
                15,
                90f,
                false,
                true
            );
        });

        // Powrót do normalnej skali
        seq.Append(
            logoBomb.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack)
        );

        seq.OnComplete(() =>
        {
          //  StartBombIdle();
        });
    }




    // =========================
    // BUTTON CLICK EFFECT
    // =========================
    void SetupButton(RectTransform button)
    {
        if (button == null) return;

        Button btn = button.GetComponent<Button>();
        if (btn == null) return;

        btn.onClick.AddListener(() =>
        {
            Sequence click = DOTween.Sequence();
            click.Append(button.DOScale(0.92f, 0.08f));
            click.Append(button.DOScale(1.05f, 0.1f).SetEase(Ease.OutBack));
            click.Append(button.DOScale(1f, 0.08f));
        });
    }

    void PlaySettingsTransition()
    {
        // Zatrzymujemy idle bomby
        logoBomb.DOKill();

        Sequence seq = DOTween.Sequence();
        /*
        // ======================
        // BOMB + TEXT
        // ======================

        CanvasGroup bombCG = GetOrAddCanvasGroup(logoBomb);
        CanvasGroup textCG = GetOrAddCanvasGroup(logoText);

        seq.Join(
            logoBomb.DOScale(settingsScaleDown, settingsDuration)
            .SetEase(settingsEase)
        );

        seq.Join(
            logoBomb.DOAnchorPosY(logoBomb.anchoredPosition.y + settingsMoveUp, settingsDuration)
            .SetEase(settingsEase)
        );

        seq.Join(
            bombCG.DOFade(0f, settingsDuration)
        );

        seq.Join(
            logoText.DOScale(settingsScaleDown, settingsDuration)
            .SetEase(settingsEase)
        );

        seq.Join(
            logoText.DOAnchorPosY(logoText.anchoredPosition.y + settingsMoveUp, settingsDuration)
            .SetEase(settingsEase)
        );

        seq.Join(
            textCG.DOFade(0f, settingsDuration)
        );

        // ======================
        // BUTTONS
        // ======================

        AnimateButtonHide(startButton);
        AnimateButtonHide(settingsButton);
        AnimateButtonHide(howToPlayButton);*/

        seq.OnComplete(() =>
        {
            ShowSettingsPanel();
        });
    }
    CanvasGroup GetOrAddCanvasGroup(RectTransform target)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    void AnimateButtonHide(RectTransform button)
    {
        if (button == null) return;

        CanvasGroup cg = GetOrAddCanvasGroup(button);

        button.DOScale(0.7f, settingsDuration)
            .SetEase(settingsEase);

        button.DORotate(new Vector3(0, 0, 10f), settingsDuration)
            .SetEase(settingsEase);

        cg.DOFade(0f, settingsDuration);
    }



}
