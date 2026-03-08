using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class StartFlowController : MonoBehaviour
{
    public static StartFlowController Instance;
    public RectTransform gameChoosePanel;

    public Transform startButton;
    public Transform countdownParent;
    public Text countdownText;
    public Text infoText;

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
        StartCoroutine(Countdown());
        gameChoosePanel.DOScale(0f, 0.12f).SetEase(Ease.OutBack);

        panel
            .DOAnchorPosY(shownY, duration)
            .SetEase(showEase);
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
            SoundManager.Instance.Play(SoundID.Countdown);
            // 🔥 animacja cyfry
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

            // jeśli 0 → mocniejszy efekt
          /*  if (i == 0)
            {
                seq.Append(
                    countdownText.transform
                        .DOPunchScale(Vector3.one * 0.25f, 0.3f, 6, 0.6f)
                );
            }*/

            yield return new WaitForSeconds(1f);
        }

        countdownParent.DOScale(0f, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                GameManager.Instance.TakeRandomPhrase();
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