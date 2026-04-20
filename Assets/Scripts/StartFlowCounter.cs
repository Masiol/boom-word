using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartFlowController : MonoBehaviour
{
    public static StartFlowController Instance;
    public RectTransform gameChoosePanel;

    public Transform startButton;
    public Transform countdownParent;
    public Text countdownText;
    public Text infoText;

    public Button backButton;
    public ReloadSceneButton reloadSceneButton;

    [SerializeField] private RectTransform panel;
    [SerializeField] private float shownY = 0f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    void Awake()
    {
        Instance = this;
        startButton.localScale = Vector3.zero;
        countdownParent.localScale = Vector3.zero;
        infoText.DOFade(0, 0);

        backButton.onClick.AddListener(()=> FindObjectOfType<MenuAnimation>().ReverseStart());
    }

    public void ShowStartButton()
    {
        startButton.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }
    public void HideStartButton()
    {
        startButton.DOScale(0f, 0.25f).SetEase(Ease.OutBack);
    }

    public void OnStartClicked()
    {
        startButton.DOScale(0f, 0.2f);

        // 🧹 wyczyść wszystkie akcje przycisku back
        backButton.onClick.RemoveAllListeners();

        StartCoroutine(Countdown());
        gameChoosePanel.DOScale(0f, 0.12f).SetEase(Ease.OutBack);

        panel
            .DOAnchorPosY(shownY, duration)
            .SetEase(showEase);

        backButton.onClick.AddListener(() => FadeManager.Instance.FadeInAndReload());
        backButton.onClick.AddListener(() => SoundManager.Instance.StopLoop());
        FindObjectOfType<DynamicGridButton>().StartGenerate();


    }
    IEnumerator Countdown()
    {
        GameManager.Instance.StartGame();
        yield return new WaitForSeconds(0.5f);
        countdownParent.localScale = Vector3.zero;
        countdownParent.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        infoText.text = GetRandomText();
        infoText.DOFade(1, 0.3f).SetDelay(0.25f);

        for (int i = 3; i >= 0; i--)
        {
            countdownText.text = i.ToString();

            // 🎵 narastający pitch
            float pitch = 1f + (3 - i) * 0.08f;
            Debug.Log(pitch);
            SoundManager.Instance.Play(SoundID.Countdown, pitch);

            // animacja cyfry
            countdownText.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(
                countdownText.transform
                    .DOScale(1.25f, 0.2f)
                    .SetEase(Ease.OutBack)
            );

            seq.Append(
                countdownText.transform
                    .DOScale(1f, 0.15f)
                    .SetEase(Ease.OutQuad)
            );

            yield return new WaitForSeconds(1f);
        }
        //yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.Play(SoundID.FinishCountdown);
        

        countdownParent.DOScale(0f, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                GameManager.Instance.TakeRandomPhrase(false);
                infoText.DOFade(0, 0.2f);
            });
    }

    string GetRandomText()
    {
        List<string> texts = new List<string>();

        switch (GameSettingsManager.Language)
        {
            case "EN":
                texts.AddRange(new[]
                {
                "You start!",
                "Pass the phone to the person on your left.",
                "Pass the phone to the person on your right.",
                "Pass the phone to the person opposite you."
            });
                break;

            case "DE":
                texts.AddRange(new[]
                {
                "Du fängst an!",
                "Gib das Handy der Person links von dir.",
                "Gib das Handy der Person rechts von dir.",
                "Gib das Handy der Person dir gegenüber."
            });
                break;

            default: // PL
                texts.AddRange(new[]
                {
                "Ty zaczynasz!",
                "Podaj telefon osobie po lewej.",
                "Podaj telefon osobie po prawej.",
                "Podaj telefon osobie naprzeciwko."
            });
                break;
        }

        return texts[Random.Range(0, texts.Count)];
    }

}