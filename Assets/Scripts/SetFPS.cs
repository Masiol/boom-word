using UnityEngine;

public class SetFPS : MonoBehaviour
{
    public int targetFPS = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0; // musi byæ wy³¹czony
        Application.targetFrameRate = targetFPS;
    }
}