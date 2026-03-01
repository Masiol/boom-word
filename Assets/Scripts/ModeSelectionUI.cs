using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ModeSelectionUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button normalButton;
    public Button premiumButton;

    [Header("Checkmarks")]
    public GameObject normalCheck;
    public GameObject premiumCheck;

    [Header("Colors")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    [Header("Scale Settings")]
    public float selectedScale = 1.05f;
    public float normalScale = 1f;
    public float scaleDuration = 0.15f;

    void Start()
    {
        normalButton.onClick.AddListener(SelectNormal);
        premiumButton.onClick.AddListener(SelectPremium);

        ResetVisual(); // 🔥 zawsze startuje od zera
    }

    void SelectNormal()
    {
        UpdateVisual(true, false, true);
        StartFlowController.Instance.ShowStartButton();
    }

    void SelectPremium()
    {
        if (!PremiumManager.Instance.IsPremiumActive())
            return;

        UpdateVisual(false, true, true);
        StartFlowController.Instance.ShowStartButton();
    }

    public void ResetVisual()
    {
        normalButton.image.color = defaultColor;
        premiumButton.image.color = defaultColor;

        normalCheck.SetActive(false);
        premiumCheck.SetActive(false);

        normalButton.transform.localScale = Vector3.one * normalScale;
        premiumButton.transform.localScale = Vector3.one * normalScale;
    }

    void UpdateVisual(bool normalSelected, bool premiumSelected, bool animate)
    {
        normalButton.image.color = normalSelected ? selectedColor : defaultColor;
        premiumButton.image.color = premiumSelected ? selectedColor : defaultColor;

        normalCheck.SetActive(normalSelected);
        premiumCheck.SetActive(premiumSelected);

        AnimateButton(normalButton.transform, normalSelected, animate);
        AnimateButton(premiumButton.transform, premiumSelected, animate);
    }

    void AnimateButton(Transform target, bool isSelected, bool animate)
    {
        float targetScale = isSelected ? selectedScale : normalScale;

        target.DOKill();

        if (animate)
        {
            target.DOScale(targetScale, scaleDuration)
                  .SetEase(Ease.OutQuad);
        }
        else
        {
            target.localScale = Vector3.one * targetScale;
        }
    }
}