using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Linq;

public class LanguageInitializer : MonoBehaviour
{
    IEnumerator Start()
    {
        // 🔴 czekamy aż Localization się zainicjalizuje
        yield return LocalizationSettings.InitializationOperation;

        string lang;

        if (PlayerPrefs.HasKey("GameLanguage"))
        {
            lang = FixLanguage(GameSettingsManager.Language);
            Debug.Log("Loaded saved lang: " + lang);
        }
        else
        {
            lang = GetDeviceLanguage();
            GameSettingsManager.Language = lang.ToUpper();
            Debug.Log("Detected device lang: " + lang);
        }

        ApplyLocale(lang);
    }

    string GetDeviceLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Polish: return "pl";
            case SystemLanguage.German: return "de";
            case SystemLanguage.English: return "en";
            default: return "en";
        }
    }

    string FixLanguage(string lang)
    {
        // zabezpieczenie na stare dane
        if (lang == "1") return "pl";
        if (lang == "0") return "en";
        if (lang == "2") return "de";

        return lang.ToLower();
    }

    void ApplyLocale(string code)
    {
        code = code.ToLower();

        var locale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(l => l.Identifier.Code == code);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log("Locale set to: " + code);
        }
        else
        {
            Debug.LogWarning("Locale not found: " + code);
        }
    }
}