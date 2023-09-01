using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages all sounds in the game
// created 22/8/23
// last modified 1/9/23

public enum MusicTheme
{
    None,
    Peace,
    Survival,
    Tension,
    Battle
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private const int SOURCECOUNT = 10;
    [Header("Base audio sources")]
    [SerializeField]private AudioSource sourceMusic;
    [SerializeField]private AudioSource sourceAmbience;
    [SerializeField]private AudioSource sourceMulti;
    [Header("Music playlists")]
    [SerializeField]private AudioClip[] clipMusicBattle; // for intense battle
    [SerializeField]private AudioClip[] clipMusicTense; // for suspenseful scenes
    [SerializeField]private AudioClip[] clipMusicSurvival; // for normal camp mode
    [SerializeField]private AudioClip[] clipMusicPeace; // for menus
    [Header("Music settings")]
    [SerializeField]private float musicTransitionTime = 1f;
    [SerializeField]private float musicTrackTimeMin = 240f;
    [SerializeField]private float musicTrackTimeMax = 480f;
    private AudioSource sourceMusicAlt; // the second music clip to allow transitions
    private AudioClip clipMusicQued; // the clip the audiomanager will transition to next after the current transition is resolved - this should be avoided generally
    private bool sourceMusicIsAlt = false; // to track whether the sourceMusicAlt is currently the primary source
    private float musicTransitionProgress; // how far the transition between music tracks has progressed
    private AudioSource[] sourceSingle;
    private int sourceSingleNext;
    private MusicTheme themeSwitcher;
    private Coroutine themeSwitcherRoutine;

    private void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else instance = this;

