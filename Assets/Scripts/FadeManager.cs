using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("References")]
    [SerializeField] private Image fadeImage;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // upewnij się, że zaczynamy z czarnym ekranem
            SetAlpha(1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // pierwszy fade po uruchomieniu gry
        FadeOut();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // po każdej zmianie sceny → fade out
        FadeOut();
    }

    // 🔹 Fade OUT (1 → 0)
    public void FadeOut()
    {
        fadeImage.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad);
    }

    // 🔹 Fade IN (0 → 1)
    public void FadeIn(System.Action onComplete = null)
    {
        fadeImage.DOFade(1f, fadeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => onComplete?.Invoke());
    }

    // 🔹 Fade IN + reload sceny
    public void FadeInAndReload()
    {
        FadeIn(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    // 🔹 manualne ustawienie alphy
    private void SetAlpha(float value)
    {
        Color c = fadeImage.color;
        c.a = value;
        fadeImage.color = c;
    }
}