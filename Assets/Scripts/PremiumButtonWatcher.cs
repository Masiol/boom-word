using UnityEngine;

public class PremiumButtonWatcher : MonoBehaviour
{
    void OnEnable()
    {
        if (PremiumManager.Instance != null)
        {
          //  PremiumManager.Instance.CheckPremiumAndAnimate();
        }
    }
}