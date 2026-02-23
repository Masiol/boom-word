using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BombTimeButton : MonoBehaviour
{
    [SerializeField] private int optionIndex; // 0,1,2,3
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.75f, 0.2f);
    [SerializeField] private Color normalColor = new Color(0.2f, 0.6f, 1f);

    private void Awake()
    {
        button.onClick.AddListener(Select);
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void Select()
    {
        GameSettingsManager.BombTimeOption = optionIndex;

        // odœwie¿ wszystkie przyciski
        BombTimeButton[] allButtons = FindObjectsOfType<BombTimeButton>();
        foreach (var btn in allButtons)
        {
            btn.UpdateVisual();
        }

        // ma³a animacja klikniêcia
        transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 8, 0.5f);
    }

    public void UpdateVisual()
    {
        bool isSelected = GameSettingsManager.BombTimeOption == optionIndex;

        if (backgroundImage != null)
            backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
}