        sourceMusicAlt = Instantiate(sourceMusic, transform);
        sourceSingle = new AudioSource[SOURCECOUNT];
        for (int i = 0; i < SOURCECOUNT; i++)
        {
            sourceSingle[i] = Instantiate(sourceMulti, transform);
        }
    }

    public void MusicPlay(MusicTheme musicTheme)
    {
        if (musicTheme == MusicTheme.None) return;

        AudioClip musicCurrent;
        AudioClip musicLoop;
        List<AudioClip> musicTracks = new List<AudioClip>();

        if (sourceMusicIsAlt)
        {
            if (sourceMusicAlt.isPlaying)
                musicCurrent = sourceMusicAlt.clip;
            else
                musicCurrent = null;
        }
        else
        {
            if (sourceMusic.isPlaying)
                musicCurrent = sourceMusicAlt.clip;
            else
                musicCurrent = null;
        }

        switch (musicTheme)
        {
            default:
            {
                return;
            }
            case MusicTheme.Peace:
            {
                musicTracks.AddRange(clipMusicPeace);
                break;
            }
            case MusicTheme.Survival:
            {
                musicTracks.AddRange(clipMusicSurvival);
                break;
            }
            case MusicTheme.Tension:
            {
                musicTracks.AddRange(clipMusicTense);
                break;
            }
            case MusicTheme.Battle:
            {
                musicTracks.AddRange(clipMusicBattle);
                break;
            }
        }

        if (musicCurrent)
            musicTracks.Remove(musicCurrent);

        if (musicTracks.Count == 0)
        {
            Debug.LogError("AudioManager - track list has 0 tracks - musicCurrent is " + musicCurrent);
            return;
        }

        musicLoop = musicTracks[Random.Range(0, musicTracks.Count)];

        StartMusic(musicLoop);
    }

    // que up the provided clip into the music transition system
    private void StartMusic(AudioClip musicLoop)
    {
        if (musicTransitionProgress > 0)
        {
            // a transition is already in place, store the clip and let the current transition finish
            clipMusicQued = musicLoop;
            return;
        }

        // note that the MusicTransition coroutine should only ever be active on the above condition so we can assume we don't need to stop it if we get here

        if (sourceMusicIsAlt)
        {
            sourceMusic.Stop();
            sourceMusic.loop = true;
            sourceMusic.clip = musicLoop;
            sourceMusic.volume = 0f;
            sourceMusic.Play();
            sourceMusicIsAlt = false;
        }
        else
        {
            sourceMusicAlt.Stop();
            sourceMusicAlt.loop = true;
            sourceMusicAlt.clip = musicLoop;
            sourceMusicAlt.volume = 0f;
            sourceMusicAlt.Play();
            sourceMusicIsAlt = true;
        }

        sourceMusic.clip = musicLoop;
        sourceMusic.Play();
        StartCoroutine(MusicTransition());
    }

    private IEnumerator MusicTransition()
    {
        while (musicTransitionProgress < musicTransitionTime)
        {
            musicTransitionProgress += Time.deltaTime;
            float progress = Mathf.Clamp(musicTransitionProgress / musicTransitionTime, 0f, 1f);
            if (sourceMusicIsAlt) progress = 1f - progress;
            sourceMusic.volume = progress;
            sourceMusicAlt.volume = 1f - progress;
            yield return new WaitForEndOfFrame();
        }

        musicTransitionProgress = 0f;
        if (sourceMusicIsAlt)
        {
            sourceMusic.Stop();
            sourceMusicAlt.volume = 1f;
        }
        else
        {
            sourceMusic.volume = 1f;
            sourceMusicAlt.Stop();
        }

        if (clipMusicQued)
        {
            // a clip has been qued during the coroutine, so start transition to that one now
            StartMusic(clipMusicQued);
            clipMusicQued = null;
        }
    }

    public void MusicPlay(AudioClip musicLoop)
    {
        StartMusic(musicLoop);
        // when forcing a specific track, turn off the track switcher
        MusicSwitcher(MusicTheme.None);
    }

    // play a non-looping music sting (sets music audio to non-looping)
    public void MusicPlaySting(AudioClip musicSting)
    {
        MusicSwitcher(MusicTheme.None);
        sourceMusic.Stop();
        sourceMusicAlt.Stop();

        sourceMusic.loop = false;
        sourceMusic.clip = musicSting;
        sourceMusic.volume = 1f;
        sourceMusic.Play();
        sourceMusicIsAlt = false;
    }

    public void MusicStop()
    {
        sourceMusic.Stop();
    }

    // tells the audio manager to periodically change the music track within the theme
    public void MusicSwitcher(MusicTheme theme)
    {
        themeSwitcher = theme;

        if (themeSwitcherRoutine != null)
            StopCoroutine(themeSwitcherRoutine);

        if (themeSwitcher != MusicTheme.None)
            themeSwitcherRoutine = StartCoroutine(MusicSwitchCycle());
    }

    private IEnumerator MusicSwitchCycle()
    {
        while (themeSwitcher != MusicTheme.None)
        {
            yield return new WaitForSeconds(Random.Range(musicTrackTimeMin, musicTrackTimeMax));
            MusicPlay(themeSwitcher); // pick a new track to transition to
        }
    }

    // defines and starts the ambient sound player
    public void AmbiencePlay(AudioClip ambienceLoop, Vector2 pos, float volume = 1f)
    {
        sourceAmbience.Stop();
        sourceAmbience.clip = ambienceLoop;
        sourceAmbience.volume = volume;
        if (pos == Vector2.zero)
        {
            sourceAmbience.spatialBlend = 0f;
        }
        else
        {
            sourceAmbience.spatialBlend = 1f;
            sourceAmbience.transform.position = pos;
        }
        sourceAmbience.Play();
    }

    public void AmbienceVolume(float volume)
    {
        sourceAmbience.volume = volume;
    }

    public void SoundPlayEven(AudioClip sound, Vector2 pos, float volume = 1f)
    {
        SoundPlay(sound, pos, volume, 1f);
    }

    public void SoundPlayVaried(AudioClip sound, Vector2 pos, float volume = 1f)
    {
        SoundPlay(sound, pos, volume, Random.Range(0.9f, 1.1f));
    }

    public void SoundPlayCustom(AudioClip sound, Vector2 pos, float volume = 1f, float pitch = 1f)
    {
        SoundPlay(sound, pos, volume, pitch);
    }

    private void SoundPlay(AudioClip sound, Vector2 pos, float volume, float pitch)
    {
        sourceSingle[sourceSingleNext].Stop();
        sourceSingle[sourceSingleNext].volume = volume;
        sourceSingle[sourceSingleNext].pitch = pitch;
        sourceSingle[sourceSingleNext].clip = sound;
        if (pos == Vector2.zero)
        {
            sourceSingle[sourceSingleNext].spatialBlend = 0f;
        }
        else
        {
            sourceSingle[sourceSingleNext].spatialBlend = 1f;
            sourceSingle[sourceSingleNext].transform.position = pos;
        }
        sourceSingle[sourceSingleNext].Play();

        sourceSingleNext++;
        sourceSingleNext %= SOURCECOUNT;
    }

    // stops all sound effect sources
    public void SoundAllStop()
    {
        sourceMulti.Stop();
        for (int i = 0; i < SOURCECOUNT; i++)
            sourceSingle[i].Stop();
    }
}
