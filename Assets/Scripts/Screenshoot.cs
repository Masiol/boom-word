using System;
using UnityEngine;

public class Screenshoot : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            ScreenCapture.CaptureScreenshot("screenshot-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +".png", 4);
        }
    }
}
