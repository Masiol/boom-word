using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using DG.Tweening;
using System.Linq;

public class LanguageSelector : MonoBehaviour
{
    [System.Serializable]
    public class LanguageButton
    {
        public Button button;
        public int languageCode; // 0 = PL, 1 = EN, 2 = DE
        public float startDelay;
        [HideInInspector] public Vector3 originalScale;
    }

    public LanguageButton[] languageButtons;

    private LanguageButton activeButton;

    void Start()
    {
        foreach (var btn in languageButtons)
        {
            btn.originalScale = btn.button.transform.localScale;

            btn.button.onClick.AddListener(() =>
            {
                SelectLanguage(btn);
            });
        }

        // ustaw aktywny przycisk na podstawie zapisu
        int saved = GetSavedLanguageIndex();

        foreach (var btn in languageButtons)
        {
            if (btn.languageCode == saved)
            {
                activeButton = btn;
                Animate(btn);
                break;
            }
        }
    }

    void SelectLanguage(LanguageButton btn)
    {
        // animacja
        if (activeButton != null)
        {
            activeButton.button.transform.DOKill();
            activeButton.button.transform.localScale = activeButton.originalScale;
        }

        activeButton = btn;
        Animate(btn);

        // 🔥 mapowanie INT → string
        string code = MapIntToCode(btn.languageCode);

        // zapis
        GameSettingsManager.Language = code.ToUpper();

        // zmiana locale
        ApplyLocale(code);

        Debug.Log("Language changed: " + code);
    }

    string MapIntToCode(int index)
    {
        switch (index)
        {
            case 0: return "pl";
            case 1: return "en";
            case 2: return "de";
            default: return "en";
        }
    }

    int GetSavedLanguageIndex()
    {
        string lang = GameSettingsManager.Language;

        switch (lang)
        {
            case "PL": return 0;
            case "EN": return 1;
            case "DE": return 2;
            default: return 1;
        }
    }

    void ApplyLocale(string code)
    {
        code = code.ToLower();

        var locale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(l => l.Identifier.Code == code);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning("Locale not found: " + code);
        }
    }

    void Animate(LanguageButton btn)
    {
        btn.button.transform
            .DOScale(btn.originalScale * 1.2f, 0.3f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}