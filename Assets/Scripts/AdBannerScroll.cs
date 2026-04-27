using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum GameTypeAd
{
    ChooseWho,
    SeventyFiveHard,
    TruthOrDare,
    TicTacWord
}

public enum GameLanguage
{
    Polish,
    English,
    German
}

[System.Serializable]
public class AdLanguageData
{
    public GameLanguage language;
    public Sprite sprite;
    public string url;
}

[System.Serializable]
public class AdGameData
{
    public GameTypeAd game;
    public List<AdLanguageData> languageVariants = new List<AdLanguageData>();
}

public class AdBannerScroll : MonoBehaviour
{
    [Header("References")]
    public RectTransform adBannerHolder;
    public GameObject adPrefab;

    [Header("Settings")]
    public List<AdGameData> games;
    public float spacing = 20f;
    public float moveDuration = 0.5f;
    public float autoMoveDelay = 2f;

    [Header("Single Ad Mode")]
    public bool showSingleGame = false;
    public GameTypeAd fixedGame;

    [Header("Pulse Effect (Single Ad)")]
    public float pulseDuration = 1.5f;
    public float pulseScale = 1.05f;

    private GameLanguage currentLanguage;
    private List<RectTransform> spawnedAds = new List<RectTransform>();
    private Dictionary<string, GameTypeAd> urlGameMap = new Dictionary<string, GameTypeAd>();

    private float adWidth;
    private bool isMoving = false;
    private float timer = 0f;
    private Tween pulseTween;

    // =========================
    // UNITY
    // =========================

    void Start()
    {
        LoadLanguageFromPrefs();
        StartCoroutine(SpawnNextFrame());
    }

    void Update()
    {
        if (showSingleGame) return;
        if (spawnedAds.Count == 0) return;

        if (!isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= autoMoveDelay)
            {
                timer = 0f;
                MoveNext();
            }
        }
    }

    private IEnumerator SpawnNextFrame()
    {
        yield return null; // czekamy aż layout się policzy
        SpawnAds();
    }

    // =========================
    // LANGUAGE
    // =========================

    private void LoadLanguageFromPrefs()
    {
        int langValue = PlayerPrefs.GetInt("SelectedLanguage", 1);
        currentLanguage = langValue switch
        {
            0 => GameLanguage.Polish,
            1 => GameLanguage.English,
            2 => GameLanguage.German,
            _ => GameLanguage.English
        };
    }

    // =========================
    // SPAWN LOGIC
    // =========================

    private void SpawnAds()
    {
        if (games == null || games.Count == 0 || adPrefab == null || adBannerHolder == null)
        {
            Debug.LogError("[AdBanner] Missing setup data");
            return;
        }

        ClearHolder();

        adWidth = adBannerHolder.rect.width;
        List<AdLanguageData> selectedAds = GetAdsForCurrentLanguage();

        if (selectedAds.Count == 0)
        {
            Debug.LogWarning("[AdBanner] No language ads found, using emergency fallback");
            EmergencyFallback();
            return;
        }

        if (!showSingleGame)
            selectedAds.AddRange(selectedAds);

        BuildUrlMap();

        for (int i = 0; i < selectedAds.Count; i++)
        {
            SpawnSingleAd(selectedAds[i], i);
        }

        if (showSingleGame && spawnedAds.Count == 1)
            StartPulseEffect(adBannerHolder);
    }

    private List<AdLanguageData> GetAdsForCurrentLanguage()
    {
        List<AdLanguageData> result = new List<AdLanguageData>();

        if (showSingleGame)
        {
            var game = games.Find(g => g.game == fixedGame);
            if (game != null)
            {
                var lang = game.languageVariants.Find(l => l.language == currentLanguage && l.sprite != null);
                if (lang != null)
                    result.Add(lang);
            }
        }
        else
        {
            foreach (var game in games)
            {
                var lang = game.languageVariants.Find(l => l.language == currentLanguage && l.sprite != null);
                if (lang != null)
                    result.Add(lang);
            }
        }

        if (result.Count == 0)
        {
            foreach (var game in games)
            {
                foreach (var lang in game.languageVariants)
                {
                    if (lang.sprite != null)
                    {
                        result.Add(lang);
                        break;
                    }
                }
            }
        }

        return result;
    }

    private void EmergencyFallback()
    {
        foreach (var game in games)
        {
            foreach (var lang in game.languageVariants)
            {
                if (lang.sprite != null)
                {
                    SpawnSingleAd(lang, 0);
                    return;
                }
            }
        }

        Debug.LogError("[AdBanner] Emergency fallback failed – no sprites at all");
    }

    private void SpawnSingleAd(AdLanguageData ad, int index)
    {
        GameObject go = Instantiate(adPrefab, adBannerHolder);
        RectTransform rt = go.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        rt.anchoredPosition = new Vector2(index * (adWidth + spacing), 0);

        Image img = go.GetComponent<Image>();
        img.sprite = ad.sprite;
        img.preserveAspect = true;

        Button btn = go.GetComponent<Button>();
        if (!string.IsNullOrEmpty(ad.url))
        {
            string urlCopy = ad.url;
            btn.onClick.AddListener(() => Application.OpenURL(urlCopy));

            //if (urlGameMap.TryGetValue(urlCopy, out var game))
               // btn.onClick.AddListener(() => //AnalyticsManager.LogAdBannerClick(game));
        }

        spawnedAds.Add(rt);
    }

    private void ClearHolder()
    {
        TextMeshProUGUI headerText = null;
        foreach (Transform child in adBannerHolder)
        {
            var text = child.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                headerText = text;
                continue;
            }
            Destroy(child.gameObject);
        }

        spawnedAds.Clear();
        pulseTween?.Kill();
    }

    private void BuildUrlMap()
    {
        urlGameMap.Clear();

        foreach (var game in games)
        {
            foreach (var lang in game.languageVariants)
            {
                if (!string.IsNullOrEmpty(lang.url) && !urlGameMap.ContainsKey(lang.url))
                    urlGameMap.Add(lang.url, game.game);
            }
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    private void MoveNext()
    {
        isMoving = true;
        float step = adWidth + spacing;

        foreach (var rt in spawnedAds)
            rt.DOAnchorPosX(rt.anchoredPosition.x - step, moveDuration);

        DOVirtual.DelayedCall(moveDuration, () =>
        {
            RectTransform first = spawnedAds[0];
            if (first.anchoredPosition.x + adWidth < 0)
            {
                RectTransform last = spawnedAds[spawnedAds.Count - 1];
                first.anchoredPosition =
                    new Vector2(last.anchoredPosition.x + adWidth + spacing, 0);

                spawnedAds.RemoveAt(0);
                spawnedAds.Add(first);
            }

            isMoving = false;
        });
    }

    // =========================
    // EFFECTS
    // =========================

    private void StartPulseEffect(RectTransform parent)
    {
        pulseTween?.Kill();

        pulseTween = parent
            .DOScale(pulseScale, pulseDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        pulseTween?.Kill();
    }
}
