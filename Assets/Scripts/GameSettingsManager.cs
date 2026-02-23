using UnityEngine;

public static class GameSettingsManager
{
    private const string SOUND_KEY = "SoundEnabled";
    private const string VIBRATION_KEY = "VibrationEnabled";
    private const string BOMB_TIME_KEY = "BombTimeOption";

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

    // 0 = 30-60
    // 1 = 60-90
    // 2 = 90-120
    // 3 = Random
    public static int BombTimeOption
    {
        get => PlayerPrefs.GetInt(BOMB_TIME_KEY, 0); // domyœlnie 30-60
        set
        {
            PlayerPrefs.SetInt(BOMB_TIME_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public static (int min, int max) GetBombTimeRange()
    {
        switch (BombTimeOption)
        {
            case 0: return (30, 60);
            case 1: return (60, 90);
            case 2: return (90, 120);
            case 3: return (30, 120); // random pe³ny zakres
            default: return (30, 60);
        }
    }

    private const string PLAYERS_KEY = "PlayersCount";

    public static int PlayersCount
    {
        get => PlayerPrefs.GetInt(PLAYERS_KEY, 8); // domyœlnie 8
        set
        {
            PlayerPrefs.SetInt(PLAYERS_KEY, value);
            PlayerPrefs.Save();
        }
    }
}