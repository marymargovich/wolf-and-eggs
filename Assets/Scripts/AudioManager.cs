using UnityEngine;

/// <summary>
/// Centralized audio controller for music and sound effects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public bool IsMusicMuted => musicSource != null && musicSource.mute;
    public bool IsSFXMuted => sfxSource != null && sfxSource.mute;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip winMusicClip;
    public AudioClip goodItemClip;
    public AudioClip badItemClip;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Reuse existing AudioSources first; create missing ones if needed.
        AudioSource[] sources = GetComponents<AudioSource>();

        if (musicSource == null && sources.Length > 0)
        {
            musicSource = sources[0];
        }

        if (sfxSource == null && sources.Length > 1)
        {
            sfxSource = sources[1];
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    public void PlayBGM()
    {
        if (musicSource == null || bgmClip == null)
        {
            return;
        }

        if (musicSource.clip == bgmClip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = bgmClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayWinMusic()
    {
        if (musicSource == null || winMusicClip == null)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = winMusicClip;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void PlayGoodItemSFX()
    {
        if (sfxSource == null || goodItemClip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(goodItemClip);
    }

    public void PlayBadItemSFX()
    {
        if (sfxSource == null || badItemClip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(badItemClip);
    }

    public void SetMusicMute(bool isMuted)
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.mute = isMuted;
    }

    public void SetSFXMute(bool isMuted)
    {
        if (sfxSource == null)
        {
            return;
        }

        sfxSource.mute = isMuted;
    }

    public bool ToggleMute()
    {
        bool newMuteState = !(IsMusicMuted && IsSFXMuted);
        SetMusicMute(newMuteState);
        SetSFXMute(newMuteState);
        return newMuteState;
    }
}