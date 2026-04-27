using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelSlider : MonoBehaviour
{
    [Header("Panele w kolejności")]
    [SerializeField] private RectTransform[] panels;

    [Header("Przyciski")]
   // [SerializeField] private Button[] buttons;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Animacja")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutCubic;

    private int currentIndex = 0; // środkowy panel startowy
    private float lastWidth = 0;
    private bool isAnimating = false;

    private RectTransform parent;

    private void Awake()
    {
        parent = panels[0].parent.GetComponent<RectTransform>();

        // Przypisz przyciski
      /*  for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }*/

        // 🔹 Ustaw kolory przycisków przy starcie
        UpdateButtonColors();
    }


    private void LateUpdate()
    {
        float parentWidth = parent.rect.width;

        // Jeśli szerokość zmieniła się od ostatniego frame, aktualizujemy panele
        if (Mathf.Abs(parentWidth - lastWidth) > 0.1f)
        {
            UpdatePanels(parentWidth);
            lastWidth = parentWidth;
        }
    }

    private void UpdatePanels(float width)
    {
        // ustaw pivot paneli na lewy środek
        foreach (RectTransform panel in panels)
        {
            panel.pivot = new Vector2(0, 0.5f);
        }

        // ustawienie paneli obok siebie
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].anchoredPosition = new Vector2(width * i, 0);
        }

        // ustaw parenta na aktualny panel
        parent.anchoredPosition = new Vector2(-width * currentIndex, parent.anchoredPosition.y);
    }

    private void OnButtonClicked(int index)
    {
        if (isAnimating || index == currentIndex) return;

        SlideTo(index);

        //ProfileManager.instance.SaveProfile();
    }

    private void SlideTo(int newIndex)
    {
        float width = parent.rect.width;
        if (width <= 0) return;

        isAnimating = true;
        float targetX = -width * newIndex;

        parent.DOLocalMoveX(targetX, slideDuration).SetEase(slideEase);
        // .OnComplete(() =>
        //{
        currentIndex = newIndex;
        //UpdateButtonColors();
        isAnimating = false;
        // });
        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
     //   for (int i = 0; i < buttons.Length; i++)
        {
       //     Color targetColor = (i == currentIndex) ? activeColor : inactiveColor;
          //  buttons[i].transform.GetChild(0).GetComponent<Image>().DOColor(targetColor, slideDuration);
        }
    }
}
