using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsToggleButton : MonoBehaviour
{
    public enum SettingType
    {
        Sound,
        Vibration
    }

    [Header("Setting Type")]
    [SerializeField] private SettingType settingType;

    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage; // tylko sprite zmieniamy

    [Header("Sprites")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Header("Button Colors")]
    [SerializeField] private Color buttonOnColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color buttonOffColor = new Color(0.4f, 0.4f, 0.4f);

    private void Awake()
    {
        button.onClick.AddListener(Toggle);
        UpdateVisual(false);
    }

    private void Toggle()
    {
        switch (settingType)
        {
            case SettingType.Sound:
                GameSettingsManager.SoundEnabled = !GameSettingsManager.SoundEnabled;
                break;

            case SettingType.Vibration:
                GameSettingsManager.VibrationEnabled = !GameSettingsManager.VibrationEnabled;
                break;
        }

        UpdateVisual(true);
    }

    private void UpdateVisual(bool animate)
    {
        bool isOn = false;

        switch (settingType)
        {
            case SettingType.Sound:
                isOn = GameSettingsManager.SoundEnabled;
                break;

            case SettingType.Vibration:
                isOn = GameSettingsManager.VibrationEnabled;
                break;
        }

        // Zmiana sprite
        if (iconImage != null)
            iconImage.sprite = isOn ? onSprite : offSprite;

        // Zmiana koloru BUTTONA (nie ikony!)
        if (button != null && button.image != null)
            button.image.color = isOn ? buttonOnColor : buttonOffColor;

        // animacja klikniêcia
        if (animate)
        {
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 8, 0.5f);
        }
    }
}