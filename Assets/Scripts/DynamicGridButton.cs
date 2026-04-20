using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerGradient
{
    public Color color1;
    public Color color2;
}

public class DynamicGridButton : MonoBehaviour
{
    public GameObject buttonPrefab;
    public RectTransform parent;
    public GridLayoutGroup grid;

    public Transform nextRoundButton;

    private List<int> buttonValues = new List<int>();

    public int minButtons = 2;
    public int maxButtons = 12;

    public float spacing = 10f;
    public float padding = 20f;

    [Header("Animation")]
    public float animationDuration = 0.4f;
    public float animationDelayStep = 0.05f;
    public Ease animationEase = Ease.OutBack;

    public PlayerGradient[] playerGradients = new PlayerGradient[]
   {
    new PlayerGradient { color1 = new Color(1f, 0.45f, 0.45f), color2 = new Color(1f, 0.1f, 0.1f) },   // czerwony neon
    new PlayerGradient { color1 = new Color(0.4f, 0.9f, 1f), color2 = new Color(0.1f, 0.5f, 1f) },     // niebieski glow
    new PlayerGradient { color1 = new Color(0.5f, 1f, 0.6f), color2 = new Color(0.1f, 0.8f, 0.3f) },   // zielony neon
    new PlayerGradient { color1 = new Color(1f, 0.95f, 0.5f), color2 = new Color(1f, 0.7f, 0.1f) },    // żółty złoty
    new PlayerGradient { color1 = new Color(1f, 0.5f, 0.9f), color2 = new Color(0.9f, 0.1f, 0.6f) },   // róż neon
    new PlayerGradient { color1 = new Color(0.5f, 1f, 1f), color2 = new Color(0.1f, 0.7f, 1f) },       // turkus
    new PlayerGradient { color1 = new Color(1f, 0.65f, 0.3f), color2 = new Color(1f, 0.3f, 0.05f) },   // pomarańcz ogień
    new PlayerGradient { color1 = new Color(0.8f, 0.6f, 1f), color2 = new Color(0.5f, 0.2f, 1f) },     // fiolet neon
    new PlayerGradient { color1 = new Color(0.7f, 1f, 0.3f), color2 = new Color(0.3f, 0.8f, 0.1f) },   // limonka
    new PlayerGradient { color1 = new Color(0.4f, 0.4f, 1f), color2 = new Color(0.1f, 0.1f, 0.9f) },   // deep blue
    new PlayerGradient { color1 = new Color(1f, 0.3f, 0.8f), color2 = new Color(0.8f, 0.1f, 0.5f) },   // hot pink
    new PlayerGradient { color1 = new Color(0.6f, 0.6f, 0.6f), color2 = new Color(0.3f, 0.3f, 0.3f) }  // szary


   };
    public void StartGenerate()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(Init());
    }
    public void NextRound()
    {
        nextRoundButton.DOScale(Vector3.zero, 0.20f).SetEase(Ease.InBack);


        transform.DOScale(0f, 0.20f).SetEase(Ease.InBack);

    }

    void Start()
    {
        nextRoundButton.DOScale(Vector3.zero, 0);
        nextRoundButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.StartNextRound());


    }

    IEnumerator Init()
    {
        yield return null; // 🔥 czekamy aż layout się policzy
        Refresh();

       // nextRoundButton.DOScale(Vector3.zero, 0);
    }

    public void Refresh()
    {
        int count = Mathf.Clamp(GameSettingsManager.PlayersCount, minButtons, maxButtons);

        grid.enabled = false;

        ClearChildren();
        GenerateButtons(count);
        ConfigureGrid(count);

        grid.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
    }

    void ClearChildren()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    public void AnimateButtonPanel()
    {

            transform.DOScale(1f, animationDuration)
                .SetEase(animationEase)
                .SetDelay(animationDelayStep);

            nextRoundButton.DOScale(1f, animationDuration)
                .SetEase(animationEase)
                .SetDelay(animationDelayStep + 1);

    }

    void GenerateButtons(int count)
    {
        buttonValues.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, parent);

            int index = i;
            buttonValues.Add(0);

            // 🎨 gradient
            PlayerGradient gradientData = playerGradients[i % playerGradients.Length];

            UIGradient gradient = btn.GetComponentInChildren<UIGradient>();
            if (gradient != null)
            {
                gradient.m_color1 = gradientData.color1;
                gradient.m_color2 = gradientData.color2;
                gradient.m_angle = 45f;
            }

            // 🔢 tekst
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = "0";
            }

            // 🖱️ kliknięcie
            Button button = btn.GetComponent<Button>();
            if (button != null && text != null)
            {
                button.onClick.AddListener(() => OnButtonClicked(index, text, btn.transform));
            }
        }
    }
    void OnButtonClicked(int index, TMP_Text text, Transform btnTransform)
    {
        buttonValues[index]++;
        text.text = buttonValues[index].ToString();

        // 🔥 zabij poprzednie animacje (ważne przy spamie)
        text.transform.DOKill();

        text.transform.localScale = Vector3.one;

        text.transform
            .DOScale(1.25f, 0.12f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                text.transform
                    .DOScale(1f, 0.12f)
                    .SetEase(Ease.InQuad);
            });
    }

    void ConfigureGrid(int count)
    {
        int rows = (count <= 6) ? 1 : 2;
        int columns = Mathf.CeilToInt((float)count / rows);

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
        grid.childAlignment = TextAnchor.MiddleCenter;

        CalculateCellSize(columns, rows);
    }

    void CalculateCellSize(int columns, int rows)
    {
        float parentWidth = parent.rect.width;
        float parentHeight = parent.rect.height;

        if (parentWidth <= 0 || parentHeight <= 0)
            return; // 🔥 zabezpieczenie

        float totalSpacingX = spacing * (columns - 1);
        float totalSpacingY = spacing * (rows - 1);

        float totalPaddingX = padding * 2;
        float totalPaddingY = padding * 2;

        float availableWidth = parentWidth - totalSpacingX - totalPaddingX;
        float availableHeight = parentHeight - totalSpacingY - totalPaddingY;

        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;

        float size = Mathf.Min(cellWidth, cellHeight);

        size = Mathf.Clamp(size, 60f, 150f);

        grid.cellSize = new Vector2(size, size);
    }
    public void ResetButtons()
    {
        buttonValues.Clear();

        for (int i = 0; i < parent.childCount; i++)
        {
            buttonValues.Add(0);

            TMP_Text text = parent.GetChild(i).GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = "0";

                text.transform.DOKill();
                text.transform.localScale = Vector3.one;
            }
        }

        // opcjonalnie animacja wejścia
        AnimateButtonPanel();
    }
}