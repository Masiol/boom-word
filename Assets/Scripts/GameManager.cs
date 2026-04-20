using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
    Normal,
    Full
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameMode SelectedMode { get; private set; }

    public bool SoundEnabled { get; private set; }
    public bool VibrationEnabled { get; private set; }
    public int PlayersCount { get; private set; }
    public int BombMinTime { get; private set; }
    public int BombMaxTime { get; private set; }
    public string Language { get; private set; }

    public bool GameStarted;
    public float BombTime { get; private set; }

    float bombTimer;
    bool bombStarted;
    bool bombAccelerated;

    [Header("Phrases")]
    public List<LanguagePackSO> normalLanguagePacks;
    public List<LanguagePackSO> premiumLanguagePacks;

    [Header("UI")]
    public Text wordText;
    public Text placementText;

    public RectTransform phraseParent;

    [Header("Bomb")]
    public RectTransform bombTransform;

    Tween bombTween;

    public Button backButton;

    void Awake()
    {
        phraseParent.DOScale(0f, 0);

        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!bombStarted)
            return;

        bombTimer -= Time.deltaTime;

        // przyspieszenie przy 1/3 czasu
        if (!bombAccelerated && bombTimer <= BombTime / 3f)
        {
            bombAccelerated = true;

            IncreasePitchSmooth();

            if (bombTween != null)
                bombTween.timeScale = 1.3f;
        }

        if (bombTimer <= 0)
        {
            BombExplode();
        }
    }
    void IncreasePitchSmooth()
    {
        float pitch = 1f;

        DOTween.To(
            () => pitch,
            x =>
            {
                pitch = x;
                SoundManager.Instance.SetLoopPitch(pitch);
            },
            1.08f,
            1.5f
        );
    }

    public void SetGameMode(GameMode mode)
    {
        SelectedMode = mode;
    }

    public void StartGame()
    {
        GameStarted = true;

        CollectGameData();

        //SceneManager.LoadScene("GameScene");
    }

    void StartBomb(bool nextRound)
    {
        bombStarted = true;
        bombTimer = BombTime;
        bombAccelerated = false;

        // start od ciszy
        SoundManager.Instance.loopSource.volume = 0f;

        SoundManager.Instance.PlayLoop(SoundID.ClockLoop);

        // płynne zwiększenie głośności
        SoundManager.Instance.loopSource
            .DOFade(1f, 1f)
            .SetEase(Ease.Linear);

        StartBombAnimation(nextRound);
    }

    void StartBombAnimation(bool nextRound)
    {
        if (bombTween != null)
            bombTween.Kill();

        bombTransform.DOKill();

        if (!nextRound)
        {
            bombTransform.localScale = Vector3.one;

            bombTween = bombTransform
                .DOScale(1.1f, 0.65f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            bombTransform.localScale = Vector3.zero;

            bombTransform
                .DOScale(1f, 0.25f)
                .SetEase(Ease.OutBack)
                .SetDelay(0.35f) // 👈 delay pojawienia
                .OnComplete(() =>
                {
                    bombTween = bombTransform
                        .DOScale(1.1f, 0.65f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });
        }
    }
    void BombExplode()
    {

        phraseParent.GetComponent<CanvasGroup>().DOFade(0, 0.25f).SetDelay(0.4f);
    


        ExplodeWithEffect();
        FindObjectOfType<DynamicGridButton>().AnimateButtonPanel();
        Debug.Log("BOOM");
    }
    void ExplodeWithEffect()
    {
        bombStarted = false;

        if (bombTween != null)
            bombTween.Kill();

        SoundManager.Instance.StopLoop();

        Vector3 startScale = bombTransform.localScale;
        Image img = bombTransform.GetComponent<Image>();

        Sequence seq = DOTween.Sequence();

        // 🔺 napięcie przed wybuchem
        seq.Append(
            bombTransform.DOScale(startScale * 1.3f, 0.25f)
            .SetEase(Ease.OutQuad)
        );

        // 💥 DŹWIĘK idealnie w timing
        seq.AppendCallback(() =>
        {
            SoundManager.Instance.Play(SoundID.BombExplode);
        });

        // 💥 blast (główna eksplozja)
        seq.Append(
            bombTransform.DOScale(startScale * 2.5f, 0.4f)
            .SetEase(Ease.OutCubic)
        );

        // 📳 lekki shake dla efektu
        seq.Join(
            bombTransform.DOShakeScale(0.2f, 0.3f, 10, 90)
        );

        // 👻 fade out (jeśli masz Image)
        if (img != null)
        {
            seq.Join(img.DOFade(0f, 0.2f));
        }

        // cleanup
        seq.OnComplete(() =>
        {
            bombTransform.localScale = Vector3.zero;

            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }
        });
    }
    public PhraseElement GetRandomPhrase()
    {
        bool premium = SelectedMode == GameMode.Full;

        List<LanguagePackSO> list = premium ? premiumLanguagePacks : normalLanguagePacks;

        LanguagePackSO pack = list
            .FirstOrDefault(p => p.languageCode == Language);

        if (pack == null)
        {
            Debug.LogError("No language pack found for: " + Language);
            return null;
        }

        if (pack.endings.Count == 0 || pack.placements.Count == 0)
        {
            Debug.LogError("Language pack is missing data");
            return null;
        }

        string ending = pack.endings[Random.Range(0, pack.endings.Count)];
        string placement = pack.placements[Random.Range(0, pack.placements.Count)];

        Debug.Log($"Ending: {ending} | Placement: {placement}");

        return new PhraseElement(ending, placement);
    }

    public void TakeRandomPhrase(bool nextRound)
    {
        if (!bombStarted)
        {
            StartBomb(nextRound);
        }

        PhraseElement phrase = GetRandomPhrase();

        if (phrase == null)
            return;

        wordText.text = phrase.word;
        placementText.text = phrase.placement;

        phraseParent.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            phraseParent.DOScale(1.2f, 0.25f)
            .SetEase(Ease.OutBack)
        );

        seq.Append(
            phraseParent.DOScale(1f, 0.15f)
            .SetEase(Ease.OutQuad)
        );

        seq.Join(wordText.DOFade(1f, 0.2f));
        seq.Join(placementText.DOFade(1f, 0.2f));
    }

    void CollectGameData()
    {
        SoundEnabled = GameSettingsManager.SoundEnabled;
        VibrationEnabled = GameSettingsManager.VibrationEnabled;
        PlayersCount = GameSettingsManager.PlayersCount;

        var range = GameSettingsManager.GetBombTimeRange();

        BombMinTime = range.min;
        BombMaxTime = range.max;

        BombTime = Random.Range(BombMinTime, BombMaxTime + 1);

        Language = GameSettingsManager.Language;

        Debug.Log("Bomb time: " + BombTime);
    }

    public void SkipPhrase()
    {
        // 🔄 reset timera
        bombTimer = BombTime;
        bombAccelerated = false;

        // 🔊 reset dźwięku
        SoundManager.Instance.StopLoop();

        SoundManager.Instance.loopSource.volume = 0f;
        SoundManager.Instance.PlayLoop(SoundID.ClockLoop);

        SoundManager.Instance.loopSource
            .DOFade(1f, 0.3f)
            .SetEase(Ease.Linear);

        // reset pitch
        SoundManager.Instance.SetLoopPitch(1f);

        // 💣 reset animacji bomby
        if (bombTween != null)
            bombTween.Kill();

        StartBombAnimation(false);

        // 📝 nowa fraza
        PhraseElement phrase = GetRandomPhrase();

        if (phrase == null)
            return;

        wordText.text = phrase.word;
        placementText.text = phrase.placement;

        // animacja jak przy normalnym losowaniu
        phraseParent.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            phraseParent.DOScale(1.2f, 0.25f)
            .SetEase(Ease.OutBack)
        );

        seq.Append(
            phraseParent.DOScale(1f, 0.15f)
            .SetEase(Ease.OutQuad)
        );

        seq.Join(wordText.DOFade(1f, 0.2f));
        seq.Join(placementText.DOFade(1f, 0.2f));
    }


    public void StartNextRound()
    {
        Debug.Log("NEXT ROUND");
        DynamicGridButton grid = FindObjectOfType<DynamicGridButton>();
        if (grid != null)
        {
            grid.NextRound();
        }

        // 🔄 reset bomby
        bombStarted = false;
        bombAccelerated = false;
        bombTimer = 0;

        if (bombTween != null)
            bombTween.Kill();

       // bombTransform.localScale = Vector3.one;

        SoundManager.Instance.StopLoop();
        SoundManager.Instance.SetLoopPitch(1f);

        // 🔄 reset fraz UI
   //     bombTransform
   // .DOScale(0f, 0.1f);


        //phraseParent
          //  .DOScale(0f, 0.1f);


        CanvasGroup cg = phraseParent.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 1f;

        // 🔄 reset przycisków

       //    grid.ResetButtons();
        

        // 🔄 nowy czas bomby
        BombTime = Random.Range(BombMinTime, BombMaxTime + 1);

        // 🚀 start nowej rundy
        TakeRandomPhrase(true);
    }
}