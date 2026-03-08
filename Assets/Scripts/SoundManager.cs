using System.Collections.Generic;
using UnityEngine;

public enum SoundID
{
    ButtonClick,
    PhraseAppear,
    BombExplode,
    ClockLoop,
    Swish,
    Countdown
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class SoundEntry
    {
        public SoundID id;
        public AudioClip clip;
    }

    [Header("Sounds")]
    public List<SoundEntry> sounds;

    Dictionary<SoundID, AudioClip> soundDict;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource loopSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDict = new Dictionary<SoundID, AudioClip>();

        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.id))
                soundDict.Add(s.id, s.clip);
        }
    }

    // ===== PLAY ENUM =====

    public void Play(SoundID id)
    {
        if (!GameSettingsManager.SoundEnabled)
            return;

        if (soundDict.TryGetValue(id, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + id);
        }
    }

    // ===== PLAY STRING (dla przycisków) =====

    public void Play(string id)
    {
        if (System.Enum.TryParse(id, out SoundID sound))
        {
            Play(sound);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + id);
        }
    }

    // ===== LOOP =====

    public void PlayLoop(SoundID id)
    {
        if (!GameSettingsManager.SoundEnabled)
            return;

        if (soundDict.TryGetValue(id, out AudioClip clip))
        {
            loopSource.clip = clip;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    public void StopLoop()
    {
        loopSource.Stop();
    }

    // ===== PITCH (np. przyspieszenie bomby) =====

    public void SetLoopPitch(float pitch)
    {
        loopSource.pitch = pitch;
    }
}