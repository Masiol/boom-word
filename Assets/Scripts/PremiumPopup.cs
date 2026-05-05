using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PremiumPopupUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform popup;
    public Image popupImage;

    [Header("Sprites")]
    public Sprite spritePL;
    public Sprite spriteEN;
    public Sprite spriteDE;

    void Start()
    {
        PremiumManager.OnPremiumActivated += ShowPopup;

        canvasGroup.alpha = 0;
       // gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        PremiumManager.OnPremiumActivated -= ShowPopup;
    }

    void ShowPopup()
    {
        //gameObject.SetActive(true);

        // 🔥 wybór sprite'a
        switch (GameSettingsManager.Language)
        {
            case "PL":
                popupImage.sprite = spritePL;
                break;

            case "DE":
                popupImage.sprite = spriteDE;
                break;

            default:
                popupImage.sprite = spriteEN;
                break;
        }

        canvasGroup.alpha = 0;
        popup.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        // 🔥 FADE + POP
        seq.Append(canvasGroup.DOFade(1f, 0.15f));
        //SoundManager.Instance.Play(SoundID.Premium);

        seq.Join(
            popup.DOScale(1.2f, 0.25f)
            .SetEase(Ease.OutBack)
        );
        SoundManager.Instance.Play(SoundID.Premium);

        // settle
        seq.Append(popup.DOScale(1f, 0.15f));

        // bounce
        seq.Append(popup.DOScale(1.05f, 0.1f));
        seq.Append(popup.DOScale(1f, 0.1f));

        // pokaz
        seq.AppendInterval(1.5f);

        // znikanie
        seq.Append(canvasGroup.DOFade(0f, 0.25f));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}