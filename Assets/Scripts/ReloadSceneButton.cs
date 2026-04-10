using UnityEngine;
using DG.Tweening;

public class PopupWindow : MonoBehaviour
{
    public RectTransform window;

    void Awake()
    {
        window.localScale = Vector3.zero;
       // gameObject.SetActive(false);
    }

    public void Open()
    {
       // gameObject.SetActive(true);

        window.localScale = Vector3.zero;

        window.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack);
    }

    public void Close()
    {
        window.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
               // gameObject.SetActive(false);
            });
    }


}