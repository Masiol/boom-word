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

    void Awake()
    {
        phraseParent.DOScale(0f, 0);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            1.15f,
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

    void StartBomb()
    {
        bombStarted = true;
        bombTimer = BombTime;
        bombAccelerated = false;

        SoundManager.Instance.PlayLoop(SoundID.ClockLoop);

        StartBombAnimation();
    }

    void StartBombAnimation()
    {
        if (bombTween != null)
            bombTween.Kill();

        bombTransform.localScale = Vector3.one;

        bombTween = bombTransform
            .DOScale(1.1f, 0.65f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    void BombExplode()
    {
        bombStarted = false;

        if (bombTween != null)
            bombTween.Kill();

        SoundManager.Instance.StopLoop();
        SoundManager.Instance.Play(SoundID.BombExplode);

        Debug.Log("BOOM");
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

    public void TakeRandomPhrase()
    {
        if (!bombStarted)
        {
            StartBomb();
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
}