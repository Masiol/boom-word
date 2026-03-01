using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetGameMode(GameMode mode)
    {
        SelectedMode = mode;
    }

    public void StartGame()
    {
        if (SelectedMode == GameMode.Full &&
            !PremiumManager.Instance.IsPremiumActive())
        {
            Debug.Log("Full mode locked");
            return;
        }

        CollectGameData();
        SceneManager.LoadScene("GameScene");
    }

    void CollectGameData()
    {
        SoundEnabled = GameSettingsManager.SoundEnabled;
        VibrationEnabled = GameSettingsManager.VibrationEnabled;
        PlayersCount = GameSettingsManager.PlayersCount;

        var range = GameSettingsManager.GetBombTimeRange();
        BombMinTime = range.min;
        BombMaxTime = range.max;

        Language = GameSettingsManager.Language;
    }
}