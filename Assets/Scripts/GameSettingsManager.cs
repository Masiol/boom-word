using UnityEngine;

public static class GameSettingsManager
{
    private const string SOUND_KEY = "SoundEnabled";
    private const string VIBRATION_KEY = "VibrationEnabled";
    private const string BOMB_TIME_KEY = "BombTimeOption";
    private const string PLAYERS_KEY = "PlayersCount";
    private const string LANGUAGE_KEY = "GameLanguage";

    public static bool SoundEnabled
    {
        get => PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(SOUND_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool VibrationEnabled
    {
        get => PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(VIBRATION_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int BombTimeOption
    {
        get => PlayerPrefs.GetInt(BOMB_TIME_KEY, 0);
        set
        {
            PlayerPrefs.SetInt(BOMB_TIME_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public static int PlayersCount
    {
        get => PlayerPrefs.GetInt(PLAYERS_KEY, 8);
        set
        {
            PlayerPrefs.SetInt(PLAYERS_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public static string Language
    {
        get
        {
            string lang = PlayerPrefs.GetString(LANGUAGE_KEY, "EN");

            // ?? migracja starych danych (int ? string)
            switch (lang)
            {
                case "0": return "EN";
                case "1": return "PL";
                case "2": return "DE";
            }

            return lang;
        }
        set
        {
            PlayerPrefs.SetString(LANGUAGE_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public static (int min, int max) GetBombTimeRange()
    {
        switch (BombTimeOption)
        {
            //  case 0: return (30, 60);
            //  case 1: return (60, 90);
            //  case 2: return (90, 120);
            //  case 3: return (30, 120);
            //  default: return (30, 60);
              case 0: return (5, 5);
              case 1: return (5, 5);
              case 2: return (5, 5);
              case 3: return (5, 5);
              default: return (5, 5);
        }
    }
}