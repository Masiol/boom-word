using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButtonSound : MonoBehaviour, IPointerDownHandler
{
    public string soundId;

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.Play(soundId);
    }

    public void PlaySound(string soundId)
    {
        SoundManager.Instance.Play(soundId);
    }
}