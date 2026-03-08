using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Header("Buttons")]
    public Button button1; // bomba
    public Button button2; // start round

    [Header("UI")]
    public RectTransform button1Transform;
    public TextMeshProUGUI phraseText;
    public CanvasGroup phraseCanvasGroup;

    [Header("Language Packs")]
    public LanguagePackSO[] languagePacks;

    [Header("Animation")]
    public float phraseScaleTime = 0.4f;

    private bool canPlay = false;
    private bool isRunning = false;

    void Start()
    {
        button1.interactable = false;

        button1.onClick.AddListener(OnButton1Clicked);
        button2.onClick.AddListener(OnButton2Clicked);

        phraseCanvasGroup.alpha = 0;
    }

    // =========================
    // BUTTON 2 – AKTYWACJA
    // =========================
    void OnButton2Clicked()
    {
        canPlay = true;
        button1.interactable = true;

        button1Transform
            .DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.5f);
    }

    // =========================
    // BUTTON 1 – START AKCJI
    // =========================
    void OnButton1Clicked()
    {
        if (!canPlay || isRunning)
            return;

        isRunning = true;

        // animacja klikniêcia
        Sequence seq = DOTween.Sequence();
        seq.Append(button1Transform.DOScale(0.9f, 0.1f));
        seq.Append(button1Transform.DOScale(1.1f, 0.15f).SetEase(Ease.OutBack));
        seq.Append(button1Transform.DOScale(1f, 0.1f));

        StartIdleAnimation();

        StartCoroutine(RunTimer());
    }

    void StartIdleAnimation()
    {
        button1Transform
            .DOScale(0.95f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    IEnumerator RunTimer()
    {
        var range = GameSettingsManager.GetBombTimeRange();
        int time = Random.Range(range.min, range.max + 1);

        yield return new WaitForSeconds(time);

    //    ShowRandomPhrase();

        button1Transform.DOKill();
        button1Transform.localScale = Vector3.one;

        isRunning = false;
    }

   /* void ShowRandomPhrase()
    {
        string lang = PlayerPrefs.GetString("SelectedLanguage", "EN");

        var pack = languagePacks.FirstOrDefault(p => p.languageCode == lang);

        if (pack == null || pack.phrases.Count == 0)
            return;

        string randomPhrase = pack.phrases[Random.Range(0, pack.phrases.Count)];

        phraseText.text = randomPhrase;

        Sequence seq = DOTween.Sequence();

        phraseCanvasGroup.alpha = 0;
        phraseText.transform.localScale = Vector3.zero;

        seq.Append(phraseCanvasGroup.DOFade(1f, 0.3f));
        seq.Join(
            phraseText.transform
                .DOScale(1f, phraseScaleTime)
                .SetEase(Ease.OutBack)
        );
    }*/
}