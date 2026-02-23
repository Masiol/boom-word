using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayersSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Text playersText;

    [Header("Limits")]
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 20;

    private void Awake()
    {
        plusButton.onClick.AddListener(AddPlayer);
        minusButton.onClick.AddListener(RemovePlayer);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void AddPlayer()
    {
        if (GameSettingsManager.PlayersCount >= maxPlayers)
            return;

        GameSettingsManager.PlayersCount++;
        Animate();
        UpdateUI();
    }

    private void RemovePlayer()
    {
        if (GameSettingsManager.PlayersCount <= minPlayers)
            return;

        GameSettingsManager.PlayersCount--;
        Animate();
        UpdateUI();
    }

    private void UpdateUI()
    {
        playersText.text = GameSettingsManager.PlayersCount.ToString();
    }

    private void Animate()
    {
        playersText.transform
            .DOPunchScale(Vector3.one * 0.2f, 0.2f, 8, 0.5f);
    }
}