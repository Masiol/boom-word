using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class PremiumManager : MonoBehaviour, IStoreListener
{
    public static PremiumManager Instance;

    private static IStoreController controller;
    private static IExtensionProvider extensions;

    [Header("IAP")]
    public string fullVersionProductId = "mtc_full_version";

    [Header("Premium UI")]
    public Transform premiumButtonTransform;   // do skalowania (RectTransform)
    public Button premiumUIButton;             // Button komponent
    public Text premiumButtonText;             // opcjonalnie tekst

    private Tween pulseTween;

    private string premiumFileName = "premium.txt";

    private const float minScale = 1.05f;
    private const float maxScale = 1.10f;
    private const float pulseDuration = 0.8f;

    public static System.Action OnPremiumActivated; // 🔥 EVENT

    // =====================================================
    // UNITY
    // =====================================================

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeIAP();

        if (IsPremiumActive())
        {
            RemovePurchaseLogic();
            StopPulseAnimationImmediate();
        }
        else
        {
            StartPulseAnimation();
            premiumButtonTransform.GetComponent<Button>().onClick.AddListener(BuyFullVersion);
        }
    }

    void OnDestroy()
    {
        if (pulseTween != null)
            pulseTween.Kill();
    }

    // =====================================================
    // ANIMACJA
    // =====================================================

    void StartPulseAnimation()
    {
        if (premiumButtonTransform == null) return;
        if (pulseTween != null && pulseTween.IsActive()) return;

        premiumButtonTransform.localScale = Vector3.one * minScale;

        pulseTween = premiumButtonTransform
            .DOScale(maxScale, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StopPulseAnimation()
    {
        if (premiumButtonTransform == null) return;

        if (pulseTween != null)
        {
            pulseTween.Kill();
            pulseTween = null;
        }

        premiumButtonTransform
            .DOScale(minScale, 0.25f)
            .SetEase(Ease.OutQuad);
    }

    void StopPulseAnimationImmediate()
    {
        if (pulseTween != null)
        {
            pulseTween.Kill();
            pulseTween = null;
        }

        if (premiumButtonTransform != null)
            premiumButtonTransform.localScale = Vector3.one * minScale;
    }

    // =====================================================
    // PREMIUM LOGIC
    // =====================================================

    public bool IsPremiumActive()
    {
        return CheckPlayerPrefs() || CheckReceipt() || CheckFile();
    }

    void ActivatePremium()
    {
        Debug.Log("Premium activated!");

        PlayerPrefs.SetInt("premium", 1);
        PlayerPrefs.Save();

        string path = Path.Combine(Application.persistentDataPath, premiumFileName);
        File.WriteAllText(path, "premium_active");

        StopPulseAnimation();
        RemovePurchaseLogic();

        OnPremiumActivated?.Invoke(); // 🔥 informujemy UI
    }

    void RemovePurchaseLogic()
    {
        Debug.Log("Removing purchase logic from premium button");

        if (premiumUIButton != null)
        {
            premiumUIButton.onClick.RemoveListener(BuyFullVersion);
        }
    }

    bool CheckPlayerPrefs()
    {
        return PlayerPrefs.GetInt("premium", 0) == 1;
    }

    bool CheckReceipt()
    {
        if (controller == null) return false;

        Product product = controller.products.WithID(fullVersionProductId);
        return product != null && product.hasReceipt;
    }

    bool CheckFile()
    {
        string path = Path.Combine(Application.persistentDataPath, premiumFileName);
        return File.Exists(path);
    }

    // =====================================================
    // IAP
    // =====================================================

    void InitializeIAP()
    {
        if (controller != null) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(fullVersionProductId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyFullVersion()
    {
        if (controller != null)
        {
            controller.InitiatePurchase(fullVersionProductId);
        }
        else
        {
            Debug.Log("IAP not initialized yet.");
        }
    }

    public void OnInitialized(IStoreController c, IExtensionProvider e)
    {
        controller = c;
        extensions = e;
        Debug.Log("IAP initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Init Failed: " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (args.purchasedProduct.definition.id == fullVersionProductId)
        {
            ActivatePremium();
            OnPremiumActivated?.Invoke(); // 🔥 informujemy UI
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase failed: " + failureReason);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Init Failed: " + error + " | " + message);
    }
}